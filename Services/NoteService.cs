using Microsoft.EntityFrameworkCore;
using TaskFlow.Data;
using TaskFlow.Dtos;
using TaskFlow.Models;

namespace TaskFlow.Services
{
    public class NoteService : INoteService
    {
        private readonly AppDbContext _dbContext;
        public NoteService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Note> CreateNoteAsync(NoteRequestDto note)
        {
            var newNote = new Note()
            {
                Id = Guid.NewGuid(),
                Content = note.Content,
                CreatedAt = note.CreatedAt,
                UpdatedAt = note.UpdatedAt,
                UserId = note.UserId
            };
            _dbContext.Notes.Add(newNote);
            await _dbContext.SaveChangesAsync();

            return newNote;
        }

        public async Task<bool> DeleteNoteAsync(Guid id)
        {
            var note = await _dbContext.Notes.FindAsync(id);
            if (note == null)
                return false;
            _dbContext.Notes.Remove(note);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<Note> GetNoteByIdAsync(Guid id)
        {
            var note = await _dbContext.Notes.FindAsync(id);
            if (note == null)
                return null;
            return note;
        }

        public async Task<IEnumerable<Note>> GetNotesByUserIdAsync(Guid userId)
        {
            var notes = await _dbContext.Notes
                .Where(n => n.UserId == userId)
                .ToListAsync();
            return notes.ToArray();
        }

        public async Task<Note> UpdateNoteAsync(Guid id, NoteRequestDto note)
        {

            var existingNote = await _dbContext.Notes.FindAsync(id);
            if (existingNote == null)
                return null;
            existingNote.Content = note.Content;
            existingNote.UpdatedAt = note.UpdatedAt;
            _dbContext.Notes.Update(existingNote);
            await _dbContext.SaveChangesAsync();
            return existingNote;
        }
    }
}
