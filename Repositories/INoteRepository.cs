using daytask.Models;
using Task = System.Threading.Tasks.Task;

namespace daytask.Repositories
{
    public interface INoteRepository
    {
        Task CreateNoteAsync(Note note);
        Task<Note?> GetNoteByIdAsync(Guid noteId);
        Task<IEnumerable<Note>> GetNotesByUserIdAsync(Guid userId);
        void RemoveNote(Note note);
        Task SaveChangesAsync();
    }
}
