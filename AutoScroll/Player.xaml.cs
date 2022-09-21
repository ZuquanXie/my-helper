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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AutoScroll
{
    /// <summary>
    /// Player.xaml 的交互逻辑
    /// </summary>
    public partial class Player : Window
    {
        private readonly DrawingGroup drawingGroup = new DrawingGroup();

        private readonly DoubleAnimation animation = new DoubleAnimation();

        private readonly Storyboard storyboard = new Storyboard();

        private readonly Rectangle rectangle = new Rectangle();

        private readonly Score score;

        private int PixelWidthTotal = 0;

        private int PixelHeightTotal = 0;

        private bool playing = false;

        private bool paused = false;

        public Player(Score s)
        {
            score = s;
            InitializeComponent();
            InitImages();
            TextBoxTime.Text = s.BPM.ToString();
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            InitCanvas();
        }

        private void InitImages()
        {
            int width = 0;
            int height = 0;
            foreach(var refrence in score.FileRefrences)
            {
                var imageDrawing = new ImageDrawing();
                var bitmap = new BitmapImage(new Uri(refrence));

                imageDrawing.ImageSource = bitmap;
                imageDrawing.Rect = new Rect(0, height, bitmap.PixelWidth, bitmap.PixelHeight);

                width = Math.Max(width, bitmap.PixelWidth);
                height += bitmap.PixelHeight;

                drawingGroup.Children.Add(imageDrawing);
            }
            PixelWidthTotal = width;
            PixelHeightTotal = height;
        }

        private void InitCanvas()
        {
            var canvas = new Canvas();
            var translate = new TranslateTransform(0, 0);

            NameScope.SetNameScope(this, new NameScope());
            RegisterName("theTranslate", translate);

            canvas.ClipToBounds = true;
            canvas.Children.Add(rectangle);

            rectangle.Width = GetDisplayWidthTotal();
            rectangle.Height = GetDisplayHeightTotal();
            rectangle.Fill = new DrawingBrush(drawingGroup);
            rectangle.Stretch = Stretch.Fill;
            rectangle.RenderTransform = translate;

            animation.AutoReverse = false;
            animation.From = 0;

            Storyboard.SetTargetName(animation, "theTranslate");
            Storyboard.SetTargetProperty(animation, new PropertyPath(TranslateTransform.YProperty));
            storyboard.Children.Add(animation);
            storyboard.Completed += AnimationCompleted;

            mainContainer.Child = canvas;
        }

        private void AnimationCompleted(object sender, EventArgs e)
        {
            playing = false;
        }

        private int GetDisplayWidthTotal()
        {
            return (int)mainContainer.ActualWidth;
        }

        private int GetDisplayHeightTotal()
        {
            int width = GetDisplayWidthTotal();
            if (PixelWidthTotal == 0 || width == 0)
            {
                return 0;
            }
            return width * PixelHeightTotal / PixelWidthTotal;
        }

        private int GetAnimationLength()
        {
            return -(GetDisplayHeightTotal() - (int)mainContainer.ActualHeight);
        }

        private void Play()
        {
            int.TryParse(TextBoxTime.Text, out int seconds);
            if (seconds <= 0)
            {
                MessageBox.Show("Time Error");
                return;
            }
            rectangle.Width = GetDisplayWidthTotal();
            rectangle.Height = GetDisplayHeightTotal();
            animation.Duration = new Duration(TimeSpan.FromSeconds(seconds));
            animation.To = GetAnimationLength();
            storyboard.Begin(this, true);
            playing = true;
            paused = false;
        }

        private void Pause()
        {
            if (playing)
            {
                storyboard.Pause(this);
                paused = true;
            }
        }

        private void Resume()
        {
            storyboard.Resume(this);
            paused = false;
        }

        private void Stop()
        {
            storyboard.Stop(this);
            playing = false;
            paused = false;
        }

        private void ButtonPause_Click(object sender, RoutedEventArgs e)
        {
            Pause();
            mainContainer.Focus();
        }

        private void ButtonStop_Click(object sender, RoutedEventArgs e)
        {
            Stop();
            mainContainer.Focus();
        }

        private void ButtonPlay_Click(object sender, RoutedEventArgs e)
        {
            if (paused)
            {
                Resume();
            } else
            {
                Play();
            }
            mainContainer.Focus();
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.OriginalSource is TextBox)
            {
                if (e.Key == Key.Enter)
                {
                    mainContainer.Focus();
                }
                return;
            }
            /// Space
            if (e.Key == Key.Space)
            {
                if (paused)
                {
                    Resume();
                } else
                {
                    if (playing)
                    {
                        Pause();
                    } else
                    {
                        Play();
                    }
                }
                return;
            }
            /// Backspace
            if (e.Key== Key.Back)
            {
                Stop();
            }
        }
    }
}
