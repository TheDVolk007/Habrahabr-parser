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
    
    public partial class Settings : Form
    {
        public delegate void UpdateHandler(object sender, UpdateEventArgs e);
        public event UpdateHandler Updated;
        public int time;
        public Settings()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            try
            {
                time = int.Parse(textBox1.Text)*1000*60;
                UpdateEventArgs args = new UpdateEventArgs(time);
                Updated(this, args);
                this.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }
    }
    public class UpdateEventArgs: System.EventArgs
    {
        private int time;

        public int Time
        {
            get { return time; }
        }
        public UpdateEventArgs(int time)
        {
            this.time = time;
        }
    }
}
