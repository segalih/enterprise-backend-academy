using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Academy.Api.Data.Curriculum;
using Academy.Api.Models;
using Academy.Api.Services;

namespace Academy.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExecutionController : ControllerBase
    {
        private readonly ICodeExecutionService _executionService;
        private readonly ICurriculumService _curriculumService;

        public ExecutionController(ICodeExecutionService executionService, ICurriculumService curriculumService)
        {
            _executionService = executionService;
            _curriculumService = curriculumService;
        }

        [HttpPost("csharp")]
        public async Task<IActionResult> ExecuteCSharp([FromBody] ExecuteRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Code))
            {
                return BadRequest(new ExecuteResponse
                {
                    Success = false,
                    Error = "Kode sumber tidak boleh kosong."
                });
            }

            var result = await _executionService.ExecuteAsync(request.Code, request.Input, request.TimeoutMs);
            return Ok(result);
        }

        [HttpPost("challenge")]
        public async Task<IActionResult> SubmitChallenge([FromBody] ChallengeSubmitRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Code))
            {
                return BadRequest(new ChallengeSubmitResult
                {
                    Passed = false,
                    Error = "Kode solusi tantangan tidak boleh kosong."
                });
            }

            // Find challenge from curriculum
            var levels = CurriculumCatalog.GetAllLevels();
            ChallengeDto? challenge = null;

            foreach (var lvl in levels)
            {
                foreach (var mod in lvl.Modules)
                {
                    foreach (var les in mod.Lessons)
                    {
                        var detail = CurriculumContentProvider.GetLessonDetail(les.Id);
                        if (detail?.CodingChallenge?.Id == request.ChallengeId)
                        {
                            challenge = detail.CodingChallenge;
                            break;
                        }
                    }
                    if (challenge != null) break;
                }
                if (challenge != null) break;
            }

            if (challenge == null)
            {
                return NotFound(new { message = $"Challenge '{request.ChallengeId}' tidak ditemukan." });
            }

            // 1. Custom Input execution mode
            if (request.CustomInput != null)
            {
                var customSw = Stopwatch.StartNew();
                var customExec = await _executionService.ExecuteAsync(request.Code, input: request.CustomInput, timeoutMs: 4000);
                customSw.Stop();

                return Ok(new ChallengeSubmitResult
                {
                    IsCustomRun = true,
                    Passed = customExec.Success,
                    TestsPassed = customExec.Success ? 1 : 0,
                    TotalTests = 1,
                    Stdout = customExec.Stdout,
                    Stderr = customExec.Stderr,
                    Output = customExec.Output,
                    Error = customExec.Error,
                    Diagnostics = customExec.Diagnostics,
                    ExecutionTimeMs = customSw.ElapsedMilliseconds
                });
            }

            // 2. Determine target test cases (all vs single test)
            var targetTestCases = new List<(int Index, ChallengeTestCase TestCase)>();
            if (request.SingleTestIndex.HasValue)
            {
                int idx = request.SingleTestIndex.Value;
                if (idx < 0 || idx >= challenge.TestCases.Count)
                {
                    return BadRequest(new { message = $"Indeks test case {idx} di luar rentang (0..{challenge.TestCases.Count - 1})." });
                }
                targetTestCases.Add((idx, challenge.TestCases[idx]));
            }
            else
            {
                for (int i = 0; i < challenge.TestCases.Count; i++)
                {
                    targetTestCases.Add((i, challenge.TestCases[i]));
                }
            }

            var totalSw = Stopwatch.StartNew();
            var testResults = new List<TestCaseResult>();
            bool compilationChecked = false;

            foreach (var (tcIndex, tc) in targetTestCases)
            {
                var tcSw = Stopwatch.StartNew();
                var execResponse = await _executionService.ExecuteAsync(request.Code, input: tc.Input, timeoutMs: 3500);
                tcSw.Stop();

                // If first run encountered compilation errors, return immediately
                if (!compilationChecked && execResponse.Diagnostics != null && execResponse.Diagnostics.Any(d => d.Severity == "Error"))
                {
                    totalSw.Stop();
                    return Ok(new ChallengeSubmitResult
                    {
                        Passed = false,
                        TestsPassed = 0,
                        TotalTests = targetTestCases.Count,
                        Diagnostics = execResponse.Diagnostics,
                        Error = execResponse.Error ?? "Kompilasi kode gagal dengan kesalahan sintaks.",
                        ExecutionTimeMs = totalSw.ElapsedMilliseconds
                    });
                }
                compilationChecked = true;

                var tcResult = new TestCaseResult
                {
                    Index = tcIndex,
                    Name = !string.IsNullOrEmpty(tc.Name) ? tc.Name : $"Test #{tcIndex + 1}",
                    Description = tc.Description,
                    Category = !string.IsNullOrEmpty(tc.Category) ? tc.Category : "Normal",
                    IsHidden = tc.IsHidden,
                    ExecutionTimeMs = tcSw.ElapsedMilliseconds,
                    Stdout = execResponse.Stdout,
                    Stderr = execResponse.Stderr
                };

                if (execResponse.TimedOut)
                {
                    tcResult.Passed = false;
                    tcResult.Status = "Timeout";
                    tcResult.Error = "Eksekusi melampaui batas waktu 3500ms. Kemungkinan terdapat infinite loop atau proses pemblokiran.";
                    tcResult.Actual = execResponse.Output;
                }
                else if (!execResponse.Success)
                {
                    tcResult.Passed = false;
                    tcResult.Status = "Error";
                    tcResult.Error = execResponse.Error ?? "Eksekusi menghasilkan runtime exception.";
                    tcResult.Actual = execResponse.Output;
                }
                else
                {
                    // Compute diff against expected
                    var (hasDiff, lineMismatch, diffs, expVis, actVis) =
                        ChallengeDiffEngine.CompareAndDiff(tc.ExpectedOutput, execResponse.Stdout);

                    tcResult.Passed = !hasDiff;
                    tcResult.Status = tcResult.Passed ? "Passed" : "Failed";
                    tcResult.LineEndingMismatch = lineMismatch;

                    if (!tcResult.Passed)
                    {
                        tcResult.Error = lineMismatch
                            ? "Output berbeda pada format akhir baris (CRLF vs LF) atau spasi/karakter."
                            : "Output yang dihasilkan tidak sesuai dengan yang diharapkan.";
                    }

                    if (tc.IsHidden)
                    {
                        if (tcResult.Passed)
                        {
                            tcResult.Input = null;
                            tcResult.Expected = "[Tersembunyi - Test Lolos]";
                            tcResult.Actual = "[Tersembunyi - Test Lolos]";
                        }
                        else
                        {
                            // Hidden test failure policy: reveal test name, category, and input if safe, but mask expected
                            tcResult.Input = tc.Input;
                            tcResult.Expected = "[Tersembunyi]";
                            tcResult.Actual = execResponse.Stdout;
                            tcResult.HasDiff = false;
                        }
                    }
                    else
                    {
                        tcResult.Input = tc.Input;
                        tcResult.Expected = tc.ExpectedOutput;
                        tcResult.Actual = execResponse.Stdout;
                        tcResult.HasDiff = hasDiff;
                        tcResult.DiffLines = diffs;
                        tcResult.ExpectedVisual = expVis;
                        tcResult.ActualVisual = actVis;
                    }
                }

                testResults.Add(tcResult);
            }

            totalSw.Stop();

            int passedCount = testResults.Count(t => t.Passed);
            var challengeResult = new ChallengeSubmitResult
            {
                Passed = (passedCount == targetTestCases.Count),
                TestsPassed = passedCount,
                TotalTests = targetTestCases.Count,
                TestResults = testResults,
                ExecutionTimeMs = totalSw.ElapsedMilliseconds,
                Stdout = testResults.FirstOrDefault()?.Stdout ?? string.Empty,
                Stderr = testResults.FirstOrDefault()?.Stderr ?? string.Empty
            };

            return Ok(challengeResult);
        }
    }
}
