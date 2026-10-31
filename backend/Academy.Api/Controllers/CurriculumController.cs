using Microsoft.AspNetCore.Mvc;
using Academy.Api.Services;

namespace Academy.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CurriculumController : ControllerBase
    {
        private readonly ICurriculumService _curriculumService;

        public CurriculumController(ICurriculumService curriculumService)
        {
            _curriculumService = curriculumService;
        }

        [HttpGet("levels")]
        public IActionResult GetLevels()
        {
            var levels = _curriculumService.GetAllLevelSummaries();
            return Ok(levels);
        }

        [HttpGet("levels/{levelId}")]
        public IActionResult GetLevelDetail(string levelId)
        {
            var level = _curriculumService.GetLevelDetail(levelId);
            if (level == null) return NotFound(new { message = $"Level '{levelId}' tidak ditemukan." });
            return Ok(level);
        }

        [HttpGet("lessons/{lessonId}")]
        public IActionResult GetLessonDetail(string lessonId)
        {
            var lesson = _curriculumService.GetLessonDetail(lessonId);
            if (lesson == null) return NotFound(new { message = $"Lesson '{lessonId}' tidak ditemukan." });
            return Ok(lesson);
        }

        [HttpGet("search")]
        public IActionResult Search([FromQuery] string q)
        {
            var results = _curriculumService.Search(q);
            return Ok(results);
        }
    }
}
