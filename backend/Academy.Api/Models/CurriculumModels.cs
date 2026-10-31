using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Academy.Api.Models
{
    public class LevelSummaryDto
    {
        public string Id { get; set; } = string.Empty;
        public int Order { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Subtitle { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Icon { get; set; } = "BookOpen";
        public int ModuleCount { get; set; }
        public int LessonCount { get; set; }
        public string Difficulty { get; set; } = "Beginner";
        public int EstimatedHours { get; set; }
        public List<string> KeyTopics { get; set; } = new();
    }

    public class LevelDetailDto : LevelSummaryDto
    {
        public List<ModuleDto> Modules { get; set; } = new();
    }

    public class ModuleDto
    {
        public string Id { get; set; } = string.Empty;
        public string LevelId { get; set; } = string.Empty;
        public int Order { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<LessonSummaryDto> Lessons { get; set; } = new();
    }

    public class LessonSummaryDto
    {
        public string Id { get; set; } = string.Empty;
        public string LevelId { get; set; } = string.Empty;
        public string ModuleId { get; set; } = string.Empty;
        public int Order { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Difficulty { get; set; } = "Beginner";
        public int EstimatedMinutes { get; set; }
        public bool HasQuiz { get; set; }
        public bool HasExercise { get; set; }
        public string? SimulatorType { get; set; }
    }

    public class LessonDetailDto : LessonSummaryDto
    {
        public string Description { get; set; } = string.Empty;
        /// <summary>Prasyarat eksplisit: lesson yang harus dipahami lebih dulu beserta alasannya.</summary>
        public List<CrossReferenceDto> Prerequisites { get; set; } = new();
        /// <summary>Istilah teknis yang diperkenalkan (didefinisikan) pertama kali di lesson ini.</summary>
        public List<string> Introduces { get; set; } = new();
        public List<string> LearningObjectives { get; set; } = new();
        public string MarkdownContent { get; set; } = string.Empty;
        /// <summary>Kerangka live code (berisi petunjuk TODO).</summary>
        public string StarterCode { get; set; } = string.Empty;
        public object? SimulatorContext { get; set; }
        public List<QuizQuestionDto> QuizQuestions { get; set; } = new();
        public ChallengeDto? CodingChallenge { get; set; }
        public List<CrossReferenceDto> RelatedLessons { get; set; } = new();
        public List<ReferenceLinkDto> References { get; set; } = new();
        public string? NextLessonId { get; set; }
        public string? PreviousLessonId { get; set; }
    }

    public class QuizQuestionDto
    {
        public string Id { get; set; } = string.Empty;
        public string Question { get; set; } = string.Empty;
        /// <summary>MultipleChoice, AnalyzeCode, atau PredictOutput.</summary>
        public string Type { get; set; } = "MultipleChoice";
        public List<string> Options { get; set; } = new();
        public int CorrectOptionIndex { get; set; }
        /// <summary>Mengapa jawaban benar tepat (merujuk bagian lesson).</summary>
        public string Explanation { get; set; } = string.Empty;
        public string ExplanationCorrect => !string.IsNullOrEmpty(Explanation) ? Explanation : "Jawaban Anda benar!";
        public string ExplanationIncorrect => OptionExplanations.Count > 0 ? string.Join(" ", OptionExplanations) : Explanation;
        /// <summary>Penjelasan per opsi: mengapa opsi itu benar/salah. Panjang sama dengan Options.</summary>
        public List<string> OptionExplanations { get; set; } = new();
        /// <summary>Teks heading di lesson tempat konsep soal ini diajarkan.</summary>
        public string SectionRef { get; set; } = string.Empty;
        public string Difficulty { get; set; } = "Intermediate";
        public string? CodeSnippet { get; set; }
    }

    public class ChallengeDto
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Difficulty { get; set; } = "Intermediate";
        public string ProblemStatement { get; set; } = string.Empty;
        public string InputFormat { get; set; } = string.Empty;
        public string OutputFormat { get; set; } = string.Empty;
        public string Constraints { get; set; } = string.Empty;
        public List<ChallengeExampleDto> Examples { get; set; } = new();
        public string StarterCode { get; set; } = string.Empty;
        public string SolutionCode { get; set; } = string.Empty;
        public List<ChallengeTestCase> TestCases { get; set; } = new();
        public List<string> Hints { get; set; } = new();
        public string Explanation { get; set; } = string.Empty;
    }

    public class ChallengeExampleDto
    {
        public string Input { get; set; } = string.Empty;
        public string Output { get; set; } = string.Empty;
        public string Explanation { get; set; } = string.Empty;
    }

    public class ChallengeTestCase
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Input { get; set; } = string.Empty;
        public string ExpectedOutput { get; set; } = string.Empty;
        public bool IsHidden { get; set; }
        public string Category { get; set; } = "Normal";
    }

    [JsonConverter(typeof(CrossReferenceConverter))]
    public class CrossReferenceDto
    {
        public string LessonId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string LevelName { get; set; } = string.Empty;
        public string Context { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
    }

    public class CrossReferenceConverter : JsonConverter<CrossReferenceDto>
    {
        public override CrossReferenceDto? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                return new CrossReferenceDto { LessonId = reader.GetString() ?? string.Empty };
            }
            if (reader.TokenType == JsonTokenType.StartObject)
            {
                using var doc = JsonDocument.ParseValue(ref reader);
                var root = doc.RootElement;
                var dto = new CrossReferenceDto();
                if (root.TryGetProperty("lessonId", out var lid) || root.TryGetProperty("LessonId", out lid))
                    dto.LessonId = lid.GetString() ?? string.Empty;
                if (root.TryGetProperty("title", out var t) || root.TryGetProperty("Title", out t))
                    dto.Title = t.GetString() ?? string.Empty;
                if (root.TryGetProperty("levelName", out var ln) || root.TryGetProperty("LevelName", out ln))
                    dto.LevelName = ln.GetString() ?? string.Empty;
                if (root.TryGetProperty("context", out var ctx) || root.TryGetProperty("Context", out ctx))
                    dto.Context = ctx.GetString() ?? string.Empty;
                if (root.TryGetProperty("reason", out var r) || root.TryGetProperty("Reason", out r))
                    dto.Reason = r.GetString() ?? string.Empty;
                return dto;
            }
            return null;
        }

        public override void Write(Utf8JsonWriter writer, CrossReferenceDto value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString("lessonId", value.LessonId);
            writer.WriteString("title", value.Title);
            writer.WriteString("levelName", value.LevelName);
            writer.WriteString("context", value.Context);
            writer.WriteString("reason", value.Reason);
            writer.WriteEndObject();
        }
    }

    public class ReferenceLinkDto
    {
        public string Title { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string Type { get; set; } = "documentation";
    }

    public class ProjectSpecificationDto
    {
        public string Id { get; set; } = string.Empty;
        public int Order { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Subtitle { get; set; } = string.Empty;
        public string Difficulty { get; set; } = "Intermediate";
        public string Overview { get; set; } = string.Empty;
        public List<string> FunctionalRequirements { get; set; } = new();
        public List<string> NonFunctionalRequirements { get; set; } = new();
        public string ArchitectureOverview { get; set; } = string.Empty;
        public string DatabaseSchema { get; set; } = string.Empty;
        public string ApiSpecification { get; set; } = string.Empty;
        public string StarterCode { get; set; } = string.Empty;
        public List<string> ImplementationSteps { get; set; } = new();
        public List<string> AcceptanceCriteria { get; set; } = new();
        public string SecurityReview { get; set; } = string.Empty;
        public string PerformanceReview { get; set; } = string.Empty;
    }
}
