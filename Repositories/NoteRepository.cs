using daytask.Data;
using daytask.Models;
using Microsoft.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;

namespace daytask.Repositories
{
    public class NoteRepository(AppDbContext dbContext) : INoteRepository
    {
        public async Task<Note?> GetNoteByIdAsync(Guid noteId)
        {
            return await dbContext.Notes.FindAsync(noteId);
        }

        public async Task<bool> CreateNoteAsync(Note note)
        {
            await dbContext.Notes.AddAsync(note);
            return await SaveChangesAsync();
        }

        public async Task<bool> CreateNotesAsync(IEnumerable<Note> notes)
        {
            await dbContext.Notes.AddRangeAsync(notes);
            return await SaveChangesAsync();
        }

        public async Task<IEnumerable<Note>> GetNotesByUserIdAsync(Guid userId)
        {
            return await dbContext.Notes
                .Where(n => n.UserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Note>> GetExistingNotesByIdsAsync(IEnumerable<Guid> noteIds)
        {
            return await dbContext.Notes
                .Where(n => noteIds.Contains(n.Id))
                .ToListAsync();
        }

        public async Task<bool> UpdateNoteAsync(Note note)
        {
            dbContext.Notes.Update(note);
            return await SaveChangesAsync();
        }

        public async Task<bool> RemoveNote(Note note)
        {
            dbContext.Notes.Remove(note);
            return await SaveChangesAsync();
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await dbContext.SaveChangesAsync() > 0;
        }
    }
}
