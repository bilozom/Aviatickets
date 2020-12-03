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
    public partial class Search : Form
    {
        Thread th;
        public string connection;
        public SqlDataAdapter sda;

        public Search()
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
                sqlconnection.Open();
                DataSet ds = new DataSet();
                // Вывод необходимой ифнормации о продажах из бд
                string query = "select c.name as 'Город вылета',s.name as 'Город прилета', Flights.departure_date as 'Дата вылета', " +
                    "Flights.arrival_date as 'Дата прилета',CONVERT(VARCHAR(8), DATEADD(MINUTE, CONVERT(int, Flights.flight_duration), 0), 108) as 'Длительность полета', " +
                    "(Select CONCAT(SUM(Service_class.service_class_id * Flights.flight_duration*100), ' руб.') FROM Service_class, Flights " +
                    "WHERE Service_class.service_class_id = 1 and Flights.path_id = Path.path_id and Flights.departure_date >= CURRENT_TIMESTAMP) as 'Эконом-класс', " +
                    "(Select CONCAT(SUM(Service_class.service_class_id * Flights.flight_duration*100), ' руб.') FROM Service_class, Flights " +
                    "WHERE Service_class.service_class_id = 2 and Flights.path_id = Path.path_id and Flights.departure_date >= CURRENT_TIMESTAMP) as 'Комфорт-класс', " +
                    "(Select CONCAT(SUM(Service_class.service_class_id * Flights.flight_duration*100), ' руб.') FROM Service_class, Flights " +
                    "WHERE Service_class.service_class_id = 3 and Flights.path_id = Path.path_id and Flights.departure_date >= CURRENT_TIMESTAMP) as 'Бизнес-класс' " +
                    "from City as c, Path, City as s, Flights " +
                    "where Path.departure_place = c.city_id and Path.arrival_place = s.city_id and Flights.path_id = Path.path_id and " +
                    "Flights.departure_date >= CURRENT_TIMESTAMP ORDER BY Convert(datetime, Flights.departure_date, 104)";
                sda = new SqlDataAdapter(query, sqlconnection);
                sda.Fill(ds);
                dataGridView1.DataSource = ds.Tables[0];
                sqlconnection.Close();
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
                string query = "select City.name as сid from City, Country where City.country_id = Country.country_id and Country.name = '" + comboBox2.SelectedValue + "' and City.name != 'Пермь'";
                sda = new SqlDataAdapter(query, sqlconnection);
                sda.Fill(ds1);
                comboBox3.DataSource = ds1.Tables[0];
                comboBox3.DisplayMember = "сid";
                comboBox3.ValueMember = "сid";
            }
        }

        public void open_Main(object obj)
        {
            Application.Run(new MainForm());
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
            th = new Thread(open_Main);
            th.SetApartmentState(ApartmentState.STA);
            th.Start();
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

        public void open_Workers(object obj)
        {
            Application.Run(new Workers());
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
            th = new Thread(open_Workers);
            th.SetApartmentState(ApartmentState.STA);
            th.Start();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (SqlConnection sqlconnection = new SqlConnection(connection))
            {
                sqlconnection.Open();
                DataSet ds = new DataSet();
                // Вывод необходимой ифнормации о продажах из бд
                string query = "select c.name as 'Город вылета',s.name as 'Город прилета', Flights.departure_date as 'Дата вылета', " +
                    "Flights.arrival_date as 'Дата прилета', CONVERT(VARCHAR(8),DATEADD(MINUTE, CONVERT(int,Flights.flight_duration), 0),108) as 'Длительность полета' " +
                    "from City as c, Path, City as s, Flights " +
                    "where Path.departure_place = c.city_id and Path.arrival_place = s.city_id and Flights.path_id = Path.path_id " +
                    "and s.name = '" + comboBox3.SelectedValue.ToString() + "' " +
                    "and Flights.departure_date>=CURRENT_TIMESTAMP ORDER BY Convert(datetime,Flights.departure_date, 104)";
                sda = new SqlDataAdapter(query, sqlconnection);
                sda.Fill(ds);
                dataGridView1.DataSource = ds.Tables[0];
                sqlconnection.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            update();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            update3();
            comboBox3.Enabled = true;
        }
    }
}
