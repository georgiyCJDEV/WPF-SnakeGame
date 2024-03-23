using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Diagnostics;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Snake
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window /*Окно игры*/
    {
        private readonly Dictionary<GridVal, ImageSource> gridValToImage = new() /*Словарь, соответствие типа ячейки отображаемому спрайту*/
        {
            {GridVal.Empty,Sprites.Empty },
            {GridVal.Snake,Sprites.Body },
            {GridVal.Apple,Sprites.Apple },
            {GridVal.Banana,Sprites.Banana },
            {GridVal.Watermelon,Sprites.Watermelon }
        };

        private readonly Dictionary<Direction, int> dirToRotation = new() /*соответствие направления углу поворота головы змейки*/
        {
            {Direction.Up,0 },
            {Direction.Right,90 },
            {Direction.Down,180 },
            {Direction.Left,270 }
        };
        private int rows, cols; /*размер игрового поля*/
        private int scoreGoal;
        private int gameDif;
        private double speedMult;
        private Image[,] gridImages;
        private GameState gameState; /*состояние игры*/
        private bool gameSetted, gameRunning;
        enum DIFFICULTY
        {
            Easy=1,
            Hard=2
        }

        public MainWindow() /*конструктор*/
        {
            InitializeComponent();
            rows = 15;
            cols = 15;
            scoreGoal = 175;
            gameDif = (int)DIFFICULTY.Hard;
            gameSetted = false;
        }

        private async Task RunGame()
        {   
            WindowState state = new WindowState();
            state = this.WindowState;
            Draw(); /*отрисовка ячеек и змейки*/
            await ShowCountDown(); /*счётчик до начала игры*/
            Overlay.Visibility = Visibility.Hidden; /*скрытие оверлея*/
            PlayerStats.IsEnabled = true;
            PlayerStats.Visibility=Visibility.Visible;
            await GameLoop(); /*Цикл отвечающий за отрисовку элементов игры и передвижение змейки за заданное время*/
            if (gameState.GameOver == true) /*если игра проиграна*/
            {
                await ShowGameOver(); /*вывод сообщения о том что игра проиграна*/
            }
            else if(gameState.GameWon==true) /*если игра выиграна*/
            {
                await GameWon(); /*вывод сообщения о том что игра выиграна*/
            }
            Menu menu = new Menu(); /*Создание окна меню*/
            menu.WindowState = state; /*передача окна меню состояния предыдущего окна*/
            this.Hide(); /*закрытие текущего окна*/
            menu.Show(); /*показ окна*/
        }

        private async void Window_PreviewKeyDown(object sender, KeyEventArgs e) 
        {

            if (Overlay.Visibility == Visibility.Visible)
            {
                e.Handled = true;
            }

            if (!gameRunning&&gameSetted) /*если игра не запущена*/
            {
                gameRunning = true;
                await RunGame();
                gameRunning = false;
            }

        }

        private void Window_KeyDown(object sender, KeyEventArgs e) /*чтение управления с клавиатуры*/
        {
            if (gameSetted)
            {
                if (gameState.GameOver)
                {
                    return;
                }

                switch (e.Key)
                {
                    case Key.A:
                        gameState.ChangeDirtection(Direction.Left);
                        break;
                    case Key.D:
                        gameState.ChangeDirtection(Direction.Right);
                        break;
                    case Key.W:
                        gameState.ChangeDirtection(Direction.Up);
                        break;
                    case Key.S:
                        gameState.ChangeDirtection(Direction.Down);
                        break;
                    case Key.Left:
                        gameState.ChangeDirtection(Direction.Left);
                        break;
                    case Key.Right:
                        gameState.ChangeDirtection(Direction.Right);
                        break;
                    case Key.Up:
                        gameState.ChangeDirtection(Direction.Up);
                        break;
                    case Key.Down:
                        gameState.ChangeDirtection(Direction.Down);
                        break;
                    case Key.NumPad4:
                        gameState.ChangeDirtection(Direction.Left);
                        break;
                    case Key.NumPad6:
                        gameState.ChangeDirtection(Direction.Right);
                        break;
                    case Key.NumPad5:
                        gameState.ChangeDirtection(Direction.Down);
                        break;
                    case Key.NumPad8:
                        gameState.ChangeDirtection(Direction.Up);
                        break;

                }
            }
        }

        private async Task GameLoop() /*цикл перемещения и отрисовки*/
        {
            while(!gameState.GameOver&&!gameState.GameWon)
            {
                await Task.Delay((int)(speedMult*100));
                gameState.Move();
                Draw();
            }
        }

        private Image[,] SetupGrid() /*установка отображения поля игры(сетки)*/
        {
            Image[,] images = new Image[rows, cols];
            GameGrid.Rows = rows;
            GameGrid.Columns = cols;
            GameGrid.Width = GameGrid.Height * (cols / (double)rows);

            for(int r=0;r<rows;r++)
            {
                for(int c =0;c<cols;c++)
                {
                    Image image = new Image
                    {
                        Source = Sprites.Empty,
                        RenderTransformOrigin = new Point(0.5, 0.5) /*вращение спрайта вокруг центральной точки спрайта*/
                    };
                    images[r, c] = image;
                    GameGrid.Children.Add(image);
                }
            }
            return images;
        }

        private void Draw() /*отрисовка игровых элементов*/
        {
            DrawGrid();
            DrawSnakeHead();
            ScoreText.Text = $"Очки: {gameState.Score}";
            if(gameState.ScoreGoal!=675)
            {
                ScoreAim.IsEnabled = true;
                ScoreAim.Text = $"Цель: {gameState.ScoreGoal}";
                ScoreAim.Visibility= Visibility.Visible;
            }
            HighScore.Text = $"Лучший результат: {gameState.HighScore}";
        }

        private void DrawGrid() /*отрисовка ячеек сетки*/
        {
            for(int r=0; r<rows; r++)
            {
                for(int c=0;c<cols;c++)
                {
                    GridVal gridVal = gameState.Grid[r, c];
                    gridImages[r, c].Source = gridValToImage[gridVal];
                    gridImages[r, c].RenderTransform = Transform.Identity; /*поворот спрайта*/
                }
            }
        }

        private void DrawSnakeHead() /*отрисовка головы змейки*/
        {
            GridPos headPos = gameState.HeadPosition();
            Image image = gridImages[headPos.Row, headPos.Col];
            image.Source = Sprites.Head;

            int rotation = dirToRotation[gameState.Dir];
            image.RenderTransform = new RotateTransform(rotation); /*поворот головы согласно направлению движения*/
        }

        private async Task DrawDeadSnake() /*отрисовка мёртвой змейки*/
        {
            List<GridPos> positions = new List<GridPos>(gameState.SnakePositions());
            for(int i=0;i<positions.Count;i++)
            {
                GridPos pos = positions[i];
                ImageSource source = (i == 0) ? Sprites.DeadHead : Sprites.DeadBody;
                gridImages[pos.Row, pos.Col].Source = source;
                await Task.Delay(50); /*задержка отрисовки*/
            }
        }

        private async Task ShowCountDown() /*обратный отсчёт*/
        {
            for(int i=3;i>=1;i--)
            {
                OverlayText.Text = i.ToString();
                await Task.Delay(1000); /*итерация цикла происходит раз в секунду*/
            }
        }

        private async Task ShowGameOver() /*показ сообщения о проигрыше*/
        {
            await DrawDeadSnake(); /*отрисовка мёртвой змейки*/
            await Task.Delay(500);
            Overlay.Visibility = Visibility.Visible; /*показ оверлея*/
            for (int i = 3; i >= 1; i--)
            {
                OverlayText.Text = $"ИГРА ОКОНЧЕНА\nВЫ НАБРАЛИ {gameState.Score} ОЧКОВ!\nВОЗВРАТ В МЕНЮ ЧЕРЕЗ {i}";
                await Task.Delay(1000);
            }
        }

        private async Task GameWon()
        {
            Overlay.Visibility = Visibility.Visible; /*показ оверлея*/
            for(int i=3; i>=1; i--)
            {
                OverlayText.Text = $"ВЫ ВЫИГРАЛИ!\nВЫ НАБРАЛИ {gameState.Score} ОЧКОВ!\nВОЗВРАТ В МЕНЮ ЧЕРЕЗ {i}";
                await Task.Delay(1000);
            }    
        }

        private void SpeedSlow_Checked(object sender, RoutedEventArgs e)
        {
            speedMult = 2;
        }
        private void SpeedMed_Checked(object sender, RoutedEventArgs e)
        {
            speedMult = 1.5;
        }
        private void SpeedHigh_Checked(object sender, RoutedEventArgs e)
        {
            speedMult = 1;
        }    

        private void FieldSmall_Checked(object sender, RoutedEventArgs e)
        {
            rows = 8;
            cols = 8;
            scoreGoal = 40;
            if (ScoreNPoints != null)
            {
                ScoreNPoints.Content = $"Набрать {scoreGoal} очков";
            }
        }
        private void FieldMedium_Checked(object sender, RoutedEventArgs e)
        {
            rows = 10;
            cols = 10;
            scoreGoal = 70;
            if (ScoreNPoints != null)
            {
                ScoreNPoints.Content = $"Набрать {scoreGoal} очков";
            }
        }
        private void FieldBig_Checked(object sender, RoutedEventArgs e)
        {
            rows = 15;
            cols = 15;
            scoreGoal = 175;
            if (ScoreNPoints != null)
            {
                ScoreNPoints.Content = $"Набрать {scoreGoal} очков";
            }
        }

        private void Aim_FillTiles_Checked(object sender, RoutedEventArgs e)
        {
            scoreGoal = 675;
            gameDif = (int)DIFFICULTY.Hard;
        }

        private void Aim_Scored_Checked(object sender, RoutedEventArgs e)
        {
            gameDif = (int)DIFFICULTY.Easy;
        }

        private void Button_Play_Click(object sender, RoutedEventArgs e)
        {
            gameSetted= true;
            SettingsGrid.IsEnabled = false;
            SettingsGrid.Visibility = Visibility.Hidden;
            gridImages = SetupGrid();
            gameState = new GameState(rows, cols, scoreGoal, gameDif);
            GridBorder.Visibility = Visibility.Visible;
            GridBorder.IsEnabled = true;
            Overlay.Visibility = Visibility.Visible;
            Overlay.IsEnabled = true;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }

    }
}
