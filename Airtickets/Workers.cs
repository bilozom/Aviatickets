using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace Airtickets
{
    public partial class Workers : Form
    {
        Thread th;

        public Workers()
        {
            InitializeComponent();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
            th = new Thread(open_Main);
            th.SetApartmentState(ApartmentState.STA);
            th.Start();
        }

        public void open_Main(object obj)
        {
            Application.Run(new MainForm());
        }

        public void open_Autorization(object obj)
        {
            Application.Run(new Autorization());
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Close();
            th = new Thread(open_Autorization);
            th.SetApartmentState(ApartmentState.STA);
            th.Start();
        }

        private void tableLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
