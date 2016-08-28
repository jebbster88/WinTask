using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WinTask
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TaskList tlist = new TaskList();
        public MainWindow()
        {
            InitializeComponent();
            tlist.Update();
            MainGrid.DataContext = tlist;
        }



        private void taskGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (taskGrid.SelectedItem != null)
            {
                editGrid.IsEnabled = true;
            }
            else
            {
                editGrid.IsEnabled = false;
            }
        }
    }
}

