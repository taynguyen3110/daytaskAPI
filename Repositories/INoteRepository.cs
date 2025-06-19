using daytask.Models;
using Task = System.Threading.Tasks.Task;

namespace daytask.Repositories
{
    public interface INoteRepository
    {
        Task<bool> CreateNoteAsync(Note note);
        Task<Note?> GetNoteByIdAsync(Guid noteId);
        Task<IEnumerable<Note>> GetNotesByUserIdAsync(Guid userId);
        Task<bool> UpdateNoteAsync(Note note);
        Task<bool> RemoveNote(Note note);
        Task<bool> SaveChangesAsync();
    }
}
