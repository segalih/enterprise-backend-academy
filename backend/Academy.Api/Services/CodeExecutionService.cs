using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Academy.Api.Models;

namespace Academy.Api.Services
{
    public static class AcademyRuntimeGuard
    {
        private static readonly AsyncLocal<CancellationToken> CurrentToken = new();

        public static void SetToken(CancellationToken token) => CurrentToken.Value = token;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Check()
        {
            CurrentToken.Value.ThrowIfCancellationRequested();
        }
    }

    public interface ICodeExecutionService
    {
        Task<ExecuteResponse> ExecuteAsync(string code, string? input = null, int timeoutMs = 4000);
    }

    public class CodeExecutionService : ICodeExecutionService
    {
        private static readonly AsyncLocal<TextWriter?> AsyncOut = new();
        private static readonly AsyncLocal<TextWriter?> AsyncError = new();
        private static readonly AsyncLocal<TextReader?> AsyncIn = new();
        private static readonly TextWriter OriginalOut = Console.Out;
        private static readonly TextWriter OriginalError = Console.Error;
        private static readonly TextReader OriginalIn = Console.In;

        private static readonly Regex ForbiddenApiRegex = new(
            @"\b(Environment\s*\.\s*(Exit|FailFast)|Process\s*\.\s*Start|ProcessStartInfo|File\s*\.\s*(Delete|WriteAllText|WriteAllBytes|WriteAllLines|Move|Copy|AppendAllText|Create|OpenWrite)|Directory\s*\.\s*(Delete|Move|CreateDirectory)|Assembly\s*\.\s*(LoadFile|LoadFrom))\b",
            RegexOptions.Compiled | RegexOptions.CultureInvariant
        );

        static CodeExecutionService()
        {
            Console.SetOut(new AsyncLocalTextWriter(() => AsyncOut.Value ?? OriginalOut));
            Console.SetError(new AsyncLocalTextWriter(() => AsyncError.Value ?? OriginalError));
            Console.SetIn(new AsyncLocalTextReader(() => AsyncIn.Value ?? OriginalIn));
        }

        private readonly MetadataReference[] _references;

        public CodeExecutionService()
        {
            var refs = new List<MetadataReference>();

            // Load all platform assemblies from runtime context
            var trustedAssemblies = AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") as string;
            if (!string.IsNullOrEmpty(trustedAssemblies))
            {
                var paths = trustedAssemblies.Split(Path.PathSeparator);
                foreach (var path in paths)
                {
                    if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
                    {
                        try
                        {
                            refs.Add(MetadataReference.CreateFromFile(path));
                        }
                        catch { }
                    }
                }
            }

            if (refs.Count == 0)
            {
                refs.Add(MetadataReference.CreateFromFile(typeof(object).Assembly.Location));
                refs.Add(MetadataReference.CreateFromFile(typeof(Console).Assembly.Location));
                refs.Add(MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location));
            }

            var selfLocation = typeof(AcademyRuntimeGuard).Assembly.Location;
            if (!string.IsNullOrWhiteSpace(selfLocation) && File.Exists(selfLocation) &&
                !refs.Any(r => string.Equals(r.Display, selfLocation, StringComparison.OrdinalIgnoreCase)))
            {
                refs.Add(MetadataReference.CreateFromFile(selfLocation));
            }

            _references = refs.ToArray();
        }

        public async Task<ExecuteResponse> ExecuteAsync(string rawCode, string? input = null, int timeoutMs = 4000)
        {
            var stopwatch = Stopwatch.StartNew();
            var response = new ExecuteResponse();

            if (string.IsNullOrWhiteSpace(rawCode))
            {
                response.Success = false;
                response.Error = "Source code cannot be empty.";
                return response;
            }

            var forbiddenMatch = ForbiddenApiRegex.Match(rawCode);
            if (forbiddenMatch.Success)
            {
                stopwatch.Stop();
                response.Success = false;
                response.ExecutionTimeMs = stopwatch.ElapsedMilliseconds;
                response.Error = $"Security Policy Violation: Penggunaan API '{forbiddenMatch.Value}' diblokir di dalam Roslyn Sandbox demi keamanan sistem.";
                return response;
            }

            string preparedCode = NormalizeCode(rawCode);
            var parseOptions = new CSharpParseOptions(LanguageVersion.Latest, documentationMode: DocumentationMode.None);
            var syntaxTree = CSharpSyntaxTree.ParseText(preparedCode, parseOptions);

            string assemblyName = $"AcademyExec_{Guid.NewGuid():N}";
            var compilationOptions = new CSharpCompilationOptions(
                OutputKind.ConsoleApplication,
                optimizationLevel: OptimizationLevel.Release,
                allowUnsafe: false,
                nullableContextOptions: NullableContextOptions.Enable
            );

            var initialCompilation = CSharpCompilation.Create(
                assemblyName,
                new[] { syntaxTree },
                _references,
                compilationOptions
            );

            var errorDiagnostics = initialCompilation.GetDiagnostics()
                .Where(d => d.Severity == DiagnosticSeverity.Error || d.Severity == DiagnosticSeverity.Warning)
                .ToList();

            if (errorDiagnostics.Any(d => d.Severity == DiagnosticSeverity.Error))
            {
                return BuildCompilationFailureResponse(errorDiagnostics, stopwatch);
            }

            // Inject cooperative cancellation checks into loops so infinite loops terminate cleanly on timeout
            var rewriter = new LoopCancellationRewriter();
            var instrumentedRoot = rewriter.Visit(syntaxTree.GetRoot());
            var instrumentedTree = CSharpSyntaxTree.Create((CSharpSyntaxNode)instrumentedRoot, parseOptions);

            var compilation = initialCompilation.ReplaceSyntaxTree(syntaxTree, instrumentedTree);

            using var ms = new MemoryStream();
            var emitResult = compilation.Emit(ms);

            if (!emitResult.Success)
            {
                return BuildCompilationFailureResponse(emitResult.Diagnostics, stopwatch);
            }

            ms.Seek(0, SeekOrigin.Begin);

            var alc = new IsolatedExecutionLoadContext();
            try
            {
                var assembly = alc.LoadFromStream(ms);
                var entryPoint = assembly.EntryPoint;

                if (entryPoint == null)
                {
                    stopwatch.Stop();
                    response.Success = false;
                    response.Error = "No valid entry point (Main method) found in compiled program.";
                    return response;
                }

                var rawOutputWriter = new StringWriter();
                var rawErrorWriter = new StringWriter();
                var outputWriter = TextWriter.Synchronized(rawOutputWriter);
                var errorWriter = TextWriter.Synchronized(rawErrorWriter);
                var inputReader = new StringReader(input ?? string.Empty);

                Exception? executionException = null;
                bool cancelledByTimeout = false;

                using var cts = new CancellationTokenSource(timeoutMs);

                AsyncOut.Value = outputWriter;
                AsyncError.Value = errorWriter;
                AsyncIn.Value = inputReader;
                AcademyRuntimeGuard.SetToken(cts.Token);

                var runTask = Task.Run(() =>
                {
                    try
                    {
                        object?[]? parameters = entryPoint.GetParameters().Length == 0
                            ? null
                            : new object?[] { Array.Empty<string>() };

                        var result = entryPoint.Invoke(null, parameters);
                        if (result is Task asyncTask)
                        {
                            asyncTask.Wait(cts.Token);
                        }
                    }
                    catch (TargetInvocationException ex)
                    {
                        var inner = ex.InnerException ?? ex;
                        if (inner is OperationCanceledException)
                        {
                            cancelledByTimeout = true;
                        }
                        else
                        {
                            executionException = inner;
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        cancelledByTimeout = true;
                    }
                    catch (Exception ex)
                    {
                        executionException = ex;
                    }
                });

                var completedTask = await Task.WhenAny(runTask, Task.Delay(timeoutMs));
                if (completedTask != runTask)
                {
                    cts.Cancel();
                    // Give the instrumented loop a brief window to observe cancellation and exit cleanly
                    await Task.WhenAny(runTask, Task.Delay(150));
                    cancelledByTimeout = true;
                }
                else
                {
                    await runTask;
                }

                stopwatch.Stop();

                if (cancelledByTimeout)
                {
                    response.TimedOut = true;
                    response.Success = false;
                    response.Error = $"Execution timed out after {timeoutMs}ms. Infinite loop or long running process detected.";
                }
                else if (executionException != null)
                {
                    response.Success = false;
                    response.Error = $"Runtime Exception: {executionException.GetType().Name}: {executionException.Message}\n{executionException.StackTrace}";
                }
                else
                {
                    response.Success = true;
                }

                response.Stdout = rawOutputWriter.ToString();
                response.Stderr = rawErrorWriter.ToString();
                response.Output = response.Stdout;
                if (!string.IsNullOrEmpty(response.Stderr))
                {
                    response.Output += (string.IsNullOrEmpty(response.Output) ? "" : "\n") + "[STDERR]: " + response.Stderr;
                }
                response.ExecutionTimeMs = stopwatch.ElapsedMilliseconds;

                return response;
            }
            finally
            {
                AsyncOut.Value = null;
                AsyncError.Value = null;
                AsyncIn.Value = null;
                AcademyRuntimeGuard.SetToken(CancellationToken.None);
                alc.Unload();
                GC.Collect();
            }
        }

        private static ExecuteResponse BuildCompilationFailureResponse(IEnumerable<Diagnostic> diagnostics, Stopwatch stopwatch)
        {
            stopwatch.Stop();
            var response = new ExecuteResponse
            {
                Success = false,
                ExecutionTimeMs = stopwatch.ElapsedMilliseconds
            };

            var errors = diagnostics
                .Where(d => d.Severity == DiagnosticSeverity.Error || d.Severity == DiagnosticSeverity.Warning)
                .Select(d =>
                {
                    var lineSpan = d.Location.GetMappedLineSpan();
                    return new CompilationError
                    {
                        Id = d.Id,
                        Severity = d.Severity.ToString(),
                        Message = d.GetMessage(),
                        Line = Math.Max(1, lineSpan.StartLinePosition.Line + 1),
                        Column = Math.Max(1, lineSpan.StartLinePosition.Character + 1)
                    };
                }).ToList();

            response.Diagnostics = errors;
            var sb = new StringBuilder("Compilation failed with errors:\n");
            foreach (var err in errors.Where(e => e.Severity == "Error"))
            {
                sb.AppendLine($"Line {err.Line}:{err.Column} [{err.Id}]: {err.Message}");
            }
            response.Error = sb.ToString();
            return response;
        }

        private static string NormalizeCode(string rawCode)
        {
            bool hasClass = rawCode.Contains("class ") || rawCode.Contains("record ");
            bool hasMain = rawCode.Contains("Main(") || rawCode.Contains("Main (");

            if (hasClass || hasMain)
            {
                var prefix = new StringBuilder();
                if (!rawCode.Contains("using System;")) prefix.AppendLine("using System;");
                if (!rawCode.Contains("using System.Collections.Generic;")) prefix.AppendLine("using System.Collections.Generic;");
                if (!rawCode.Contains("using System.Linq;")) prefix.AppendLine("using System.Linq;");
                if (!rawCode.Contains("using System.Threading.Tasks;")) prefix.AppendLine("using System.Threading.Tasks;");
                prefix.AppendLine("#line 1 \"Program.cs\"");
                return prefix + rawCode;
            }

            var lines = rawCode.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            var usings = new HashSet<string>
            {
                "using System;",
                "using System.Collections.Generic;",
                "using System.Linq;",
                "using System.Text;",
                "using System.Threading.Tasks;",
                "using System.IO;",
                "using System.Text.Json;"
            };

            var bodyLines = new List<string>();

            foreach (var line in lines)
            {
                string trimmed = line.Trim();
                if (trimmed.StartsWith("using ") && trimmed.EndsWith(";") && !trimmed.Contains("("))
                {
                    usings.Add(trimmed);
                    // Preserve exact line count so #line 1 maps every line 1:1 with user code
                    bodyLines.Add("// " + line);
                }
                else
                {
                    bodyLines.Add(line);
                }
            }

            var sb = new StringBuilder();
            foreach (var u in usings)
            {
                sb.AppendLine(u);
            }
            sb.AppendLine();
            sb.AppendLine("namespace Academy.InteractiveSandbox");
            sb.AppendLine("{");
            sb.AppendLine("    public class Program");
            sb.AppendLine("    {");
            sb.AppendLine("        public static async Task Main(string[] args)");
            sb.AppendLine("        {");
            sb.AppendLine("#line 1 \"Program.cs\"");
            foreach (var b in bodyLines)
            {
                sb.AppendLine(b);
            }
            sb.AppendLine("#line default");
            sb.AppendLine("            await Task.CompletedTask;");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            return sb.ToString();
        }

        private sealed class LoopCancellationRewriter : CSharpSyntaxRewriter
        {
            private static readonly StatementSyntax GuardCheckStatement =
                SyntaxFactory.ParseStatement("global::Academy.Api.Services.AcademyRuntimeGuard.Check();");

            private static BlockSyntax InjectGuard(StatementSyntax body)
            {
                if (body is BlockSyntax block)
                {
                    return block.WithStatements(block.Statements.Insert(0, GuardCheckStatement));
                }
                return SyntaxFactory.Block(GuardCheckStatement, body);
            }

            public override SyntaxNode? VisitWhileStatement(WhileStatementSyntax node)
            {
                var visited = (WhileStatementSyntax)base.VisitWhileStatement(node)!;
                return visited.WithStatement(InjectGuard(visited.Statement));
            }

            public override SyntaxNode? VisitForStatement(ForStatementSyntax node)
            {
                var visited = (ForStatementSyntax)base.VisitForStatement(node)!;
                return visited.WithStatement(InjectGuard(visited.Statement));
            }

            public override SyntaxNode? VisitDoStatement(DoStatementSyntax node)
            {
                var visited = (DoStatementSyntax)base.VisitDoStatement(node)!;
                return visited.WithStatement(InjectGuard(visited.Statement));
            }

            public override SyntaxNode? VisitForEachStatement(ForEachStatementSyntax node)
            {
                var visited = (ForEachStatementSyntax)base.VisitForEachStatement(node)!;
                return visited.WithStatement(InjectGuard(visited.Statement));
            }
        }

        private sealed class AsyncLocalTextWriter : TextWriter
        {
            private readonly Func<TextWriter> _targetProvider;
            public AsyncLocalTextWriter(Func<TextWriter> targetProvider) => _targetProvider = targetProvider;
            public override Encoding Encoding => Encoding.UTF8;
            public override void Write(char value) => _targetProvider().Write(value);
            public override void Write(string? value) => _targetProvider().Write(value);
            public override void WriteLine(string? value) => _targetProvider().WriteLine(value);
            public override void WriteLine() => _targetProvider().WriteLine();
            public override void Flush() => _targetProvider().Flush();
        }

        private sealed class AsyncLocalTextReader : TextReader
        {
            private readonly Func<TextReader> _targetProvider;
            public AsyncLocalTextReader(Func<TextReader> targetProvider) => _targetProvider = targetProvider;
            public override int Read() => _targetProvider().Read();
            public override int Peek() => _targetProvider().Peek();
            public override string? ReadLine() => _targetProvider().ReadLine();
            public override string ReadToEnd() => _targetProvider().ReadToEnd();
        }

        private class IsolatedExecutionLoadContext : AssemblyLoadContext
        {
            public IsolatedExecutionLoadContext() : base(isCollectible: true) { }

            protected override Assembly? Load(AssemblyName assemblyName)
            {
                return null;
            }
        }
    }
}
