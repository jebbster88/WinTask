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
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace WinTask
{
    public class TaskItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public int id { get; set; }
        public string description { get; set; }
        public string due { get; set; }
        public string entry { get; set; }
        public string modified { get; set; }
        public string project { get; set; }
        public string status { get; set; }
        public string uuid { get; set; }
        public double urgency { get; set; }
        public List<string> tags { get; set; } // Implement something like this http://stackoverflow.com/questions/15167809/how-can-i-create-a-tagging-control-similar-to-evernote-in-wpf or https://github.com/niieani/TokenizedInputCs

        public TaskItem()
        {
            tags = new List<string>();
        }
       

        private enum TaskProp
        {
            Description = 0,
            Project = 1,
            Due = 2,
            Status = 3,
            Tags = 4
        }

        private string writepattern = "yyyy-MM-dd'T'HH:mm:ss";
        private string readpattern = "yyyyMMdd'T'HHmmss'Z'";

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
        private DateTime? _Due;


        public DateTime? Due
        {
            get
            {
                if (_Due != null)
                {
                    return _Due;
                }
                else
                {
                    DateTime retDate;
                    if (DateTime.TryParseExact(due, readpattern, null, DateTimeStyles.None, out retDate))
                    {
                        return retDate;
                    }
                    else return null;
                }
            }

            set
            {
                string duestring = "";
                if (value == null)
                {
                    _Due = value;
                }
                else
                {
                    DateTime newDate = (DateTime)value;
                    _Due = newDate;
                    duestring = newDate.ToString(writepattern, CultureInfo.InvariantCulture);
                }
                ModifyProperty(TaskProp.Due, duestring);
            }
        }

        private string _Status;
        public string Status
        {
            get { return _Status ?? status; }
            set { _Status = value; ModifyProperty(TaskProp.Status, value); }
        }
        public bool Completed
        {
            get { return (Status == "completed"); }
            set { if (value == true) { Status = "completed"; } else { Status = "pending"; } }
        }
    
        private string taskCommand
        {
            get
            {
                if (Changes.Count > 0)
                {
                    return BuildTaskString();
                }
                else
                {
                    return null;
                }
            }
        }
        public string TaskCommand {
            get { return "Task " + taskCommand; } }

        private string BuildTaskString()
        {
            string rcdateformat = "rc.dateformat:Y-M-DTH:N:S";
            string strCommand = uuid + " mod " + " ";
            bool hasdate = false;
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
                    case TaskProp.Due:
                        strCommand += "due:" + entry.Value + " ";
                        hasdate = true;
                        break;
                    case TaskProp.Status:
                        strCommand += "status:" + entry.Value + " ";
                        break;
                    default:
                        break;
                }
            }
            if (hasdate) { strCommand += rcdateformat; }
            Debug.WriteLine(strCommand);
            return strCommand;
        }


        private void ModifyProperty(TaskProp property, string value)
        {
            Changes[property] = value;
            Debug.WriteLine(Changes.Keys.Count);
            Debug.WriteLine(BuildTaskString());
            NotifyPropertyChanged("TaskCommand");
        }

        private void Save()
        {
            //Am I a new Task?
            if (uuid != null)
            {
                //Run the update command
                TaskBinary tbin = new TaskBinary();
                string strResult = tbin.RunCommand(taskCommand);
                Debug.WriteLine(strResult);

                //Update the form with the result of task uuid export

            }
        }

        private bool CanSave() { return (Changes.Count > 0); }

        private ICommand _saveCommand;
        public ICommand SaveCommand
        {
            get
            {
                if (_saveCommand == null)
                {
                    _saveCommand = new RelayCommand(
                        param => this.Save(),
                        param => this.CanSave()
                        );
                }
                return _saveCommand;
            }
        }

    }

}
