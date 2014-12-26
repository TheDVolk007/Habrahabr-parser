using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Habrahabr_news.Properties;
using HtmlAgilityPack;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;


namespace Habrahabr_news
{
    class FavoriteParser
    {
        readonly List<LinkLabel> favoritesList;
        readonly List<LinkLabel> tags;
        readonly Favorites form;
        int y1 = 10;

        public List<LinkLabel> Tags
        {
            get { return tags; }
        }
        public FavoriteParser(Favorites form)
        {
            this.form = form;
            favoritesList = new List<LinkLabel>();
            tags = new List<LinkLabel>();
            new List<LinkLabel>();
        }

        public async void OnLoad(string name)
        {
            string path = "http://habrahabr.ru/users/" + name + "/favorites/";
            form.ProgressBar1.Visible = true;
            form.ProgressBar1.Style = ProgressBarStyle.Marquee;
            form.ProgressBar1.MarqueeAnimationSpeed = 25;
            await GetFavoritesContentAsync(path);
            
            PrintContent(name);
        }
        private Task GetFavoritesContentAsync(string path)
        {
            return Task.Run(() =>
                {
                    GetFavoritesContent(path);
                });
        }
        private void GetFavoritesContent(string path)
        {
            //favoritesList = new List<LinkLabel>();
            WebClient client = new WebClient();
            client.Encoding = UTF8Encoding.UTF8;

            HtmlNodeCollection h1;
            HtmlDocument doc;
            do
            {
                try
                {
                    doc = new HtmlDocument();
                    doc.LoadHtml(client.DownloadString(path));
                    var check = doc.DocumentNode.SelectSingleNode("//h1[@class='title']");
                    if (check == null)
                        throw new NotAvailableFavoritesException("У данного пользователя ничего нет в \"Избранном\".");
                    h1 = doc.DocumentNode.SelectNodes("//h1[@class='title']");
                    doc.DocumentNode.SelectNodes("//div[@class='hubs']");

                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                    //form.Close();
                    return;
                }
            } while (h1 == null);
            //System.Diagnostics.Debug.WriteLine(count);
            LinkLabel label;
            int x = 5;
            foreach (var item in h1)
            {
                label = new LinkLabel();
                label.Location = new Point(x, y1);
                y1 += 30;
                label.Width = 300;
                label.Height = 30;
                string text = item.ChildNodes.Where(w => w.Name == "a").First().InnerText.Trim();
                int sepPos = text.IndexOf(" ", StringComparison.Ordinal);
                label.Text = text + "\n";
                string link = item.ChildNodes.Where(w => w.Name == "a").First().Attributes["href"].Value;
                label.Links.Add(0, sepPos, link);
                label.LinkClicked += form.label_LinkClicked;
                favoritesList.Add(label);
            }
            /*foreach (var item in divTags)
            {
                var tags = item.ChildNodes.Where(w => w.Name == "a");
                foreach (var innerItem in tags)
                {
                    label = new LinkLabel();
                    label.Location = new Point(x, y);
                    string text = innerItem.InnerText.Trim();
                    string link = innerItem.Attributes["href"].Value;
                    label.Width = 300;
                    label.Height = 30;
                    int sepPos = text.IndexOf(" ");
                    label.Text = text + "\n";
                    label.Links.Add(0, sepPos, link);
                    label.LinkClicked += form.label_LinkClicked;
                    
                    int index = this.tags.FindIndex(f => f.Text == label.Text);
                    if (index == -1)
                    {
                        y += 30;
                        this.tags.Add(label);
                    }
                    

                }
            }*/
            if (doc.DocumentNode.SelectSingleNode("//a[@id='next_page' and @class='next']") != null)
            {
                string nextPagePath = "http://habrahabr.ru" + doc.DocumentNode.SelectSingleNode("//a[@id='next_page' and @class='next']").Attributes["href"].Value;
                GetFavoritesContent(nextPagePath);
            }

            //System.Diagnostics.Debug.WriteLine(favoritesList.Count);
        }
        public List<LinkLabel> GetSortedByTagsFavoritesContent(string path)
        {
            WebClient client = new WebClient();
            client.Encoding = UTF8Encoding.UTF8;
            HtmlNodeCollection favoriteArticles;

            HtmlDocument doc;
            do
            {
                try
                {
                    doc = new HtmlDocument();
                    doc.LoadHtml(client.DownloadString(path));
                    var check = doc.DocumentNode.SelectSingleNode("//div[@class='user_favorites']");
                    if (check == null)
                        throw new NotAvailableFavoritesException("У данного пользователя ничего нет в \"Избранном\".");
                    //favoriteArticles = doc.DocumentNode.SelectNodes("//div[@class='post shortcuts_item']");
                    favoriteArticles = doc.DocumentNode.SelectNodes("//*[contains(@class,'post shortcuts_item')]");
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                    //form.Close();
                    return null;
                }
                
            } while (favoriteArticles == null);
            //System.Diagnostics.Debug.WriteLine(count);
            foreach (var item in favoriteArticles)
            {
                Debug.WriteLine(item.ChildNodes.Where(w => w.Name == "h1").First().ChildNodes.Where(b => b.Name == "a").First().InnerText.Trim());
                
                //подобный выбор вложенных нодов работает. можно параллельно создавать листы или словари
            }

            
            LinkLabel label;
            int x1 = 5;
            foreach (var item in favoriteArticles)
            {
                label = new LinkLabel();
                label.Location = new Point(x1, y1);
                y1 += 30;
                label.Width = 300;
                label.Height = 30;
                string text = item.ChildNodes.Where(w => w.Name == "h1").First().ChildNodes.First(b => b.Name == "a").InnerText.Trim();
                int sepPos = text.IndexOf(" ", StringComparison.Ordinal);
                label.Text = text + "\n";
                string link = item.ChildNodes.Where(w => w.Name == "h1").First().ChildNodes.First(b => b.Name == "a").Attributes["href"].Value;
                label.Links.Add(0, sepPos, link);
                label.LinkClicked += form.label_LinkClicked;

                List<LinkLabel> tagList = new List<LinkLabel>();

                LinkLabel tag;
                foreach (var innerItem in item.ChildNodes.Where(w => w.Name == "div[@class='hubs']"))
                {
                    
                    string text2 = item.ChildNodes.First(b => b.Name == "a").InnerText.Trim();
                    
                    string link2 = item.ChildNodes.First(b => b.Name == "a").Attributes["href"].Value;
                    tag = new LinkLabel();
                    tag.Text = text2;
                    tag.Links.Add(0, 1, link2);
                    tagList.Add(tag);
                }
                label.Tag = tagList;

                favoritesList.Add(label);
            }
            
            if (doc.DocumentNode.SelectSingleNode("//a[@id='next_page' and @class='next']") != null)
            {
                string nextPagePath;
                nextPagePath = "http://habrahabr.ru" + doc.DocumentNode.SelectSingleNode("//a[@id='next_page' and @class='next']").Attributes["href"].Value;
                GetFavoritesContent(nextPagePath);
            }
            return favoritesList;
            //System.Diagnostics.Debug.WriteLine(favoritesList.Count);
        }
        private void PrintContent(string name)
        {
            form.groupBox1.Controls.Clear();
            Panel pnl = new Panel();
            pnl.AutoScroll = true;
            pnl.Dock = DockStyle.Fill;
            if (favoritesList.Count == 0)
            {
                form.Dispose();
                return;
            }
            foreach (LinkLabel label in favoritesList)
            {
                pnl.Controls.Add(label);
            }

            /*if (tags.Count == 0)
            {
                form.Dispose();
                return;
            }
            foreach (LinkLabel label in tags)
            {
                pnl.Controls.Add(label);
                System.Diagnostics.Debug.WriteLine("отрисован элемент");
            }*/

            form.groupBox1.Text = Resources.Favorite_Parser_PrintContent_Избранные_статьи_пользователя_ + name.ToUpper();
            form.groupBox1.Controls.Add(pnl);
            y1 = 10;
        }
        
        [Serializable]
        public class NotAvailableFavoritesException : Exception
        {
            public NotAvailableFavoritesException() { }
            public NotAvailableFavoritesException(string message) : base(message) { }
            public NotAvailableFavoritesException(string message, Exception inner) : base(message, inner) { }
            protected NotAvailableFavoritesException(
              SerializationInfo info,
              StreamingContext context)
                : base(info, context) { }
        }
    }
}