using System.Text.Json.Serialization;

namespace RESTfulAPI_Technical_Task_.Model
{
    public class TaskDTO
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
    public class UpdateTaskDTO
    {
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = "ToDo";
    }
}
