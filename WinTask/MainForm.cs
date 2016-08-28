using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;

namespace WinTask
{
    public partial class MainForm : Form
    {

        TaskList tlist = new TaskList();
        BindingSource taskBS = new BindingSource();
        BindingSource tasklistBS = new BindingSource();
        SortableBindingList<TaskItem> filteredTaskList;

        public MainForm()
        {
            InitializeComponent();
            tlist.Update();
            filteredTaskList = new SortableBindingList<TaskItem>(tlist.Tasks.Where(x=> (x.status != "completed")).ToList());
            tasklistBS.DataSource = filteredTaskList; // tlist.Tasks;
            taskGrid.AutoGenerateColumns = false;
            taskGrid.Columns.Add("id", "ID");
            taskGrid.Columns.Add("description", "Description");
            taskGrid.Columns.Add("project", "Project");
            taskGrid.Columns.Add("urgency", "Urg");
            taskGrid.Columns["id"].DataPropertyName = "id";
            taskGrid.Columns["description"].DataPropertyName = "description";
            taskGrid.Columns["project"].DataPropertyName = "project";
            taskGrid.Columns["urgency"].DataPropertyName = "urgency";
            foreach (DataGridViewColumn col in taskGrid.Columns)
            {
                col.SortMode = DataGridViewColumnSortMode.Automatic;
            }
            taskGrid.DataSource = tasklistBS;
            taskGrid.CurrentCell = taskGrid[0,0];
            txtDescription.DataBindings.Add("Text", taskBS, "Description", false, DataSourceUpdateMode.OnPropertyChanged);
            foreach (string project in tlist.Projects)
            {
                Debug.WriteLine(project);
                comboProject.Items.Add(project);
            }
            comboProject.DataBindings.Add("SelectedItem", taskBS, "Project", false, DataSourceUpdateMode.OnPropertyChanged);
            //dateTimeDue.DataBindings.Add("Value", taskBS, "Due", false, DataSourceUpdateMode.OnPropertyChanged); //http://blogs.interknowlogy.com/2007/01/21/winforms-databinding-datetimepicker-to-a-nullable-type/

            txtCommand.DataBindings.Add("Text", taskBS, "TaskCommand");
        }

        //private void PopulateNullableDateControl(DateTime
        
        private void PopulateControls()
        {
            /*
            if (taskGrid.SelectedRows.Count > 0)
            {
                dateTimeDue.DataBindings.Clear();
                dateTimeDue.Checked = false;
                TaskItem task = taskGrid.SelectedRows[0].DataBoundItem as TaskItem;
                EditTaskItem edittask = new EditTaskItem(task);
                Debug.WriteLine(edittask.Description);
                Debug.WriteLine(edittask.Project);
                taskBS.DataSource = edittask;
                if (edittask.Due != null)
                {
                    dateTimeDue.Checked = true;
                    dateTimeDue.DataBindings.Add("Value", taskBS, "Due", false, DataSourceUpdateMode.OnPropertyChanged);
                }
            }
            */
        }
        private void taskGrid_CurrentCellChanged(object sender, EventArgs e)
        {
            //taskBS.DataSource = taskGrid.CurrentRow.DataBoundItem;
            PopulateControls();
        }

        private void comboProject_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboProject.DataBindings[0].WriteValue();
        }

    }
}
