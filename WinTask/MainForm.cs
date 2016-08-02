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

        public MainForm()
        {
            InitializeComponent();
            tlist.Update();
            tasklistBS.DataSource = tlist.Tasks;
            taskGrid.AutoGenerateColumns = false;
            taskGrid.Columns.Add("id", "ID");
            taskGrid.Columns.Add("description", "Description");
            taskGrid.Columns["id"].DataPropertyName = "id";
            taskGrid.Columns["description"].DataPropertyName = "description";
            foreach (DataGridViewColumn col in taskGrid.Columns)
            {
                col.SortMode = DataGridViewColumnSortMode.Automatic;
            }
            taskGrid.DataSource = tasklistBS;
            taskBS.DataSource = tlist.Tasks.First();
            txtDescription.DataBindings.Add("Text", taskBS, "Description", false, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void taskGrid_CurrentCellChanged(object sender, EventArgs e)
        {
            //taskBS.DataSource = taskGrid.CurrentRow.DataBoundItem;
            if (taskGrid.SelectedRows.Count > 0)
            {
                TaskItem task = taskGrid.SelectedRows[0].DataBoundItem as TaskItem;
                Debug.WriteLine(task.description);
                taskBS.DataSource = task;
            }
        }
    }
}
