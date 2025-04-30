using TodoList.Core.Interfaces;
using TodoList.Core.Models;

namespace TodoList.Core.Services;

public class TodoService
{
    private readonly ITodoRepository _todoRepository;

    public TodoService(ITodoRepository todoRepository)
    {
        _todoRepository = todoRepository;
    }

    public TodoTask AddTodoTask(string title, string description, DateTime dueDate, Priority priority)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty", nameof(title));

        var task = new TodoTask
        {
            Title = title,
            Description = description,
            DueDate = dueDate,
            Priority = priority,
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow
        };
        return _todoRepository.AddTask(task);
    }

    public bool UpdateTask(int id, string title, string description, DateTime dueDate, Priority priority)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty", nameof(title));

        var task =  _todoRepository.GetTaskById(id) ;
        if (task == null) return false;

        task.Title = title;
        task.Description = description;
        task.DueDate = dueDate;
        task.Priority = priority;

        _todoRepository.UpdateTask(task);
        return true;
    }

    public bool DeleteTask(int id)
    {
        return _todoRepository.DeleteTask(id);
    }

    public TodoTask? GetTask(int id)
    {
        return _todoRepository.GetTaskById(id);
    }

    public IEnumerable<TodoTask> GetAllTasks()
    {
        return _todoRepository.GetAllTasks();
    }

    public IEnumerable<TodoTask> GetTasksByCompletionStatus(bool isCompleted)
    {
        return _todoRepository.GetTaskByCompletionStatus(isCompleted);
    }

    public IEnumerable<TodoTask> GetTasksByPriority(Priority priority)
    {
        return _todoRepository.GetTaskByPriority(priority);
    }

    public bool MarkTaskAsCompleted(int id)
    {
        var task = _todoRepository.GetTaskById(id);
        if (task == null) return false;

        task.IsCompleted = true;
        _todoRepository.UpdateTask(task);
        return true;
    }

    public bool MarkTaskAsPending(int id)
    {
        var task = _todoRepository.GetTaskById(id);
        if (task == null) return false;

        task.IsCompleted = false;
        _todoRepository.UpdateTask(task);
        return true;
    }
}