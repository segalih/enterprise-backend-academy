using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Academy.Api.Models;

namespace Academy.Api.Data.Curriculum
{
    public static class CurriculumContentProvider
    {
        private static readonly ConcurrentDictionary<string, LessonDetailDto> _cache = new(StringComparer.OrdinalIgnoreCase);
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true
        };

        private static string? _resolvedLessonsDir;

        public static void ClearCache() => _cache.Clear();

        public static string GetLessonsDirectory()
        {
            if (_resolvedLessonsDir != null && Directory.Exists(_resolvedLessonsDir))
            {
                return _resolvedLessonsDir;
            }

            // 1. Direct candidate paths
            var candidates = new[]
            {
                Path.Combine(AppContext.BaseDirectory, "Content", "lessons"),
                Path.Combine(Directory.GetCurrentDirectory(), "Content", "lessons"),
                Path.Combine(Directory.GetCurrentDirectory(), "backend", "Academy.Api", "Content", "lessons")
            };

            foreach (var candidate in candidates)
            {
                var full = Path.GetFullPath(candidate);
                if (Directory.Exists(full) && Directory.GetFiles(full, "lesson-*.json").Length > 0)
                {
                    _resolvedLessonsDir = full;
                    return full;
                }
            }

            // 2. Climb up parent directories from AppContext.BaseDirectory and CurrentDirectory
            var searchRoots = new[] { AppContext.BaseDirectory, Directory.GetCurrentDirectory() };
            foreach (var root in searchRoots)
            {
                var curr = new DirectoryInfo(root);
                for (int i = 0; i < 6 && curr != null; i++)
                {
                    var opt1 = Path.Combine(curr.FullName, "Content", "lessons");
                    if (Directory.Exists(opt1) && Directory.GetFiles(opt1, "lesson-*.json").Length > 0)
                    {
                        _resolvedLessonsDir = Path.GetFullPath(opt1);
                        return _resolvedLessonsDir;
                    }

                    var opt2 = Path.Combine(curr.FullName, "backend", "Academy.Api", "Content", "lessons");
                    if (Directory.Exists(opt2) && Directory.GetFiles(opt2, "lesson-*.json").Length > 0)
                    {
                        _resolvedLessonsDir = Path.GetFullPath(opt2);
                        return _resolvedLessonsDir;
                    }

                    curr = curr.Parent;
                }
            }

            // Fallback default
            _resolvedLessonsDir = Path.Combine(AppContext.BaseDirectory, "Content", "lessons");
            return _resolvedLessonsDir;
        }

        public static LessonDetailDto? GetLessonDetail(string lessonId)
        {
            if (string.IsNullOrWhiteSpace(lessonId)) return null;

            if (_cache.TryGetValue(lessonId, out var cached))
            {
                return cached;
            }

            var dir = GetLessonsDirectory();
            var filePath = Path.Combine(dir, $"{lessonId}.json");

            if (!File.Exists(filePath))
            {
                return null;
            }

            try
            {
                var json = File.ReadAllText(filePath);
                var detail = JsonSerializer.Deserialize<LessonDetailDto>(json, _jsonOptions);

                if (detail != null)
                {
                    // Compute previous and next lesson ids based on curriculum catalog ordering
                    ComputeNavigationLinks(detail);
                    _cache[lessonId] = detail;
                }

                return detail;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CurriculumContentProvider] Error loading lesson {lessonId}: {ex.Message}");
                return null;
            }
        }

        private static void ComputeNavigationLinks(LessonDetailDto detail)
        {
            var allLevels = CurriculumCatalog.GetAllLevels();
            var flatLessons = allLevels.SelectMany(l => l.Modules.SelectMany(m => m.Lessons)).ToList();
            int currentIndex = flatLessons.FindIndex(l => l.Id.Equals(detail.Id, StringComparison.OrdinalIgnoreCase));

            if (currentIndex >= 0)
            {
                detail.PreviousLessonId = currentIndex > 0 ? flatLessons[currentIndex - 1].Id : null;
                detail.NextLessonId = currentIndex < flatLessons.Count - 1 ? flatLessons[currentIndex + 1].Id : null;
            }
        }

        public static List<LessonDetailDto> GetAllLoadedLessons()
        {
            var dir = GetLessonsDirectory();
            if (!Directory.Exists(dir)) return new List<LessonDetailDto>();

            var list = new List<LessonDetailDto>();
            foreach (var file in Directory.GetFiles(dir, "lesson-*.json"))
            {
                var id = Path.GetFileNameWithoutExtension(file);
                var detail = GetLessonDetail(id);
                if (detail != null)
                {
                    list.Add(detail);
                }
            }
            return list;
        }
    }
}
