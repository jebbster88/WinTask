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
        public List<string> Projects = new List<string>();
        
        
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

                AddChild(task);
            }
        }
        public void AddChild(TaskItem newChild)
        {
            // Omitted: error checking, and ensuring newChild isn't already in the list
            Tasks.Add(newChild);
            if (newChild.project != null && !(Projects.Contains(newChild.project)))
            {
                Projects.Add(newChild.project);
                Projects.Sort();
            }
        }

    }

    public class TaskItem
    {
        public int id { get; set; }
        public string description { get; set; }
        public string entry { get; set; }
        public string modified { get; set; }
        public string project { get; set; }
        public string status { get; set; }
        public string uuid { get; set; }
        public double urgency { get; set; }
        
    }
    public class EditTaskItem : TaskItem
    {
        private enum TaskProp
        {            
            Description = 0,
            Project = 1
        }

        private SortedDictionary<TaskProp, string> Changes = new SortedDictionary<TaskProp, string>();
        private string _Description;
        public string Description
        {
            get { return _Description ?? description; }
            set { _Description = value; ModifyProperty(TaskProp.Description, value); }
        }
        private string _Project;
        public string Project
        {
            get { return _Project ?? project; }
            set { if (value != project) { _Project = value; ModifyProperty(TaskProp.Project, value); } }
        }
        public string TaskCommand
        {
            get { return "task " + BuildTaskString(); }
        }
        public EditTaskItem(TaskItem source)
        {
            if(source != null)
            {
                id = source.id;
                description = source.description;
                entry = source.entry;
                modified = source.modified;
                project = source.project;
                status = source.status;
                uuid = source.uuid;
                urgency = source.urgency;
            }
        }

        private string BuildTaskString()
        {
            string strCommand = "mod " + uuid + " ";
            foreach (KeyValuePair<TaskProp, string> entry in Changes)
            {
                switch (entry.Key)
                {
                    case TaskProp.Description:
                        strCommand += entry.Value + " ";
                        break;
                    case TaskProp.Project:
                        strCommand += "pro:" + entry.Value + " ";
                        break;
                    default:
                        break;
                }
            }
            Debug.WriteLine(strCommand);
            return strCommand;
        }
       

        private void ModifyProperty(TaskProp property, string value)
        {
            Changes[property] = value;
            Debug.WriteLine(Changes.Keys.Count);
            Debug.WriteLine(BuildTaskString());
        }
    }

}
