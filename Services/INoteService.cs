using daytask.Dtos;
using daytask.Models;

namespace daytask.Services
{
    public interface INoteService
    {
        Task<ApiResponse<IEnumerable<Note>>> GetNotesByUserIdAsync(Guid userId);
        Task<ApiResponse<Note>> GetNoteByIdAsync(Guid id);
        Task<ApiResponse<Note>> CreateNoteAsync(NoteRequestDto note);
        Task<ApiResponse<Note>> UpdateNoteAsync(Guid id, NoteRequestDto note);
        Task<ApiResponse<bool>> DeleteNoteAsync(Guid id);
    }
}
