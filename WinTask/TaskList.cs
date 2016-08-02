using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.ComponentModel;

namespace WinTask
{
    class TaskList
    {
        public SortableSearchableList<TaskItem> Tasks = new SortableSearchableList<TaskItem>();
        List<EventHandler<PropertyChangedEventArgs>> changedHandlers =
            new List<EventHandler<PropertyChangedEventArgs>>();
        
        
        public void Update()
        {
            Tasks.Clear();   
            TaskBinary taskB = new TaskBinary();
            string result = taskB.RunCommand("export");
            Debug.WriteLine(result);
            IList<TaskItem> tasks = new List<TaskItem>();

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

                tasks.Add(task);
            }

            foreach (TaskItem task in tasks)
            {
                Debug.WriteLine(task.uuid + " " + task.description);
                this.AddChild(task);
            }

            Tasks.First().description = "Fuck off";            
            


        }
        public void AddChild(TaskItem newChild)
        {
            // Omitted: error checking, and ensuring newChild isn't already in the list
            this.Tasks.Add(newChild);
            newChild.PropertyChanged += new PropertyChangedEventHandler(ChildChanged);
            
        }

        public void ChildChanged(object sender, PropertyChangedEventArgs e)
        {
            TaskItem child = sender as TaskItem;
            if (this.Tasks.Contains(child))
            {
                Debug.WriteLine(child);
                Debug.WriteLine("Is Changed");
                Debug.WriteLine(e.PropertyName);

            }
        }
    }

    public class TaskItem
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public bool isChanged;
        public int id { get; private set; }
        private string _description;
        private string _descriptionNew;
        public string description
        {
            get
            {
                if (_descriptionNew == null)
                {
                    return _description;
                }
                else
                {
                    return _descriptionNew;
                }
            }
            set
            {
                if (_description == null)
                {
                    _description = value;
                }
                else
                {
                    _descriptionNew = value;
                    isChanged = true;
                }
                OnPropertyChanged("description");
                Console.WriteLine("desc: " + _description);
                Console.WriteLine("descNew: " + _descriptionNew);
            }
        }
        public string entry { get; set; }
        public string modified { get; set; }
        public string project { get; set; }
        public string status { get; set; }
        public string uuid { get; set; }
        public double urgency { get; set; }

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
        
    }

}
