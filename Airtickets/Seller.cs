using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Threading;
using System.Text.RegularExpressions;

namespace Airtickets
{
    public partial class Seller : Form
    {
        Thread th;
        public string connection;
        public SqlDataAdapter sda;

        public Seller()
        {
            InitializeComponent();
            connection = @"Data Source=DESKTOP-TFL5VB9\SQLEXPRESS;Initial Catalog=Aviatickets;Integrated Security=True";
            update();
            update2();
        }

        public void update()
        {
            using (SqlConnection sqlconnection = new SqlConnection(connection))
            {
                DataSet ds = new DataSet();
                DataSet ds2 = new DataSet();
                string query = "select CONCAT(Flights.flights_id,'. ','Пермь - ',City.name,' | ',Flights.departure_date,' - ',Flights.arrival_date) as smth" +
                    " from Flights, Path, City WHERE Flights.path_id = Path.path_id and City.city_id = Path.arrival_place and Flights.departure_date>=CURRENT_TIMESTAMP ORDER BY Convert(datetime,Flights.departure_date, 104)";
                sda = new SqlDataAdapter(query, sqlconnection);
                sda.Fill(ds);
                comboBox6.DataSource = ds.Tables[0];
                comboBox6.DisplayMember = ds.Tables[0].Rows[0]["smth"].ToString();
                comboBox6.ValueMember = "smth";
            }
        }

        public void update2()
        {
            using (SqlConnection sqlconnection = new SqlConnection(connection))
            {
                DataSet ds = new DataSet();
                DataSet ds2 = new DataSet();
                string query = "SELECT Service_class.name as class_name FROM Service_class";
                sda = new SqlDataAdapter(query, sqlconnection);
                sda.Fill(ds);
                comboBox1.DataSource = ds.Tables[0];
                comboBox1.DisplayMember = ds.Tables[0].Rows[0]["class_name"].ToString();
                comboBox1.ValueMember = "class_name";
            }
        }

        private void Seller_FormClosing(object sender, FormClosingEventArgs e)
        {
            th = new Thread(open_Main);
            th.SetApartmentState(ApartmentState.STA);
            th.Start();
        }
        public void open_Main(object obj)
        {
            Application.Run(new MainForm());
        }

        public string login;

        private void button4_Click(object sender, EventArgs e)
        {
            // Получение данных из заполненных полей
            string surename = textBox1.Text;
            string name = textBox2.Text;
            string patronymic = textBox3.Text;
            DateTime date_of_birth = dateTimePicker1.Value;
            string passport = maskedTextBox1.Text;
            string flight_id = comboBox6.SelectedValue.ToString();
            flight_id = Regex.Replace(flight_id.Split()[0], @"[^0-9a-zA-Z\ ]+", "");
            string service_class = Convert.ToString(comboBox1.SelectedIndex + 1);
            int age = DateTime.Today.Year - date_of_birth.Year;

            if (surename == "" || name == "" || patronymic == "" || (age>=14 && (String.IsNullOrEmpty(maskedTextBox1.Text)==true) 
                || String.IsNullOrWhiteSpace(maskedTextBox1.Text)))
            {
                MessageBox.Show("Вы не заполнили одно из полей!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                // Занесение рейса в базу данных
                DataSet ds = new DataSet();
                DataSet ds2 = new DataSet();
                DataSet ds3 = new DataSet();
                using (SqlConnection sqlconnection = new SqlConnection(connection))
                {
                    sqlconnection.Open();

                    string query = "SELECT ISNULL(MAX(Passengers.passengers_id),0) as passengers_id FROM Passengers";
                    sda = new SqlDataAdapter(query, sqlconnection);
                    sda.Fill(ds);
                    int passengers_id = Convert.ToInt32(ds.Tables[0].Rows[0]["passengers_id"].ToString());

                    query = "SELECT ISNULL(MAX(Tickets.tickets_id),0) as tickets_id FROM Tickets";
                    sda = new SqlDataAdapter(query, sqlconnection);
                    sda.Fill(ds);
                    int tickets_id = Convert.ToInt32(ds.Tables[0].Rows[1]["tickets_id"].ToString()) + 1;

                    query = "SELECT Workers.worker_id as wid FROM Workers WHERE Workers.login = '" + login + "'";
                    sda = new SqlDataAdapter(query, sqlconnection);
                    sda.Fill(ds);
                    int worker_id = Convert.ToInt32(ds.Tables[0].Rows[2]["wid"].ToString());

                    query = "SELECT Flights.flight_duration as fdur FROM Flights WHERE Flights.flights_id = '" + flight_id + "'";
                    sda = new SqlDataAdapter(query, sqlconnection);
                    sda.Fill(ds);
                    int flight_duration = Convert.ToInt32(ds.Tables[0].Rows[3]["fdur"].ToString());

                    int price = 100 * flight_duration*Convert.ToInt32(service_class);

                    query = "SELECT passport FROM Passengers WHERE passport = '" + passport + "'";
                    sda = new SqlDataAdapter(query, sqlconnection);
                    sda.Fill(ds2);

                    query = "Select COUNT(Tickets.tickets_id) as count From Tickets,Flights, Service_class " +
                        "WHERE Flights.flights_id = Tickets.flights_id and  Service_class.service_class_id = Tickets.service_class_id " +
                        "and Service_class.service_class_id = "+service_class.ToString()+" and Tickets.flights_id = "+flight_id.ToString()+"";
                    sda = new SqlDataAdapter(query, sqlconnection);
                    sda.Fill(ds3);
                    int count = Convert.ToInt32(ds3.Tables[0].Rows[0]["count"].ToString());

                    query = "select seats_count as seats from Service_class where service_class_id = " + service_class.ToString() + "";
                    sda = new SqlDataAdapter(query, sqlconnection);
                    sda.Fill(ds3);
                    int seats = Convert.ToInt32(ds3.Tables[0].Rows[1]["seats"].ToString());

                    if (seats != count)
                    {
                        if (ds2.Tables[0].Rows.Count == 1)
                        {
                            query = "INSERT INTO [dbo].[Tickets]([tickets_id],[passengers_id],[flights_id],[service_class_id],[worker_id],[price]) " +
                                "VALUES('" + tickets_id + "','" + passengers_id + "','" + flight_id + "','" + service_class + "','" + worker_id + "','" + price + "')";
                            sda = new SqlDataAdapter(query, sqlconnection);
                            sda.Fill(ds);
                        }
                        else
                        {
                            query = "INSERT INTO [dbo].[Passengers]([passengers_id],[surname],[name],[patronymic],[date_of_birth],[passport]) " +
                                "VALUES('" + (passengers_id+1) + "','" + surename + "','" + name + "','" + patronymic + "','" + date_of_birth + "','" + passport + "')";
                            sda = new SqlDataAdapter(query, sqlconnection);
                            sda.Fill(ds);

                            query = "INSERT INTO [dbo].[Tickets]([tickets_id],[passengers_id],[flights_id],[service_class_id],[worker_id],[price]) " +
                                "VALUES('" + tickets_id + "','" + (passengers_id + 1) + "','" + flight_id + "','" + service_class + "','" + worker_id + "','" + price + "')";
                            sda = new SqlDataAdapter(query, sqlconnection);
                            sda.Fill(ds);
                        }

                        MessageBox.Show("Билет продан!", "Добавление", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Билетов в данный класс рейса нет!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    sqlconnection.Close();
                }
            }
        }
    }
}
