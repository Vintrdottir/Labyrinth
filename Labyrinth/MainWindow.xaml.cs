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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Labirynt
{
    /// <summary>

    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly int SIZE = 10;
        private MyWorm _worm;
        private int _directionX = 1;
        private int _directionY = 0;
        private DispatcherTimer _timer;
        private WormPart _food;
        private int _partsToAdd;
        private List<Wall> _walls;
        private Wall _finish;
        private int points = 0;
        private bool FoodContainer;
        private int l = 100;
        private int level = 1;

        public MainWindow()
        {
            InitializeComponent();

            MessageBox.Show("Help the worm find some food and take it home (green square in the lower right corner). Be careful, it accelerates after each delivered portion!");
            InitBoard();
            InitWorm();
            InitTimer();
            InitWall();
            InitFood();
        }
                
        void InitBoard()
        {
            for (int i = 0; i < grid.Width / SIZE; i++)
            {
                ColumnDefinition columnDefinitions = new ColumnDefinition();
                columnDefinitions.Width = new GridLength(SIZE);
                grid.ColumnDefinitions.Add(columnDefinitions);
            }
            for (int j = 0; j < grid.Height / SIZE; j++)
            {
                RowDefinition rowDefinition = new RowDefinition();
                rowDefinition.Height = new GridLength(SIZE);
                grid.RowDefinitions.Add(rowDefinition);
            }
            _finish = new Wall(70, 50, 10, 10);
            _finish.Rect.Fill = Brushes.DarkGreen;
            grid.Children.Add(_finish.Rect);
            Grid.SetColumn(_finish.Rect, _finish.X);
            Grid.SetRow(_finish.Rect, _finish.Y);
            Grid.SetColumnSpan(_finish.Rect, _finish.Width);
            Grid.SetRowSpan(_finish.Rect, _finish.Height);
            _worm = new MyWorm();
        }

        void InitWorm()
        {
            grid.Children.Add(_worm.Head.Ell);
            foreach (WormPart wormPart in _worm.Parts)
                grid.Children.Add(wormPart.Ell);
            _worm.RedrawWorm();
        }
        
        void InitFood()
        {
            _food = new WormPart(5, 20);
            var r = new Random();
            for (;;)
            {
                int x = r.Next(10, 70);
                int y = r.Next(5, 45);
                if (IsFieldFree(x, y))
                {
                    _food.X = x;
                    _food.Y = y;
                    break;
                }
            }
            _food.Ell.Width = _food.Ell.Height = 10;
            _food.Ell.Fill = Brushes.Blue;
            grid.Children.Add(_food.Ell);
            Grid.SetColumn(_food.Ell, _food.X);
            Grid.SetRow(_food.Ell, _food.Y);
        }

        void InitTimer()
        {
            _timer = new DispatcherTimer();
            _timer.Tick += new EventHandler(_timer_Tick);
            _timer.Interval = new TimeSpan(0, 0, 0, 0, l);
            _timer.Start();
        }

        void InitWall()
        {
            _walls = new List<Wall>();
            var r = new Random();

            for (int a = 0; a < 7; a++)
            {
                for (int b = 0; b < 5; b++)
                {
                    if (!(a*b == 24))
                    {
                        Wall wall1 = new Wall(a * 10 + 10, b * 10 + 5, r.Next(3, 10), r.Next(3, 10));
                        grid.Children.Add(wall1.Rect);
                        Grid.SetColumn(wall1.Rect, wall1.X);
                        Grid.SetRow(wall1.Rect, wall1.Y);
                        Grid.SetColumnSpan(wall1.Rect, wall1.Width);
                        Grid.SetRowSpan(wall1.Rect, wall1.Height);
                        _walls.Add(wall1);
                    }
                }
            }

            for (int i = 0; i < 10; i++)
            {
                Wall wall1 = new Wall(r.Next(10, 60), r.Next(10, 40), r.Next(1, 10), r.Next(1, 10));
                grid.Children.Add(wall1.Rect);
                Grid.SetColumn(wall1.Rect, wall1.X);
                Grid.SetRow(wall1.Rect, wall1.Y);
                Grid.SetColumnSpan(wall1.Rect, wall1.Width);
                Grid.SetRowSpan(wall1.Rect, wall1.Height);
                _walls.Add(wall1);
            }
        }

        void _timer_Tick(object sender, EventArgs e)
        {
            MoveWorm();
        }

       
        private void MoveWorm()
        {
            int wormPartCount = _worm.Parts.Count;
            if (_partsToAdd > 0)
            {
                WormPart newPart = new WormPart(_worm.Parts[_worm.Parts.Count - 1].X,
                    _worm.Parts[_worm.Parts.Count - 1].Y);
                grid.Children.Add(newPart.Ell);
                _worm.Parts.Add(newPart);
                _partsToAdd--;
            }

            for (int i = wormPartCount - 1; i >= 1; i--)
            {
                _worm.Parts[i].X = _worm.Parts[i - 1].X;
                _worm.Parts[i].Y = _worm.Parts[i - 1].Y;
            }
            _worm.Parts[0].X = _worm.Head.X;
            _worm.Parts[0].Y = _worm.Head.Y;
            _worm.Head.X += _directionX;
            _worm.Head.Y += _directionY;
            if (CheckCollision())
                EndGame();
            else
            {
                if (CheckFood())
                {
                    Grid.SetColumn(_food.Ell, 0);
                    Grid.SetRow(_food.Ell, 0);
                    _food.Ell.Fill = Brushes.White;
                    FoodContainer = true;
                }
                _worm.RedrawWorm();
            }
        }

        private void RedrawFood()
        {
            var rand = new Random();
             for (;;)
             {
                 int x = rand.Next(10, 70);
                 int y = rand.Next(5, 45);
                 if (IsFieldFree(x, y))
                 {
                    _food.X = x;
                    _food.Y = y;
                    break;
                 }
             }
            Grid.SetColumn(_food.Ell, _food.X);
            Grid.SetRow(_food.Ell, _food.Y);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                _directionX = -1;
                _directionY = 0;
            }

            if (e.Key == Key.Right)
            {
                _directionX = 1;
                _directionY = 0;
            }

            if (e.Key == Key.Up)
            {
                _directionX = 0;
                _directionY = -1;
            }

            if (e.Key == Key.Down)
            {
                _directionX = 0;
                _directionY = 1;
            }
        }

        private bool CheckFood()
        {
            if (_worm.Head.X == _food.X && _worm.Head.Y == _food.Y)
            {
                return true;
            }
            return false;
        }

        private bool IsFieldFree(int x, int y)
        {
            if (_worm.Head.X == x && _worm.Head.Y == y)
                return false;
            foreach (WormPart wormPart in _worm.Parts)
            {
                if (wormPart.X == x && wormPart.Y == y)
                    return false;
            }
            foreach (Wall wall in _walls)
            {
                if (x >= wall.X && x <= wall.X + wall.Width &&
                    y >= wall.Y && y <= wall.Y + wall.Height)
                    return false;
            }
            return true;
        }

        void EndGame()
        {
            _timer.Stop(); 
            MessageBox.Show("Oh no, he's dead :(");
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }

        bool CheckCollision()
        {
            if (CheckBoardCollision())
                return true;
            if (CheckWallCollision())
                return true;
            return false;
        }

        bool CheckBoardCollision()
        {
            if (_worm.Head.X < 0 || _worm.Head.X > grid.Width / SIZE)
                return true;
            if (_worm.Head.Y < 0 || _worm.Head.Y > grid.Height / SIZE)
                return true;
            return false;
        }

        bool CheckWallCollision()
        {
            foreach (Wall wall in _walls)
            {
                if (_worm.Head.X >= wall.X && _worm.Head.X < wall.X + wall.Width &&
                    _worm.Head.Y >= wall.Y && _worm.Head.Y < wall.Y + wall.Height)
                    return true;
            }
            if (((_worm.Head.X == _finish.X && _worm.Head.Y > _finish.Y)||
                (_worm.Head.X > _finish.X && _worm.Head.Y == _finish.Y)) && FoodContainer == true)
            {
                points += 100;
                l -= 20;
                level += 1;
                Info.Content = "Level " + level.ToString();
                _timer.Stop();
                if (!(l == 0))
                {
                    InitTimer();
                    FoodContainer = false;
                    RedrawFood();
                    _food.Ell.Fill = Brushes.Blue;
                }
                else
                {
                    MessageBox.Show("Yay, congrats! :)");
                    System.Diagnostics.Process.GetCurrentProcess().Kill();
                } 
            }
            return false;
        }
    }
}
