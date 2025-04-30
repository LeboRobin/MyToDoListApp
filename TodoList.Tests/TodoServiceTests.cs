using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoList.Core.Interfaces;
using TodoList.Core.Models;
using TodoList.Core.Services;
using Xunit;

namespace TodoList.Tests
{
    public class TodoServiceTests
    {
        private readonly Mock<ITodoRepository> _mockRepo;
        private readonly TodoService _service;

        public TodoServiceTests()
        {
            // Arrange - Create mock repository and service instance
            _mockRepo = new Mock<ITodoRepository>();
            _service = new TodoService(_mockRepo.Object);
        }

        [Fact]
        public void AddTodoTask_Should_Call_Repository_Add()
        {
            var testTask = new TodoTask { Id = 1, Title = "Test Task" };
            _mockRepo.Setup(x => x.AddTask(It.IsAny<TodoTask>()))
                    .Returns(testTask);
            
            var result = _service.AddTodoTask("Test Task", "Description", DateTime.Now, Priority.Medium);
            
            _mockRepo.Verify(x => x.AddTask(It.IsAny<TodoTask>()), Times.Once);
            Assert.Equal(testTask, result);
        }

        [Fact]
        public void GetTask_Should_Return_Task_When_Exists()
        {
            var testTask = new TodoTask { Id = 1, Title = "Existing Task" };
            _mockRepo.Setup(x => x.GetTaskById(1)).Returns(testTask);
            
            var result = _service.GetTask(1);
            
            Assert.Equal(testTask, result);
        }

        [Fact]
        public void GetTask_Should_Return_Null_When_Not_Exists()
        {
            _mockRepo.Setup(x => x.GetTaskById(1)).Returns((TodoTask?)null);
            
            var result = _service.GetTask(1);
            
            Assert.Null(result);
        }

        [Fact]
        public void MarkTaskAsCompleted_Should_Update_Task()
        {
            var testTask = new TodoTask { Id = 1, IsCompleted = false };
            _mockRepo.Setup(x => x.GetTaskById(1)).Returns(testTask);
            
            var result = _service.MarkTaskAsCompleted(1);
            
            Assert.True(result);
            Assert.True(testTask.IsCompleted);
            _mockRepo.Verify(x => x.UpdateTask(testTask), Times.Once);
        }

        [Fact]
        public void DeleteTask_Should_Return_True_When_Successful()
        {
            
            _mockRepo.Setup(x => x.DeleteTask(1)).Returns(true);
            
            var result = _service.DeleteTask(1);
            
            Assert.True(result);
        }

        [Fact]
        public void GetAllTasks_Should_Return_All_Tasks()
        {
            var testTasks = new List<TodoTask>
            {
                new TodoTask { Id = 1 },
                new TodoTask { Id = 2 }
            };
            _mockRepo.Setup(x => x.GetAllTasks()).Returns(testTasks);
            
            var result = _service.GetAllTasks();
            
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public void GetTasksByPriority_Should_Filter_Correctly()
        {
            var testTasks = new List<TodoTask>
            {
                new TodoTask { Id = 1, Priority = Priority.High },
                new TodoTask { Id = 2, Priority = Priority.Low }
            };
            _mockRepo.Setup(x => x.GetTaskByPriority(Priority.High))
                   .Returns(testTasks.Where(t => t.Priority == Priority.High));
            
            var result = _service.GetTasksByPriority(Priority.High);
            
            Assert.Single(result);
            Assert.Equal(1, result.First().Id);
        }
    }
}