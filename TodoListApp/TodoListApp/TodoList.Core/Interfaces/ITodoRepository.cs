using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoList.Core.Models;

namespace TodoList.Core.Interfaces
{
    public interface ITodoRepository
    {
        TodoTask AddTask(TodoTask task);
        TodoTask UpdateTask(TodoTask task);
        bool DeleteTask(int id);
        TodoTask? GetTaskById(int id);
        IEnumerable<TodoTask> GetAllTasks();
        IEnumerable<TodoTask> GetTaskByCompletionStatus(bool isCompleted);
        IEnumerable<TodoTask> GetTaskByPriority(Priority priority);
    }
}
