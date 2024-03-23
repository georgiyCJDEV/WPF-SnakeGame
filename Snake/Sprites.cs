using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Snake
{
    
    public static class Sprites
    {   
        public readonly static ImageSource Empty = LoadImage("Empty.png");
        public readonly static ImageSource Body = LoadImage("Body.png");
        public readonly static ImageSource Head = LoadImage("Head.png");
        public readonly static ImageSource Apple = LoadImage("Apple.png");
        public readonly static ImageSource Banana = LoadImage("Banana.png");
        public readonly static ImageSource Watermelon = LoadImage("Watermelon.png");
        public readonly static ImageSource DeadBody = LoadImage("DeadBody.png");
        public readonly static ImageSource DeadHead = LoadImage("DeadHead.png");

        /*загрузка спрайтов*/
        private static ImageSource LoadImage(string fName)
        {
            return new BitmapImage(new Uri($"Assets/{fName}", UriKind.Relative));
        }
    }
}