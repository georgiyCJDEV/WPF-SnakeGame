using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake
{
    public class GridPos /*позиция в игровом поле(сетке)*/
    {
        public int Row { get; }
        public int Col { get; }

        public GridPos(int row, int col)
        {
            this.Row = row;
            this.Col = col;
        }

        public GridPos Translate (Direction dir) /*перемещение в сетке*/
        {
            return new GridPos(Row + dir.RowOffset, Col + dir.ColOffset);
        }

        public override bool Equals(object obj) /*переопределение метода проверки равенства*/
        {
            return obj is GridPos pos &&
                   Row == pos.Row &&
                   Col == pos.Col;
        }

        public override int GetHashCode() /*получение хэшкода для сравнения объектов*/
        {
            return HashCode.Combine(Row, Col);
        }

        public static bool operator ==(GridPos left, GridPos right) /*переопределение оператора равенства*/
        {
            return EqualityComparer<GridPos>.Default.Equals(left, right);
        }

        public static bool operator !=(GridPos left, GridPos right) /*переопределение оператора неравенства*/
        {
            return !(left == right);
        }
    }
}
