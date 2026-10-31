using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Academy.Api.Data.Curriculum;
using Academy.Api.Services;
using Academy.Api.Models;

namespace Academy.Tests
{
    public class CodeExecutionServiceTests
    {
        private readonly CodeExecutionService _service = new();

        [Fact]
        public async Task ExecuteAsync_SimpleConsoleWriteLine_ReturnsExpectedOutput()
        {
            string code = @"using System;
Console.WriteLine(""Halo Enterprise Backend Academy!"");";

            var result = await _service.ExecuteAsync(code);

            Assert.True(result.Success, $"Execution failed with error: {result.Error} | Output: {result.Output}");
            Assert.Contains("Halo Enterprise Backend Academy!", result.Output);
        }

        [Fact]
        public async Task ExecuteAsync_SyntaxError_ReturnsDiagnostics()
        {
            string code = @"using System;
Console.WriteLine(""Missing semicolon"")";

            var result = await _service.ExecuteAsync(code);

            Assert.False(result.Success);
            Assert.NotEmpty(result.Diagnostics);
            Assert.Contains("Compilation failed", result.Error);
        }

        [Fact]
        public async Task ExecuteAsync_InfiniteLoop_TimesOutSafely()
        {
            string code = @"using System;
while (true) { }";

            var result = await _service.ExecuteAsync(code, timeoutMs: 1500);

            Assert.False(result.Success);
            Assert.True(result.TimedOut);
            Assert.Contains("timed out", result.Error);
        }

        [Fact]
        public async Task ExecuteAsync_LINQCalculation_EvaluatesCorrectly()
        {
            string code = @"using System;
using System.Linq;

int[] numbers = { 10, 20, 30, 40 };
int sum = numbers.Where(n => n > 15).Sum();
Console.WriteLine($""Total: {sum}"");";

            var result = await _service.ExecuteAsync(code);

            Assert.True(result.Success);
            Assert.Contains("Total: 90", result.Output);
        }

        [Fact]
        public async Task ExecuteAsync_AfterInfiniteLoopTimeout_SubsequentExecutionSucceedsQuickly()
        {
            var loopResult = await _service.ExecuteAsync("while (true) { }", timeoutMs: 800);
            Assert.False(loopResult.Success);
            Assert.True(loopResult.TimedOut);

            var normalResult = await _service.ExecuteAsync(@"Console.WriteLine(""Still alive after loop!"");", timeoutMs: 1500);
            Assert.True(normalResult.Success, $"Expected success after loop timeout, got error: {normalResult.Error}");
            Assert.False(normalResult.TimedOut);
            Assert.Contains("Still alive after loop!", normalResult.Output);
        }

        [Fact]
        public async Task ExecuteAsync_ConsoleReadLineWithNullOrEmptyInput_DoesNotHangAndReturnsNull()
        {
            string code = @"var line = Console.ReadLine();
Console.WriteLine(""ReadResult:"" + (line ?? ""EOF""));";

            var result = await _service.ExecuteAsync(code, input: null, timeoutMs: 1500);

            Assert.True(result.Success, $"ReadLine with null input failed/timed out: {result.Error}");
            Assert.False(result.TimedOut);
            Assert.Contains("ReadResult:EOF", result.Output);
        }

        [Fact]
        public async Task ExecuteAsync_DiagnosticLineAndColumn_MatchesUserSourceCodeLine()
        {
            string code = @"int x = ""wrong_type"";";

            var result = await _service.ExecuteAsync(code);

            Assert.False(result.Success);
            Assert.NotEmpty(result.Diagnostics);
            Assert.Equal(1, result.Diagnostics[0].Line);
            Assert.Equal(9, result.Diagnostics[0].Column);
        }

        [Theory]
        [InlineData("Environment.Exit(1);")]
        [InlineData("System.Diagnostics.Process.Start(\"cmd.exe\");")]
        [InlineData("System.IO.File.Delete(\"academy.db\");")]
        public async Task ExecuteAsync_DangerousApis_AreBlockedBySandbox(string dangerousCode)
        {
            var result = await _service.ExecuteAsync(dangerousCode, timeoutMs: 1500);

            Assert.False(result.Success);
            Assert.NotNull(result.Error);
        }

        [Fact]
        public async Task ExecuteAsync_ParallelRequests_IsolateConsoleStdoutCorrectly()
        {
            var tasks = new Task<ExecuteResponse>[6];
            for (int i = 0; i < tasks.Length; i++)
            {
                int idx = i;
                tasks[i] = _service.ExecuteAsync($@"
for (int j = 0; j < 5; j++) {{
    Console.WriteLine(""TOKEN_{idx}_"" + j);
    await Task.Delay(10);
}}", timeoutMs: 4000);
            }

            var results = await Task.WhenAll(tasks);
            for (int i = 0; i < results.Length; i++)
            {
                Assert.True(results[i].Success, $"Parallel task {i} failed: {results[i].Error}");
                Assert.Contains($"TOKEN_{i}_0", results[i].Output);
                Assert.Contains($"TOKEN_{i}_4", results[i].Output);
                for (int other = 0; other < results.Length; other++)
                {
                    if (other != i)
                    {
                        Assert.DoesNotContain($"TOKEN_{other}_", results[i].Output);
                    }
                }
            }
        }
    }

    public class CurriculumServiceTests
    {
        private readonly CurriculumService _service = new();

        [Fact]
        public void GetAllLevelSummaries_ReturnsAll18MasterLevels()
        {
            var levels = _service.GetAllLevelSummaries();

            Assert.NotNull(levels);
            Assert.True(levels.Count >= 18, $"Expected at least 18 levels, but got {levels.Count}");
            Assert.Equal("level-00", levels[0].Id);
            Assert.Equal("level-18", levels[18].Id);
        }

        [Fact]
        public void GetLessonDetail_ReturnsSubstantialIndonesianTechnicalDocumentation()
        {
            var detail = _service.GetLessonDetail("lesson-02-04");

            Assert.NotNull(detail);
            Assert.Contains("Value Types vs Reference Types", detail.Title);
            Assert.NotEmpty(detail.MarkdownContent);
            Assert.NotEmpty(detail.QuizQuestions);
            Assert.NotNull(detail.CodingChallenge);
        }

        [Fact]
        public void Search_MatchesKeywordsAcrossCurriculum()
        {
            var results = _service.Search("Stack");

            Assert.NotNull(results);
            Assert.NotEmpty(results);
            Assert.Contains(results, r => r.Title.Contains("Stack", System.StringComparison.OrdinalIgnoreCase) || r.MatchContext.Contains("Stack", System.StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public void GetAllProjects_ReturnsAll12EnterpriseProjects()
        {
            var projects = _service.GetAllProjects();

            Assert.NotNull(projects);
            Assert.Equal(12, projects.Count);
            for (int i = 1; i <= 12; i++)
            {
                string expectedId = $"proj-{i:D2}";
                Assert.Contains(projects, p => p.Id == expectedId && p.Order == i && !string.IsNullOrWhiteSpace(p.StarterCode));
            }
            Assert.Contains(projects, p => p.Id == "proj-12" && p.Title.Contains("Banking"));
        }

        [Fact]
        public void GetLessonDetail_InvalidLessonId_ReturnsNull_AndControllerReturns404()
        {
            var detail = _service.GetLessonDetail("invalid-id-999");
            Assert.Null(detail);

            var controller = new Academy.Api.Controllers.CurriculumController(_service);
            var actionResult = controller.GetLessonDetail("invalid-id-999");
            Assert.IsType<Microsoft.AspNetCore.Mvc.NotFoundObjectResult>(actionResult);
        }

        [Fact]
        public void CurriculumCatalog_HasNoDuplicateLessonIdsAndNoNullContent()
        {
            var levels = CurriculumCatalog.GetAllLevels();
            Assert.Equal(19, levels.Count); // level-00 through level-18

            var seenLessonIds = new System.Collections.Generic.HashSet<string>();
            foreach (var lvl in levels)
            {
                Assert.NotEmpty(lvl.Modules);
                foreach (var mod in lvl.Modules)
                {
                    Assert.NotEmpty(mod.Lessons);
                    foreach (var les in mod.Lessons)
                    {
                        Assert.True(seenLessonIds.Add(les.Id), $"Duplicate lesson ID found: {les.Id}");
                        var detail = _service.GetLessonDetail(les.Id);
                        Assert.NotNull(detail);
                        Assert.False(string.IsNullOrWhiteSpace(detail!.MarkdownContent));
                        Assert.False(string.IsNullOrWhiteSpace(detail.StarterCode));
                    }
                }
            }
        }

        [Fact]
        public void CurriculumContent_NoBoilerplateSectionsAndHighDiversity()
        {
            var levels = CurriculumCatalog.GetAllLevels();
            var allLessonIds = levels.SelectMany(l => l.Modules.SelectMany(m => m.Lessons.Select(les => les.Id))).ToList();
            Assert.Equal(88, allLessonIds.Count);

            var forbidden = new[]
            {
                "Lesson Title & Learning Objectives",
                "Historical Context & Mengapa Teknologi Ini Ada",
                "Konsep Fundamental & Landasan Teori",
                "Mekanisme Internal .NET Runtime & OS",
                "Contoh Implementasi C# Enterprise",
                "Best Practices vs Anti-Patterns",
                "Pertimbangan Keamanan & Performa",
                "Ringkasan & Referensi Lanjutan"
            };

            var lessonWords = new List<(string Id, HashSet<string> Words)>();

            foreach (var id in allLessonIds)
            {
                var detail = _service.GetLessonDetail(id);
                Assert.NotNull(detail);

                foreach (var phrase in forbidden)
                {
                    Assert.DoesNotContain(phrase, detail!.MarkdownContent);
                }

                var words = new HashSet<string>(
                    detail!.MarkdownContent
                        .ToLowerInvariant()
                        .Split(new[] { ' ', '\r', '\n', '\t', '#', '-', '*', '`', '[', ']', '(', ')' }, StringSplitOptions.RemoveEmptyEntries)
                        .Where(w => w.Length > 3)
                );
                lessonWords.Add((id, words));
            }

            // Verify Jaccard similarity across sample pairs is low (< 40%)
            for (int i = 0; i < Math.Min(lessonWords.Count, 25); i++)
            {
                for (int j = i + 1; j < Math.Min(lessonWords.Count, 25); j++)
                {
                    var a = lessonWords[i].Words;
                    var b = lessonWords[j].Words;
                    var intersection = a.Count(w => b.Contains(w));
                    var union = a.Count + b.Count - intersection;
                    double jaccard = (double)intersection / union;
                    Assert.True(jaccard < 0.40, $"Similarity between {lessonWords[i].Id} and {lessonWords[j].Id} too high: {jaccard:P1}");
                }
            }
        }

        [Fact]
        public void CurriculumQuizzes_HaveValidOptionsAndExplanations()
        {
            var levels = CurriculumCatalog.GetAllLevels();
            var allLessonIds = levels.SelectMany(l => l.Modules.SelectMany(m => m.Lessons.Select(les => les.Id))).ToList();

            foreach (var id in allLessonIds)
            {
                var detail = _service.GetLessonDetail(id);
                Assert.NotNull(detail);

                if (detail!.HasQuiz)
                {
                    Assert.NotEmpty(detail.QuizQuestions);
                    foreach (var q in detail.QuizQuestions)
                    {
                        Assert.InRange(q.Options.Count, 2, 6);
                        Assert.InRange(q.CorrectOptionIndex, 0, q.Options.Count - 1);
                        Assert.False(string.IsNullOrWhiteSpace(q.Explanation));
                        if (q.OptionExplanations.Count > 0)
                        {
                            Assert.Equal(q.Options.Count, q.OptionExplanations.Count);
                        }
                    }
                }
            }
        }

        [Fact]
        public void CurriculumSimulators_MappedOnlyToValidTypes()
        {
            var levels = CurriculumCatalog.GetAllLevels();
            var allLessonIds = levels.SelectMany(l => l.Modules.SelectMany(m => m.Lessons.Select(les => les.Id))).ToList();
            var allowedSimulators = new HashSet<string?> { null, "binary", "stack-heap", "gc", "btree", "deadlock", "async-flow" };

            foreach (var id in allLessonIds)
            {
                var detail = _service.GetLessonDetail(id);
                Assert.NotNull(detail);
                Assert.Contains(detail!.SimulatorType, allowedSimulators);
            }
        }

        [Fact]
        public void CurriculumContent_HasNoRawDollarMathDelimiters()
        {
            var levels = CurriculumCatalog.GetAllLevels();
            var allLessonIds = levels.SelectMany(l => l.Modules.SelectMany(m => m.Lessons.Select(les => les.Id))).ToList();

            var mathRegex = new Regex(@"(?<!\\)\$[^$\r\n]+\$", RegexOptions.Compiled);
            var violations = new List<string>();

            foreach (var id in allLessonIds)
            {
                var detail = _service.GetLessonDetail(id);
                Assert.NotNull(detail);

                // Strip fenced code blocks
                string prose = Regex.Replace(detail!.MarkdownContent ?? "", @"```[\s\S]*?```", "");
                // Strip inline code spans
                prose = Regex.Replace(prose, @"`[^`\r\n]*`", "");

                var matches = mathRegex.Matches(prose);
                foreach (Match m in matches)
                {
                    violations.Add($"Lesson {id} in markdownContent: '{m.Value}'");
                }

                if (detail.QuizQuestions != null)
                {
                    foreach (var q in detail.QuizQuestions)
                    {
                        string qText = Regex.Replace((q.Question ?? "") + " " + (q.Explanation ?? "") + " " + string.Join(" ", q.Options), @"`[^`\r\n]*`", "");
                        var qMatches = mathRegex.Matches(qText);
                        foreach (Match m in qMatches)
                        {
                            violations.Add($"Lesson {id} in quiz question/options: '{m.Value}'");
                        }
                    }
                }
            }

            Assert.True(violations.Count == 0, $"Found {violations.Count} raw math dollar expressions outside code blocks:\n" + string.Join("\n", violations));
        }

        [Fact]
        public void CurriculumMermaidDiagrams_AreWellFormedAndRecognized()
        {
            var levels = CurriculumCatalog.GetAllLevels();
            var allLessonIds = levels.SelectMany(l => l.Modules.SelectMany(m => m.Lessons.Select(les => les.Id))).ToList();

            var mermaidRegex = new Regex(@"```mermaid\r?\n([\s\S]*?)```", RegexOptions.Compiled);
            int totalMermaidDiagrams = 0;
            var recognizedTypes = new[] { "flowchart", "sequenceDiagram", "graph", "classDiagram", "stateDiagram", "erDiagram" };

            foreach (var id in allLessonIds)
            {
                var detail = _service.GetLessonDetail(id);
                Assert.NotNull(detail);

                var matches = mermaidRegex.Matches(detail!.MarkdownContent ?? "");
                foreach (Match m in matches)
                {
                    totalMermaidDiagrams++;
                    string diagramBody = m.Groups[1].Value.Trim();
                    Assert.False(string.IsNullOrWhiteSpace(diagramBody), $"Empty mermaid diagram found in lesson {id}");
                    bool startsWithValidType = recognizedTypes.Any(t => diagramBody.StartsWith(t, StringComparison.OrdinalIgnoreCase));
                    Assert.True(startsWithValidType, $"Unrecognized mermaid diagram type in lesson {id}: {diagramBody.Split('\n')[0]}");
                }
            }

            Assert.True(totalMermaidDiagrams >= 50, $"Expected at least 50 mermaid diagrams across curriculum, but found {totalMermaidDiagrams}");
        }

        [Fact]
        public void CurriculumContent_NoUnallowedAsciiDiagramBlocksExist()
        {
            // Explicit allowlist of non-Mermaid blocks that legitimately remain monospace:
            // Key: lessonId, Value: reason why it must remain monospace
            var allowedMonospaceDiagrams = new System.Collections.Generic.Dictionary<string, string>(System.StringComparer.OrdinalIgnoreCase)
            {
                ["lesson-01-03"] = "Pseudocode algoritma transfer dana perbankan dengan tanda panah penugasan variabel (<-).",
                ["lesson-03-03"] = "Presisi tata letak memori byte Heap (SyncBlock 8-byte, MethodTable 8-byte, slot VTable pointer).",
                ["lesson-05-01"] = "Plot kurva sumbu koordinat kartesius X-Y kompleksitas Big-O.",
                ["lesson-07-03"] = "Tabel perbandingan tabular monospaced HTTP/1.1 vs HTTP/2 vs HTTP/3.",
                ["lesson-13-02"] = "Struktur hierarki folder directory tree (Features/ Orders/ CreateOrder...).",
                ["lesson-17-01"] = "Posisi karakter byte header format W3C traceparent (version, trace-id, parent-id, flags).",
                ["lesson-17-03"] = "Representasi visual teks flamegraph CPU Speedscope."
            };

            var levels = CurriculumCatalog.GetAllLevels();
            var allLessonIds = levels.SelectMany(l => l.Modules.SelectMany(m => m.Lessons.Select(les => les.Id))).ToList();

            var codeBlockRegex = new Regex(@"```([a-zA-Z0-9_\-\+]*)[^\n]*\r?\n([\s\S]*?)```", RegexOptions.Compiled);
            var violations = new System.Collections.Generic.List<string>();

            // Heuristics for ASCII diagrams: +---+ boxes, box-drawing characters, arrows
            var boxRegex = new Regex(@"\+[-=]{2,}\+", RegexOptions.Compiled);
            var boxDrawingRegex = new Regex(@"[┌─│└┐┘├┤┬┴┼]", RegexOptions.Compiled);
            var arrowRegex = new Regex(@"(-->|--->|<--|<---|==>|====>|->|<-|▼|▲|──>)", RegexOptions.Compiled);
            var verticalArrowRegex = new Regex(@"(\|\s*\r?\n\s*v|\s+v\s*\r?\n\s+\|)", RegexOptions.Compiled);

            foreach (var id in allLessonIds)
            {
                var detail = _service.GetLessonDetail(id);
                Assert.NotNull(detail);

                var matches = codeBlockRegex.Matches(detail!.MarkdownContent ?? "");
                int blockIdx = 0;
                foreach (Match m in matches)
                {
                    blockIdx++;
                    string lang = m.Groups[1].Value.Trim().ToLowerInvariant();
                    string code = m.Groups[2].Value.Trim();

                    // Skip mermaid blocks (already valid SVG)
                    if (lang == "mermaid") continue;

                    // Skip known non-diagram code blocks (csharp, bash, sql, json, dockerfile, yaml)
                    // unless they contain explicit ASCII boxes with diagram arrows
                    if (lang == "csharp" || lang == "cs" || lang == "sql" || lang == "bash" || lang == "sh" || lang == "json" || lang == "dockerfile" || lang == "yaml")
                    {
                        continue;
                    }

                    bool hasBox = boxRegex.IsMatch(code);
                    bool hasBoxDrawing = boxDrawingRegex.IsMatch(code);
                    bool hasArrows = arrowRegex.IsMatch(code);
                    bool hasVerticalArrow = verticalArrowRegex.IsMatch(code);

                    if (hasBox || hasBoxDrawing || (hasArrows && (code.Contains('|') || hasVerticalArrow)))
                    {
                        if (!allowedMonospaceDiagrams.ContainsKey(id))
                        {
                            violations.Add($"Lesson {id} (Block #{blockIdx}) contains unallowed ASCII diagram:\n{code.Substring(0, System.Math.Min(code.Length, 120))}...");
                        }
                    }
                }
            }

            Assert.True(violations.Count == 0, $"Found {violations.Count} unallowed ASCII diagrams outside explicit allowlist:\n" + string.Join("\n\n", violations));
        }
    }

    public class TerminalSandboxTests
    {
        private readonly TerminalSandboxService _service = new();

        [Fact]
        public async Task ExecuteCommand_HelpCommand_ReturnsAllowedCommands()
        {
            var request = new TerminalCommandRequest
            {
                SessionId = "test-session",
                CommandLine = "help"
            };

            var response = await _service.ExecuteCommandAsync(request);

            Assert.Equal(0, response.ExitCode);
            Assert.Contains("dotnet", response.Output);
            Assert.Contains("help", response.Output);
        }

        [Fact]
        public async Task ExecuteCommand_RestrictedCommand_IsBlockedSafely()
        {
            var request = new TerminalCommandRequest
            {
                SessionId = "test-session",
                CommandLine = "rmdir /s /q C:\\"
            };

            var response = await _service.ExecuteCommandAsync(request);

            Assert.Equal(1, response.ExitCode);
            Assert.Contains("restricted", response.Output);
        }

        [Fact]
        public async Task ExecuteCommand_SessionIdPathTraversal_AndSiblingSessionCat_AreBlocked()
        {
            var traversalRes = await _service.ExecuteCommandAsync(new TerminalCommandRequest
            {
                SessionId = "../../outside_escape",
                CommandLine = "pwd"
            });
            Assert.DoesNotContain("/workspace/outside_escape", traversalRes.Output);
            Assert.DoesNotContain("..", traversalRes.CurrentDirectory);

            // Create sibling session 'sec2'
            await _service.ExecuteCommandAsync(new TerminalCommandRequest
            {
                SessionId = "sec2",
                CommandLine = "pwd"
            });

            // Attempt to read '../session_sec2/Program.cs' from session 'sec'
            var siblingCat = await _service.ExecuteCommandAsync(new TerminalCommandRequest
            {
                SessionId = "sec",
                CommandLine = "cat ../session_sec2/Program.cs"
            });
            Assert.NotEqual(0, siblingCat.ExitCode);
            Assert.DoesNotContain("Enterprise Backend Engineering Terminal Ready", siblingCat.Output);

            // Attempt URL-encoded path traversal
            var urlEncodedCat = await _service.ExecuteCommandAsync(new TerminalCommandRequest
            {
                SessionId = "sec",
                CommandLine = "cat ..%2Fsession_sec2%2FProgram.cs"
            });
            Assert.NotEqual(0, urlEncodedCat.ExitCode);
        }
    }

    public class ProgressControllerTests
    {
        static ProgressControllerTests()
        {
            var lessonsDir = Academy.Api.Data.Curriculum.CurriculumContentProvider.GetLessonsDirectory();
            if (System.IO.Directory.Exists(lessonsDir))
            {
                Academy.Tests.Seeders.ChallengeMasterCatalog.SyncToLessonJsonFiles(lessonsDir);
            }

            var srcDir = System.IO.Path.GetFullPath(System.IO.Path.Combine(System.AppContext.BaseDirectory, "../../../../Academy.Api/Content/lessons"));
            if (System.IO.Directory.Exists(srcDir) && !string.Equals(srcDir, lessonsDir, System.StringComparison.OrdinalIgnoreCase))
            {
                Academy.Tests.Seeders.ChallengeMasterCatalog.SyncToLessonJsonFiles(srcDir);
            }

            Academy.Api.Data.Curriculum.CurriculumContentProvider.ClearCache();
        }

        private Academy.Api.Data.AcademyDbContext CreateInMemoryDbContext()
        {
            var options = new Microsoft.EntityFrameworkCore.DbContextOptionsBuilder<Academy.Api.Data.AcademyDbContext>()
                .UseSqlite($"Data Source=test_{System.Guid.NewGuid():N}.db")
                .Options;
            var ctx = new Academy.Api.Data.AcademyDbContext(options);
            ctx.Database.EnsureCreated();
            return ctx;
        }

        [Fact]
        public async Task ToggleCompleted_TogglesState_AndPreventsInfiniteXp()
        {
            using var db = CreateInMemoryDbContext();
            var controller = new Academy.Api.Controllers.ProgressController(db);
            string userId = "test-learner-1";
            db.Users.Add(new Academy.Api.Data.Entities.LearnerUser { Id = userId, Xp = 100 });
            await db.SaveChangesAsync();

            // First click: Complete -> XP +25 = 125
            await controller.MarkCompleted("lesson-00-01", userId);
            var userAfterFirst = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(db.Users, u => u.Id == userId);
            Assert.Equal(125, userAfterFirst!.Xp);

            // Second click: Uncomplete -> XP -25 = 100 (No point inflation!)
            await controller.MarkCompleted("lesson-00-01", userId);
            var userAfterSecond = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(db.Users, u => u.Id == userId);
            Assert.Equal(100, userAfterSecond!.Xp);
        }

        [Fact]
        public async Task RecordVisit_UpdatesLastVisitedLesson_ReturnedInDashboard()
        {
            using var db = CreateInMemoryDbContext();
            var controller = new Academy.Api.Controllers.ProgressController(db);
            string userId = "test-learner-2";
            db.Users.Add(new Academy.Api.Data.Entities.LearnerUser { Id = userId });
            await db.SaveChangesAsync();

            // Visit lesson-05-03
            await controller.RecordVisit("lesson-05-03", userId);

            var dashboardResult = await controller.GetDashboard(userId) as Microsoft.AspNetCore.Mvc.OkObjectResult;
            Assert.NotNull(dashboardResult);
            var dash = dashboardResult.Value as Academy.Api.Controllers.DashboardResponse;
            Assert.NotNull(dash);
            Assert.Equal("lesson-05-03", dash.LastVisitedLessonId);
        }

        [Fact]
        public async Task ToggleBookmark_DoesNotMarkLessonAsCompleted()
        {
            using var db = CreateInMemoryDbContext();
            var controller = new Academy.Api.Controllers.ProgressController(db);
            string userId = "test-learner-3";
            db.Users.Add(new Academy.Api.Data.Entities.LearnerUser { Id = userId });
            await db.SaveChangesAsync();

            // Toggle bookmark on lesson-02-04
            await controller.ToggleBookmark("lesson-02-04", userId);

            var dashboardResult = await controller.GetDashboard(userId) as Microsoft.AspNetCore.Mvc.OkObjectResult;
            var dash = dashboardResult!.Value as Academy.Api.Controllers.DashboardResponse;
            
            Assert.Contains("lesson-02-04", dash!.BookmarkedLessonIds);
            Assert.DoesNotContain("lesson-02-04", dash.CompletedLessonIds);
            Assert.Equal(0, dash.TotalLessonsCompleted);
        }

        [Fact]
        public async Task SubmitQuiz_PersistsAttempt_AndMarksLessonCompleted()
        {
            using var db = CreateInMemoryDbContext();
            var quizController = new Academy.Api.Controllers.QuizController(db);
            var progressController = new Academy.Api.Controllers.ProgressController(db);
            string userId = "quiz-learner";

            var lesson = CurriculumContentProvider.GetLessonDetail("lesson-02-04")!;
            var answers = new System.Collections.Generic.Dictionary<string, int>();
            foreach (var q in lesson.QuizQuestions)
            {
                answers[q.Id] = q.CorrectOptionIndex;
            }

            var res = await quizController.SubmitQuiz(new Academy.Api.Controllers.QuizSubmitRequest
            {
                UserId = userId,
                LessonId = "lesson-02-04",
                Answers = answers
            }) as Microsoft.AspNetCore.Mvc.OkObjectResult;

            Assert.NotNull(res);
            var payload = res!.Value as Academy.Api.Controllers.QuizSubmitResponse;
            Assert.NotNull(payload);
            Assert.True(payload!.Passed);
            Assert.Equal(100, payload.Score);

            var attemptsCount = await db.QuizAttempts.CountAsync(a => a.UserId == userId && a.LessonId == "lesson-02-04");
            Assert.Equal(1, attemptsCount);

            var dashRes = await progressController.GetDashboard(userId) as Microsoft.AspNetCore.Mvc.OkObjectResult;
            var dash = dashRes!.Value as Academy.Api.Controllers.DashboardResponse;
            Assert.Contains("lesson-02-04", dash!.CompletedLessonIds);
        }

        [Fact]
        public async Task EnsureSchemaAndSeed_UpgradesLegacySqliteDatabaseMissingLastVisitedLessonId()
        {
            string dbPath = $"legacy_{System.Guid.NewGuid():N}.db";
            try
            {
                var options = new Microsoft.EntityFrameworkCore.DbContextOptionsBuilder<Academy.Api.Data.AcademyDbContext>()
                    .UseSqlite($"Data Source={dbPath}")
                    .Options;

                using (var db = new Academy.Api.Data.AcademyDbContext(options))
                {
                    // Create legacy Users table without LastVisitedLessonId column
                    await db.Database.ExecuteSqlRawAsync(@"
                        CREATE TABLE ""Users"" (
                            ""Id"" TEXT NOT NULL CONSTRAINT ""PK_Users"" PRIMARY KEY,
                            ""Username"" TEXT NOT NULL,
                            ""DisplayName"" TEXT NOT NULL,
                            ""Email"" TEXT NOT NULL,
                            ""CurrentStreak"" INTEGER NOT NULL,
                            ""BestStreak"" INTEGER NOT NULL,
                            ""Xp"" INTEGER NOT NULL,
                            ""CreatedAt"" TEXT NOT NULL,
                            ""LastActiveDate"" TEXT NOT NULL
                        );
                        CREATE TABLE ""Progress"" (
                            ""Id"" INTEGER NOT NULL CONSTRAINT ""PK_Progress"" PRIMARY KEY AUTOINCREMENT,
                            ""UserId"" TEXT NOT NULL,
                            ""LessonId"" TEXT NOT NULL,
                            ""Completed"" INTEGER NOT NULL,
                            ""QuizScore"" INTEGER NOT NULL,
                            ""SavedCode"" TEXT NULL,
                            ""CompletedAt"" TEXT NULL,
                            ""LastVisitedAt"" TEXT NOT NULL
                        );
                        CREATE TABLE ""Bookmarks"" (
                            ""Id"" INTEGER NOT NULL CONSTRAINT ""PK_Bookmarks"" PRIMARY KEY AUTOINCREMENT,
                            ""UserId"" TEXT NOT NULL,
                            ""LessonId"" TEXT NOT NULL,
                            ""CreatedAt"" TEXT NOT NULL
                        );
                    ");

                    Academy.Api.Data.AcademyDbContext.EnsureSchemaAndSeed(db);

                    var controller = new Academy.Api.Controllers.ProgressController(db);
                    var actionResult = await controller.GetDashboard("default-learner") as Microsoft.AspNetCore.Mvc.OkObjectResult;
                    Assert.NotNull(actionResult);
                    var dash = actionResult!.Value as Academy.Api.Controllers.DashboardResponse;
                    Assert.NotNull(dash);
                    Assert.Equal("default-learner", dash!.UserId);
                    Assert.Equal("lesson-00-01", dash.LastVisitedLessonId);
                }
            }
            finally
            {
                try { if (System.IO.File.Exists(dbPath)) System.IO.File.Delete(dbPath); } catch { }
            }
        }

        [Fact]
        public async Task SubmitChallenge_InvalidChallengeId_ReturnsNotFound()
        {
            var execService = new CodeExecutionService();
            var currService = new CurriculumService();
            var controller = new Academy.Api.Controllers.ExecutionController(execService, currService);

            var actionResult = await controller.SubmitChallenge(new ChallengeSubmitRequest
            {
                ChallengeId = "non-existent-challenge-999",
                Code = "using System; Console.WriteLine(123);"
            });

            Assert.IsType<Microsoft.AspNetCore.Mvc.NotFoundObjectResult>(actionResult);
        }

        [Fact]
        public void CurriculumContent_SyncAndVerifyCodingChallengesContract()
        {
            var lessonsDir = Academy.Api.Data.Curriculum.CurriculumContentProvider.GetLessonsDirectory();
            if (System.IO.Directory.Exists(lessonsDir))
            {
                Academy.Tests.Seeders.ChallengeMasterCatalog.SyncToLessonJsonFiles(lessonsDir);
            }

            var srcDir = System.IO.Path.GetFullPath(System.IO.Path.Combine(AppContext.BaseDirectory, "../../../../Academy.Api/Content/lessons"));
            if (System.IO.Directory.Exists(srcDir) && !string.Equals(srcDir, lessonsDir, StringComparison.OrdinalIgnoreCase))
            {
                Academy.Tests.Seeders.ChallengeMasterCatalog.SyncToLessonJsonFiles(srcDir);
            }

            Academy.Api.Data.Curriculum.CurriculumContentProvider.ClearCache();

            var allLevels = Academy.Api.Data.Curriculum.CurriculumCatalog.GetAllLevels();
            var allLessons = allLevels.SelectMany(l => l.Modules.SelectMany(m => m.Lessons)).ToList();

            Assert.Equal(88, allLessons.Count);

            int nonCodingCount = 0;
            int codingCount = 0;

            foreach (var summary in allLessons)
            {
                var detail = Academy.Api.Data.Curriculum.CurriculumContentProvider.GetLessonDetail(summary.Id);
                Assert.NotNull(detail);

                if (!summary.HasExercise)
                {
                    nonCodingCount++;
                    Assert.Null(detail!.CodingChallenge);
                }
                else
                {
                    codingCount++;
                    var ch = detail!.CodingChallenge;
                    Assert.NotNull(ch);
                    Assert.False(string.IsNullOrWhiteSpace(ch!.Title), $"Challenge in {summary.Id} missing Title");
                    Assert.False(string.IsNullOrWhiteSpace(ch.ProblemStatement), $"Challenge in {summary.Id} missing ProblemStatement");
                    Assert.False(string.IsNullOrWhiteSpace(ch.InputFormat), $"Challenge in {summary.Id} missing InputFormat");
                    Assert.False(string.IsNullOrWhiteSpace(ch.OutputFormat), $"Challenge in {summary.Id} missing OutputFormat");
                    Assert.False(string.IsNullOrWhiteSpace(ch.Constraints), $"Challenge in {summary.Id} missing Constraints");
                    Assert.True(ch.Examples.Count >= 2, $"Challenge in {summary.Id} has fewer than 2 examples (has {ch.Examples.Count})");
                    Assert.Contains("Main", ch.StarterCode);
                    Assert.Contains("Main", ch.SolutionCode);
                    Assert.True(ch.TestCases.Count >= 3, $"Challenge in {summary.Id} has fewer than 3 test cases");
                    Assert.NotEmpty(ch.Hints);
                    Assert.False(string.IsNullOrWhiteSpace(ch.Explanation));

                    foreach (var tc in ch.TestCases)
                    {
                        Assert.False(string.IsNullOrWhiteSpace(tc.Name));
                        Assert.False(string.IsNullOrWhiteSpace(tc.Description));
                        Assert.NotNull(tc.Input);
                        Assert.NotNull(tc.ExpectedOutput);
                        Assert.False(string.IsNullOrWhiteSpace(tc.Category));
                    }
                }
            }

            Assert.Equal(38, nonCodingCount);
            Assert.Equal(50, codingCount);
        }

        [Theory]
        [InlineData("ch-00-04")]
        [InlineData("ch-01-04")]
        [InlineData("ch-02-04")]
        [InlineData("ch-03-03")]
        [InlineData("ch-04-03")]
        [InlineData("ch-05-02")]
        [InlineData("ch-08-03")]
        [InlineData("ch-09-02")]
        [InlineData("cc-11-04")]
        [InlineData("cc-15-02")]
        public async Task ChallengeExecution_ReferenceSolution_PassesAllTestCases(string challengeId)
        {
            var execService = new CodeExecutionService();
            var currService = new CurriculumService();
            var controller = new Academy.Api.Controllers.ExecutionController(execService, currService);

            var allLevels = Academy.Api.Data.Curriculum.CurriculumCatalog.GetAllLevels();
            ChallengeDto? challenge = null;
            foreach (var lvl in allLevels)
            {
                foreach (var m in lvl.Modules)
                {
                    foreach (var l in m.Lessons)
                    {
                        var det = Academy.Api.Data.Curriculum.CurriculumContentProvider.GetLessonDetail(l.Id);
                        if (det?.CodingChallenge?.Id == challengeId)
                        {
                            challenge = det.CodingChallenge;
                            break;
                        }
                    }
                    if (challenge != null) break;
                }
                if (challenge != null) break;
            }

            Assert.NotNull(challenge);

            var actionResult = await controller.SubmitChallenge(new ChallengeSubmitRequest
            {
                ChallengeId = challengeId,
                Code = challenge!.SolutionCode
            }) as Microsoft.AspNetCore.Mvc.OkObjectResult;

            Assert.NotNull(actionResult);
            var result = actionResult!.Value as ChallengeSubmitResult;
            Assert.NotNull(result);

            Assert.True(result!.Passed, $"Challenge {challengeId} failed: {result.Error ?? string.Join("; ", result.TestResults.Where(t => !t.Passed).Select(t => t.Error))}");
            Assert.Equal(challenge.TestCases.Count, result.TestsPassed);
            Assert.Equal(challenge.TestCases.Count, result.TotalTests);
        }

        [Fact]
        public async Task ChallengeExecution_FaultySolution_ReturnsDetailedDiff()
        {
            var execService = new CodeExecutionService();
            var currService = new CurriculumService();
            var controller = new Academy.Api.Controllers.ExecutionController(execService, currService);

            var actionResult = await controller.SubmitChallenge(new ChallengeSubmitRequest
            {
                ChallengeId = "ch-00-04",
                Code = "using System; public class Solution { public static void Main() { Console.WriteLine(\"99999\"); } }"
            }) as Microsoft.AspNetCore.Mvc.OkObjectResult;

            Assert.NotNull(actionResult);
            var result = actionResult!.Value as ChallengeSubmitResult;
            Assert.NotNull(result);

            Assert.False(result!.Passed);
            var firstFailed = result.TestResults.FirstOrDefault(t => !t.Passed);
            Assert.NotNull(firstFailed);
            Assert.True(firstFailed!.HasDiff);
            Assert.NotEmpty(firstFailed.DiffLines);
            Assert.Contains("99999", firstFailed.Actual);
            Assert.NotNull(firstFailed.ExpectedVisual);
            Assert.NotNull(firstFailed.ActualVisual);
        }

        [Fact]
        public async Task ChallengeExecution_CompilationError_ReturnsDiagnostics()
        {
            var execService = new CodeExecutionService();
            var currService = new CurriculumService();
            var controller = new Academy.Api.Controllers.ExecutionController(execService, currService);

            var actionResult = await controller.SubmitChallenge(new ChallengeSubmitRequest
            {
                ChallengeId = "ch-00-04",
                Code = "public class BadSolution { this is invalid syntax }"
            }) as Microsoft.AspNetCore.Mvc.OkObjectResult;

            Assert.NotNull(actionResult);
            var result = actionResult!.Value as ChallengeSubmitResult;
            Assert.NotNull(result);

            Assert.False(result!.Passed);
            Assert.NotEmpty(result.Diagnostics);
            Assert.True(result.Diagnostics[0].Line >= 1);
            Assert.False(string.IsNullOrEmpty(result.Error));
        }

        [Fact]
        public async Task ChallengeExecution_InfiniteLoop_ReturnsTimeoutWithLimitMessage()
        {
            var execService = new CodeExecutionService();
            var currService = new CurriculumService();
            var controller = new Academy.Api.Controllers.ExecutionController(execService, currService);

            var actionResult = await controller.SubmitChallenge(new ChallengeSubmitRequest
            {
                ChallengeId = "ch-00-04",
                Code = "using System; public class Solution { public static void Main() { while (true) {} } }"
            }) as Microsoft.AspNetCore.Mvc.OkObjectResult;

            Assert.NotNull(actionResult);
            var result = actionResult!.Value as ChallengeSubmitResult;
            Assert.NotNull(result);

            Assert.False(result!.Passed);
            var timeoutTest = result.TestResults.FirstOrDefault(t => t.Status == "Timeout");
            Assert.NotNull(timeoutTest);
            Assert.Contains("3500ms", timeoutTest!.Error);
        }

        [Fact]
        public async Task ChallengeExecution_CustomInput_ReturnsOutputWithoutGrading()
        {
            var execService = new CodeExecutionService();
            var currService = new CurriculumService();
            var controller = new Academy.Api.Controllers.ExecutionController(execService, currService);

            var allLevels = Academy.Api.Data.Curriculum.CurriculumCatalog.GetAllLevels();
            var ch = Academy.Api.Data.Curriculum.CurriculumContentProvider.GetLessonDetail("lesson-00-04")!.CodingChallenge!;

            var actionResult = await controller.SubmitChallenge(new ChallengeSubmitRequest
            {
                ChallengeId = "ch-00-04",
                Code = ch.SolutionCode,
                CustomInput = "0x2A"
            }) as Microsoft.AspNetCore.Mvc.OkObjectResult;

            Assert.NotNull(actionResult);
            var result = actionResult!.Value as ChallengeSubmitResult;
            Assert.NotNull(result);

            Assert.True(result!.IsCustomRun);
            Assert.True(result.Passed);
            Assert.Contains("42", result.Stdout);
        }

        [Fact]
        public async Task ChallengeExecution_SingleTestRun_ExecutesOnlySelectedTest()
        {
            var execService = new CodeExecutionService();
            var currService = new CurriculumService();
            var controller = new Academy.Api.Controllers.ExecutionController(execService, currService);

            var allLevels = Academy.Api.Data.Curriculum.CurriculumCatalog.GetAllLevels();
            var ch = Academy.Api.Data.Curriculum.CurriculumContentProvider.GetLessonDetail("lesson-00-04")!.CodingChallenge!;

            var actionResult = await controller.SubmitChallenge(new ChallengeSubmitRequest
            {
                ChallengeId = "ch-00-04",
                Code = ch.SolutionCode,
                SingleTestIndex = 1
            }) as Microsoft.AspNetCore.Mvc.OkObjectResult;

            Assert.NotNull(actionResult);
            var result = actionResult!.Value as ChallengeSubmitResult;
            Assert.NotNull(result);

            Assert.Equal(1, result!.TotalTests);
            Assert.Single(result.TestResults);
            Assert.Equal(1, result.TestResults[0].Index);
            Assert.True(result.TestResults[0].Passed);
        }
    }
}
