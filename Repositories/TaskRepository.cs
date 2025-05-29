using daytask.Data;
using daytask.Dtos;
using daytask.Models;
using Microsoft.EntityFrameworkCore;

namespace daytask.Repositories
{
    public class TaskRepository (AppDbContext dbContext) : ITaskRepository
    {
        public async Task<UserTask?> GetTaskByIdAsync(Guid id)
        {
            return await dbContext.Tasks.FindAsync(id);
        }

        public async Task<IEnumerable<UserTask>> GetAllTasksAsync()
        {
            return await dbContext.Tasks.ToListAsync();
        }

        public async Task<IEnumerable<UserTask>> GetTasksByUserIdAsync(Guid userId)
        {
            return await dbContext.Tasks
                .Where(t => t.UserId == userId)
                .ToListAsync();
        }

        public async Task<bool> MergeTasksAsync(UserTask[] taskDtos)
        {
            try
            {
                var taskIds = taskDtos.Select(t => t.Id).ToArray();
                var existingTasks = await dbContext.Tasks
                .Where(t => taskIds.Contains(t.Id))
                .ToDictionaryAsync(t => t.Id);

                foreach (var task in taskDtos)
                {
                    if (existingTasks.TryGetValue(task.Id, out var existing))
                    {
                        existing.Title = task.Title;
                        existing.Description = task.Description;
                        existing.Completed = task.Completed;
                        existing.DueDate = task.DueDate;
                        existing.Priority = task.Priority;
                        existing.Labels = task.Labels;
                        existing.UpdatedAt = task.UpdatedAt;
                        existing.CompletedAt = task.CompletedAt;
                        existing.Recurrence = task.Recurrence;
                        existing.Reminder = task.Reminder;
                        existing.SnoozedUntil = task.SnoozedUntil;

                        dbContext.Tasks.Update(existing);
                    }
                    else
                    {
                        var newTask = new UserTask
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
                            SnoozedUntil = task.SnoozedUntil,
                            UserId = task.UserId
                        };
                        dbContext.Tasks.Add(newTask);
                    }
                }

                await dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> CreateTaskAsync(UserTask task)
        {
            await dbContext.Tasks.AddAsync(task);
            return await SaveChangesAsync();
        }

        public async Task<bool> CreateTasksAsync(IEnumerable<UserTask> tasks)
        {
            await dbContext.Tasks.AddRangeAsync(tasks);
            return await SaveChangesAsync();
        }

        public async Task<bool> UpdateTaskAsync(UserTask task)
        {
            dbContext.Tasks.Update(task);
            return await SaveChangesAsync();
        }

        public async Task<bool> RemoveTaskAsync(UserTask task)
        {
            dbContext.Tasks.Remove(task);
            return await SaveChangesAsync();
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await dbContext.SaveChangesAsync() > 0;
        }
    }
}
