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
    /// Логика взаимодействия для Auth.xaml
    /// </summary>
    public partial class Auth : Window
    {
        string connectionString;
        SqlDataAdapter adapter = new SqlDataAdapter();
        DataTable usersTable = new DataTable();

        public Auth()
        {
            InitializeComponent();
            //получаем строку подключения из app.config
            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        private void back(object sender, RoutedEventArgs e)
        {
            //вернуться назад
            MainWindow main = new MainWindow();
            main.Show();
            Windows.Menu menu = new Windows.Menu();
            menu.ShowDialog();            
            Close();
        }

        private void reg(object sender, RoutedEventArgs e)
        {           
            //перейти к регистрации
            Windows.Reg reg = new Windows.Reg();
            reg.Show();
            Close();
        }

        private void auth(object sender, RoutedEventArgs e)
        {
            //обработчик ошибок при авторизации
            if(login.Text == "" || password.Password == "")
            {
                MessageBox.Show("Ошибка! Пустые поля.");
                return;
            }

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            SqlCommand command = new SqlCommand();
            command.CommandText = "SELECT * FROM Users WHERE Login = '" + login.Text + "' AND Password = '" + password.Password + "'";
            command.Connection = connection;

            adapter.SelectCommand = command;
            adapter.Fill(usersTable);

            if(usersTable.Rows.Count != 0)
            {
                //успех, переход в профиль
                MessageBox.Show("Авторизация прошла успешно.");
                
                Windows.Account acc = new Windows.Account(Convert.ToInt32(usersTable.Rows[0][0]));
                acc.Show();
                Close();
            }
            else
            {
                MessageBox.Show("Ошибка! Неверный логин и/или пароль.");
                return;
            }

            connection.Close();
        }
    }
}
