﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using daytask.Services;
using daytask.Dtos;
using daytask.Models;

namespace daytask.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaskController(ITaskService taskService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<UserTask>>>> GetAllTasks()
        {
            var response = await taskService.GetAllTasksAsync();
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<UserTask>>> GetTaskById(Guid id)
        {
            var response = await taskService.GetTaskByIdAsync(id);
            return Ok(response);
        }

        [HttpGet("user/{id}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<UserTask>>>> GetTasksByUserId(Guid id)
        {
            var response = await taskService.GetTasksByUserIdAsync(id);
            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<UserTask>>> CreateTask(CreateTaskDto taskDto)
        {
            var response = await taskService.CreateTaskAsync(taskDto);
            return CreatedAtAction(nameof(GetTaskById), new { id = response.Data?.Id }, response);
        }

        [HttpPost("bulk")]
        public async Task<ActionResult<ApiResponse<IEnumerable<UserTask>>>> CreateTasks(IEnumerable<CreateTaskDto> taskDtos)
        {
            var response = await taskService.CreateTasksAsync(taskDtos);
            var taskDtoList = taskDtos.ToList();
            return CreatedAtAction(nameof(GetTasksByUserId), new { id = taskDtoList[0].UserId }, response);
        }

        [HttpPost("merge")]
        public async Task<ActionResult<ApiResponse<bool>>> MergeTasks(UserTask[] tasks)
        {
            var response = await taskService.MergeTasksAsync(tasks);
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<UserTask>>> UpdateTask(Guid id, UpdateTaskDto taskDto)
        {
            var response = await taskService.UpdateTaskAsync(id, taskDto);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteTask(Guid id)
        {
            var response = await taskService.DeleteTaskAsync(id);
            return Ok(response);
        }
    }
}
