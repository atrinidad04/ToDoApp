namespace BlazorTodoApp.Services
{
	using BlazorTodoApp.Models;
	using System.Collections.Generic;
	using System.Linq;

	public class TodoService
	{
		private List<TodoTask> todos = new();
		private int nextId = 1;

		public TodoService()
		{
			// Mock data
			todos.Add(new TodoTask
			{
				Id = nextId++,
				Title = "Fase 1",
				Description = "El Modelo y el Servicio",
				IsCompleted = true,
				CreatedAt = DateTime.Now
			});

			todos.Add(new TodoTask
			{
				Id = nextId++,
				Title = "Fase 2",
				Description = "Mostrar la Lista de Tareas (Blazor)",
				IsCompleted = true,
				CreatedAt = DateTime.Now
			});

			todos.Add(new TodoTask
			{
				Id = nextId++,
				Title = "Fase 3",
				Description = "Agregar Nuevas Tareas (Binding)",
				IsCompleted = true,
				CreatedAt = DateTime.Now
			});

			todos.Add(new TodoTask
			{
				Id = nextId++,
				Title = "Fase 4",
				Description = "Marcar Tareas como Completadas (Update)",
				IsCompleted = true,
				CreatedAt = DateTime.Now
			});

			todos.Add(new TodoTask
			{
				Id = nextId++,
				Title = "Fase 5",
				Description = "Eliminar Tareas (Delete)",
				IsCompleted = true,
				CreatedAt = DateTime.Now
			});

			todos.Add(new TodoTask
			{
				Id = nextId++,
				Title = "Tarea Extra",
				Description = "Cualquier otra cosa...",
				IsCompleted = false,
				CreatedAt = DateTime.Now
			});
		}

		// Obtener todas las tareas
		public List<TodoTask> GetAllTodos()
		{
			return todos.OrderByDescending(t => t.CreatedAt).ToList();
		}

		// Agregar una nueva tarea
		public void AddTodo(string title, string description)
		{
			if (string.IsNullOrWhiteSpace(title)) return;

			var newTodo = new TodoTask
			{
				Id = nextId++,
				Title = title.Trim(),
				Description = description?.Trim() ?? "",
				IsCompleted = false,
				CreatedAt = DateTime.Now
			};

			todos.Add(newTodo);
		}

		// Marcar tarea como completada
		public void ToggleTodo(int id)
		{
			var todo = todos.FirstOrDefault(t => t.Id == id);
			if (todo != null)
			{
				todo.IsCompleted = !todo.IsCompleted;
			}
		}

		// Eliminar una tarea
		public void DeleteTodo(int id)
		{
			todos.RemoveAll(t => t.Id == id);
		}

		// Actualizar una tarea
		public void UpdateTodo(int id, string title, string description)
		{
			var todo = todos.FirstOrDefault(t => t.Id == id);
			if (todo != null)
			{
				todo.Title = title.Trim();
				todo.Description = description?.Trim() ?? "";
			}
		}
	}
}
