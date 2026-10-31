using System;
using System.Collections.Generic;

namespace Academy.Api.Data.Entities
{
    public class LearnerUser
    {
        public string Id { get; set; } = "default-learner";
        public string Username { get; set; } = "principal_learner";
        public string DisplayName { get; set; } = "Enterprise Engineer";
        public string Email { get; set; } = "engineer@enterprise.internal";
        public int CurrentStreak { get; set; } = 3;
        public int BestStreak { get; set; } = 7;
        public int Xp { get; set; } = 450;
        public string LastVisitedLessonId { get; set; } = "lesson-00-01";
        public DateTime LastActiveDate { get; set; } = DateTime.UtcNow;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public List<LearnerProgress> ProgressList { get; set; } = new();
        public List<LearnerBookmark> Bookmarks { get; set; } = new();
        public List<QuizAttempt> QuizAttempts { get; set; } = new();
        public List<CodeSubmission> Submissions { get; set; } = new();
        public List<UserAchievement> Achievements { get; set; } = new();
    }

    public class LearnerProgress
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string LessonId { get; set; } = string.Empty;
        public bool Completed { get; set; }
        public int QuizScore { get; set; }
        public string? SavedCode { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime LastVisitedAt { get; set; } = DateTime.UtcNow;
    }

    public class LearnerBookmark
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string LessonId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class QuizAttempt
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string QuizId { get; set; } = string.Empty;
        public string LessonId { get; set; } = string.Empty;
        public int Score { get; set; }
        public int TotalQuestions { get; set; }
        public bool Passed { get; set; }
        public string AnswersJson { get; set; } = "[]";
        public DateTime AttemptedAt { get; set; } = DateTime.UtcNow;
    }

    public class CodeSubmission
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string ChallengeId { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public bool Passed { get; set; }
        public int TestsPassed { get; set; }
        public int TotalTests { get; set; }
        public long ExecutionTimeMs { get; set; }
        public string Output { get; set; } = string.Empty;
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    }

    public class UserAchievement
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string BadgeCode { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Icon { get; set; } = "Award";
        public DateTime UnlockedAt { get; set; } = DateTime.UtcNow;
    }
}
