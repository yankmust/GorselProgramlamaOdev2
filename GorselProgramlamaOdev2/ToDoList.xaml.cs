using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using Firebase.Database;
using Microsoft.Maui.Controls;
using System.Threading.Tasks;
using Firebase.Database.Query;

namespace GorselProgramlamaOdev2
{
    // New class to represent a task with additional details
    public class TaskItem
    {
        public string Key { get; set; }
        public string Name { get; set; }
        public string Details { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string FormattedDateTime => CreatedDateTime.ToString("yyyy-MM-dd HH:mm:ss");
    }

    public partial class ToDoList : ContentPage
    {
        private readonly FirebaseClient firebaseClient;
        private const string TasksNode = "tasks";

        public ObservableCollection<TaskItem> Tasks { get; set; }

        public ToDoList()
        {
            InitializeComponent();
            BindingContext = this;

            // Initialize Firebase Realtime Database
            firebaseClient = new FirebaseClient("https://gorselproje-a67ec-default-rtdb.europe-west1.firebasedatabase.app/");

            // Load tasks when the page appears
            _ = LoadAndDisplayTasks();
        }

        private async Task LoadAndDisplayTasks()
        {
            Tasks = await LoadTasks();
            taskListView.ItemsSource = Tasks;
        }

        private async Task<ObservableCollection<TaskItem>> LoadTasks()
        {
            ObservableCollection<TaskItem> tasks = new ObservableCollection<TaskItem>();

            try
            {
                var taskSnapshot = await firebaseClient
                    .Child(TasksNode)
                    .OnceAsync<TaskItem>();

                foreach (var task in taskSnapshot)
                {
                    TaskItem taskItem = task.Object;
                    taskItem.Key = task.Key;
                    tasks.Add(taskItem);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading tasks: {ex.Message}");
            }

            return tasks;
        }

        private async void OnCreateClicked(object sender, EventArgs e)
        {
            string taskName = taskEntry.Text;
            if (!string.IsNullOrEmpty(taskName))
            {
                // Show a details entry page to get additional details
                string details = await DisplayPromptAsync("Task Details", "Enter task details:", initialValue: "");

                TaskItem newTask = new TaskItem
                {
                    Name = taskName,
                    Details = details,
                    CreatedDateTime = DateTime.Now
                };

                Tasks.Add(newTask);
                taskEntry.Text = string.Empty;
                SaveTasks();
            }
        }

        private async void OnEditClicked(object sender, EventArgs e)
        {
            if (sender is ImageButton button && button.CommandParameter is TaskItem task)
            {
                string newTaskName = await DisplayPromptAsync("Edit Task", "Enter the new task name", initialValue: task.Name);
                string newDetails = await DisplayPromptAsync("Edit Details", "Enter the new details", initialValue: task.Details);

                if (!string.IsNullOrEmpty(newTaskName))
                {
                    task.Name = newTaskName;
                    task.Details = newDetails;
                    SaveTasks();
                }
            }
        }

        private void OnDeleteClicked(object sender, EventArgs e)
        {
            if (sender is ImageButton button && button.CommandParameter is TaskItem task)
            {
                Tasks.Remove(task);
                DeleteTask(task.Key);
                SaveTasks();
            }
        }

        private async void SaveTasks()
        {
            try
            {
                var tasksToSave = Tasks.Select(task => new { task.Key, task.Name, task.Details, task.CreatedDateTime });

                await firebaseClient
                    .Child(TasksNode)
                    .PutAsync(tasksToSave);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving tasks: {ex.Message}");
            }
        }

        private async void DeleteTask(string key)
        {
            try
            {
                await firebaseClient
                    .Child(TasksNode)
                    .Child(key)
                    .DeleteAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting task: {ex.Message}");
            }
        }
    }
}
