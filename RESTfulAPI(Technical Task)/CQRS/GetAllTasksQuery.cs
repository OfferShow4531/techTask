using MediatR;
using RESTfulAPI_Technical_Task_.DbContext;
using RESTfulAPI_Technical_Task_.Model;
using TaskStatus = RESTfulAPI_Technical_Task_.Model.TaskStatus;

namespace RESTfulAPI_Technical_Task_.CQRS
{
    public class GetAllTasksQuery : IRequest<IEnumerable<TaskModel>>
    {
        public TaskStatus? Status { get; }

        public GetAllTasksQuery(TaskStatus? status)
        {
            Status = status;
        }
    }

    public class GetAllTasksHandler(DapperConnection db) : IRequestHandler<GetAllTasksQuery, IEnumerable<TaskModel>>
    {
        private readonly DapperConnection _db = db;

        public async Task<IEnumerable<TaskModel>> Handle(GetAllTasksQuery request, CancellationToken cancellationToken)
        {
            return await _db.GetTasksAsync(request.Status);
        }
    }
}
