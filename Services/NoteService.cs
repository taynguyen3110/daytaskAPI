using daytask.Dtos;
using daytask.Models;
using daytask.Repositories;

namespace daytask.Services
{
    public class NoteService (INoteRepository noteRepository) : INoteService
    {
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
            await noteRepository.CreateNoteAsync(newNote);
            await noteRepository.SaveChangesAsync();

            return newNote;
        }

        public async Task<bool> DeleteNoteAsync(Guid id)
        {
            var note = await noteRepository.GetNoteByIdAsync(id);
            if (note == null)
                return false;
            noteRepository.RemoveNote(note);
            await noteRepository.SaveChangesAsync();
            return true;
        }

        public async Task<Note> GetNoteByIdAsync(Guid id)
        {
            var note = await noteRepository.GetNoteByIdAsync(id);
            if (note == null)
                return null;
            return note;
        }

        public async Task<IEnumerable<Note>> GetNotesByUserIdAsync(Guid userId)
        {
            var notes = await noteRepository.GetNotesByUserIdAsync(userId);
            return notes.ToArray();
        }

        public async Task<Note> UpdateNoteAsync(Guid id, NoteRequestDto note)
        {

            var existingNote = await noteRepository.GetNoteByIdAsync(id);
            if (existingNote == null)
                return null;
            existingNote.Content = note.Content;
            existingNote.UpdatedAt = note.UpdatedAt;
            await noteRepository.SaveChangesAsync();
            return existingNote;
        }
    }
}
