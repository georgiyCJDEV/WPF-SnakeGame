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

namespace Snake
{
    /// <summary>
    /// Interaction logic for Menu.xaml
    /// </summary>
    public partial class Menu : Window
    {
        public Menu()
        {
            InitializeComponent();
        }

        private void Button_Play_Click(object sender, RoutedEventArgs e) /*при нажатии вызывает окно игры*/
        {
            WindowState state = new WindowState();
            state = this.WindowState;
            MainWindow mainWindow = new MainWindow();
            this.Hide();
            mainWindow.WindowState=state;
            mainWindow.Show();
        }

        private void Button_Scoreboard_Click(object sender, RoutedEventArgs e) /*при нажатии вызывает окно таблицы рекордов*/
        {
            WindowState state = new WindowState();
            state = this.WindowState;
            ScoreMenu scoremenu = new ScoreMenu();
            this.Hide();
            scoremenu.WindowState = state;
            scoremenu.Show();
        }
        private void Button_Info_Click(object sender, RoutedEventArgs e) /*при нажатии показывает информацию об игре*/
        {
            WindowState state = new WindowState();
            state = this.WindowState;
            GameInfo GI = new GameInfo();
            GI.WindowState = state;
            this.Hide();
            GI.Show();
        }
        private void Button_Exit_Click(object sender, RoutedEventArgs e) /*при нажатии закрывает игру*/
        {
            Application.Current.Shutdown();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
