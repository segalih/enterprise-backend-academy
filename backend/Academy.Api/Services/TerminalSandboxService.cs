using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Academy.Api.Services
{
    public class TerminalCommandRequest
    {
        public string SessionId { get; set; } = "default-session";
        public string CommandLine { get; set; } = string.Empty;
    }

    public class TerminalCommandResponse
    {
        public int ExitCode { get; set; }
        public string Output { get; set; } = string.Empty;
        public string CurrentDirectory { get; set; } = string.Empty;
        public long ExecutionTimeMs { get; set; }
    }

    public interface ITerminalSandboxService
    {
        Task<TerminalCommandResponse> ExecuteCommandAsync(TerminalCommandRequest request);
        void ResetSession(string sessionId);
    }

    public class TerminalSandboxService : ITerminalSandboxService
    {
        private static readonly string SandboxRoot = Path.Combine(Path.GetTempPath(), "academy_terminal_sandbox");
        private static readonly ConcurrentDictionary<string, string> SessionDirs = new();

        public TerminalSandboxService()
        {
            if (!Directory.Exists(SandboxRoot))
            {
                Directory.CreateDirectory(SandboxRoot);
            }
        }

        private static string SanitizeSessionId(string? sessionId)
        {
            if (string.IsNullOrWhiteSpace(sessionId)) return "default-session";
            var chars = sessionId.Where(c => char.IsLetterOrDigit(c) || c == '-' || c == '_').ToArray();
            string cleaned = new string(chars);
            return string.IsNullOrWhiteSpace(cleaned) ? "default-session" : cleaned;
        }

        private string GetSessionDirectory(string sessionId)
        {
            string safeId = SanitizeSessionId(sessionId);
            return SessionDirs.GetOrAdd(safeId, id =>
            {
                string dir = Path.GetFullPath(Path.Combine(SandboxRoot, $"session_{id}"));
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                    // Create an initial sample Program.cs and csproj
                    string initialCode = @"using System;

Console.WriteLine(""Enterprise Backend Engineering Terminal Ready."");
Console.WriteLine($""Runtime: .NET {Environment.Version} on {Environment.OSVersion}"");
";
                    File.WriteAllText(Path.Combine(dir, "Program.cs"), initialCode);
                    string initialCsproj = @"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
</Project>";
                    File.WriteAllText(Path.Combine(dir, "App.csproj"), initialCsproj);
                }
                return dir;
            });
        }

        public void ResetSession(string sessionId)
        {
            string safeId = SanitizeSessionId(sessionId);
            if (SessionDirs.TryRemove(safeId, out var dir) && Directory.Exists(dir))
            {
                try
                {
                    Directory.Delete(dir, true);
                }
                catch { }
            }
        }

        public async Task<TerminalCommandResponse> ExecuteCommandAsync(TerminalCommandRequest request)
        {
            var sw = Stopwatch.StartNew();
            string sessionDir = GetSessionDirectory(request.SessionId);
            string raw = (request.CommandLine ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(raw))
            {
                return new TerminalCommandResponse
                {
                    ExitCode = 0,
                    Output = string.Empty,
                    CurrentDirectory = "~/" + Path.GetFileName(sessionDir),
                    ExecutionTimeMs = 0
                };
            }

            // Built-in safe commands
            if (raw.Equals("clear", StringComparison.OrdinalIgnoreCase) || raw.Equals("cls", StringComparison.OrdinalIgnoreCase))
            {
                return new TerminalCommandResponse
                {
                    ExitCode = 0,
                    Output = "__CLEAR__",
                    CurrentDirectory = "~/" + Path.GetFileName(sessionDir)
                };
            }

            if (raw.Equals("reset", StringComparison.OrdinalIgnoreCase))
            {
                string safeId = SanitizeSessionId(request.SessionId);
                ResetSession(safeId);
                return new TerminalCommandResponse
                {
                    ExitCode = 0,
                    Output = "Sandbox workspace reset successfully. Ready for new project.",
                    CurrentDirectory = "~/session_" + safeId
                };
            }

            if (raw.Equals("help", StringComparison.OrdinalIgnoreCase))
            {
                var helpSb = new StringBuilder();
                helpSb.AppendLine("Enterprise Backend Academy — Dotnet Sandbox Terminal");
                helpSb.AppendLine("Allowed commands:");
                helpSb.AppendLine("  help                       : Show this list of allowed commands");
                helpSb.AppendLine("  dotnet --version           : Check .NET SDK version");
                helpSb.AppendLine("  dotnet --info              : Show detailed .NET runtime environment");
                helpSb.AppendLine("  dotnet new console -n App  : Scaffold a new console project");
                helpSb.AppendLine("  dotnet build               : Compile the C# project");
                helpSb.AppendLine("  dotnet run                 : Run the compiled application");
                helpSb.AppendLine("  dotnet test                : Execute test runner");
                helpSb.AppendLine("  ls / dir                   : List files in current directory");
                helpSb.AppendLine("  cat <file>                 : Display contents of a file");
                helpSb.AppendLine("  pwd                        : Print current working directory");
                helpSb.AppendLine("  reset                      : Clean and recreate sandbox directory");
                helpSb.AppendLine("  clear / cls                : Clear terminal screen");
                return new TerminalCommandResponse
                {
                    ExitCode = 0,
                    Output = helpSb.ToString(),
                    CurrentDirectory = "~/" + Path.GetFileName(sessionDir)
                };
            }

            if (raw.Equals("pwd", StringComparison.OrdinalIgnoreCase))
            {
                return new TerminalCommandResponse
                {
                    ExitCode = 0,
                    Output = "/workspace/" + Path.GetFileName(sessionDir),
                    CurrentDirectory = "~/" + Path.GetFileName(sessionDir)
                };
            }

            if (raw.Equals("ls", StringComparison.OrdinalIgnoreCase) || raw.Equals("dir", StringComparison.OrdinalIgnoreCase))
            {
                var files = Directory.GetFileSystemEntries(sessionDir);
                var sb = new StringBuilder();
                foreach (var f in files)
                {
                    bool isDir = Directory.Exists(f);
                    string name = Path.GetFileName(f);
                    if (isDir)
                    {
                        sb.AppendLine($"[DIR]  {name}/");
                    }
                    else
                    {
                        var info = new FileInfo(f);
                        sb.AppendLine($"[FILE] {name,-24} {info.Length,8} bytes");
                    }
                }
                return new TerminalCommandResponse
                {
                    ExitCode = 0,
                    Output = sb.ToString(),
                    CurrentDirectory = "~/" + Path.GetFileName(sessionDir)
                };
            }

            if (raw.StartsWith("cat ", StringComparison.OrdinalIgnoreCase) || raw.StartsWith("type ", StringComparison.OrdinalIgnoreCase))
            {
                string fileName = raw.Split(' ', 2)[1].Trim().Trim('"', '\'');
                string decodedFileName = Uri.UnescapeDataString(fileName);

                if (string.IsNullOrWhiteSpace(decodedFileName) ||
                    decodedFileName.Contains("..") ||
                    fileName.Contains('%') ||
                    Path.IsPathRooted(decodedFileName))
                {
                    return new TerminalCommandResponse
                    {
                        ExitCode = 1,
                        Output = $"cat: {fileName}: Access denied (path traversal outside sandbox is prohibited).",
                        CurrentDirectory = "~/" + Path.GetFileName(sessionDir)
                    };
                }

                string fullSessionDir = Path.GetFullPath(sessionDir).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;
                string targetPath = Path.GetFullPath(Path.Combine(sessionDir, decodedFileName));

                // Strict boundary and symlink check
                if (!targetPath.StartsWith(fullSessionDir, StringComparison.OrdinalIgnoreCase) ||
                    !File.Exists(targetPath) ||
                    (File.GetAttributes(targetPath) & FileAttributes.ReparsePoint) != 0)
                {
                    return new TerminalCommandResponse
                    {
                        ExitCode = 1,
                        Output = $"cat: {fileName}: No such file or file outside sandbox.",
                        CurrentDirectory = "~/" + Path.GetFileName(sessionDir)
                    };
                }

                string content = await File.ReadAllTextAsync(targetPath);
                return new TerminalCommandResponse
                {
                    ExitCode = 0,
                    Output = content,
                    CurrentDirectory = "~/" + Path.GetFileName(sessionDir)
                };
            }

            // Allowed external commands: dotnet whitelist only
            var allowedDotnetCommands = new[]
            {
                "dotnet --version",
                "dotnet --info",
                "dotnet build",
                "dotnet run",
                "dotnet test",
                "dotnet new console -n App",
                "dotnet new console"
            };

            if (!allowedDotnetCommands.Any(cmd => raw.Equals(cmd, StringComparison.OrdinalIgnoreCase)))
            {
                return new TerminalCommandResponse
                {
                    ExitCode = 1,
                    Output = $"Command '{raw}' is restricted in this isolated academy sandbox. Type 'help' to see allowed commands.",
                    CurrentDirectory = "~/" + Path.GetFileName(sessionDir)
                };
            }

            // Run dotnet command securely inside sessionDir
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = raw.Substring("dotnet".Length).Trim(),
                    WorkingDirectory = sessionDir,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var process = new Process { StartInfo = psi };
                var outputBuilder = new StringBuilder();
                var errorBuilder = new StringBuilder();

                process.OutputDataReceived += (s, e) => { if (e.Data != null) outputBuilder.AppendLine(e.Data); };
                process.ErrorDataReceived += (s, e) => { if (e.Data != null) errorBuilder.AppendLine(e.Data); };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                bool finished = await Task.Run(() => process.WaitForExit(10000));
                sw.Stop();

                if (!finished)
                {
                    try { process.Kill(true); } catch { }
                    return new TerminalCommandResponse
                    {
                        ExitCode = -1,
                        Output = "Command timed out after 10000ms and was terminated for safety.",
                        CurrentDirectory = "~/" + Path.GetFileName(sessionDir),
                        ExecutionTimeMs = sw.ElapsedMilliseconds
                    };
                }

                string outText = outputBuilder.ToString();
                string errText = errorBuilder.ToString();

                string combined = outText;
                if (!string.IsNullOrEmpty(errText))
                {
                    combined = (string.IsNullOrEmpty(combined) ? "" : combined + "\n") + errText;
                }

                return new TerminalCommandResponse
                {
                    ExitCode = process.ExitCode,
                    Output = combined.TrimEnd(),
                    CurrentDirectory = "~/" + Path.GetFileName(sessionDir),
                    ExecutionTimeMs = sw.ElapsedMilliseconds
                };
            }
            catch (Exception ex)
            {
                return new TerminalCommandResponse
                {
                    ExitCode = 1,
                    Output = $"Execution error: {ex.Message}",
                    CurrentDirectory = "~/" + Path.GetFileName(sessionDir)
                };
            }
        }
    }
}
