using MediatR;
using RESTfulAPI_Technical_Task_.DbContext;
using RESTfulAPI_Technical_Task_.Model;

namespace RESTfulAPI_Technical_Task_.CQRS
{
    public class DeleteTaskCommand : IRequest<bool>
    {
        public Guid Id { get; }

        public DeleteTaskCommand(Guid id)
        {
            Id = id;
        }
    }

    public class DeleteHandler(DapperConnection db) : IRequestHandler<DeleteTaskCommand, bool>
    {
        private readonly DapperConnection _db = db;

        public async Task<bool> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
        {
            var task = await _db.GetTaskByIdAsync<TaskDTO>(request.Id);
            if (task == null)
                return false;

            int result = await _db.DeleteTaskAsync(request.Id);
            return result == 1;
        }
    }
}
