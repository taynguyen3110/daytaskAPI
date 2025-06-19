using Azure;
using daytask.Dtos;
using daytask.Models;
using daytask.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace daytask.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NoteController(INoteService noteService) : ControllerBase
    {
        [HttpGet]
        public async Task<ApiResponse<IEnumerable<Note>>> GetNotesByUserId(Guid userId)
        {
            var response = await noteService.GetNotesByUserIdAsync(userId);
            return response;
        }

        [HttpGet("{id}")]
        public async Task<ApiResponse<Note>> GetNoteById(Guid id)
        {
            var response = await noteService.GetNoteByIdAsync(id);
            return response;
        }

        [HttpPost]
        public async Task<ApiResponse<Note>> CreateNote([FromBody] NoteRequestDto note)
        {
            var response = await noteService.CreateNoteAsync(note);
            return response;
        }

        [HttpPut("{id}")]
        public async Task<ApiResponse<Note>> UpdateNote(Guid id, [FromBody] NoteRequestDto note)
        {
            var response = await noteService.UpdateNoteAsync(id, note);
            return response;
        }

        [HttpDelete("{id}")]
        public async Task<ApiResponse<Boolean>> DeleteNote(Guid id)
        {
            var response = await noteService.DeleteNoteAsync(id);
            return response;
        }
    }
}
