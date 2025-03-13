using MediatR;
using RESTfulAPI_Technical_Task_.DbContext;
using RESTfulAPI_Technical_Task_.Model;

namespace RESTfulAPI_Technical_Task_.CQRS
{
    public class UpdateTaskCommand : IRequest<bool>
    {
        public Guid Id { get; }
        public UpdateTaskDTO UpdatedTask { get; }

        public UpdateTaskCommand(Guid id, UpdateTaskDTO updatedTask)
        {
            Id = id;
            UpdatedTask = updatedTask;
        }
    }

    public class UpdateHandler(DapperConnection db) : IRequestHandler<UpdateTaskCommand, bool>
    {
        private readonly DapperConnection _db = db;

        public async Task<bool> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
        {
            var existingTask = await _db.GetTaskByIdAsync<UpdateTaskDTO>(request.Id);
            if (existingTask == null)
                return false;

            existingTask.Description = request.UpdatedTask.Description;
            existingTask.Status = request.UpdatedTask.Status.ToString();

            int result = await _db.UpdateTaskAsync(existingTask, request.Id);
            return result == 1;
        }
    }
}
