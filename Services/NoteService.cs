using daytask.Dtos;
using daytask.Exceptions;
using daytask.Models;
using daytask.Repositories;
using System.Threading.Tasks;

namespace daytask.Services
{
    public class NoteService (INoteRepository noteRepository, ILogger<TaskService> logger) : INoteService
    {
        public async Task<ApiResponse<Note>> CreateNoteAsync(NoteRequestDto note)
        {
            var newNote = new Note()
            {
                Id = Guid.NewGuid(),
                Content = note.Content,
                CreatedAt = note.CreatedAt,
                UpdatedAt = note.UpdatedAt,
                UserId = note.UserId
            };
            var success = await noteRepository.CreateNoteAsync(newNote);
            if (!success)
            {
                throw new AppException("Failed to create note");
            }
            logger.LogInformation($"Note created: [{newNote.Id}]");
            return ApiResponse<Note>.SuccessResponse(newNote, "Note created successfully"); ;
        }

        public async Task<ApiResponse<Note>> GetNoteByIdAsync(Guid id)
        {
            var note = await noteRepository.GetNoteByIdAsync(id);
            if (note == null)
            {
                throw new NotFoundException($"Note with ID [{id}] not found");
            }
            logger.LogInformation($"Note [{id}] retrieved successfully");
            return ApiResponse<Note>.SuccessResponse(note);
        }

        public async Task<ApiResponse<IEnumerable<Note>>> GetNotesByUserIdAsync(Guid userId)
        {
            var notes = await noteRepository.GetNotesByUserIdAsync(userId);
            logger.LogInformation($"Notes for user [{userId}] retrieved successfully");
            return ApiResponse<IEnumerable<Note>>.SuccessResponse(notes);
        }

        public async Task<ApiResponse<Note>> UpdateNoteAsync(Guid id, NoteRequestDto note)
        {
            var existingNote = await noteRepository.GetNoteByIdAsync(id);
            if (existingNote == null)
            {
                throw new NotFoundException($"Note with ID {id} not found");
            }
            existingNote.Content = note.Content;
            existingNote.UpdatedAt = note.UpdatedAt;
            var success = await noteRepository.UpdateNoteAsync(existingNote);
            if (!success)
            {
                throw new AppException("Failed to update note");
            }
            logger.LogInformation($"Note [{id}] updated");
            return ApiResponse<Note>.SuccessResponse(existingNote, "Note updated successfully");
        }

        public async Task<ApiResponse<bool>> DeleteNoteAsync(Guid id)
        {
            var note = await noteRepository.GetNoteByIdAsync(id);
            if (note == null)
            {
                throw new NotFoundException($"Note with ID [{id}] not found");
            }
            var success = await noteRepository.RemoveNote(note);
            if (!success)
            {
                throw new AppException("Failed to delete note");
            }
            logger.LogInformation($"Note [{id}] deleted");
            return ApiResponse<bool>.SuccessResponse(true, "Note deleted successfully");
        }
    }
}
