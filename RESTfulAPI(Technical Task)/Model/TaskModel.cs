using System.Text.Json.Serialization;

namespace RESTfulAPI_Technical_Task_.Model
{
    public class TaskModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string? Title { get; set; }
        public string? Description { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TaskStatus Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set;}
    }

    public enum TaskStatus
    {
        ToDo,
        InProgress,
        Done
    }
}
