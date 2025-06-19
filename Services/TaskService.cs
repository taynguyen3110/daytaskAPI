using daytask.Dtos;
using daytask.Exceptions;
using daytask.Models;
using daytask.Repositories;
using System.Threading.Tasks;

namespace daytask.Services
{
    public class TaskService (ITaskRepository taskRepository, IUserRepository userRepository, IReminderService reminderService, ILogger<TaskService> logger) : ITaskService
    {
        public async Task<ApiResponse<UserTask>> CreateTaskAsync(CreateTaskDto taskDto, string userId)
        {
            var task = new UserTask
            {
                Id = taskDto.Id,
                Title = taskDto.Title,
                Description = taskDto.Description,
                DueDate = taskDto.DueDate,
                Priority = taskDto.Priority,
                Labels = taskDto.Labels,
                CreatedAt = taskDto.CreatedAt,
                UpdatedAt = taskDto.CreatedAt,
                Recurrence = taskDto.Recurrence,
                Reminder = taskDto.Reminder,
                UserId = taskDto.UserId
            };
            var success = await taskRepository.CreateTaskAsync(task);
            if (!success)
            {
                throw new AppException("Failed to create task");
            }
            logger.LogInformation($"Task created: {task.Id}");
            await reminderService.CreateReminderAsync(task);
            return ApiResponse<UserTask>.SuccessResponse(task, "Task created successfully");
        }

        public async Task<ApiResponse<IEnumerable<UserTask>>> CreateTasksAsync(IEnumerable<CreateTaskDto> tasks)
        {
            var chatId = await userRepository.GetChatIdByUserIdAsync(tasks.First().UserId);
            var userTasks = tasks.Select(taskDto => {
                return new UserTask
                {
                    Id = taskDto.Id,
                    Title = taskDto.Title,
                    Description = taskDto.Description,
                    DueDate = taskDto.DueDate,
                    Priority = taskDto.Priority,
                    Labels = taskDto.Labels,
                    CreatedAt = taskDto.CreatedAt,
                    UpdatedAt = taskDto.CreatedAt,
                    Recurrence = taskDto.Recurrence,
                    Reminder = taskDto.Reminder,
                    UserId = taskDto.UserId
                };
            }
            ).ToArray();

            var success = await taskRepository.CreateTasksAsync(userTasks);
            if (!success)
            {
                throw new AppException("Failed to create tasks");
            }
            logger.LogInformation($"Tasks created: {string.Join(" ,",userTasks.Select(t => t.Id))}");
            await reminderService.CreateRemindersAsync(userTasks);
            return ApiResponse<IEnumerable<UserTask>>.SuccessResponse(userTasks, "Tasks created successfully");
        }

        public async Task<ApiResponse<IEnumerable<UserTask>>> GetAllTasksAsync()
        {
            var tasks = await taskRepository.GetAllTasksAsync();
            logger.LogInformation($"Tasks retrieved successfully");
            return ApiResponse<IEnumerable<UserTask>>.SuccessResponse(tasks);
        }

        public async Task<ApiResponse<UserTask>> GetTaskByIdAsync(Guid id)
        {
            var task = await taskRepository.GetTaskByIdAsync(id);
            if (task == null)
            {
                throw new NotFoundException($"Task with ID [{id}] not found");
            }
            logger.LogInformation($"Task [{id}] retrieved successfully");
            return ApiResponse<UserTask>.SuccessResponse(task);
        }

        public async Task<ApiResponse<IEnumerable<UserTask>>> GetTasksByUserIdAsync(Guid userId)
        {
            var tasks = await taskRepository.GetTasksByUserIdAsync(userId);
            logger.LogInformation($"Tasks for user [{userId}] retrieved successfully");
            return ApiResponse<IEnumerable<UserTask>>.SuccessResponse(tasks);
        }

        public async Task<ApiResponse<UserTask>> UpdateTaskAsync(Guid id, UpdateTaskDto taskDto)
        {
            var task = await taskRepository.GetTaskByIdAsync(id);
            if (task == null)
            {
                throw new NotFoundException($"Task with ID [{id}] not found");
            }

            task.Title = taskDto.Title;
            task.Description = taskDto.Description;
            task.Completed = taskDto.Completed;
            task.DueDate = taskDto.DueDate;
            task.Priority = taskDto.Priority;
            task.Labels = taskDto.Labels;
            task.UpdatedAt = taskDto.UpdatedAt;
            task.CompletedAt = taskDto.CompletedAt;
            task.Recurrence = taskDto.Recurrence;
            task.Reminder = taskDto.Reminder;
            task.SnoozedUntil = taskDto.SnoozedUntil;

            var success = await taskRepository.UpdateTaskAsync(task);
            if (!success)
            {
                throw new AppException("Failed to update task");
            }

            await reminderService.UpdateReminderAsync(task);
            logger.LogInformation($"Task [{id}] updated");
            return ApiResponse<UserTask>.SuccessResponse(task, "Task updated successfully");
        }

        public async Task<ApiResponse<bool>> MergeTasksAsync(UserTask[] tasks)
        {
            var taskIds = tasks.Select(t => t.Id).ToArray();
            var existingTasks = await taskRepository.GetExistingTasksByIdsAsync(taskIds);
            var existingTaskDict = existingTasks.ToDictionary(t => t.Id);
            var hasChange = false;
            var tasksToCreate = new List<UserTask>();
            var tasksToUpdate = new List<UserTask>(); //to log out task Ids
            foreach (var task in tasks)
            {
                if (existingTaskDict.TryGetValue(task.Id, out var trackedTask))
                {
                    if (task.UpdatedAt > trackedTask.UpdatedAt)
                    {   
                        tasksToUpdate.Add(task);
                        hasChange = true;
                        trackedTask.Title = task.Title;
                        trackedTask.Description = task.Description;
                        trackedTask.DueDate = task.DueDate;
                        trackedTask.Completed = task.Completed;
                        trackedTask.DueDate = task.DueDate; 
                        trackedTask.Priority = task.Priority;
                        trackedTask.Labels = task.Labels;
                        trackedTask.UpdatedAt = task.UpdatedAt;
                        trackedTask.CompletedAt = task.CompletedAt;
                        trackedTask.Recurrence = task.Recurrence;
                        trackedTask.Reminder = task.Reminder;
                        trackedTask.SnoozedUntil = task.SnoozedUntil;
                    }
                }
                else
                {
                    tasksToCreate.Add(task);
                }
            }

            var result = false;

            if (tasksToCreate.Count > 0)
            {
                result = await taskRepository.CreateTasksAsync(tasksToCreate);
                if (!result)
                {
                    logger.LogError("Failed to create new tasks during merge operation");
                }
                await reminderService.CreateRemindersAsync(tasksToCreate);
            } 
            else if (hasChange)
            {
                result = await taskRepository.SaveChangesAsync();
                if (!result)
                {
                    logger.LogError("Failed to update existing tasks during merge operation");
                }
                await reminderService.UpdateRemindersAsync(tasksToUpdate);
            } 
            else
            {
                result = true;
            }
            if (!result)
            {
                throw new AppException("Failed to merge tasks");
            }
            logger.LogInformation($"Tasks merged successfully, tasks created: {string.Join(" ,", tasksToCreate.Select(t => t.Id))}. Tasks updated: {string.Join(" ,", tasksToCreate.Select(t => t.Id))} ");
            return ApiResponse<bool>.SuccessResponse(true, "Tasks merged successfully");
        }

        public async Task<ApiResponse<bool>> DeleteTaskAsync(Guid id)
        {
            var task = await taskRepository.GetTaskByIdAsync(id);
            if (task == null)
            {
                throw new NotFoundException($"Task with ID [{id}] not found");
            }

            await reminderService.DeleteReminderAsync(id.ToString());

            var success = await taskRepository.RemoveTaskAsync(task);
            if (!success)
            {
                throw new AppException("Failed to delete task");
            }
            logger.LogInformation($"Tasks deleted [{id}]");
            return ApiResponse<bool>.SuccessResponse(true, "Task deleted successfully");
        }
    }
}
