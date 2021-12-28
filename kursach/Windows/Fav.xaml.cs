using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;

namespace kursach.Windows
{
    /// <summary>
    /// Логика взаимодействия для Fav.xaml
    /// </summary>
    public partial class Fav : Window
    {
        string connectionString;
        SqlDataAdapter adapter = new SqlDataAdapter();
        DataTable usersTable = new DataTable();

        List<Plane> planeList = new List<Plane>();
        public Fav(int IdUser)
        {
            InitializeComponent();

            //получаем строку подключения из app.config
            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            SqlCommand command = new SqlCommand();
            command.CommandText = "SELECT Icao24 FROM Favourites WHERE IdUser = " + Menu.UserId;
            command.Connection = connection;

            adapter.SelectCommand = command;
            adapter.Fill(usersTable);

            try
            {
                foreach (DataRow dr in usersTable.Rows)
                {
                    string answer = Connect("https://opensky-network.org/api/states/all?icao24=" + dr[0]);
                    planeList.Add(GetPlane(answer));
                }
                planeGrid.ItemsSource = planeList;
            }
            catch
            {
                MessageBox.Show("Проблемы с подключением...");
            }
            connection.Close();
        }

        //подключение к api
        public string Connect(string apiUrl)
        {
            WebRequest request = WebRequest.Create(apiUrl);
            request.Method = "GET";
            WebResponse response = request.GetResponse();
            string answer = string.Empty;
            using (Stream s = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    answer = reader.ReadToEnd();
                }
            }
            response.Close();
            return answer;
        }

        //заполнение таблицы
        public Plane GetPlane(string answer)
        {
            Plane plane = new Plane();

            JObject answerJson = JObject.Parse(answer);

            // get JSON result objects into a list
            IList<JToken> results = answerJson["states"].Children().ToList();

            // serialize JSON results into .NET objects            
            foreach (JToken result in results)
            {
                List<string> resultString = result.ToString().TrimStart('[').TrimEnd(']').Split(',').ToList<string>();
                plane.Icao24 = resultString[0].Trim();
                plane.Callsign = resultString[1].Trim().TrimStart('"').TrimEnd('"');
                plane.Country = resultString[2].Trim().TrimStart('"').TrimEnd('"');
                plane.Longitude = resultString[5].Trim();
                plane.Latitude = resultString[6].Trim();
                plane.On_ground = resultString[8].Trim();
                plane.Velocity = resultString[9].Trim();
                plane.Altitude = resultString[13].Trim();
            }

            return plane;
        }

        //возвращение к предыдущему окну
        private void Back(object sender, RoutedEventArgs e)
        {
            //вернуться назад
            Windows.Account acc = new Windows.Account(Menu.UserId);
            acc.Show();
            Close();
        }

        //удаление строки из избранного
        private void Delete(object sender, RoutedEventArgs e)
        {
            if (planeGrid.SelectedItem != null)
            {
                Plane plane = (Plane)planeGrid.SelectedItem;

                if(string.IsNullOrEmpty(plane.Icao24))
                {
                    MessageBox.Show("Не удаётся удалить, повторите попытку позже");
                    return;
                }

                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();

                SqlCommand command = new SqlCommand();
                command.CommandText = "DELETE Favourites WHERE IdUser = " + Menu.UserId + " AND Icao24 = '" + plane.Icao24.Trim('"') + "'";
                command.Connection = connection;
                command.ExecuteNonQuery();

                connection.Close();

                Fun();
                
            }
            else
            {
                MessageBox.Show("Нет выделенной строки");
            }
        }

        //кнопка обновление
        private void Upd(object sender, RoutedEventArgs e)
        {
            Fun();
        }

        //функция обновление
        private void Fun()
        {
            try
            {
                planeGrid.ItemsSource = new List<int>();
                planeList.Clear();
                usersTable.Clear();

                //получаем строку подключения из app.config
                connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();

                SqlCommand command = new SqlCommand();
                command.CommandText = "SELECT Icao24 FROM Favourites WHERE IdUser = " + Menu.UserId;
                command.Connection = connection;

                adapter.SelectCommand = command;
                adapter.Fill(usersTable);

                foreach (DataRow dr in usersTable.Rows)
                {
                    string answer = Connect("https://opensky-network.org/api/states/all?icao24=" + dr[0]);
                    planeList.Add(GetPlane(answer));
                }
                planeGrid.ItemsSource = planeList;
                connection.Close();
            }
            catch
            {
                MessageBox.Show("Проблемы с подключением...");
            }
        }
    }
}
