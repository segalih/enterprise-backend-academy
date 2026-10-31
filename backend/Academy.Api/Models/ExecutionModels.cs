using System.Collections.Generic;

namespace Academy.Api.Models
{
    public class ExecuteRequest
    {
        public string Code { get; set; } = string.Empty;
        public string? Input { get; set; }
        public int TimeoutMs { get; set; } = 4000;
    }

    public class ExecuteResponse
    {
        public bool Success { get; set; }
        public string Output { get; set; } = string.Empty;
        public string Stdout { get; set; } = string.Empty;
        public string Stderr { get; set; } = string.Empty;
        public string? Error { get; set; }
        public List<CompilationError> Diagnostics { get; set; } = new();
        public long ExecutionTimeMs { get; set; }
        public bool TimedOut { get; set; }
    }

    public class CompilationError
    {
        public int Line { get; set; }
        public int Column { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Severity { get; set; } = "Error";
        public string Id { get; set; } = string.Empty;
    }

    public class ChallengeSubmitRequest
    {
        public string ChallengeId { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public int? SingleTestIndex { get; set; }
        public string? CustomInput { get; set; }
    }

    public class ChallengeSubmitResult
    {
        public bool Passed { get; set; }
        public int TestsPassed { get; set; }
        public int TotalTests { get; set; }
        public List<TestCaseResult> TestResults { get; set; } = new();
        public string Stdout { get; set; } = string.Empty;
        public string Stderr { get; set; } = string.Empty;
        public string Output { get; set; } = string.Empty;
        public string? Error { get; set; }
        public List<CompilationError> Diagnostics { get; set; } = new();
        public long ExecutionTimeMs { get; set; }
        public bool IsCustomRun { get; set; }
    }

    public class TestCaseResult
    {
        public int Index { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = "Normal";
        public bool IsHidden { get; set; }
        public string Status { get; set; } = "Passed";
        public bool Passed { get; set; }
        public string? Input { get; set; }
        public string? Expected { get; set; }
        public string? Actual { get; set; }
        public string? Stdout { get; set; }
        public string? Stderr { get; set; }
        public string? Error { get; set; }
        public long ExecutionTimeMs { get; set; }
        public bool HasDiff { get; set; }
        public bool LineEndingMismatch { get; set; }
        public string? ExpectedVisual { get; set; }
        public string? ActualVisual { get; set; }
        public List<DiffLineDto> DiffLines { get; set; } = new();
    }

    public class DiffLineDto
    {
        public int LineNumber { get; set; }
        public string Type { get; set; } = "Equal";
        public string? Expected { get; set; }
        public string? Actual { get; set; }
        public string? ExpectedVisual { get; set; }
        public string? ActualVisual { get; set; }
        public string? Note { get; set; }
    }
}
