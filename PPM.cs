using MaterialSkin.Controls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pope_Process_Monitor
{
    public partial class PPM : MaterialForm
    {
        public PPM()
        {
            InitializeComponent();
        }

        public List<string> ActiveWatchers = new List<string>();

        private void Form1_Load(object sender, EventArgs e)
        {
            foreach (System.Diagnostics.Process p in System.Diagnostics.Process.GetProcesses())
            {
                if (!materialCheckedListBox1.Items.Where(x => x.Text == p.ProcessName).Any())
                {
                    materialCheckedListBox1.Items.Add(p.ProcessName);
                }
            }

            Task Watcher = new Task(() =>
            {
                while (true)
                {
                    foreach (System.Diagnostics.Process p in System.Diagnostics.Process.GetProcesses())
                    {
                        if (!materialCheckedListBox1.Items.Where(x => x.Text == p.ProcessName).Any())
                        {
                            materialCheckedListBox1.Invoke(new Action(() =>
                            {
                                materialCheckedListBox1.Items.Add(p.ProcessName);
                            }));
                        }
                    }

                    foreach (var item in materialCheckedListBox1.Items.Where(x => x.Checked))
                    {
                        if (!ActiveWatchers.Where(x => x.Split('\\').Last().Split('.').First() == item.Text).Any())
                        {
                            Invoke(new Action(() =>
                            {
                                foreach (var proc in System.Diagnostics.Process.GetProcessesByName(item.Text))
                                {
                                    ActiveWatchers.Add(proc.MainModule.FileName);
                                }
                            }));
                        }
                        else if (!materialCheckedListBox1.Items.Where(x => x.Text == item.Text).Any())
                        {
                            ActiveWatchers.Remove(item.Text);
                        }
                    }

                    foreach (var item in ActiveWatchers)
                    {
                        if (!System.Diagnostics.Process.GetProcessesByName(item.Split('\\').Last().Split('.').First()).Any())
                        {
                            System.Diagnostics.Process.Start(item);
                        }
                        else
                        {
                            if (System.Diagnostics.Process.GetProcessesByName(item.Split('\\').Last().Split('.').First()).Where(x => x.Responding == false).Any())
                            {
                                System.Diagnostics.Process.GetProcessesByName(item.Split('\\').Last().Split('.').First()).Where(x => x.Responding == false).First().Kill();
                            }
                        }
                    }

                    System.Threading.Thread.Sleep(1000);
                }
            });
            Watcher.Start();
        }

        private void materialCheckedListBox1_Click(object sender, EventArgs e)
        {
        }
    }
}
