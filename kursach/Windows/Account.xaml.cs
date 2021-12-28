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
using System.Data.SqlClient;
using System.Data;
using System.Configuration;

namespace kursach.Windows
{
    /// <summary>
    /// Логика взаимодействия для Account.xaml
    /// </summary>
    public partial class Account : Window
    {
        public static int UserId = 0;

        string connectionString;
        SqlDataAdapter adapter = new SqlDataAdapter();
        DataTable usersTable = new DataTable();

        public Account(int IdUser)
        {
            Menu.UserId = IdUser;
            InitializeComponent();
            //получаем строку подключения из app.config
            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            //вывод в профиль фио, логина и аватарки
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            SqlCommand command = new SqlCommand();
            command.CommandText = "SELECT CONCAT(LastName, ' ', FirstName, ' ', MiddleName), Login, Image FROM Users WHERE IdUser = " + IdUser.ToString();
            command.Connection = connection;

            adapter.SelectCommand = command;
            adapter.Fill(usersTable);

            LFM.Text = usersTable.Rows[0][0].ToString();
            Login.Text = usersTable.Rows[0][1].ToString();
            ImageAva.Source = new BitmapImage(new Uri(usersTable.Rows[0][2].ToString()));

            connection.Close();
        }

        //для теста
        public string testaccLFM()
        {
            usersTable.Clear();

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            SqlCommand command = new SqlCommand();
            command.CommandText = "SELECT CONCAT(LastName, ' ', FirstName, ' ', MiddleName), Login, Image FROM Users WHERE IdUser = 1";
            command.Connection = connection;

            adapter.SelectCommand = command;
            adapter.Fill(usersTable);

            string LFM;
            LFM = usersTable.Rows[0][0].ToString();

            connection.Close();

            return LFM;
        }

        public string testaccLogin()
        {
            usersTable.Clear();

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            SqlCommand command = new SqlCommand();
            command.CommandText = "SELECT CONCAT(LastName, ' ', FirstName, ' ', MiddleName), Login, Image FROM Users WHERE IdUser = 1";
            command.Connection = connection;

            adapter.SelectCommand = command;
            adapter.Fill(usersTable);

            string log;
            log = usersTable.Rows[0][1].ToString();

            connection.Close();

            return log;
        }
        //

        string filename;

        private void AlterImage(object sender, RoutedEventArgs e)
        {
            //изменение аватарки
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            dlg.DefaultExt = ".png";
            dlg.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg";

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                filename = dlg.FileName;
                ImageAva.Source = new BitmapImage(new Uri(filename));
            }
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            //сохранение аватарки
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            SqlCommand command = new SqlCommand();
            command.CommandText = "UPDATE Users SET Image = '" + filename + "' WHERE IdUser = " + Menu.UserId.ToString();
            command.Connection = connection;

            adapter.SelectCommand = command;
            adapter.Fill(usersTable);

            connection.Close();

            MessageBox.Show("Сохранения изменены");
        }

        private void Home(object sender, RoutedEventArgs e)
        {
            //возврат на главную
            MainWindow main = new MainWindow();
            main.Show();
            Close();
        }

        private void Like(object sender, MouseButtonEventArgs e)
        {
            //переход в избранное
            Windows.Fav fav = new Windows.Fav(UserId);
            fav.Show();
            Close();
        }

        private void Exit(object sender, MouseButtonEventArgs e)
        {
            //выход из аккаунта, возврат на главную
            Menu.UserId = 0;
            MessageBox.Show("Вы вышли из аккаунта");

            MainWindow main = new MainWindow();
            main.Show();
            Close();
        }
    }
}
