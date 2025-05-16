using daytask.Data;
using daytask.Models;
using Microsoft.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;

namespace daytask.Repositories
{
    public class NoteRepository (AppDbContext dbContext) : INoteRepository
    {
        public async Task CreateNoteAsync(Note note)
        {
            await dbContext.Notes.AddAsync(note);
        }
        public async Task<Note?> GetNoteByIdAsync(Guid noteId)
        {
            return await dbContext.Notes.FindAsync(noteId);
        }
        public async Task<IEnumerable<Note>> GetNotesByUserIdAsync(Guid userId)
        {
            return await dbContext.Notes
                .Where(n => n.UserId == userId)
                .ToListAsync();
        }
        public void RemoveNote(Note note)
        {
            dbContext.Notes.Remove(note);
        }
        public async Task SaveChangesAsync()
        {
            await dbContext.SaveChangesAsync();
        }
    }
}
