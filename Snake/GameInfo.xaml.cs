using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
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
    /// Interaction logic for GameInfo.xaml
    /// </summary>
    public partial class GameInfo : Window
    {
        public GameInfo()
        {
            InitializeComponent();
        }

        private void Button_Back_Click(object sender, RoutedEventArgs e) /*при нажатии на кнопку*/
        {
            var state = this.WindowState; /*получает состояние текущего окна*/ 
            Menu menu = new Menu(); /*создаёт окно меню*/
            menu.WindowState = state; /*присваивает окну меню состояние предыдущего окна*/
            this.Hide(); /*закрывает текущее окно*/
            menu.Show(); /*показывает окно меню*/
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
