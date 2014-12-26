using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Habrahabr_news.Properties;
using HtmlAgilityPack;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;


namespace Habrahabr_news
{
    [Serializable]
    public class MainParser
    {
        int time;
        List<LinkLabel> newsList;
        List<LinkLabel> updateTempList;
        readonly WebClient client;
        readonly Form1 form;
        ProgressBar pb = new ProgressBar();
        public int Time
        {
            get { return time; }
            set { time = value; }
        }
        public MainParser(Form1 form)
        {
            client = new WebClient();
            time = 1000 * 60 * 5;
            this.form = form;
            newsList = new List<LinkLabel>();

            client = new WebClient();
            client.Encoding = UTF8Encoding.UTF8;

            pb.Visible = true;
            pb.Style = ProgressBarStyle.Marquee;
            pb.MarqueeAnimationSpeed = 25;
            pb.Dock = DockStyle.Bottom;
            
        }
        public MainParser()
        {

        }
        public async void OnLoad()
        {
            //Action<List<LinkLabel>> GMC = new Action<List<LinkLabel>>(GetMainContent);
            //GetMainContent(newsList);
            //IAsyncResult ar = GMC.BeginInvoke(newsList,null,null);
            //ProgressBar pb = new ProgressBar();
            
            form.Controls.Add(pb);
            /*form.ProgressBar1.Visible = true;
            form.ProgressBar1.Style = ProgressBarStyle.Marquee;
            form.ProgressBar1.MarqueeAnimationSpeed = 25;*/
            await GetMainContent(newsList);
            pb.Visible = false;
            PrintContent();
            
        }
        public async void Update()
        {
            updateTempList = new List<LinkLabel>();
            bool somethingNew = false;
            form.Controls.Add(pb);
            await GetMainContent(updateTempList);
            if (updateTempList != newsList)
            {
                foreach (var item in updateTempList)
                {
                    int index = newsList.FindIndex(f => f.Text == item.Text);
                    if (index != -1)
                    {
                        somethingNew = true;
                    }
                }

                newsList = updateTempList;
                PrintContent();
                if(somethingNew)
                    GetUserAttention();
            }
            else
            {
                pb.Visible = false;
            }


        }
        private Task GetMainContent(List<LinkLabel> list)
        {
            return Task.Run(() =>
                {
                    HtmlNodeCollection h1;
                    int count = 0;
                    do
                    {
                        HtmlDocument doc = new HtmlDocument();
                        doc.LoadHtml(client.DownloadString("http://habrahabr.ru/posts/top/daily/"));
                        h1 = doc.DocumentNode.SelectNodes("//h1[@class='title']");
                        count++;
                        //System.Threading.Thread.Sleep(5000);
                        Debug.WriteLine("Попытка обработать страницу...");
                    } while (h1 == null);
                    Debug.WriteLine(count);
                    /*Пришлось использовать цикл do..while, т.к. по пока неустановленным причинам
                      часто загружается поврежденная страница, которая не поддается обработке*/
                    LinkLabel label;
                    int x = 5;
                    int y = 10;

                    foreach (var item in h1)
                    {
                        label = new LinkLabel();
                        label.Location = new Point(x, y);
                        
                        label.Width = 300;
                        
                        string text = item.ChildNodes.Where(w => w.Name == "a").First().InnerText.Trim();
                        label.Text = text + "\n";
                        //попытка уместить длинный текст в лэйбле
                        if (label.Text.Length >= "Дайджест интересных материалов из мира веб-разработки и IT за последнюю неделю №131 (20 — 26".Length)
                        {
                            label.Height = 48;
                            y += 48;
                        }
                        else if (label.Text.Length <= "Дайджест интересных новостей и материалов из мира".Length)
                        {
                            label.Height = 23;
                            y += 23;
                        }
                        else
                        {
                            y += 36;
                            label.Height = 36;
                        }
                        string link = item.ChildNodes.Where(w => w.Name == "a").First().Attributes["href"].Value;
                        label.Links.Add(0, label.Text.Length, link);
                        label.LinkClicked += form.label_LinkClicked;
                        list.Add(label);
                    }
                }
            );
        }
        private void GetUserAttention()
        {
            form.NotifyIcon1.BalloonTipText = Resources.Main_Parser_GetUserAttention_Появились_новые_статьи_;
            form.NotifyIcon1.BalloonTipTitle = Resources.Main_Parser_GetUserAttention_Habrahabr_News;
            form.NotifyIcon1.ShowBalloonTip(3000);
        }
        private void PrintContent()
        {
            form.groupBox1.Controls.Clear();
            Panel pnl = new Panel();
            pnl.AutoScroll = true;
            pnl.Dock = DockStyle.Fill;
            foreach (LinkLabel label in newsList)
            {
                pnl.Controls.Add(label);
            }
            form.groupBox1.Controls.Add(pnl);
        }
        
    }
}
