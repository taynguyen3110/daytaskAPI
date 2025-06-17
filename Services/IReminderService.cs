using daytask.Dtos;
using daytask.Models;

namespace daytask.Services
{
    public interface IReminderService
    {
        Task CreateReminderAsync(UserTask task);
        Task CreateRemindersAsync(IEnumerable<UserTask> tasks);
        Task UpdateReminderAsync(UserTask task);
        Task UpdateRemindersAsync(IEnumerable<UserTask> tasks);
        Task DeleteReminderAsync(string taskId);
    }
}
