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
    /// Логика взаимодействия для Search.xaml
    /// </summary>
    public partial class Search : Window
    {
        string connectionString;
        SqlDataAdapter adapter = new SqlDataAdapter();
        DataTable usersTable = new DataTable();

        public Search()
        {
            InitializeComponent();

            try
            {
                string answer = Connect("https://opensky-network.org/api/states/all");
                planeGrid.ItemsSource = GetPlaneList(answer);
            }
            catch
            {
                MessageBox.Show("Проблемы с подключением...");
            }

            //получаем строку подключения из app.config
            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
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

        List<Plane> planeList = new List<Plane>();
        List<Plane> subPlaneList = new List<Plane>();

        //заполнение основного листа
        public List<Plane> GetPlaneList(string answer)
        {
            JObject answerJson = JObject.Parse(answer);

            IList<JToken> results = answerJson["states"].Children().ToList();
          
            foreach (JToken result in results)
            {
                List<string> resultString = result.ToString().TrimStart('[').TrimEnd(']').Split(',').ToList<string>();
                Plane plane = new Plane();
                plane.Icao24 = resultString[0].Trim().TrimStart('"').TrimEnd('"');
                plane.Callsign = resultString[1].Trim().TrimStart('"').TrimEnd('"');
                plane.Country = resultString[2].Trim().TrimStart('"').TrimEnd('"');
                plane.Longitude = resultString[5].Trim();
                plane.Latitude = resultString[6].Trim();
                plane.On_ground = resultString[8].Trim();
                plane.Velocity = resultString[9].Trim();
                plane.Altitude = resultString[13].Trim();

                planeList.Add(plane);
            }

            return planeList;
        }

        //возврат к предыдущему окну
        private void Back(object sender, RoutedEventArgs e)
        {
            //вернуться назад
            MainWindow main = new MainWindow();
            main.Show();
            Close();
        }

        //поиск
        private void Find(object sender, RoutedEventArgs e)
        {
            planeGrid.ItemsSource = new List<Plane>();
            subPlaneList.Clear();

            Regex regex1 = new Regex(@"(\w*)" + SearchCountry.Text + @"(\w*)");
            Regex regex2 = new Regex(@"(\w*)" + SearchCallsign.Text + @"(\w*)");
            foreach (Plane plane in planeList)
            {
                if (regex1.IsMatch(plane.Country) && SearchCallsign.Text == "")
                { subPlaneList.Add(plane); }

                if (regex2.IsMatch(plane.Callsign) && SearchCountry.Text == "")
                { subPlaneList.Add(plane); }

                if (regex1.IsMatch(plane.Country) && regex2.IsMatch(plane.Callsign))
                { subPlaneList.Add(plane); }
            }

            planeGrid.ItemsSource = subPlaneList;
        }

        //добавление в избранное
        private void Fav(object sender, RoutedEventArgs e)
        {
            if (Menu.UserId == 0)
            {
                MessageBox.Show("Эта функция доступна только авторизированным пользователям");
            }
            else
            {
                if(planeGrid.SelectedItem != null)
                {
                    Plane plane = (Plane)planeGrid.SelectedItem;
                    MessageBox.Show(plane.Icao24);

                    SqlConnection connection = new SqlConnection(connectionString);
                    connection.Open();

                    SqlCommand command = new SqlCommand();
                    command.CommandText = "INSERT INTO Favourites VALUES(" + Menu.UserId + ", '" + plane.Icao24 + "')";
                    command.Connection = connection;

                    adapter.SelectCommand = command;
                    adapter.Fill(usersTable);

                    connection.Close();
                }
                else
                {
                    MessageBox.Show("Нет выделенной строки");
                }
            }
        }

        //обновление данных
        private void Upd(object sender, RoutedEventArgs e)
        {
            planeGrid.ItemsSource = null;
            planeList.Clear();

            try
            {
                string answer = Connect("https://opensky-network.org/api/states/all");
                planeGrid.ItemsSource = GetPlaneList(answer);
            }
            catch
            {
                MessageBox.Show("Проблемы с подключением...");
            }
        }
    }

    public class Plane
    {
        public string Icao24 { get; set; }
        public string Callsign { get; set; }
        public string Country { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public string Altitude { get; set; }
        public string Velocity { get; set; }
        public string On_ground { get; set; }
    }
}
