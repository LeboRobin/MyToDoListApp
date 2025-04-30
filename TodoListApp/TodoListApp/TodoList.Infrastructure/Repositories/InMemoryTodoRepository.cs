using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoList.Core.Interfaces;
using TodoList.Core.Models;

namespace TodoList.Infrastructure.Repositories
{
    public class InMemoryTodoRepository : ITodoRepository
    {
        private readonly ConcurrentDictionary<int, TodoTask> _tasks = new();
        private int _nextId = 1;

        public TodoTask AddTask(TodoTask task)
        {
            task.Id = _nextId++;
            _tasks.TryAdd(task.Id, task);
            return task;
        }

        public TodoTask UpdateTask(TodoTask task)
        {
            _tasks[task.Id] = task;
            return task;
        }

        public bool DeleteTask(int id)
        {
            return _tasks.TryRemove(id, out _);
        }

        public TodoTask? GetTaskById(int id)
        {
            return _tasks.TryGetValue(id, out var task) ? task : null;

        }

        public IEnumerable<TodoTask> GetAllTasks()
        {
            return _tasks.Values.OrderBy(t => t.DueDate);
        }

        public IEnumerable<TodoTask> GetTaskByCompletionStatus(bool isCompleted)
        {
            return _tasks.Values.Where(t => t.IsCompleted == isCompleted);
        }

        public IEnumerable<TodoTask> GetTaskByPriority(Priority priority)
        {
            return _tasks.Values.Where(t => t.Priority == priority);
        }
    }
}
