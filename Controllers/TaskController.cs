using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Dtos;
using TaskFlow.Models;
using TaskFlow.Services;

namespace TaskFlow.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController(ITaskService taskService) : ControllerBase
    {

        [HttpGet]
        public async Task<ActionResult<TaskResponseDto[]>> GetAllTasks()
        {
            var response = await taskService.GetAllTasksAsync();
            if (response == null || !response.Any())
            {
                return BadRequest("No tasks found.");
            }
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TaskResponseDto>> GetTaskById(Guid id)
        {
            var response = await taskService.GetTaskByIdAsync(id);
            if (response == null)
            {
                return NotFound("Task not found.");
            }
            return Ok(response);
        }

        [HttpGet("user/{id}")]
        public async Task<ActionResult<TaskResponseDto[]>> GetTasksByUserId(Guid id)
        {
            var response = await taskService.GetTasksByUserIdAsync(id);
            if (response == null || !response.Any())
                return NotFound("No tasks found for this user.");
            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<TaskResponseDto>> CreateTask([FromBody] TaskDto taskDto)
        {
            if (taskDto == null)
            {
                return BadRequest("Task data is null.");
            }
            var response = await taskService.CreateTaskAsync(taskDto);
            if (response == null)
            {
                return BadRequest("Failed to create task.");
            }
            return CreatedAtAction(nameof(GetTaskById), new { id = response.Id }, response);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TaskResponseDto>> UpdateTask(Guid id, [FromBody] TaskDto taskDto)
        {
            if (taskDto == null)
                return BadRequest("Task data is null.");
            var response = await taskService.UpdateTaskAsync(id, taskDto);
            if (response == null)
                return NotFound("Task not found.");
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> DeleteTask(Guid id)
        {
            var response = await taskService.DeleteTaskAsync(id);
            if (!response)
                return NotFound("Task not found.");
            return Ok(response);
        }
    }
}
