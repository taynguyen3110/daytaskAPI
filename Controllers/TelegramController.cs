using daytask.Dtos;
using daytask.Models;
using daytask.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace daytask.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TelegramController(ITelegramService telegramService) : ControllerBase
    {
        [HttpPost]
        public async Task<ApiResponse<string>> AddChatId([FromBody] AddChatIDRequestDto addChatIdRequestDto)
        {
            var response = await telegramService.AddChatIdAsync(addChatIdRequestDto);
            return response;
        }

        [HttpDelete("{id}")]
        public async Task<ApiResponse<string>> RemoveChatId(Guid id)
        {
            var response = await telegramService.RemoveChatIdAsync(id);
            return response;
        }

        //[HttpPost("send-message")]
        //public async Task<ActionResult<ApiResponse<bool>>> SendMessage([FromQuery] string chatId, [FromQuery] string message)
        //{
        //    var response = await telegramService.SendMessageAsync(chatId, message);
        //    return Ok(response);
        //}
    }
}
