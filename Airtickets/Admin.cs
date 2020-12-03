using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace Airtickets
{
    public partial class Admin : Form
    {
        Thread th;
        public string connection;
        public SqlDataAdapter sda;

        public Admin()
        {
            InitializeComponent();
            connection = @"Data Source=DESKTOP-TFL5VB9\SQLEXPRESS;Initial Catalog=Aviatickets;Integrated Security=True";
            update();
            update2();
            update4();
            update5();
        }

        public void open_Main(object obj)
        {
            Application.Run(new MainForm());
        }

        private void Admin_FormClosing(object sender, FormClosingEventArgs e)
        {
            th = new Thread(open_Main);
            th.SetApartmentState(ApartmentState.STA);
            th.Start();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Получение данных из заполненных полей
            string surename = textBox1.Text;
            string name = textBox2.Text;
            string patronymic = textBox3.Text;
            string login = textBox4.Text;
            string password = textBox5.Text;

            if (login == "" || password == "" || patronymic == "" || surename == "" || name == "")
            {
                MessageBox.Show("Вы не заполнили одно из полей!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                // Занесение пользователя в базу данных
                DataSet ds = new DataSet();
                using (SqlConnection sqlconnection = new SqlConnection(connection))
                {
                    sqlconnection.Open();
                    // Получение списка пользователей
                    string query = "SELECT * FROM Workers WHERE login = '" + login + "'";
                    sda = new SqlDataAdapter(query, sqlconnection);
                    sda.Fill(ds);

                    if (ds.Tables[0].Rows.Count == 1)
                    {
                        MessageBox.Show("Пользователь с данным логином уже зарегистрирован!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        // Занесение пользователя в базу данных
                        query = "SELECT MAX(Workers.worker_id) as max FROM Workers";
                        sda = new SqlDataAdapter(query, sqlconnection);
                        sda.Fill(ds);
                        int i = Convert.ToInt32(ds.Tables[0].Rows[0]["max"].ToString()) + 1;

                        query = "INSERT INTO [dbo].[Workers]([worker_id],[surname],[name],[patronymic],[login],[password],[rights_id]) " +
                            "VALUES('" + i + "','" + surename + "','" + name + "','" + patronymic + "','" + login + "','" + password + "','" + 2 + "')";
                        sda = new SqlDataAdapter(query, sqlconnection);
                        sda.Fill(ds);
                        MessageBox.Show("Вы зарегистрировали нового кассира!", "Регистрация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    sqlconnection.Close();
                }
            }
        }

        public void update()
        {
            using (SqlConnection sqlconnection = new SqlConnection(connection))
            {
                DataSet ds1 = new DataSet();
                string query = "select Workers.login as uid from Workers WHERE rights_id !=1";
                sda = new SqlDataAdapter(query, sqlconnection);
                sda.Fill(ds1);
                comboBox1.DataSource = ds1.Tables[0];
                comboBox1.DisplayMember = "uid";
                comboBox1.ValueMember = "uid";
                comboBox5.DataSource = ds1.Tables[0];
                comboBox5.DisplayMember = "uid";
                comboBox5.ValueMember = "uid";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                DataSet ds = new DataSet();
                using (SqlConnection sqlconnection = new SqlConnection(connection))
                {
                    sqlconnection.Open();
                    // Удаление данных из бд
                    string query = "DELETE FROM Workers WHERE Workers.login= '" + comboBox1.SelectedValue + "'";
                    sda = new SqlDataAdapter(query, sqlconnection);
                    sda.Fill(ds);
                    MessageBox.Show("Операция совершена!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch
            {
                MessageBox.Show("Error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void update2()
        {
            using (SqlConnection sqlconnection = new SqlConnection(connection))
            {
                DataSet ds1 = new DataSet();
                string query = "select Country.name as сid from Country";
                sda = new SqlDataAdapter(query, sqlconnection);
                sda.Fill(ds1);
                comboBox2.DataSource = ds1.Tables[0];
                comboBox2.DisplayMember = "сid";
                comboBox2.ValueMember = "сid";
                update3();
            }
        }


        public void update3()
        {
            using (SqlConnection sqlconnection = new SqlConnection(connection))
            {
                DataSet ds1 = new DataSet();
                string query = "select City.name as сid from City, Country where City.country_id = Country.country_id and Country.name = '"+comboBox2.SelectedValue+ "' and City.name != 'Пермь'";
                sda = new SqlDataAdapter(query, sqlconnection);
                sda.Fill(ds1);
                comboBox3.DataSource = ds1.Tables[0];
                comboBox3.DisplayMember = "сid";
                comboBox3.ValueMember = "сid";
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Получение данных из заполненных полей
            string departure_date = maskedTextBox1.Text;
            string arrival_date = maskedTextBox2.Text;
            string flight_duration = textBox8.Text;
            string country = comboBox2.SelectedValue.ToString();
            string city = comboBox3.SelectedValue.ToString();
            string count = textBox10.Text;

            if (departure_date == "" || arrival_date == "" || flight_duration == "" || country == "" || city == "" || count == "")
            {
                MessageBox.Show("Вы не заполнили одно из полей!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                // Занесение рейса в базу данных
                DataSet ds = new DataSet();
                using (SqlConnection sqlconnection = new SqlConnection(connection))
                {
                    sqlconnection.Open();

                    // Занесение рейса в базу данных
                    string query = "SELECT City.city_id as departure FROM City where City.name = 'Пермь'";
                    sda = new SqlDataAdapter(query, sqlconnection);
                    sda.Fill(ds);
                    int departure = Convert.ToInt32(ds.Tables[0].Rows[0]["departure"].ToString());

                    query = "SELECT City.city_id as cid FROM City where City.name = '" + comboBox3.SelectedValue.ToString() + "'";
                    sda = new SqlDataAdapter(query, sqlconnection);
                    sda.Fill(ds);
                    int cid = Convert.ToInt32(ds.Tables[0].Rows[1]["cid"].ToString());

                    query = "SELECT ISNULL(MAX(Path.path_id),0) as path_id FROM Path";
                    sda = new SqlDataAdapter(query, sqlconnection);
                    sda.Fill(ds);
                    int path_id = Convert.ToInt32(ds.Tables[0].Rows[2]["path_id"].ToString()) + 1;

                    query = "SELECT ISNULL(MAX(Flights.flights_id),0) as max FROM Flights";
                    sda = new SqlDataAdapter(query, sqlconnection);
                    sda.Fill(ds);
                    int i = Convert.ToInt32(ds.Tables[0].Rows[3]["max"].ToString()) + 1;

                    query = "INSERT INTO [dbo].[Flights]([flights_id],[departure_date],[arrival_date],[flight_duration],[path_id],[seats_count]) " +
                        "VALUES('" + i + "','" + departure_date + "','" + arrival_date + "','" + flight_duration + "','" + path_id + "','" + count + "')";
                    sda = new SqlDataAdapter(query, sqlconnection);
                    sda.Fill(ds);

                    query = "INSERT INTO [dbo].[Path]([path_id],[departure_place],[arrival_place]) " +
                        "VALUES('" + path_id + "','" + departure + "','" + cid + "')";
                    sda = new SqlDataAdapter(query, sqlconnection);
                    sda.Fill(ds);
                    MessageBox.Show("Вы добавили новый рейс!", "Добавление", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    sqlconnection.Close();
                }
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            update3();
            comboBox3.Enabled = true;
        }

        public void update4()
        {
            using (SqlConnection sqlconnection = new SqlConnection(connection))
            {
                DataSet ds = new DataSet();
                DataSet ds2 = new DataSet();
                string query = "select CONCAT(Flights.flights_id,'. ','Пермь - ',City.name,' | ',Flights.departure_date,' - ',Flights.arrival_date) as smth" +
                    " from Flights, Path, City WHERE Flights.path_id = Path.path_id and City.city_id = Path.arrival_place";
                sda = new SqlDataAdapter(query, sqlconnection);
                sda.Fill(ds);
                comboBox6.DataSource = ds.Tables[0];
                comboBox6.DisplayMember = ds.Tables[0].Rows[0]["smth"].ToString();
                comboBox6.ValueMember = "smth";
                comboBox4.DataSource = ds.Tables[0];
                comboBox4.DisplayMember = ds.Tables[0].Rows[0]["smth"].ToString();
                comboBox4.ValueMember = "smth";
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                DataSet ds = new DataSet();
                string c1 = comboBox6.SelectedValue.ToString();
                c1 = Regex.Replace(c1.Split()[0], @"[^0-9a-zA-Z\ ]+", "");

                using (SqlConnection sqlconnection = new SqlConnection(connection))
                {
                    sqlconnection.Open();
                    // Изменение данных в бд
                    string query = "Update dbo.Flights SET departure_date = '" + maskedTextBox4.Text + "', arrival_date = '" + maskedTextBox3.Text + "', flight_duration = '" + textBox6.Text + "' WHERE Flights.flights_id= " + c1 + "";
                    sda = new SqlDataAdapter(query, sqlconnection);
                    sda.Fill(ds);
                    MessageBox.Show("Операция совершена!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch
            {
                MessageBox.Show("Ошибка", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                maskedTextBox3.Enabled = true;
                maskedTextBox4.Enabled = true;
                textBox6.Enabled = true;
                button4.Enabled = true;

                using (SqlConnection sqlconnection = new SqlConnection(connection))
                {
                    DataSet ds2 = new DataSet();
                    string c1 = comboBox6.SelectedValue.ToString();
                    c1 = Regex.Replace(c1.Split()[0], @"[^0-9a-zA-Z\ ]+", "");
                    string query = "SELECT flight_duration as flight_duration " +
                        "FROM Flights where flights_id = '" + c1 + "'";
                    sda = new SqlDataAdapter(query, sqlconnection);
                    sda.Fill(ds2);
                    textBox6.Text = ds2.Tables[0].Rows[0]["flight_duration"].ToString();

                    query = "SELECT  CONCAT(CONVERT(varchar,departure_date,104),' ',CONVERT(varchar,departure_date,108)) as departure_date " +
                        "FROM Flights where flights_id = '" + c1 + "'";
                    sda = new SqlDataAdapter(query, sqlconnection);
                    sda.Fill(ds2);
                    maskedTextBox4.Text = ds2.Tables[0].Rows[1]["departure_date"].ToString();

                    query = "SELECT  CONCAT(CONVERT(varchar,arrival_date,104),' ',CONVERT(varchar,arrival_date,108)) as arrival_date" +
                        " FROM Flights where flights_id = '" + c1 + "'";
                    sda = new SqlDataAdapter(query, sqlconnection);
                    sda.Fill(ds2);
                    maskedTextBox3.Text = ds2.Tables[0].Rows[2]["arrival_date"].ToString();
                }
            }
            catch
            {
                MessageBox.Show("Ошибка", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                DataSet ds = new DataSet();
                DataSet ds2 = new DataSet();
                string c1 = comboBox6.SelectedValue.ToString();
                c1 = Regex.Replace(c1.Split()[0], @"[^0-9a-zA-Z\ ]+", "");
                using (SqlConnection sqlconnection = new SqlConnection(connection))
                {
                    sqlconnection.Open();
                    // Удаление данных из бд
                    string query = "SELECT Flights.path_id as pid FROM Flights WHERE Flights.flights_id= " + c1 + "";
                    sda = new SqlDataAdapter(query, sqlconnection);
                    sda.Fill(ds2);
                    int p = Convert.ToInt32(ds2.Tables[0].Rows[0]["pid"].ToString());

                    query = "DELETE FROM Path WHERE Path.path_id= " + p + "";
                    sda = new SqlDataAdapter(query, sqlconnection);
                    sda.Fill(ds);

                    query = "DELETE FROM Flights WHERE Flights.flights_id= " + c1 + "";
                    sda = new SqlDataAdapter(query, sqlconnection);
                    sda.Fill(ds);
                    MessageBox.Show("Операция совершена!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch
            {
                MessageBox.Show("Ошибка", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void update5()
        {
            using (SqlConnection sqlconnection = new SqlConnection(connection))
            {
                sqlconnection.Open();
                DataSet ds = new DataSet();
                // Вывод необходимой ифнормации о продажах из бд
                string query = "select c.name as 'Город вылета',s.name as 'Город прилета', Flights.departure_date as 'Дата вылета', " +
                    "Flights.arrival_date as 'Дата прилета', Workers.login as 'Продавец' " +
                    "from City as c, Path, City as s, Flights, Tickets, Workers " +
                    "where Path.departure_place = c.city_id and Path.arrival_place = s.city_id and Flights.path_id = Path.path_id " +
                    "and Tickets.flights_id = Flights.flights_id and Tickets.worker_id = Workers.worker_id";
                sda = new SqlDataAdapter(query, sqlconnection);
                sda.Fill(ds);
                dataGridView1.DataSource = ds.Tables[0];
                sqlconnection.Close();
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            string seller = comboBox5.SelectedValue.ToString();

            using (SqlConnection sqlconnection = new SqlConnection(connection))
            {
                sqlconnection.Open();
                DataSet ds = new DataSet();
                string year = textBox1.Text;
                int month = comboBox2.SelectedIndex + 1;

                string query = "select c.name as 'Город вылета',s.name as 'Город прилета', Flights.departure_date as 'Дата вылета', " +
                    "Flights.arrival_date as 'Дата прилета', Workers.login as 'Продавец' " +
                    "from City as c, Path, City as s, Flights, Tickets, Workers " +
                    "where Path.departure_place = c.city_id and Path.arrival_place = s.city_id and Flights.path_id = Path.path_id " +
                    "and Tickets.flights_id = Flights.flights_id and Tickets.worker_id = Workers.worker_id and Workers.login = '"+seller+"'";

                sda = new SqlDataAdapter(query, sqlconnection);
                sda.Fill(ds);
                dataGridView1.DataSource = ds.Tables[0];
                sqlconnection.Close();
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            update5();
        }
    }
}
