using DAADScraper.Core.Handlers;

namespace DAAD_Scraper
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            new DaadHandler().Begin();
        }
    }
}
