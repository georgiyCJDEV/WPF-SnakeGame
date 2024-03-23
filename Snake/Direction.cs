using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake
{
    public class Direction /*Класс направления движения змейки*/
    {
       /*поскольку начальные, нулевые координаты рядов и столбцов
        * находятся в левом верхнем углу окна, для смещения влево на одну ячейку
        * нужно вычесть 1 столбец, для движения вправо прибавить 1,
        * для движения вверх вычесть 1 ряд и для движения вниз прибавить 1*/
        public readonly static Direction Left = new Direction(0, -1);
        public readonly static Direction Right = new Direction(0, 1);
        public readonly static Direction Up = new Direction(-1, 0);
        public readonly static Direction Down = new Direction(+1, 0);
        public int RowOffset { get; }  /*Смещение относительно рядов*/
        public int ColOffset { get; } /*Смещение относительно столбцов*/

        private Direction(int rowOffset, int colOffset) /*конструктор принимающий значения рядов и столбцов*/
        {
            this.RowOffset = rowOffset;
            this.ColOffset = colOffset;
        }

        public Direction Opposite() /*получает новый объект класса направления движения с противоположными значениями
                                     * смещения относительно рядов и столбцов*/
        {
            return new Direction(-RowOffset, -ColOffset);
        }

        public override bool Equals(object obj) /*переопределение метода проверки равенства*/
        {
            return obj is Direction direction &&
                   RowOffset == direction.RowOffset &&
                   ColOffset == direction.ColOffset;
        }

        public override int GetHashCode() /*получает хэшкод для сравнения объектов*/
        {
            return HashCode.Combine(RowOffset, ColOffset);
        }

        public static bool operator ==(Direction left, Direction right) /*переопределение оператора равенства*/
        {
            return EqualityComparer<Direction>.Default.Equals(left, right);
        }

        public static bool operator !=(Direction left, Direction right) /*переопределения оператора неравенства*/
        {
            return !(left == right);
        }
    }
}
