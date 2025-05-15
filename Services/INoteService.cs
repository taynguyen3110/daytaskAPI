using daytask.Dtos;
using daytask.Models;

namespace daytask.Services
{
    public interface INoteService
    {
        Task<IEnumerable<Note>> GetNotesByUserIdAsync(Guid userId);
        Task<Note> GetNoteByIdAsync(Guid id);
        Task<Note> CreateNoteAsync(NoteRequestDto note);
        Task<Note> UpdateNoteAsync(Guid id, NoteRequestDto note);
        Task<bool> DeleteNoteAsync(Guid id);
    }
}
