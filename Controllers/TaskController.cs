using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using daytask.Services;
using daytask.Dtos;
using daytask.Models;
using System.Security.Claims;

namespace daytask.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaskController(ITaskService taskService, ILogger<TaskController> logger) : ControllerBase
    {
        [HttpGet]
        public async Task<ApiResponse<IEnumerable<UserTask>>> GetAllTasks()
        {
            var response = await taskService.GetAllTasksAsync();
            return response;
        }

        [HttpGet("{id}")]
        public async Task<ApiResponse<UserTask>> GetTaskById(Guid id)
        {
            var response = await taskService.GetTaskByIdAsync(id);
            return response;
        }

        [HttpGet("user/{id}")]
        public async Task<ApiResponse<IEnumerable<UserTask>>> GetTasksByUserId(Guid id)
        {
            var response = await taskService.GetTasksByUserIdAsync(id);
            return response;
        }

        [HttpPost]
        public async Task<ApiResponse<UserTask>> CreateTask(CreateTaskDto taskDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var response = await taskService.CreateTaskAsync(taskDto, userId);
            return response;
        }

        [HttpPost("bulk")]
        public async Task<ApiResponse<IEnumerable<UserTask>>> CreateTasks(IEnumerable<CreateTaskDto> taskDtos)
        {
            var response = await taskService.CreateTasksAsync(taskDtos);
            var taskDtoList = taskDtos.ToList();
            return response;
        }

        [HttpPost("merge")]
        public async Task<ApiResponse<bool>> MergeTasks(UserTask[] tasks)
        {
            var response = await taskService.MergeTasksAsync(tasks);
            return response;
        }

        [HttpPut("{id}")]
        public async Task<ApiResponse<UserTask>> UpdateTask(Guid id, UpdateTaskDto taskDto)
        {
            var response = await taskService.UpdateTaskAsync(id, taskDto);
            return response;
        }

        [HttpDelete("{id}")]
        public async Task<ApiResponse<bool>> DeleteTask(Guid id)
        {
            var response = await taskService.DeleteTaskAsync(id);
            return response;
        }
    }
}
