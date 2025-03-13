using Dapper;
using RESTfulAPI_Technical_Task_.Model;
using System.Data;
using System.Threading.Tasks;
using TaskStatus = RESTfulAPI_Technical_Task_.Model.TaskStatus;

namespace RESTfulAPI_Technical_Task_.DbContext
{
    public class DapperConnection
    {
        private readonly IDbConnection _db;
        private readonly ILogger<DapperConnection> _logger;

        public DapperConnection(IDbConnection db, ILogger<DapperConnection> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<IEnumerable<TaskModel>> GetTasksAsync(TaskStatus? status = null)
        {
            var query = "SELECT * FROM \"TechnicalAPI\".\"Tasks\"";
            if (status.HasValue)
            {
                query += " WHERE \"Status\" = @Status::\"TechnicalAPI\".enum_type;";
                return await _db.QueryAsync<TaskModel>(query, new { Status = status.Value.ToString() });
            }
            return await _db.QueryAsync<TaskModel>(query);
        }

        public async Task<T?> GetTaskByIdAsync<T>(Guid id)
        {
            _logger.LogInformation("Запрос в БД: получение задачи с ID {TaskId}", id);

            var query = "SELECT * FROM \"TechnicalAPI\".\"Tasks\" WHERE \"Id\" = @Id;";
            var task = await _db.QueryFirstOrDefaultAsync<T>(query, new { Id = id });

            if (task is null)
                _logger.LogWarning("Задача с ID {TaskId} не найдена в БД", id);

            return task;
        }

        public async Task<int> AddTaskAsync(TaskModel task)
        {
            var query = @"INSERT INTO ""TechnicalAPI"".""Tasks"" 
                 (""Id"", ""Title"", ""Description"", ""Status"", ""CreatedAt"", ""UpdatedAt"") 
                 VALUES (@Id, @Title, @Description, @Status::""TechnicalAPI"".enum_type, @CreatedAt, @UpdatedAt);";

            var parameters = new
            {
                task.Id,
                task.Title,
                task.Description,
                Status = task.Status.ToString(), // 🔥 Преобразуем Enum в строку
                task.CreatedAt,
                task.UpdatedAt
            };

            return await _db.ExecuteAsync(query, parameters);
        }

        public async Task<int> UpdateTaskAsync(UpdateTaskDTO task, Guid id)
        {
            var query = @"UPDATE ""TechnicalAPI"".""Tasks"" 
                  SET ""Description"" = @Description, 
                      ""Status"" = @Status::""TechnicalAPI"".enum_type, ""UpdatedAt"" = CURRENT_TIMESTAMP
                  WHERE ""Id"" = @Id;";

            var parameters = new
            {
                task.Description,
                Status = task.Status.ToString(), 
                Id = id
            };

            return await _db.ExecuteAsync(query, parameters);
        }

        public async Task<int> DeleteTaskAsync(Guid id)
        {
            var query = @"DELETE FROM ""TechnicalAPI"".""Tasks"" WHERE ""Id"" = @Id;";
            return await _db.ExecuteAsync(query, new { Id = id });
        }

        // Проверить уникальность Title
        public async Task<bool> IsTitleUniqueAsync(string title)
        {
            var query = "SELECT COUNT(*) FROM \"TechnicalAPI\".\"Tasks\" WHERE \"Title\" = @Title;";
            var count = await _db.ExecuteScalarAsync<int>(query, new { Title = title });
            return count == 0;
        }
    }
}
