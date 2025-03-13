namespace RESTfulAPI_Technical_Task_.Model
{
    public class TokenDTO
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Token { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ExpiresAt { get; set; }
    }
}
