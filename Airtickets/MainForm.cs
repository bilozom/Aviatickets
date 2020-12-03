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
    public partial class MainForm : Form
    {
        Thread th;

        public MainForm()
        {
            InitializeComponent();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
            th = new Thread(open_Workers);
            th.SetApartmentState(ApartmentState.STA);
            th.Start();
        }

        public void open_Workers(object obj)
        {
            Application.Run(new Workers());
        }

        public void open_Schedule(object obj)
        {
            Application.Run(new Schedule());
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Close();
            th = new Thread(open_Schedule);
            th.SetApartmentState(ApartmentState.STA);
            th.Start();
        }

        public void open_Search(object obj)
        {
            Application.Run(new Search());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
            th = new Thread(open_Search);
            th.SetApartmentState(ApartmentState.STA);
            th.Start();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
            th = new Thread(open_Search);
            th.SetApartmentState(ApartmentState.STA);
            th.Start();
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}
