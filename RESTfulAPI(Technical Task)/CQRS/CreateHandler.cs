using MediatR;
using RESTfulAPI_Technical_Task_.DbContext;
using RESTfulAPI_Technical_Task_.Model;
using TaskStatus = RESTfulAPI_Technical_Task_.Model.TaskStatus;

namespace RESTfulAPI_Technical_Task_.CQRS
{
    public record TaskCommand(string Title, string Description, string Status) : IRequest<Guid>;
    public class CreateHandler(DapperConnection db) : IRequestHandler<TaskCommand, Guid>
    {
        private readonly DapperConnection _db = db;

        public async Task<Guid> Handle(TaskCommand request, CancellationToken cancellationToken)
        {
            var id = Guid.NewGuid();
            var task = new TaskModel
            {
                Id = id,
                Title = request.Title,
                Description = request.Description,
                Status = Enum.Parse<TaskStatus>(request.Status, true),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _db.AddTaskAsync(task);
            return id;
        }
    }
}
