using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Dtos;
using TaskFlow.Models;
using TaskFlow.Services;

namespace TaskFlow.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NoteController(INoteService noteService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<Note[]>> GetNotesByUserId (Guid userId)
        {
            var notes = await noteService.GetNotesByUserIdAsync(userId);
            if (notes == null || !notes.Any())
            {
                return NotFound("No notes found for this user.");
            }
            return Ok(notes);
        }

        [HttpGet("id")]
        public async Task<ActionResult<Note>> GetNoteById(Guid id)
        {
            var note = await noteService.GetNoteByIdAsync(id);
            if (note == null)
            {
                return NotFound("Note not found.");
            }
            return Ok(note);
        }

        [HttpPost]
        public async Task<ActionResult<Note>> CreateNote([FromBody] NoteRequestDto note)
        {
            if (note == null)
            {
                return BadRequest("Note data is null.");
            }
            var createdNote = await noteService.CreateNoteAsync(note);
            if (createdNote == null)
            {
                return BadRequest("Failed to create note.");
            }
            return CreatedAtAction(nameof(GetNoteById), new { id = createdNote.Id }, createdNote);
        }

        [HttpPut("id")]
        public async Task<ActionResult<Note>> UpdateNote(Guid id, [FromBody] NoteRequestDto note)
        {
            if (note == null)
            {
                return BadRequest("Note data is null.");
            }
            var updatedNote = await noteService.UpdateNoteAsync(id, note);
            if (updatedNote == null)
            {
                return NotFound("Failed to update note.");
            }
            return Ok(updatedNote);
        }

        [HttpDelete("id")]
        public async Task<IActionResult> DeleteNote(Guid id)
        {
            var result = await noteService.DeleteNoteAsync(id);
            if (!result)
            {
                return NotFound("Failed to delete note.");
            }
            return NoContent();
        }
    }
}
