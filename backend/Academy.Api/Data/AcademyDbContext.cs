using Microsoft.EntityFrameworkCore;
using Academy.Api.Data.Entities;

namespace Academy.Api.Data
{
    public class AcademyDbContext : DbContext
    {
        public AcademyDbContext(DbContextOptions<AcademyDbContext> options) : base(options)
        {
        }

        public DbSet<LearnerUser> Users => Set<LearnerUser>();
        public DbSet<LearnerProgress> Progress => Set<LearnerProgress>();
        public DbSet<LearnerBookmark> Bookmarks => Set<LearnerBookmark>();
        public DbSet<QuizAttempt> QuizAttempts => Set<QuizAttempt>();
        public DbSet<CodeSubmission> CodeSubmissions => Set<CodeSubmission>();
        public DbSet<UserAchievement> Achievements => Set<UserAchievement>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<LearnerUser>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasMany(e => e.ProgressList).WithOne().HasForeignKey(p => p.UserId);
                entity.HasMany(e => e.Bookmarks).WithOne().HasForeignKey(b => b.UserId);
                entity.HasMany(e => e.QuizAttempts).WithOne().HasForeignKey(q => q.UserId);
                entity.HasMany(e => e.Submissions).WithOne().HasForeignKey(s => s.UserId);
                entity.HasMany(e => e.Achievements).WithOne().HasForeignKey(a => a.UserId);
            });

            modelBuilder.Entity<LearnerProgress>(entity =>
            {
                entity.HasIndex(e => new { e.UserId, e.LessonId }).IsUnique();
            });

            modelBuilder.Entity<LearnerBookmark>(entity =>
            {
                entity.HasIndex(e => new { e.UserId, e.LessonId }).IsUnique();
            });
        }

        public static void EnsureSchemaAndSeed(AcademyDbContext db)
        {
            db.Database.EnsureCreated();

            var conn = db.Database.GetDbConnection();
            bool wasClosed = conn.State != System.Data.ConnectionState.Open;
            if (wasClosed) conn.Open();

            try
            {
                using (var pragmaCmd = conn.CreateCommand())
                {
                    pragmaCmd.CommandText = "PRAGMA journal_mode=WAL; PRAGMA busy_timeout=5000;";
                    pragmaCmd.ExecuteNonQuery();
                }

                using (var tblCmd = conn.CreateCommand())
                {
                    tblCmd.CommandText = @"
                        CREATE TABLE IF NOT EXISTS ""QuizAttempts"" (
                            ""Id"" INTEGER NOT NULL CONSTRAINT ""PK_QuizAttempts"" PRIMARY KEY AUTOINCREMENT,
                            ""UserId"" TEXT NOT NULL,
                            ""QuizId"" TEXT NOT NULL,
                            ""LessonId"" TEXT NOT NULL,
                            ""Score"" INTEGER NOT NULL,
                            ""TotalQuestions"" INTEGER NOT NULL,
                            ""Passed"" INTEGER NOT NULL,
                            ""AnswersJson"" TEXT NOT NULL DEFAULT '[]',
                            ""AttemptedAt"" TEXT NOT NULL
                        );
                        CREATE TABLE IF NOT EXISTS ""CodeSubmissions"" (
                            ""Id"" INTEGER NOT NULL CONSTRAINT ""PK_CodeSubmissions"" PRIMARY KEY AUTOINCREMENT,
                            ""UserId"" TEXT NOT NULL,
                            ""LessonId"" TEXT NOT NULL,
                            ""SourceCode"" TEXT NOT NULL,
                            ""Passed"" INTEGER NOT NULL,
                            ""Output"" TEXT NOT NULL,
                            ""SubmittedAt"" TEXT NOT NULL
                        );
                        CREATE TABLE IF NOT EXISTS ""Achievements"" (
                            ""Id"" INTEGER NOT NULL CONSTRAINT ""PK_Achievements"" PRIMARY KEY AUTOINCREMENT,
                            ""UserId"" TEXT NOT NULL,
                            ""Code"" TEXT NOT NULL,
                            ""Title"" TEXT NOT NULL,
                            ""UnlockedAt"" TEXT NOT NULL
                        );
                    ";
                    tblCmd.ExecuteNonQuery();
                }

                bool hasLastVisited = false;
                using (var colCmd = conn.CreateCommand())
                {
                    colCmd.CommandText = "PRAGMA table_info('Users');";
                    using var reader = colCmd.ExecuteReader();
                    while (reader.Read())
                    {
                        var colName = reader.GetString(1);
                        if (string.Equals(colName, "LastVisitedLessonId", System.StringComparison.OrdinalIgnoreCase))
                        {
                            hasLastVisited = true;
                            break;
                        }
                    }
                }

                if (!hasLastVisited)
                {
                    using var alterCmd = conn.CreateCommand();
                    alterCmd.CommandText = "ALTER TABLE \"Users\" ADD COLUMN \"LastVisitedLessonId\" TEXT NOT NULL DEFAULT 'lesson-00-01';";
                    alterCmd.ExecuteNonQuery();
                }
            }
            finally
            {
                if (wasClosed) conn.Close();
            }

            if (!db.Users.Any())
            {
                var defaultUser = new LearnerUser
                {
                    Id = "default-learner",
                    Username = "principal_learner",
                    DisplayName = "Enterprise Engineer",
                    Email = "engineer@enterprise.internal",
                    CurrentStreak = 4,
                    BestStreak = 9,
                    Xp = 620,
                    LastVisitedLessonId = "lesson-00-01",
                    LastActiveDate = System.DateTime.UtcNow
                };
                db.Users.Add(defaultUser);

                db.Progress.AddRange(
                    new LearnerProgress { UserId = defaultUser.Id, LessonId = "lesson-00-01", Completed = true, QuizScore = 100, CompletedAt = System.DateTime.UtcNow.AddDays(-2) },
                    new LearnerProgress { UserId = defaultUser.Id, LessonId = "lesson-00-02", Completed = true, QuizScore = 90, CompletedAt = System.DateTime.UtcNow.AddDays(-1) }
                );

                db.Bookmarks.Add(new LearnerBookmark { UserId = defaultUser.Id, LessonId = "lesson-02-04", CreatedAt = System.DateTime.UtcNow });

                db.SaveChanges();
            }
        }
    }
}
