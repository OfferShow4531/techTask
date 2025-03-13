using FluentValidation;
using RESTfulAPI_Technical_Task_.Model;
using TaskStatus = RESTfulAPI_Technical_Task_.Model.TaskStatus;

namespace RESTfulAPI_Technical_Task_.FluentValidator
{
    public class UpdateTaskValidator : AbstractValidator<UpdateTaskDTO>
    {
        public UpdateTaskValidator()
        {
            RuleFor(task => task.Status)
                .NotEmpty().WithMessage("Status is required.")
                .Must(status => Enum.TryParse<TaskStatus>(status, true, out _))
                .WithMessage("Invalid status. Allowed values: ToDo, InProgress, Done.");
        }
    }
}
