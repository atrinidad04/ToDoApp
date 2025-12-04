using TodoApp.Data;
using TodoApp.Models;

namespace TodoApp.Services
{
    public class TodoService
    {
        private readonly ITaskRepository _repository;

        public TodoService(ITaskRepository repository)
        {
            _repository = repository;
        }

        public List<TodoTask> GetAllTodos()
        {
            return _repository.GetAllTasks();
        }

        public TodoTask AddTodo(string title, string? description)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("El título no puede estar vacío", nameof(title));

            return _repository.InsertTask(title.Trim(), description?.Trim());
        }

        public void ToggleTodo(int id)
        {
            _repository.ToggleTaskCompletion(id);
        }

        public void UpdateTodo(int id, string title, string? description)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("El título no puede estar vacío", nameof(title));

            _repository.UpdateTask(id, title.Trim(), description?.Trim());
        }

        public void DeleteTodo(int id)
        {
            _repository.DeleteTask(id);
        }

        public (int pending, int completed) GetTaskCounts()
        {
            return _repository.GetTaskCounts();
        }
    }
}
