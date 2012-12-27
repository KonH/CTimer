using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.IO;
using System.Media;


namespace ComputerTimer
{
    public partial class Form1 : Form
    {
        List<Reminder> reminders;
        int current_time;
        bool disable;
        const string reminders_loc = "reminders.xml";
        SoundPlayer player = new SoundPlayer();

        public Form1()
        {

            InitializeComponent();
            Hide();
            WindowState = FormWindowState.Minimized;
            ShowInTaskbar = false;

            reminders = new List<Reminder>();
            if (File.Exists(reminders_loc))
            {
                XmlSerializer xmlFormat = new XmlSerializer(typeof(List<Reminder>));
                FileStream fStream = new FileStream(reminders_loc, FileMode.Open);
                reminders = (List<Reminder>)xmlFormat.Deserialize(fStream);
                fStream.Close();
                UpdateList();
            }
            else
            {
                reminders = new List<Reminder>();
            }

            current_time = 0;
            timer1.Start();
            disable = false;
        }

        public void UpdateList()
        {
            if (reminders.Count > 0)
            {
                dataGridView1.Rows.Clear();
                dataGridView1.Rows.Add(reminders.Count);
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    dataGridView1.Rows[i].Cells[0].Value = reminders[i].interval;
                    dataGridView1.Rows[i].Cells[1].Value = reminders[i].text;
                }
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == WindowState)
            {
                Hide();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
                WindowState = FormWindowState.Minimized;
            }
            else
            {
                string filename = reminders_loc;
                XmlSerializer xmlFormat = new XmlSerializer(typeof(List<Reminder>));
                FileMode file_mode = new FileMode();

                if (File.Exists(filename))
                    file_mode = FileMode.Truncate;
                else
                    file_mode = FileMode.Create;

                FileStream fStream = new FileStream(filename, file_mode, FileAccess.Write, FileShare.None);
                xmlFormat.Serialize(fStream, reminders);
                fStream.Close();
            }
        }

        private void oneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                Show();
                WindowState = FormWindowState.Normal;
            }
        }

        public void AddReminder(string text, int interval)
        {
            reminders.Add(new Reminder(text, interval));
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (disable == false)
            {  
                if (current_time + 1 >= int.MaxValue)
                {
                    current_time = 0;
                }
                current_time++;
                foreach (Reminder remind in reminders)
                {
                    if (WindowState != FormWindowState.Normal)
                    {
                        int next_time = current_time % remind.interval;
                        if (next_time == 0)
                        {
                            notifyIcon1.Text = "Computer Timer";
                            player.SoundLocation = "alert.wav";
                            player.Load();
                            player.Play();
                            MessageBox.Show(remind.text);
                        }
                        else
                        {
                            int remains = remind.interval - next_time;
                            int min = remains / 60;
                            string time = "";
                            if (min > 0)
                            {
                                time = min + "m " + (remains / min) + "s";
                            }
                            else
                            {
                                time = remains + "s";
                            }
                            notifyIcon1.Text = "Computer Timer (next in " + time + ")";
                        }
                    }
                }
            }
        }

        private void disableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (disable == false)
            {
                disable = true;
                contextMenuStrip1.Items[1].Text = "Enable";
            }
            else
            {
                disable = false;
                contextMenuStrip1.Items[1].Text = "Disable";
            }
        }

        private void btRemove_Click(object sender, EventArgs e)
        {
            int index = dataGridView1.SelectedCells[0].RowIndex;
            reminders.RemoveAt(index);
            dataGridView1.Rows.RemoveAt(index);
        }

        private void btAdd_Click(object sender, EventArgs e)
        {
            if (tbInterval.Text != "")
            {
                AddReminder(tbText.Text, int.Parse(tbInterval.Text));
                tbInterval.Text = "";
                tbText.Text = "";
                UpdateList();
            }
        }

        private void remindersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
        }

        private void autolaunchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Launch program at Windows start?", "Attention! Admin access requires", MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
            {
                Microsoft.Win32.RegistryKey myKey =
                    Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run\", true);
                myKey.SetValue("ComputerTimer", Application.ExecutablePath);
            }
            else
            {
                Microsoft.Win32.RegistryKey myKey =
                    Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run\", true);
                myKey.DeleteValue("ComputerTimer");
            }
        }

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            current_time = 0;
            timer1.Start();
        }
    }
}
