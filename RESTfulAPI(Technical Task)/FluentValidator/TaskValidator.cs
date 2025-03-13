using FluentValidation;
using RESTfulAPI_Technical_Task_.Model;
using TaskStatus = RESTfulAPI_Technical_Task_.Model.TaskStatus;

namespace RESTfulAPI_Technical_Task_.FluentValidator
{
    public class TaskValidator : AbstractValidator<TaskDTO>
    {
        public TaskValidator()
        {
            RuleFor(task => task.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MinimumLength(5).WithMessage("Title must be at least 5 characters long.");
        }
    }
}
