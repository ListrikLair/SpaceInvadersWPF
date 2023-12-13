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

namespace SpaceInvadersWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool goLeft, goRight;

        private List<Rectangle> itemsToRemove = new List<Rectangle>();

        private int enemyImages = 0;
        private int bulletTimer = 0;
        private int bulletTimerLimit = 70;
        private int totalEnemies = 0;
        private int enemySpeed = 6;
        private bool gameOver = false;

        DispatcherTimer gameTimer = new DispatcherTimer();
        ImageBrush playerSkin = new ImageBrush();
        public MainWindow()
        {
            InitializeComponent();

            gameTimer.Tick += gameLoop;
            gameTimer.Interval = TimeSpan.FromMilliseconds(20);
            gameTimer.Start();

            playerSkin.ImageSource = new BitmapImage(new Uri("C:\\Users\\kevin\\OneDrive\\Dokumenter\\CodeStuffz\\C# projects\\SpaceInvadersWPF\\SpaceInvadersWPF\\Images\\player.png"));
            player.Fill = playerSkin;

            MyCanvas.Focus();

            MakeEnemies(10);
        }

        private void gameLoop(object sender, EventArgs e)
        {
            Rect playerHitBox = new Rect(Canvas.GetLeft(player), Canvas.GetTop(player), player.Width, player.Height);
            enemiesLeft.Content = "Enemies Left: " + totalEnemies;

            // movement up to walls, but not past them
            if (goLeft && Canvas.GetLeft(player) > 0)
            {
                Canvas.SetLeft(player, Canvas.GetLeft(player) - 10);
            }
            if (goRight && Canvas.GetLeft(player) + 80 < Application.Current.MainWindow.Width)
            {
                Canvas.SetLeft(player, Canvas.GetLeft(player) + 10);
            }

            //Timer for enemy bullets firing off
            bulletTimer -= 3;

            if (bulletTimer < 0)
            {
                EnemyBulletMaker(Canvas.GetLeft(player) + 20, 10);

                bulletTimer = bulletTimerLimit;
            }

            foreach (var x in MyCanvas.Children.OfType<Rectangle>())
            {
                //Animate own bullets
                if (x is Rectangle && (string)x.Tag == "bullet")
                {
                    Canvas.SetTop(x, Canvas.GetTop(x) - 20);

                    if (Canvas.GetTop(x) < 10)
                    {
                        itemsToRemove.Add(x);
                    }

                    Rect bulletHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                    foreach (var y in MyCanvas.Children.OfType<Rectangle>())
                    {
                        if (y is Rectangle && (string)y.Tag == "enemy")
                        {
                            Rect enemyHit = new Rect(Canvas.GetLeft(y), Canvas.GetTop(y), y.Width, y.Height);

                            if (bulletHitBox.IntersectsWith(enemyHit))
                            {
                                itemsToRemove.Add(x);
                                itemsToRemove.Add(y);
                                totalEnemies -= 1;
                            }
                        }
                    }
                }

                //Animate enemy bullets
                if (x is Rectangle && (string)x.Tag == "enemyBullet")
                {
                    Canvas.SetTop(x, Canvas.GetTop(x) + 10);

                    if (Canvas.GetTop(x) > 480)
                    {
                        itemsToRemove.Add(x);
                    }

                    Rect enemyBulletHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                    if (playerHitBox.IntersectsWith(enemyBulletHitBox))
                    {
                        ShowGameOver("You were shot down by an invader!");
                    }
                }

                //Animate Enemy
                if (x is Rectangle && (string)x.Tag == "enemy")
                {
                    Canvas.SetLeft(x, Canvas.GetLeft(x) + enemySpeed);

                    if (Canvas.GetLeft(x) > 820)
                    {
                        Canvas.SetLeft(x, -80);
                        Canvas.SetTop(x, Canvas.GetTop(x) + (x.Height + 10));
                    }

                    Rect enemyHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                    if (playerHitBox.IntersectsWith(enemyHitBox))
                    {
                        ShowGameOver("You were killed by an invader!");
                    }
                }
            }

            foreach (Rectangle i in itemsToRemove)
            {
                MyCanvas.Children.Remove(i);
            }
        }

        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.A)
            {
                goLeft = true;
            }
            if (e.Key == Key.D)
            {
                goRight = true;
            }
        }

        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.A)
            {
                goLeft = false;
            }
            if (e.Key == Key.D)
            {
                goRight = false;
            }

            if (e.Key == Key.Space)
            {
                Rectangle newBullet = new Rectangle
                {
                    Tag = "bullet",
                    Height = 20,
                    Width = 5,
                    Fill = Brushes.White,
                    Stroke = Brushes.Red
                };

                Canvas.SetTop(newBullet, Canvas.GetTop(player) - newBullet.Height);
                Canvas.SetLeft(newBullet, Canvas.GetLeft(player) + player.Width / 2);

                MyCanvas.Children.Add(newBullet);
            }
        }

        private void EnemyBulletMaker(double x, double y)
        {
            Rectangle enemyBullet = new Rectangle
            {
                Tag = "enemyBullet",
                Height = 35,
                Width = 10,
                Fill = Brushes.Yellow,
                Stroke = Brushes.Black
            };

            Canvas.SetTop(enemyBullet, y);
            Canvas.SetLeft(enemyBullet, x);

            MyCanvas.Children.Add(enemyBullet);
        }

        private void MakeEnemies(int limit)
        {
            int left = 0;

            totalEnemies = limit;

            for (int i = 0; i < limit; i++)
            {
                ImageBrush enemySkin = new ImageBrush();

                Rectangle newEnemy = new Rectangle
                {
                    Tag = "enemy",
                    Height = 45,
                    Width = 45,
                    Fill = enemySkin
                };

                Canvas.SetTop(newEnemy, 10);
                Canvas.SetLeft(newEnemy, left);
                MyCanvas.Children.Add(newEnemy);
                left -= 60;

                enemyImages++;
                if (enemyImages > 8)
                {
                    enemyImages = 1;
                }

                switch (enemyImages)
                {
                    case 1:
                        enemySkin.ImageSource = new BitmapImage(new Uri(
                            "C:\\Users\\kevin\\OneDrive\\Dokumenter\\CodeStuffz\\C# projects\\SpaceInvadersWPF\\SpaceInvadersWPF\\Images\\invader1.gif"));
                        break;
                    case 2:
                        enemySkin.ImageSource = new BitmapImage(new Uri(
                            "C:\\Users\\kevin\\OneDrive\\Dokumenter\\CodeStuffz\\C# projects\\SpaceInvadersWPF\\SpaceInvadersWPF\\Images\\invader2.gif"));
                        break;
                    case 3:
                        enemySkin.ImageSource = new BitmapImage(new Uri(
                            "C:\\Users\\kevin\\OneDrive\\Dokumenter\\CodeStuffz\\C# projects\\SpaceInvadersWPF\\SpaceInvadersWPF\\Images\\invader3.gif"));
                        break;
                    case 4:
                        enemySkin.ImageSource = new BitmapImage(new Uri(
                            "C:\\Users\\kevin\\OneDrive\\Dokumenter\\CodeStuffz\\C# projects\\SpaceInvadersWPF\\SpaceInvadersWPF\\Images\\invader4.gif"));
                        break;
                    case 5:
                        enemySkin.ImageSource = new BitmapImage(new Uri(
                            "C:\\Users\\kevin\\OneDrive\\Dokumenter\\CodeStuffz\\C# projects\\SpaceInvadersWPF\\SpaceInvadersWPF\\Images\\invader5.gif"));
                        break;
                    case 6:
                        enemySkin.ImageSource = new BitmapImage(new Uri(
                            "C:\\Users\\kevin\\OneDrive\\Dokumenter\\CodeStuffz\\C# projects\\SpaceInvadersWPF\\SpaceInvadersWPF\\Images\\invader6.gif"));
                        break;
                    case 7:
                        enemySkin.ImageSource = new BitmapImage(new Uri(
                            "C:\\Users\\kevin\\OneDrive\\Dokumenter\\CodeStuffz\\C# projects\\SpaceInvadersWPF\\SpaceInvadersWPF\\Images\\invader7.gif"));
                        break;
                    case 8:
                        enemySkin.ImageSource = new BitmapImage(new Uri(
                            "C:\\Users\\kevin\\OneDrive\\Dokumenter\\CodeStuffz\\C# projects\\SpaceInvadersWPF\\SpaceInvadersWPF\\Images\\invader8.gif"));
                        break;
                }
            }
        }

        private void ShowGameOver(string msg)
        {

        }
    }
}
