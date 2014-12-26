using System;
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
                if(Updated != null)
                {
                    Updated(this, args);
                }
                Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }
    }
    public class UpdateEventArgs: EventArgs
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
