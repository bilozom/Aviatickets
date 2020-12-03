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
using System.Data.SqlClient;

namespace Airtickets
{
    public partial class Autorization : Form
    {
        Thread th;
        public string connection;
        public SqlDataAdapter sda;

        public Autorization()
        {
            InitializeComponent();
            connection = @"Data Source=DESKTOP-TFL5VB9\SQLEXPRESS;Initial Catalog=Aviatickets;Integrated Security=True";
        }

        private void button3_Click(object sender, EventArgs e)
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

        public void open_Admin(object obj)
        {
            Application.Run(new Admin());
        }

        public string login;
        string password;

        private void button1_Click(object sender, EventArgs e)
        {
            login = textBox1.Text;
            password = textBox2.Text;

            // Вход в систему под администратором
            if (login == "kirill" && password == "kirill")
            {
                MessageBox.Show("Вы зашли как администратор!", "Вход", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                // Открытие формы администратора
                this.Close();
                th = new Thread(open_Admin);
                th.SetApartmentState(ApartmentState.STA);
                th.Start();
            }
            else
            {
                string query = "SELECT login,password FROM Workers WHERE login = '" + login + "' AND password = '" + password + "'";
                using (SqlConnection sqlconnection = new SqlConnection(connection))
                {
                    sqlconnection.Open();

                    sda = new SqlDataAdapter(query, sqlconnection);
                    DataSet ds = new DataSet();
                    sda.Fill(ds);
                    // Проверка корректности введенных логина и пароля
                    if (ds.Tables[0].Rows.Count == 0)
                    {
                        MessageBox.Show("Логин или пароль указаны неверно!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        Seller seller = new Seller();
                        seller.login = this.login;
                        MessageBox.Show("Вы зашли как кассир " + login + "!", "Вход", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        seller.Show();
                        // Открытие формы кассира
                        this.Hide();
                    }
                    sqlconnection.Close();
                }
            }
        }
    }
}
