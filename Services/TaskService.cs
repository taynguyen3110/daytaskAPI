using daytask.Dtos;
using daytask.Models;
using daytask.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace daytask.Services
{
    public class TaskService : ITaskService
    {
        private readonly AppDbContext _dbContext;
        public TaskService(AppDbContext context)
        {
            _dbContext = context;
        }
        public async Task<TaskResponseDto> CreateTaskAsync(TaskDto taskDto)
        {
            var task = new Models.Task
            {
                Id = Guid.NewGuid(),
                Title = taskDto.Title,
                Description = taskDto.Description,
                Completed = taskDto.Completed,
                DueDate = taskDto.DueDate,
                Priority = taskDto.Priority,
                Labels = taskDto.Labels,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Recurrence = taskDto.Recurrence,
                Reminder = taskDto.Reminder,
                SnoozedUntil = taskDto.SnoozedUntil,
                UserId = taskDto.UserId
            };

            _dbContext.Tasks.Add(task);
            await _dbContext.SaveChangesAsync();

            return new TaskResponseDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Completed = task.Completed,
                DueDate = task.DueDate,
                Priority = task.Priority,
                Labels = task.Labels,
                CreatedAt = task.CreatedAt,
                UpdatedAt = task.UpdatedAt,
                CompletedAt = task.CompletedAt,
                Recurrence = task.Recurrence,
                Reminder = task.Reminder,
                SnoozedUntil = task.SnoozedUntil
            };
        }

        public async Task<bool> DeleteTaskAsync(Guid taskId)
        {
            var task = await _dbContext.Tasks.FindAsync(taskId);
            if (task == null)
            {
                return false;
            }
            _dbContext.Tasks.Remove(task);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<TaskResponseDto>> GetAllTasksAsync()
        {
            
            var tasks = await _dbContext.Tasks.ToListAsync();
            return tasks.Select(t => new TaskResponseDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                Completed = t.Completed,
                DueDate = t.DueDate,
                Priority = t.Priority,
                Labels = t.Labels,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt,
                CompletedAt = t.CompletedAt,
                Recurrence = t.Recurrence,
                Reminder = t.Reminder,
                SnoozedUntil = t.SnoozedUntil,
            });
        }

        public async Task<TaskResponseDto> GetTaskByIdAsync(Guid taskId)
        {
            var task = await _dbContext.Tasks.FindAsync(taskId);
            if (task == null)
            {
                return null;
            }
            return new TaskResponseDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Completed = task.Completed,
                DueDate = task.DueDate,
                Priority = task.Priority,
                Labels = task.Labels,
                CreatedAt = task.CreatedAt,
                UpdatedAt = task.UpdatedAt,
                CompletedAt = task.CompletedAt,
                Recurrence = task.Recurrence,
                Reminder = task.Reminder,
                SnoozedUntil = task.SnoozedUntil
            };
        }

        public async Task<IEnumerable<TaskResponseDto>> GetTasksByUserIdAsync(Guid userId)
        {

            var tasks = await _dbContext.Tasks
                .Where(t => t.UserId == userId)
                .ToListAsync();
            return tasks.Select(t => new TaskResponseDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                Completed = t.Completed,
                DueDate = t.DueDate,
                Priority = t.Priority,
                Labels = t.Labels,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt,
                CompletedAt = t.CompletedAt,
                Recurrence = t.Recurrence,
                Reminder = t.Reminder,
                SnoozedUntil = t.SnoozedUntil
            });
        }

         public async Task<TaskResponseDto> UpdateTaskAsync(Guid taskId, TaskDto taskDto)
        {

            var task = await _dbContext.Tasks.FindAsync(taskId);
            if (task == null)
            {
                return null;
            }
            task.Title = taskDto.Title;
            task.Description = taskDto.Description;
            task.Completed = taskDto.Completed;
            task.DueDate = taskDto.DueDate;
            task.Priority = taskDto.Priority;
            task.Labels = taskDto.Labels;
            task.UpdatedAt = DateTime.UtcNow;
            task.Recurrence = taskDto.Recurrence;
            task.Reminder = taskDto.Reminder;
            task.SnoozedUntil = taskDto.SnoozedUntil;

            await _dbContext.SaveChangesAsync();

            return new TaskResponseDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Completed = task.Completed,
                DueDate = task.DueDate,
                Priority = task.Priority,
                Labels = task.Labels,
                CreatedAt = task.CreatedAt,
                UpdatedAt = DateTime.UtcNow,
                CompletedAt = task.CompletedAt,
                Recurrence = task.Recurrence,
                Reminder = task.Reminder,
                SnoozedUntil = task.SnoozedUntil
            };
        }
    }
}
