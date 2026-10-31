using Microsoft.AspNetCore.Mvc;
using Academy.Api.Services;

namespace Academy.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly ICurriculumService _curriculumService;

        public ProjectsController(ICurriculumService curriculumService)
        {
            _curriculumService = curriculumService;
        }

        [HttpGet]
        public IActionResult GetProjects()
        {
            var projects = _curriculumService.GetAllProjects();
            return Ok(projects);
        }

        [HttpGet("{projectId}")]
        public IActionResult GetProjectDetail(string projectId)
        {
            var project = _curriculumService.GetProjectDetail(projectId);
            if (project == null) return NotFound(new { message = $"Project '{projectId}' tidak ditemukan." });
            return Ok(project);
        }
    }
}
