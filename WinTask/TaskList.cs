using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.ComponentModel;
using System.Globalization;
using System.Collections.ObjectModel;
using System.Windows.Data;

namespace WinTask
{
    class TaskList 
    {
        List<EventHandler<PropertyChangedEventArgs>> changedHandlers =
            new List<EventHandler<PropertyChangedEventArgs>>();
        private List<string> _projects = new List<string>();
        public ObservableCollection<string> Projects { get { return new ObservableCollection<string>(_projects); } set { _projects = new List<string>(value); } }

        private bool _showCompleted = false;
        private bool _showDeleted = false;

        public bool ShowCompleted { get { return _showCompleted; } set { _showCompleted = value; TasksView.Refresh(); } }
        public bool ShowDeleted { get { return _showDeleted; } set { _showDeleted = value; TasksView.Refresh(); } }

        public ObservableCollection<TaskItem> Tasks { get; private set; }
        public ICollectionView TasksView { get; set;}

        
        private bool TasksFilter(object item)
        {
            TaskItem task = item as TaskItem;
            bool show = false;
            switch (task.status) 
            {
                case "completed":
                    show = ShowCompleted;
                    break;
                case "deleted":
                    show = ShowDeleted;
                    break;
                default:
                    show = true;
                    break;
            }
            return show;
        }

        public TaskList()
        {
            Tasks = new ObservableCollection<TaskItem>();
            TasksView = CollectionViewSource.GetDefaultView(Tasks);
            TasksView.Filter = TasksFilter;
        }
        public void Update()
        {
            Tasks.Clear();
            TaskBinary taskB = new TaskBinary();
            string result = taskB.RunCommand("export");
            Debug.WriteLine(result);

            JsonTextReader reader = new JsonTextReader(new StringReader(result));
            reader.SupportMultipleContent = true;

            while (true)
            {
                if (!reader.Read())
                {
                    break;
                }

                JsonSerializer serializer = new JsonSerializer();
                TaskItem task = serializer.Deserialize<TaskItem>(reader);
                Debug.WriteLine(task.tags.Count);
                AddChild(task);
            }
        }
        public void AddChild(TaskItem newChild)
        {
            // Omitted: error checking, and ensuring newChild isn't already in the list
            Tasks.Add(newChild);
            if (newChild.project != null && !(_projects.Contains(newChild.project)))
            {
                _projects.Add(newChild.project);
                _projects.Sort();
            }
        }
    }


}
