using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HtmlAgilityPack;
using System.IO;
using System.Xml.Serialization;

namespace Habrahabr_news
{
    public partial class Form1 : Form
    {

        Main_Parser parser;
        List<LinkLabel> newsList;
        public Form1()
        {
            InitializeComponent();
            newsList = new List<LinkLabel>();
            parser = new Main_Parser(this);
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
                System.Diagnostics.Process.Start(target);
            }
            LinkLabel label = sender as LinkLabel;
            label.LinkVisited = true;
            
        }
        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("DVolk inc., 2014\ndvolk007@mail.ru");
        }
        private void UpdateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings setForm = new Settings();
            setForm.Updated += new Settings.UpdateHandler(Settings_ButtonClicked);
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
                
                this.ShowInTaskbar = false;
                notifyIcon1.Visible = true;
                notifyIcon1.BalloonTipText = "Приложение работает в фоновом режиме!";
                notifyIcon1.BalloonTipTitle = "Habrahabr News";
                notifyIcon1.ShowBalloonTip(3000);
                
            }
            
        }
        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;
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
