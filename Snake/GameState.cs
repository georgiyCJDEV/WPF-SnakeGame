using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Snake
{
    public class GameState /*состояние игры*/
    {
        public int Rows { get; } /*ряды*/
        public int Cols { get; } /*столбцы*/
        public GridVal[,] Grid { get; } /*элемент ячейки игрового поля*/
        public Direction Dir { get; private set; } /*направление движения змейки*/

        public int ScoreGoal { get; private set; }
        public int Score { get; private set; } /*очки*/
        public int HighScore { get; private set; }
        public bool GameOver { get; private set; } /*переменная конца игры*/
        public bool GameWon { get; private set; } /*переменная победы*/
        LinkedList<int> scoreList = new LinkedList<int>(); /*двусвязный список хранящий очки*/

        private readonly LinkedList<Direction> dirChanges = new LinkedList<Direction>(); /*двусвязный список хранящий изменения в движении*/
        public readonly LinkedList<GridPos> snakePositions = new LinkedList<GridPos>(); /*двусвязный список хранящий позиции ячеек со змейкой*/
        private readonly Random random = new Random(); /*генератор псевдослучайных чисел для добавления фруктов в случайную свободную ячейку*/
        private readonly Random randFruit = new Random(); /*генератор псевдослучайных чисел для добавления случайного фрукта*/
        public GameState(int rows, int cols, int ScoreGoal, int gameDif)
        {
            Rows = rows;
            Cols = cols;
            Grid = new GridVal[rows, cols];
            Dir = Direction.Right; /*направление по умолчанию*/

            if (gameDif == 1)
            {
                this.ScoreGoal = ScoreGoal;
            }
            else
            {
                this.ScoreGoal = 675;
            }

            Scoreboard scoreboard = new Scoreboard(ref scoreList); /*таблица рекордов*/
            scoreboard.ReadFile(); /*чтение файла рекордов*/
            if(scoreList.First!=null)
            {
                HighScore = scoreList.First();
            }
            else
            {
                HighScore = 0;
            }
            AddSnake();
            AddFood();
        }

        public void setGoal(int ScoreGoal)
        {
            this.ScoreGoal= ScoreGoal;
        }

        private void AddSnake() /*добавление змейки*/
        {
            int r = Rows / 2;

            for (int c = 1; c <= 3; c++)
            {
                Grid[r, c] = GridVal.Snake; /*Задаёт на заданных по умолчанию координатах тип ячейки, содержащей змейку*/
                snakePositions.AddFirst(new GridPos(r, c));/*добавляет позицию по умолчанию в двусвязный список позиций, занимаемой змейкой*/
            }

        }

        private IEnumerable<GridPos> EmptyPositions() /*получает пустые ячейки*/
        {
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    if (Grid[r, c] == GridVal.Empty)
                    {
                        yield return new GridPos(r, c);
                    }
                }
            }
        }

        private void AddFood() /*Добавляет еду*/
        {
            List<GridPos> empty = new List<GridPos>(EmptyPositions());

            if (empty.Count == 0 || Score >= ScoreGoal) /*Если пустых ячеек не осталось - игра считается выигранной*/
            {
                GameWon = true;
                return;
            }

            GridPos pos = empty[random.Next(empty.Count)];
            Grid[pos.Row, pos.Col] = (GridVal)randFruit.Next((int)GridVal.Apple, (int)GridVal.Watermelon+1);
        }

        public GridPos HeadPosition() /*позиция головы*/
        {
            return snakePositions.First.Value; /*голова представлена первым элементом в двусвязном списке позиций змейки*/
        }

        public GridPos TailPosition() /*позиция последнего элемента хвоста*/
        {
            return snakePositions.Last.Value;
        }

        public IEnumerable<GridPos> SnakePositions()
        {
            return snakePositions;
        }

        private void AddHead(GridPos pos) /*Добавление головы в начало змейки*/
        {
            snakePositions.AddFirst(pos);
            Grid[pos.Row, pos.Col] = GridVal.Snake;
        }

        private void RemoveTail()
        {
            GridPos tail = snakePositions.Last.Value;
            Grid[tail.Row, tail.Col] = GridVal.Empty; /*Убираем элемент хвоста с конца змейки*/ 

            snakePositions.RemoveLast();
        }

        public Direction GetLastDirection() /*получение направления из буфера направлений змейки*/
        {
            if(dirChanges.Count==0)
            {
                return Dir;
            }
            return dirChanges.Last.Value;
        }

        private bool CanChangeDirection(Direction newDir)
        {
            if(dirChanges.Count==2)
            {
                return false;
            }

            Direction lastDir = GetLastDirection();
            return newDir != lastDir && newDir != lastDir.Opposite(); /*если новое направление не равно последнему направлению из буфера
                                                                       * и новое направление не равно противоположному направлению
                                                                       * (для избежания ошибки, из-за которой при повороте в противоположное текущему направление
                                                                       * змейка умирает*/
        }

        public void ChangeDirtection(Direction dir)
        {
            if (CanChangeDirection(dir))
            {
                dirChanges.AddLast(dir);
            }
        }
        
        private bool OutsideGrid(GridPos pos) /*граница поля*/
        {
            return pos.Row < 0 || pos.Row >= Rows || pos.Col < 0 || pos.Col >= Cols;
        }

        private GridVal WillHit(GridPos newHeadPos) /*коллизия*/
        {
            if(OutsideGrid(newHeadPos))
            {
                return GridVal.Outside;
            }

            if(newHeadPos == TailPosition()) /*если новая позиция головы змейки совпала с хвостом*/
            {
                return GridVal.Empty;
            }
            return Grid[newHeadPos.Row, newHeadPos.Col];
        }

        public void Move()
        {
            if(dirChanges.Count>0) 
            {
                Dir = dirChanges.First.Value;
                dirChanges.RemoveFirst();
            }

            GridPos newHeadPos = HeadPosition().Translate(Dir);
            GridVal hit = WillHit(newHeadPos);

            /*коллизия головы змейки с хвостом, змейки с границами поля*/
            if(hit==GridVal.Outside || hit == GridVal.Snake)
            {
                GameOver = true;
                Scoreboard scoreboard = new Scoreboard(Score, ref scoreList);
                scoreboard.Sort();
                scoreboard.FileWrite();
            }

            /*коллизия с пустой ячейкой (добавление головы змейки в новую координату, удаление конца хвоста с последней позиции*/
            else if(hit == GridVal.Empty)
            {
                RemoveTail();
                AddHead(newHeadPos);
            }

            /*коллизия с фруктами, начисление очков*/
            else if(hit==GridVal.Apple)
            {
                AddHead(newHeadPos);
                Score+=1;
                AddFood();
            }
            else if (hit == GridVal.Banana)
            {
                AddHead(newHeadPos);
                Score+=2;
                AddFood();
            }
            else if (hit == GridVal.Watermelon)
            {
                AddHead(newHeadPos);
                Score+=3;
                AddFood();
            }
        }
    }
}
