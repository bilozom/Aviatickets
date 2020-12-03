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
    public partial class Schedule : Form
    {
        Thread th;
        public string connection;
        public SqlDataAdapter sda;

        public Schedule()
        {
            InitializeComponent();
            connection = @"Data Source=DESKTOP-TFL5VB9\SQLEXPRESS;Initial Catalog=Aviatickets;Integrated Security=True";
            update();
            dataGridView1.Font = new Font(dataGridView1.ColumnHeadersDefaultCellStyle.Font.FontFamily, 16f, FontStyle.Bold);
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

        public void open_Search(object obj)
        {
            Application.Run(new Search());
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Close();
            th = new Thread(open_Search);
            th.SetApartmentState(ApartmentState.STA);
            th.Start();
        }

        private void tableLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
