using TodoApp.Models;

namespace TodoApp.Data
{
    public interface ITaskRepository
    {
        List<TodoTask> GetAllTasks();
        TodoTask? GetTaskById(int id);
        TodoTask InsertTask(string title, string? description);
        TodoTask? UpdateTask(int id, string title, string? description);
        TodoTask? ToggleTaskCompletion(int id);
        bool DeleteTask(int id);
        (int pending, int completed) GetTaskCounts();
    }
}