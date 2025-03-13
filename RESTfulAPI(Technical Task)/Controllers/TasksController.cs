using AutoMapper;
using Dapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RESTfulAPI_Technical_Task_.CQRS;
using RESTfulAPI_Technical_Task_.DbContext;
using RESTfulAPI_Technical_Task_.Model;
using System.Data;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TaskStatus = RESTfulAPI_Technical_Task_.Model.TaskStatus;

namespace RESTfulAPI_Technical_Task_.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController(IMediator mediator, IValidator<TaskDTO> taskValidator, IValidator<UpdateTaskDTO> updateTaskValidator, ILogger<TasksController> logger, IMapper mapper) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;
        private readonly IValidator<TaskDTO> _taskValidator = taskValidator;
        private readonly IValidator<UpdateTaskDTO> _updateTaskValidator = updateTaskValidator;
        private readonly ILogger<TasksController> _logger = logger;
        private readonly IMapper _mapper = mapper;

        private static List<TaskModel> _tasks = new();

       
        [HttpPost("tasks")]
        public async Task<IActionResult> CreateTask([FromBody] TaskDTO task)
        {
            _logger.LogInformation("Создание задачи: {Title}", task.Title);

            var validationResult = await _taskValidator.ValidateAsync(task);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var taskId = await _mediator.Send(new TaskCommand(task.Title, task.Description, "ToDo"));

            _logger.LogInformation("Задача создана с ID: {TaskId}", taskId);

            return Ok(new { message = "Task created", taskId });
        }

        [HttpGet("tasks/{id}")]
        public async Task<IActionResult> GetTaskById(Guid id)
        {
            _logger.LogInformation("Получение задачи по ID: {TaskId}", id);

            var task = await _mediator.Send(new GetTaskByIdQuery(id));

            if (task is null)
            {
                _logger.LogWarning("Задача с ID {TaskId} не найдена", id);
                return NotFound();
            }

            return Ok(task);
        }

        [HttpGet("tasks")]
        public async Task<IActionResult> GetAllTaskModel([FromQuery] TaskStatus status)
        {
            var tasks = await _mediator.Send(new GetAllTasksQuery(status));

            if (tasks.Any(task => string.IsNullOrEmpty(task.Title)))
            {
                return BadRequest("One or more tasks have a missing Title.");
            }

            var taskDTOs = _mapper.Map<List<TaskModel>>(tasks);

            return Ok(new { taskDTOs.Count, Tasks = taskDTOs });
        }

        [HttpPut("tasks/{id}")]
        public async Task<IActionResult> UpdateTask(Guid id, [FromBody] UpdateTaskDTO updatedTask)
        {
            _logger.LogInformation("Обновление задачи с ID: {TaskId} и статусом: {Status}", id, updatedTask.Status);

            var validationResult = await _updateTaskValidator.ValidateAsync(updatedTask);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var success = await _mediator.Send(new UpdateTaskCommand(id, updatedTask));
            if (!success)
            {
                _logger.LogWarning("Задача с ID {TaskId} не найдена для обновления", id);
                return NotFound();
            }

            _logger.LogInformation("Задача с ID {TaskId} успешно обновлена", id);
            return Ok($"Database updated with UUID: {id}");
        }

        [HttpDelete("tasks/{id}")]
        public async Task<IActionResult> DeleteTask(Guid id)
        {
            _logger.LogInformation("Удаление задачи с ID: {TaskId}", id);

            var success = await _mediator.Send(new DeleteTaskCommand(id));

            if (!success)
            {
                _logger.LogWarning("Задача с ID {TaskId} не найдена для удаления", id);
                return NotFound();
            }

            _logger.LogInformation("Задача с ID {TaskId} успешно удалена", id);

            return Ok($"Deleted row with UUID {id}");
        }
    }
}
