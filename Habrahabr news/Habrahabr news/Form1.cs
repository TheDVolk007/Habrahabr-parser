using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;
using Habrahabr_news.Properties;


namespace Habrahabr_news
{
    public partial class Form1 : Form
    {
        readonly MainParser parser;

        public Form1()
        {
            InitializeComponent();
            // ReSharper disable once ObjectCreationAsStatement
            new List<LinkLabel>();
            parser = new MainParser(this);
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            
        }
        void updateTimer_Tick(object sender, EventArgs e)
        {
            parser.Update();
        }
        public void label_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string target = e.Link.LinkData as string;
            if (null != target)
            {
                Process.Start(target);
            }
            LinkLabel label = sender as LinkLabel;
            if(label != null)
            {
                label.LinkVisited = true;
            }
        }
        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(Resources.Form1_AboutToolStripMenuItem_Click_);
        }
        private void UpdateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings setForm = new Settings();
            setForm.Updated += Settings_ButtonClicked;
            setForm.Show();
        }
        private void Settings_ButtonClicked(object sender, UpdateEventArgs e)
        {
            parser.Time = e.Time;
            parser.OnLoad();
        }
        private void FavoritesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Favorites favForm = new Favorites();
            favForm.Show();
        }
        private void Form1_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                
                ShowInTaskbar = false;
                notifyIcon1.Visible = true;
                notifyIcon1.BalloonTipText = Resources.Form1_Form1_Resize_Приложение_работает_в_фоновом_режиме_;
                notifyIcon1.BalloonTipTitle = Resources.Form1_Form1_Resize_Habrahabr_News;
                notifyIcon1.ShowBalloonTip(3000);
                
            }
            
        }
        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            WindowState = FormWindowState.Normal;
            ShowInTaskbar = true;
            notifyIcon1.Visible = false;
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            parser.OnLoad();
            Timer updateTimer = new Timer();
            updateTimer.Interval = parser.Time;
            updateTimer.Tick += updateTimer_Tick;
            updateTimer.Start();
        }

        private void WriteDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileStream fs = new FileStream("dataLog.xml", FileMode.OpenOrCreate, FileAccess.Write);
            XmlSerializer xmls = new XmlSerializer(parser.GetType());
            xmls.Serialize(fs, parser);
            fs.Close();
        }
        
    }
}
