using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Academy.Api.Data;
using Academy.Api.Data.Curriculum;
using Academy.Api.Data.Entities;

namespace Academy.Api.Controllers
{
    public class DashboardResponse
    {
        public string UserId { get; set; } = "default-learner";
        public string DisplayName { get; set; } = "Enterprise Engineer";
        public int TotalLessonsCompleted { get; set; }
        public int TotalLessons { get; set; }
        public int OverallProgressPercentage { get; set; }
        public int CurrentStreak { get; set; } = 3;
        public int BestStreak { get; set; } = 7;
        public int Xp { get; set; } = 450;
        public List<LevelProgressDto> LevelProgress { get; set; } = new();
        public List<string> CompletedLessonIds { get; set; } = new();
        public List<string> BookmarkedLessonIds { get; set; } = new();
        public List<AchievementDto> Achievements { get; set; } = new();
        public List<string> WeakTopics { get; set; } = new();
        public string RecommendedLessonId { get; set; } = "lesson-02-04";
        public string LastVisitedLessonId { get; set; } = "lesson-00-01";
    }

    public class LevelProgressDto
    {
        public string LevelId { get; set; } = string.Empty;
        public string LevelTitle { get; set; } = string.Empty;
        public int CompletedLessons { get; set; }
        public int TotalLessons { get; set; }
        public int Percentage { get; set; }
    }

    public class AchievementDto
    {
        public string Code { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Icon { get; set; } = "Award";
        public bool Unlocked { get; set; }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class ProgressController : ControllerBase
    {
        private readonly AcademyDbContext _dbContext;

        public ProgressController(AcademyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard([FromQuery] string userId = "default-learner")
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
            var completedProgress = await _dbContext.Progress
                .Where(p => p.UserId == userId && p.Completed)
                .ToListAsync();

            var bookmarks = await _dbContext.Bookmarks
                .Where(b => b.UserId == userId)
                .Select(b => b.LessonId)
                .ToListAsync();

            var allLevels = CurriculumCatalog.GetAllLevels();
            int totalLessons = allLevels.Sum(l => l.LessonCount);
            int completedCount = completedProgress.Count;
            int overallPercentage = totalLessons > 0 ? (int)Math.Round((double)completedCount / totalLessons * 100) : 0;

            var levelProgressList = new List<LevelProgressDto>();
            foreach (var lvl in allLevels)
            {
                var lessonIds = lvl.Modules.SelectMany(m => m.Lessons).Select(l => l.Id).ToHashSet();
                int levelCompleted = completedProgress.Count(p => lessonIds.Contains(p.LessonId));
                int levelTotal = lessonIds.Count;
                int percent = levelTotal > 0 ? (int)Math.Round((double)levelCompleted / levelTotal * 100) : 0;

                levelProgressList.Add(new LevelProgressDto
                {
                    LevelId = lvl.Id,
                    LevelTitle = lvl.Title,
                    CompletedLessons = levelCompleted,
                    TotalLessons = levelTotal,
                    Percentage = percent
                });
            }

            var achievements = new List<AchievementDto>
            {
                new AchievementDto
                {
                    Code = "FIRST_CODE",
                    Title = "First Compilation",
                    Description = "Menjalankan baris kode C# pertama di platform.",
                    Icon = "Play",
                    Unlocked = true
                },
                new AchievementDto
                {
                    Code = "STACK_MASTER",
                    Title = "Memory Architect",
                    Description = "Memahami dan menyelesaikan kuis alokasi Stack vs Heap.",
                    Icon = "Layers",
                    Unlocked = completedProgress.Any(p => p.LessonId == "lesson-02-04")
                },
                new AchievementDto
                {
                    Code = "CONCURRENCY_HERO",
                    Title = "Concurrency Master",
                    Description = "Menyelesaikan materi State Machine Async/Await tanpa deadlock.",
                    Icon = "Zap",
                    Unlocked = completedProgress.Any(p => p.LessonId == "lesson-12-01")
                },
                new AchievementDto
                {
                    Code = "BANKING_PRINCIPAL",
                    Title = "Double-Entry Principal",
                    Description = "Menguasai integritas transaksi buku besar perbankan enterprise.",
                    Icon = "ShieldCheck",
                    Unlocked = completedProgress.Any(p => p.LessonId == "lesson-18-01")
                }
            };

            // Find recommended next lesson
            string recommended = "lesson-00-01";
            var firstIncomplete = allLevels
                .SelectMany(l => l.Modules.SelectMany(m => m.Lessons))
                .FirstOrDefault(l => !completedProgress.Any(p => p.LessonId == l.Id));
            if (firstIncomplete != null) recommended = firstIncomplete.Id;

            var completedLessonIds = completedProgress.Select(p => p.LessonId).ToList();

            var failedAttempts = await _dbContext.QuizAttempts
                .Where(q => q.UserId == userId && !q.Passed)
                .OrderByDescending(q => q.AttemptedAt)
                .Take(5)
                .ToListAsync();

            var weakTopics = failedAttempts
                .Select(f => {
                    var l = CurriculumContentProvider.GetLessonDetail(f.LessonId);
                    return l?.Title ?? f.LessonId;
                })
                .Distinct()
                .ToList();

            return Ok(new DashboardResponse
            {
                UserId = userId,
                DisplayName = user?.DisplayName ?? "Enterprise Engineer",
                TotalLessonsCompleted = completedCount,
                TotalLessons = totalLessons,
                OverallProgressPercentage = overallPercentage,
                CurrentStreak = user?.CurrentStreak ?? (completedCount > 0 ? 1 : 0),
                BestStreak = user?.BestStreak ?? (completedCount > 0 ? 1 : 0),
                Xp = user?.Xp ?? (completedCount * 100),
                LevelProgress = levelProgressList,
                CompletedLessonIds = completedLessonIds,
                BookmarkedLessonIds = bookmarks,
                Achievements = achievements,
                WeakTopics = weakTopics,
                RecommendedLessonId = recommended,
                LastVisitedLessonId = user?.LastVisitedLessonId ?? (completedLessonIds.LastOrDefault() ?? "lesson-00-01")
            });
        }

        [HttpPost("visit/{lessonId}")]
        public async Task<IActionResult> RecordVisit(string lessonId, [FromQuery] string userId = "default-learner")
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user != null)
            {
                user.LastVisitedLessonId = lessonId;
                user.LastActiveDate = DateTime.UtcNow;
            }

            var progress = await _dbContext.Progress
                .FirstOrDefaultAsync(p => p.UserId == userId && p.LessonId == lessonId);
            if (progress == null)
            {
                progress = new LearnerProgress
                {
                    UserId = userId,
                    LessonId = lessonId,
                    Completed = false,
                    LastVisitedAt = DateTime.UtcNow
                };
                _dbContext.Progress.Add(progress);
            }
            else
            {
                progress.LastVisitedAt = DateTime.UtcNow;
            }

            await _dbContext.SaveChangesAsync();
            return Ok(new { success = true, lessonId, lastVisited = lessonId });
        }

        [HttpPost("complete/{lessonId}")]
        public async Task<IActionResult> MarkCompleted(string lessonId, [FromQuery] string userId = "default-learner")
        {
            var progress = await _dbContext.Progress
                .FirstOrDefaultAsync(p => p.UserId == userId && p.LessonId == lessonId);

            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
            bool isNowCompleted = false;

            if (progress == null)
            {
                progress = new LearnerProgress
                {
                    UserId = userId,
                    LessonId = lessonId,
                    Completed = true,
                    CompletedAt = DateTime.UtcNow,
                    LastVisitedAt = DateTime.UtcNow
                };
                _dbContext.Progress.Add(progress);
                isNowCompleted = true;
                if (user != null)
                {
                    user.Xp += 25;
                }
            }
            else
            {
                // Toggle completion state
                if (progress.Completed)
                {
                    progress.Completed = false;
                    isNowCompleted = false;
                    if (user != null)
                    {
                        user.Xp = Math.Max(0, user.Xp - 25);
                    }
                }
                else
                {
                    progress.Completed = true;
                    progress.CompletedAt = DateTime.UtcNow;
                    isNowCompleted = true;
                    if (user != null)
                    {
                        user.Xp += 25;
                    }
                }
                progress.LastVisitedAt = DateTime.UtcNow;
            }

            if (user != null)
            {
                user.LastVisitedLessonId = lessonId;
                user.LastActiveDate = DateTime.UtcNow;
            }

            await _dbContext.SaveChangesAsync();
            return Ok(new { success = true, lessonId, completed = isNowCompleted });
        }

        [HttpPost("bookmark/{lessonId}")]
        public async Task<IActionResult> ToggleBookmark(string lessonId, [FromQuery] string userId = "default-learner")
        {
            var bookmark = await _dbContext.Bookmarks
                .FirstOrDefaultAsync(b => b.UserId == userId && b.LessonId == lessonId);

            bool isBookmarked = false;
            if (bookmark != null)
            {
                _dbContext.Bookmarks.Remove(bookmark);
                isBookmarked = false;
            }
            else
            {
                _dbContext.Bookmarks.Add(new LearnerBookmark
                {
                    UserId = userId,
                    LessonId = lessonId,
                    CreatedAt = DateTime.UtcNow
                });
                isBookmarked = true;
            }

            await _dbContext.SaveChangesAsync();
            return Ok(new { success = true, lessonId, bookmarked = isBookmarked });
        }
    }
}
