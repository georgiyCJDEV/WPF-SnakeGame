using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Snake
{
    public class Scoreboard /*таблица рекордов*/
    {
        LinkedList<int> scores = new LinkedList<int>(); /*список, хранящий записи рекордов*/
        string filename = "scores.txt"; /*путь до файла с таблицей рекордов*/
        public Scoreboard(int Points, ref LinkedList<int> scorepoints) 
        {
            scores = scorepoints;
            var node = scores.First;
            if (scores.First == null)
            {
                scores.AddFirst(Points);
                node = scores.First;
                Menu menu = new Menu();
            }
            else
            {
                scores.AddAfter(node, Points);
                node = node.Next;
            }
        }
        public Scoreboard(ref LinkedList<int> scorepoints)
        {
            scores = scorepoints;
        }

        public void Sort() /*сортировка по убыванию*/
        {

            if (scores.First == null)
            {
                return;
            }

            var cur = scores.First;
            var temp = scores.First.Value;
            if (cur.Next == null)
            {
                return;
            }
            var next = cur.Next;

            while (cur != null)
            {
                if (cur.Value < next.Value)
                {
                    temp = cur.Value;
                    cur.ValueRef = next.Value;
                    next.ValueRef = temp;
                }
                cur = cur.Next;
                if (cur.Next != null)
                {
                    next = cur.Next;
                }
                else
                {
                    break;
                }
            }

            cur = scores.First;
            next = cur.Next;
            while(cur!=null) /*прохождение списка пока все записи не будут отображаться по убыванию*/
            {
                if(cur.Value < next.Value)
                {
                    Sort(); /*рекурсивный вызов функции сортировки*/
                }
                cur= cur.Next;
                if(cur.Next!=null)
                {
                    next = cur.Next;
                }
                else
                {
                    break;
                }
            }
        }

        public void ReadFile() /*чтение рекордов из файла*/
        {
            if(File.Exists(filename))
            {
                using (StreamReader sr = new StreamReader(filename))
                {
                    string num;
                    var curNode = scores.First;
                    while ((num = sr.ReadLine()) != null)
                    {
                        if (scores.First==null)
                        {
                            scores.AddFirst(Convert.ToInt32(num));
                            curNode = scores.First;
                        }
                        else
                        {
                            scores.AddAfter(curNode, Convert.ToInt32(num));
                            curNode = curNode.Next;
                        }
                    }
                }
            }
            else
            {
                File.WriteAllText(filename, ""); /*если файл ещё не создан*/
            }
        }
        public void FileWrite() /*Запись в файл*/
        {
            File.Delete(filename);
            int lcount = scores.Count();
            foreach (var el in scores) /*перебор узлов списка*/
            {
                File.AppendAllText($"{filename}", $"{el}\n");
            }
        }
    }
}
