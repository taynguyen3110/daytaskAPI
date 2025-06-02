using daytask.Models;
using daytask.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace daytask.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TelegramController (ITelegramService telegramService) : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<ApiResponse<string>>> AddChatId([FromBody] Guid userId, [FromBody] string chatId)
        {
                var response = await telegramService.AddChatIdAsync(userId, chatId);
                return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<string>>> RemoveChatId(Guid id)
        {
            var response = await telegramService.RemoveChatIdAsync(id);
            return Ok(response);
        }
    }
}
