using Microsoft.Extensions.DependencyInjection;
using TodoList.Core.Interfaces;
using TodoList.Core.Models;
using TodoList.Core.Services;
using TodoList.Infrastructure.Repositories;

// Setup DI
var services = new ServiceCollection();
services.AddSingleton<ITodoRepository, InMemoryTodoRepository>();
services.AddTransient<TodoService>();

var serviceProvider = services.BuildServiceProvider();
var todoService = serviceProvider.GetRequiredService<TodoService>();

// Main loop
while (true)
{
    Console.Clear();
    Console.WriteLine("My Todo List Application");
    Console.WriteLine("1. Add a Task");
    Console.WriteLine("2. List all Tasks");
    Console.WriteLine("3. View Specific Task Details");
    Console.WriteLine("4. Update a Task");
    Console.WriteLine("5. Mark The Task as Completed");
    Console.WriteLine("6. Delete the  Task");
    Console.WriteLine("7. Exit application");
    Console.Write("Choose one option: ");

    var choice = Console.ReadLine();

    switch (choice)
    {
        case "1":
            AddTask(todoService);
            break;
        case "2":
            ListTasks(todoService);
            break;
        case "3":
            ViewTaskDetails(todoService);
            break;
        case "4":
            UpdateTask(todoService);
            break;
        case "5":
            MarkTaskCompleted(todoService);
            break;
        case "6":
            DeleteTask(todoService);
            break;
        case "7":
            return;
        default:
            Console.WriteLine("Invalid option. Press any key to continue...");
            Console.ReadKey();
            break;
    }
}

void AddTask(TodoService service)
{
    Console.Write("Title: ");
    var title = Console.ReadLine() ?? string.Empty;

    Console.Write("Description: ");
    var description = Console.ReadLine() ?? string.Empty;

    Console.Write("Due Date (yyyy-mm-dd): ");
    if (!DateTime.TryParse(Console.ReadLine(), out var dueDate))
    {
        Console.WriteLine("Invalid date format. Using today's date.");
        dueDate = DateTime.Today;
    }

    Console.Write("Priority (1-Low, 2-Medium, 3-High): ");
    if (!int.TryParse(Console.ReadLine(), out var priorityInput) || priorityInput < 1 || priorityInput > 3)
    {
        Console.WriteLine("Invalid priority. Defaulting to Medium.");
        priorityInput = 2;
    }
    var priority = (Priority)(priorityInput - 1);

    service.AddTodoTask(title, description, dueDate, priority);
    Console.WriteLine("Task added successfully!");
    Console.ReadKey();
}

void ListTasks(TodoService service)
{
    Console.Clear();
    Console.WriteLine("View Tasks Options:");
    Console.WriteLine("1. All Tasks");
    Console.WriteLine("2. Pending Tasks");
    Console.WriteLine("3. Completed Tasks");
    Console.WriteLine("4. By Priority");
    Console.Write("Choose an option: ");

    var option = Console.ReadLine();
    IEnumerable<TodoTask> tasks;

    switch (option)
    {
        case "1":
            tasks = service.GetAllTasks();
            break;
        case "2":
            tasks = service.GetTasksByCompletionStatus(false);
            break;
        case "3":
            tasks = service.GetTasksByCompletionStatus(true);
            break;
        case "4":
            Console.Write("Enter priority (1-Low, 2-Medium, 3-High): ");
            if (!int.TryParse(Console.ReadLine(), out var priorityInput) || priorityInput < 1 || priorityInput > 3)
            {
                Console.WriteLine("Invalid priority. Showing all tasks.");
                priorityInput = 2;
            }
            var priority = (Priority)(priorityInput - 1);
            tasks = service.GetTasksByPriority(priority);
            break;
        default:
            Console.WriteLine("Invalid option. Showing all tasks...");
            tasks = service.GetAllTasks();
            break;
    }

    DisplayTasks(tasks);
    Console.WriteLine("\nPress any key to continue...");
    Console.ReadKey();
}

void DisplayTasks(IEnumerable<TodoTask> tasks)
{
    if (!tasks.Any())
    {
        Console.WriteLine("No tasks found.");
        return;
    }

    Console.WriteLine("\nTasks:");
    Console.WriteLine("------------------------------------------------------------------");
    Console.WriteLine("| ID | Task Title            |Task Due Date   | Task Priority | Task Status      |");
    Console.WriteLine("------------------------------------------------------------------");

    foreach (var task in tasks)
    {
        Console.WriteLine($"| {task.Id.ToString().PadRight(3)}| {task.Title.PadRight(16)} " +
            $"| {task.DueDate:yyyy-MM-dd} | {task.Priority.ToString().PadRight(8)} " +
            $"| {(task.IsCompleted ? "Completed" : "Pending").PadRight(10)} |");
    }

    Console.WriteLine("------------------------------------------------------------------");
}

void MarkTaskCompleted(TodoService service)
{
    ListTasks(service);
    Console.Write("\nEnter task ID to mark as completed: ");

    if (int.TryParse(Console.ReadLine(), out var id))
    {
        var success = service.MarkTaskAsCompleted(id);
        Console.WriteLine(success ? "Task marked as completed!" : "Task not found.");
    }
    else
    {
        Console.WriteLine("Invalid ID format.");
    }

    Console.WriteLine("Press any key to continue...");
    Console.ReadKey();
}

void UpdateTask(TodoService service)
{
    ListTasks(service);
    Console.Write("\nEnter task ID to update: ");

    if (int.TryParse(Console.ReadLine(), out var id))
    {
        var task = service.GetTask(id);
        if (task == null)
        {
            Console.WriteLine("Task not found.");
            Console.ReadKey();
            return;
        }

        Console.Write($"Task Title ({task.Title}): ");
        var title = Console.ReadLine();

        Console.Write($"Task Description ({task.Description}): ");
        var description = Console.ReadLine();

        Console.Write($"Task Due Date ({task.DueDate:yyyy-MM-dd}): ");
        var dueDateInput = Console.ReadLine();
        var dueDate = string.IsNullOrEmpty(dueDateInput) ? task.DueDate : DateTime.Parse(dueDateInput);

        Console.Write($"Priority (1-LOW, 2-MEDIUM, 3-HIGH) (current: {task.Priority}): ");
        var priorityInput = Console.ReadLine();
        var priority = string.IsNullOrEmpty(priorityInput) ? task.Priority : (Priority)(int.Parse(priorityInput) - 1);

        var success = service.UpdateTask(
            id,
            string.IsNullOrEmpty(title) ? task.Title : title,
            string.IsNullOrEmpty(description) ? task.Description : description,
            dueDate,
            priority
        );

        Console.WriteLine(success ? "Task was updated successfully!" : "Failed to the  update task.");
    }
    else
    {
        Console.WriteLine("Invalid ID format.");
    }

    Console.WriteLine("Press any key to continue...");
    Console.ReadKey();
}

void DeleteTask(TodoService service)
{
    ListTasks(service);
    Console.Write("\nEnter task ID to delete: ");

    if (int.TryParse(Console.ReadLine(), out var id))
    {
        var success = service.DeleteTask(id);
        Console.WriteLine(success ? "Task was deleted successfully!" : "Task was not found.");
    }
    else
    {
        Console.WriteLine("Invalid ID format.");
    }

    Console.WriteLine("Press any key to continue with other operations...");
    Console.ReadKey();
}

void ViewTaskDetails(TodoService service)
{
    ListTasks(service);
    Console.Write("\nEnter task ID to view details: ");

    if (int.TryParse(Console.ReadLine(), out var id))
    {
        var task = service.GetTask(id);
        if (task != null)
        {
            Console.WriteLine("\nTask Details:");
            Console.WriteLine("----------------------------");
            Console.WriteLine($"ID: {task.Id}");
            Console.WriteLine($"Task Title: {task.Title}");
            Console.WriteLine($"Task Description: {task.Description}");
            Console.WriteLine($"Task Due Date: {task.DueDate:yyyy-MM-dd}");
            Console.WriteLine($"Task Priority: {task.Priority}");
            Console.WriteLine($"Task Status: {(task.IsCompleted ? "Completed" : "Pending")}");
            Console.WriteLine($"Created task date: {task.CreatedAt:yyyy-MM-dd HH:mm:ss}");
            Console.WriteLine("----------------------------");
        }
        else
        {
            Console.WriteLine("Task was not found.");
        }
    }
    else
    {
        Console.WriteLine("Invalid Task ID format.");
    }

    Console.WriteLine("Press any key to continue with other operations...");
    Console.ReadKey();
}