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

namespace kursach.Windows
{
    /// <summary>
    /// Логика взаимодействия для Menu.xaml
    /// </summary>
    public partial class Menu : Window
    {
        public static int UserId = 0;

        public Menu()
        {
            InitializeComponent();
        }

        private void Back(object sender, RoutedEventArgs e)
        {
            //вернуться назад
            this.Close();
        }

        private void Auth(object sender, RoutedEventArgs e)
        {           
            if(Menu.UserId != 0)
            {
                MessageBox.Show("Вы уже вошли, переходим в профиль...");
                Windows.Account acc = new Windows.Account(UserId);
                acc.Show();
                Close();
            }
            else
            {
                //перейти к авторизации
                Windows.Auth auth = new Windows.Auth();
                auth.Show();
                Close();
                Application.Current.MainWindow.Close();
            }             
        }

        private void Search(object sender, RoutedEventArgs e)
        {
            //перейти к поиску
        }
    }
}
