using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Academy.Api.Services;

namespace Academy.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TerminalController : ControllerBase
    {
        private readonly ITerminalSandboxService _terminalSandbox;

        public TerminalController(ITerminalSandboxService terminalSandbox)
        {
            _terminalSandbox = terminalSandbox;
        }

        [HttpPost("command")]
        public async Task<IActionResult> ExecuteCommand([FromBody] TerminalCommandRequest request)
        {
            var response = await _terminalSandbox.ExecuteCommandAsync(request);
            return Ok(response);
        }

        [HttpPost("reset")]
        public IActionResult Reset([FromBody] TerminalCommandRequest request)
        {
            _terminalSandbox.ResetSession(request.SessionId);
            return Ok(new { message = $"Sesi sandbox '{request.SessionId}' berhasil di-reset." });
        }
    }
}
