using System.Net;
using System.Net.Sockets;

namespace Monitoring
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }


        private void MainForm_Load(object sender, EventArgs e)
        {
            Task.Factory.StartNew(async () =>
            {
                await this.ReceiveData(8088);
            },
            TaskCreationOptions.LongRunning
            );
        }
    }
}