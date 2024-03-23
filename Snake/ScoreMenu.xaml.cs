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
    /// Interaction logic for ScoreMenu.xaml
    /// </summary>
    public partial class ScoreMenu : Window
    {
        public LinkedList<int> scores = new LinkedList<int>();
        public Scoreboard leaderboard;
        public ScoreMenu()
        {
            InitializeComponent();
            leaderboard = new Scoreboard(ref scores);
            leaderboard.ReadFile();
            ScoreRows();
        }

        private void Button_Back_Click(object sender, RoutedEventArgs e) /*при нажатии возвращает пользователя в меню*/
        {
            WindowState state = new WindowState();
            state = this.WindowState;
            this.Hide();
            Menu menu = new Menu();
            menu.WindowState = state;
            menu.Show();
        }

        private void ScoreRows() /*ряды с записями рекордов, полученных из файла*/
        {      
            int i = 0, count = 1;
            foreach(var el in scores) /*перебор узлов списка*/
            {
                TextBox tb = new TextBox();
                tb.IsReadOnly = true;
                tb.VerticalAlignment = VerticalAlignment.Top;
                tb.HorizontalAlignment = HorizontalAlignment.Center;
                tb.Margin = new Thickness(0,72+i,0,0);
                tb.Width = 169;
                tb.Height = 60;
                tb.TextWrapping = TextWrapping.Wrap;
                tb.Text = $"{count}. " + el.ToString();
                tb.FontSize = 40;
                i += 70;
                tb.Background = Brushes.Transparent;
                tb.VerticalContentAlignment = VerticalAlignment.Center;
                tb.HorizontalContentAlignment=HorizontalAlignment.Left;
                tb.BorderBrush = Brushes.Transparent;
                tb.Foreground= Brushes.Black;
                MainGrid.Children.Add(tb);
                count += 1;
                if(count==11) /*максимальное число отображаемых рекордов = 10*/
                {
                    break;
                }
            }
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
