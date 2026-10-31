using System;
using System.Collections.Generic;
using System.Linq;
using Academy.Api.Data.Curriculum;
using Academy.Api.Models;

namespace Academy.Api.Services
{
    public interface ICurriculumService
    {
        List<LevelSummaryDto> GetAllLevelSummaries();
        LevelDetailDto? GetLevelDetail(string levelId);
        LessonDetailDto? GetLessonDetail(string lessonId);
        List<SearchResultDto> Search(string query);
        List<ProjectSpecificationDto> GetAllProjects();
        ProjectSpecificationDto? GetProjectDetail(string projectId);
    }

    public class SearchResultDto
    {
        public string LessonId { get; set; } = string.Empty;
        public string LevelId { get; set; } = string.Empty;
        public string LevelTitle { get; set; } = string.Empty;
        public string ModuleTitle { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string MatchContext { get; set; } = string.Empty;
        public string Difficulty { get; set; } = "Beginner";
    }

    public class CurriculumService : ICurriculumService
    {
        private readonly List<LevelDetailDto> _levels;
        private readonly List<ProjectSpecificationDto> _projects;

        public CurriculumService()
        {
            _levels = CurriculumCatalog.GetAllLevels();
            _projects = ProjectsDataRepository.GetAllProjects();
        }

        public List<LevelSummaryDto> GetAllLevelSummaries()
        {
            return _levels.Select(CurriculumCatalog.ToSummary).ToList();
        }

        public LevelDetailDto? GetLevelDetail(string levelId)
        {
            return _levels.FirstOrDefault(l => l.Id.Equals(levelId, StringComparison.OrdinalIgnoreCase));
        }

        public LessonDetailDto? GetLessonDetail(string lessonId)
        {
            return CurriculumContentProvider.GetLessonDetail(lessonId);
        }

        public List<ProjectSpecificationDto> GetAllProjects()
        {
            return _projects;
        }

        public ProjectSpecificationDto? GetProjectDetail(string projectId)
        {
            return _projects.FirstOrDefault(p => p.Id.Equals(projectId, StringComparison.OrdinalIgnoreCase));
        }

        public List<SearchResultDto> Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
            {
                return new List<SearchResultDto>();
            }

            var q = query.Trim().ToLowerInvariant();
            var results = new List<SearchResultDto>();

            foreach (var lvl in _levels)
            {
                foreach (var mod in lvl.Modules)
                {
                    foreach (var les in mod.Lessons)
                    {
                        bool titleMatch = les.Title.ToLowerInvariant().Contains(q);
                        bool topicMatch = lvl.KeyTopics.Any(t => t.ToLowerInvariant().Contains(q));
                        bool modMatch = mod.Title.ToLowerInvariant().Contains(q);

                        if (titleMatch || topicMatch || modMatch)
                        {
                            string context = titleMatch
                                ? $"Cocok pada judul materi pelajaran ({les.Title})"
                                : topicMatch
                                    ? $"Cocok pada topik terkait kurikulum {lvl.Title}"
                                    : $"Cocok pada modul {mod.Title}";

                            results.Add(new SearchResultDto
                            {
                                LessonId = les.Id,
                                LevelId = lvl.Id,
                                LevelTitle = lvl.Title,
                                ModuleTitle = mod.Title,
                                Title = les.Title,
                                Difficulty = les.Difficulty,
                                MatchContext = context
                            });
                        }
                    }
                }
            }

            return results.Take(25).ToList();
        }
    }
}
