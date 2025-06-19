using daytask.Dtos;
using daytask.Exceptions;
using daytask.Models;
using daytask.Repositories;
using System.Threading.Tasks;

namespace daytask.Services
{
    public class NoteService (INoteRepository noteRepository, ILogger<NoteService> logger) : INoteService
    {
        public async Task<ApiResponse<Note>> CreateNoteAsync(Note note)
        {
            var newNote = new Note()
            {
                Id = note.Id,
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

        public async Task<ApiResponse<bool>> MergeNotesAsync(Note[] notes)
        {
            var noteIds = notes.Select(t => t.Id).ToArray();
            var existingNotes = await noteRepository.GetExistingNotesByIdsAsync(noteIds);
            var existingNoteDict = existingNotes.ToDictionary(t => t.Id);
            var hasChange = false;
            var notesToCreate = new List<Note>();
            var notesToUpdate = new List<Note>(); //to log out note Ids
            foreach (var note in notes)
            {
                if (existingNoteDict.TryGetValue(note.Id, out var trackedNote))
                {
                    if (note.UpdatedAt > trackedNote.UpdatedAt)
                    {
                        notesToUpdate.Add(note);
                        hasChange = true;
                        trackedNote.Content = note.Content;
                        trackedNote.UpdatedAt = note.UpdatedAt;
                    }
                }
                else
                {
                    notesToCreate.Add(note);
                }
            }

            var result = false;

            if (notesToCreate.Count > 0)
            {
                result = await noteRepository.CreateNotesAsync(notesToCreate);
                if (!result)
                {
                    logger.LogError("Failed to create new notes during merge operation");
                }
            }
            else if (hasChange)
            {
                result = await noteRepository.SaveChangesAsync();
                if (!result)
                {
                    logger.LogError("Failed to update existing notes during merge operation");
                }
            }
            else
            {
                result = true;
            }
            if (!result)
            {
                throw new AppException("Failed to merge notes");
            }
            logger.LogInformation($"Notes merged successfully, notes created: {string.Join(" ,", notesToCreate.Select(t => t.Id))}. Notes updated: {string.Join(" ,", notesToUpdate.Select(t => t.Id))} ");
            return ApiResponse<bool>.SuccessResponse(true, "Notes merged successfully");
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
