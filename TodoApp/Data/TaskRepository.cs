using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;
using TodoApp.Models;

namespace TodoApp.Data
{
    public class TaskRepository : ITaskRepository
    {
        private readonly string _connectionString;

        public TaskRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string not found");
        }

        private IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }

        // READ - Obtener todas las tareas
        public List<TodoTask> GetAllTasks()
        {
            using var connection = CreateConnection();

            var tasks = connection.Query<TodoTask>(
                "sp_GetAllTasks",
                commandType: CommandType.StoredProcedure
            );

            return tasks.ToList();
        }

        // READ - Obtener tarea por ID
        public TodoTask? GetTaskById(int id)
        {
            using var connection = CreateConnection();

            var task = connection.QueryFirstOrDefault<TodoTask>(
                "sp_GetTaskById",
                new { Id = id },
                commandType: CommandType.StoredProcedure
            );

            return task;
        }

        // CREATE - Insertar nueva tarea
        public TodoTask InsertTask(string title, string? description)
        {
            using var connection = CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@Title", title);
            parameters.Add("@Description", description);
            parameters.Add("@NewTaskId", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var task = connection.QueryFirst<TodoTask>(
                "sp_InsertTask",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return task;
        }

        // UPDATE - Actualizar tarea
        public TodoTask? UpdateTask(int id, string title, string? description)
        {
            using var connection = CreateConnection();

            var task = connection.QueryFirstOrDefault<TodoTask>(
                "sp_UpdateTask",
                new { Id = id, Title = title, Description = description },
                commandType: CommandType.StoredProcedure
            );

            return task;
        }

        // UPDATE - Toggle completado/pendiente
        public TodoTask? ToggleTaskCompletion(int id)
        {
            using var connection = CreateConnection();

            var task = connection.QueryFirstOrDefault<TodoTask>(
                "sp_ToggleTaskCompletion",
                new { Id = id },
                commandType: CommandType.StoredProcedure
            );

            return task;
        }

        // DELETE - Eliminar tarea
        public bool DeleteTask(int id)
        {
            using var connection = CreateConnection();

            var result = connection.ExecuteScalar<int>(
                "sp_DeleteTask",
                new { Id = id },
                commandType: CommandType.StoredProcedure
            );

            return result > 0;
        }

        // EXTRA - Obtener contadores con funciones SQL
        public (int pending, int completed) GetTaskCounts()
        {
            using var connection = CreateConnection();

            var pending = connection.ExecuteScalar<int>(
                "SELECT dbo.fn_GetPendingTasksCount()"
            );

            var completed = connection.ExecuteScalar<int>(
                "SELECT dbo.fn_GetCompletedTasksCount()"
            );

            return (pending, completed);
        }
    }
}
