using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Habrahabr_news
{
    public partial class Favorites : Form
    {
        Favorite_Parser fParser;
        
        public Favorites()
        {
            InitializeComponent();
            fParser = new Favorite_Parser(this);
            
        }
        public void label_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string target = e.Link.LinkData as string;
            if (null != target)
            {
                System.Diagnostics.Process.Start(target);
            }

        }

        private void Favorites_Load(object sender, EventArgs e)
        {
            Name nameForm = new Name();
            //nameForm.NameSelected += new Name.NameSelectHandler(Name_ButtonClicked);
            nameForm.ShowDialog();
            fParser.OnLoad(nameForm.textBox1.Text);
            //string path = "http://habrahabr.ru/users/" + nameForm.textBox1.Text + "/favorites/";
            //fParser.GetSortedByTagsFavoritesContent(path);
            //tAnalizer = new TagsAnalizer(fParser.GetSortedByTagsFavoritesContent(path));
            //tAnalizer.AnalizeTags();
        }

        

    }
    
}
