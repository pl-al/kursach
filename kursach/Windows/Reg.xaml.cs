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
    /// Логика взаимодействия для Reg.xaml
    /// </summary>
    public partial class Reg : Window
    {
        string connectionString;
        SqlDataAdapter adapter = new SqlDataAdapter();
        DataTable usersTable = new DataTable();

        public Reg()
        {
            InitializeComponent();
            //получаем строку подключения из app.config
            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        }

        private void back(object sender, RoutedEventArgs e)
        {           
            //вернуться назад
            Windows.Auth auth = new Windows.Auth();
            auth.Show();
            Close();
        }

        private void home(object sender, RoutedEventArgs e)
        {
            //вернуться на главную
            MainWindow main = new MainWindow();
            main.Show();
            Close();
        }

        private void okay(object sender, RoutedEventArgs e)
        {
            //обработчик ошибок при регистрации
            if(lname.Text == "" || fname.Text == "" || mname.Text == "" || login.Text == "" || pass1.Password == "" || pass2.Password == "")
            {
                MessageBox.Show("Не все обязательные поля заполнены.");
                return;
            }

            if(login.Text.Length < 5 || pass1.Password.Length < 5)
            {
                MessageBox.Show("Слишком короткий логин и/или пароль.");
                return;
            }

            if(pass1.Password != pass2.Password)
            {
                MessageBox.Show("Пароли не совпадают.");
                return;
            }

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            SqlCommand command = new SqlCommand();
            command.CommandText = "SELECT * FROM Users WHERE Login = '" + login.Text + "'";
            command.Connection = connection;

            adapter.SelectCommand = command;
            adapter.Fill(usersTable);

            if (usersTable.Rows.Count != 0)
            {
                MessageBox.Show("Такой логин уже существует, попробуйте другой.");
                return;
            }
            else
            {
                //успех, переход в профиль
                command.CommandText = "INSERT INTO Users (Login, Password, LastName, FirstName, MiddleName) VALUES ('" + login.Text + "', '" + pass1.Password + "', '" + lname.Text + "', '" + fname.Text + "', '" + mname.Text + "')";
                command.Connection = connection;

                adapter.InsertCommand = command;
                adapter.Fill(usersTable);

                MessageBox.Show("Регистрация прошла успешно.");

                Windows.Auth auth = new Windows.Auth();
                auth.Show();
                Close();
            }

            connection.Close();
        }

        //для теста
        public bool testreg(string LName, string FName, string MName, string log, string pass1, string pass2)
        {
            usersTable.Clear();

            //обработчик ошибок при регистрации
            if (LName == "" || FName == "" || MName == "" || log == "" || pass1 == "" || pass2 == "")
            {
                return false;
            }

            if (log.Length < 5 || pass1.Length < 5)
            {
                return false;
            }

            if (pass1 != pass2)
            {
                return false;
            }

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            SqlCommand command = new SqlCommand();
            command.CommandText = "SELECT * FROM Users WHERE Login = '" + log + "'";
            command.Connection = connection;

            adapter.SelectCommand = command;
            adapter.Fill(usersTable);

            if (usersTable.Rows.Count != 0)
            {
                connection.Close();
                return false;
            }
            else
            {
                //успех, переход в профиль
                connection.Close();
                return true;
            } 
        }
    }
}
