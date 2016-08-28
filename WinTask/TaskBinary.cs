using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace WinTask
{
    class TaskBinary 
    {
        string GetBinaryPath()
        {
            return "G:\\babun\\.babun\\cygwin\\bin\\task.exe";
        }
        //task add test23 rc.verbose=new-uuid
        Process p;

        public TaskBinary()
        {
            p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = GetBinaryPath();
        }

        public string RunCommand(string arguments)
        {
            p.StartInfo.Arguments = arguments;
            p.Start();
            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            return output;
        }

    }
}
