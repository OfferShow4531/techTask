using MediatR;
using RESTfulAPI_Technical_Task_.DbContext;
using RESTfulAPI_Technical_Task_.Model;

namespace RESTfulAPI_Technical_Task_.CQRS
{
    public record GetTaskByIdQuery(Guid Id) : IRequest<TaskDTO?>;
    public class GetTaskQueryById(DapperConnection db) : IRequestHandler<GetTaskByIdQuery, TaskDTO?>
    {
        private readonly DapperConnection _db = db;

        public async Task<TaskDTO?> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
        {
            return await _db.GetTaskByIdAsync<TaskDTO>(request.Id);
        }
    }
}
