using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Academy.Api.Data;
using Academy.Api.Data.Curriculum;
using Academy.Api.Data.Entities;

namespace Academy.Api.Controllers
{
    public class QuizSubmitRequest
    {
        public string UserId { get; set; } = "default-learner";
        public string LessonId { get; set; } = string.Empty;
        public Dictionary<string, int> Answers { get; set; } = new();
    }

    public class QuizSubmitResponse
    {
        public string LessonId { get; set; } = string.Empty;
        public int Score { get; set; }
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
        public bool Passed { get; set; }
        public int PassingScorePercentage { get; set; } = 80;
        public int XpEarned { get; set; }
        public List<QuestionFeedback> Feedback { get; set; } = new();
    }

    public class QuestionFeedback
    {
        public string QuestionId { get; set; } = string.Empty;
        public string QuestionText { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
        public int SelectedOption { get; set; }
        public int CorrectOption { get; set; }
        public string Explanation { get; set; } = string.Empty;
    }

    [ApiController]
    [Route("api/[controller]")]
    public class QuizController : ControllerBase
    {
        private readonly AcademyDbContext _dbContext;

        public QuizController(AcademyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost("submit")]
        public async Task<IActionResult> SubmitQuiz([FromBody] QuizSubmitRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.LessonId))
            {
                return BadRequest(new { message = "LessonId wajib diisi." });
            }

            var lesson = CurriculumContentProvider.GetLessonDetail(request.LessonId);
            if (lesson == null || lesson.QuizQuestions.Count == 0)
            {
                return BadRequest(new { message = "Tidak ada kuis aktif untuk materi pelajaran ini." });
            }

            int total = lesson.QuizQuestions.Count;
            int correctCount = 0;
            var feedbackList = new List<QuestionFeedback>();

            foreach (var q in lesson.QuizQuestions)
            {
                bool hasAnswer = request.Answers.TryGetValue(q.Id, out int selected);
                bool isCorrect = hasAnswer && selected == q.CorrectOptionIndex;

                if (isCorrect) correctCount++;

                feedbackList.Add(new QuestionFeedback
                {
                    QuestionId = q.Id,
                    QuestionText = q.Question,
                    IsCorrect = isCorrect,
                    SelectedOption = hasAnswer ? selected : -1,
                    CorrectOption = q.CorrectOptionIndex,
                    Explanation = isCorrect ? q.ExplanationCorrect : q.ExplanationIncorrect
                });
            }

            int scorePercent = (int)Math.Round((double)correctCount / total * 100);
            bool passed = scorePercent >= 80;
            int xpEarned = passed ? 50 + (scorePercent / 2) : 10;

            // Save to database
            try
            {
                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == request.UserId);
                if (user == null)
                {
                    user = new LearnerUser { Id = request.UserId };
                    _dbContext.Users.Add(user);
                }

                user.Xp += xpEarned;
                user.LastActiveDate = DateTime.UtcNow;

                var attempt = new QuizAttempt
                {
                    UserId = request.UserId,
                    QuizId = $"quiz-{request.LessonId}",
                    LessonId = request.LessonId,
                    Score = scorePercent,
                    TotalQuestions = total,
                    Passed = passed,
                    AnswersJson = JsonSerializer.Serialize(request.Answers),
                    AttemptedAt = DateTime.UtcNow
                };
                _dbContext.QuizAttempts.Add(attempt);

                if (passed)
                {
                    var progress = await _dbContext.Progress
                        .FirstOrDefaultAsync(p => p.UserId == request.UserId && p.LessonId == request.LessonId);

                    if (progress == null)
                    {
                        progress = new LearnerProgress
                        {
                            UserId = request.UserId,
                            LessonId = request.LessonId,
                            Completed = true,
                            QuizScore = scorePercent,
                            CompletedAt = DateTime.UtcNow
                        };
                        _dbContext.Progress.Add(progress);
                    }
                    else
                    {
                        progress.Completed = true;
                        progress.QuizScore = Math.Max(progress.QuizScore, scorePercent);
                        progress.CompletedAt ??= DateTime.UtcNow;
                    }
                }

                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[QuizController] Failed to persist quiz attempt: {ex.Message}");
            }

            return Ok(new QuizSubmitResponse
            {
                LessonId = request.LessonId,
                Score = scorePercent,
                TotalQuestions = total,
                CorrectAnswers = correctCount,
                Passed = passed,
                XpEarned = xpEarned,
                Feedback = feedbackList
            });
        }
    }
}
