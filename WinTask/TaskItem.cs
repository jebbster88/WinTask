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
    public class TaskData
    {
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

        public bool HasSetValues { get { return (description != null || due != null || entry != null || project != null || status != null); } }

        public TaskData() {
            tags = new List<string>();
        }
    }
    public class TaskItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public TaskData BaseData = new TaskData();
        public TaskData ChangedData = new TaskData();

        public TaskItem()
        {
            Debug.WriteLine("Birgin Here");
            TaskData BaseData = new TaskData();
        }     

        const string writepattern = "yyyy-MM-dd'T'HH:mm:ss";
        const string readpattern = "yyyyMMdd'T'HHmmss'Z'";

        public int ID
        {
            get { return BaseData.id; }
        }
        public double Urgency
        {
            get
            {
                return BaseData.urgency;
            }
        }

        public string Description
        {
            get { return ChangedData.description ?? BaseData.description; }
            set { ChangedData.description = value; ModifyProperty(); }
        }
        public string Project
        {
            get { return ChangedData.project ?? BaseData.project; }
            set { if (value == BaseData.project) { ChangedData.project = null; } else { ChangedData.project = value; ModifyProperty(); } }
        }

        public DateTime? Due
        {
            get
            {
                if (ChangedData.due != null)
                {
                    DateTime retDate;
                    if (DateTime.TryParseExact(ChangedData.due, readpattern, null, DateTimeStyles.None, out retDate))
                    {
                        return retDate;
                    }
                    else return null;
                }
                else
                {
                    DateTime retDate;
                    if (DateTime.TryParseExact(BaseData.due, readpattern, null, DateTimeStyles.None, out retDate))
                    {
                        return retDate;
                    }
                    else return null;
                }
            }

            set
            {
                if (value == null)
                {
                    ChangedData.due = "";
                }
                else
                {
                    DateTime newDate = (DateTime)value;
                    ChangedData.due = newDate.ToString(writepattern, CultureInfo.InvariantCulture);
                }
                ModifyProperty();
            }
        }

        public string Status
        {
            get { Debug.WriteLine(BaseData.status); return ChangedData.status ?? BaseData.status; }
            set { ChangedData.status = value; ModifyProperty(); }
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
                string ret = (ChangedData.HasSetValues) ? BuildTaskString() : null;
                return ret;
            }
        }
        public string TaskCommand {
            get { if (taskCommand != null) { return "Task " + taskCommand; } else { return null; } }
        }

        private string BuildTaskString()
        {
            string rcdateformat = "rc.dateformat:Y-M-DTH:N:S";
            string strCommand = BaseData.uuid + " mod " + " ";
            bool hasdate = false;
            if (ChangedData.description != null) { strCommand += ChangedData.description + " "; }
            if (ChangedData.project != null) { strCommand += "project:" + ChangedData.project + " "; }
            if (ChangedData.due != null) { strCommand += "due:" + ChangedData.due; hasdate = true; }
            if (ChangedData.status != null) { strCommand += "status:" + ChangedData.status; }
            if (hasdate) { strCommand += rcdateformat; }
            Debug.WriteLine(strCommand);
            return strCommand;
        }


        private void ModifyProperty([CallerMemberName] String propertyName = "")
        {
            Debug.WriteLine("Property Changed: " + propertyName);
            Debug.WriteLine(BuildTaskString());
            NotifyPropertyChanged("TaskCommand");
        }

        private void Save()
        {
            //Am I a new Task?
            if (BaseData.uuid != null)
            {
                //Run the update command
                TaskBinary tbin = new TaskBinary();
                string strResult = tbin.RunCommand(taskCommand);
                Debug.WriteLine(strResult);

                //Update the form with the result of task uuid export

            }
        }

        private bool CanSave() { return (ChangedData.HasSetValues); }

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
