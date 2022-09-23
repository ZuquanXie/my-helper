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

namespace AutoScroll
{
    /// <summary>
    /// ScoreCutter.xaml 的交互逻辑
    /// </summary>
    public partial class ScoreCutter : Window
    {
        private readonly string imagePath;
        private bool cutting = false;
        private bool dragging = false;
        private BitmapImage sourceImage;

        public ScoreCutter()
        {
            imagePath = "C:\\Users\\7733\\Pictures\\Camera Roll\\example.jpg";
            sourceImage = new BitmapImage(new Uri(imagePath));
            InitializeComponent();
        }
        public ScoreCutter(string path)
        {
            imagePath = path;
            sourceImage = new BitmapImage(new Uri(imagePath));
            InitializeComponent();
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            InitImage();
            InitCursor();
        }

        private void InitImage()
        {
            var imageDrawing = new ImageDrawing
            {
                Rect = new Rect(0, 0, sourceImage.PixelWidth, sourceImage.PixelHeight),
                ImageSource = sourceImage
            };
            var drawingBrush = new DrawingBrush
            {
                Drawing = imageDrawing
            };
            drawingBrush.Stretch = Stretch.None;
            Canvas.SetTop(theImage, 0);
            Canvas.SetLeft(theImage, 0);
            theImage.Width = sourceImage.PixelWidth;
            theImage.Height = sourceImage.PixelHeight;
            theImage.Fill = drawingBrush;
        }

        private void InitCursor()
        {
            theCursorWidth.Text = "100";
            theCursorHeight.Text = "100";
            theCursor.Width = 100;
            theCursor.Height = 100;
            Canvas.SetTop(theCursor, 0);
            Canvas.SetLeft(theCursor, 0);
        }

        private void SetCursorPosition(Point position)
        {
            if (position.X < theCursor.Width / 2)
            {
                Canvas.SetLeft(theCursor, 0);
            } else if (position.X > theCanvas.Width - theCursor.Width / 2)
            {
                Canvas.SetLeft(theCursor, theCanvas.Width - theCursor.Width);
            } else
            {
                Canvas.SetLeft(theCursor, position.X - theCursor.Width / 2);
            }
            if (position.Y < theCursor.Height / 2)
            {
                Canvas.SetTop(theCursor, 0);
            } else if (position.Y > theCanvas.Height - theCursor.Height / 2)
            {
                Canvas.SetTop(theCursor, theCanvas.Height - theCursor.Height);
            } else
            {
                Canvas.SetTop(theCursor, position.Y - theCursor.Height / 2);
            }
        }

        private void CreatePreview(Point position)
        {
            var visual = new VisualBrush
            {
                ViewboxUnits = BrushMappingMode.Absolute,
                Viewbox = new Rect(position.X - theCursor.Width / 2, position.Y - theCursor.Height / 2, theCursor.Width, theCursor.Height),
                Visual = theImage
            };
            var rectangle = new Rectangle
            {
                Width = theCursor.Width,
                Height = theCursor.Height,
                Fill = visual
            };
            thePreview.Children.Add(rectangle);
        }

        private void Content_MouseMove(object sender, MouseEventArgs e)
        {
            SetCursorPosition(e.GetPosition(theCanvas));
        }

        private void Content_MouseUp(object sender, MouseButtonEventArgs e)
        {
            CreatePreview(e.GetPosition(theImage));
        }

        private void ButtonCut_Click(object sender, RoutedEventArgs e)
        {
            if (cutting)
            {
                cutting = false;
                return;
            }
            int.TryParse(theCursorWidth.Text, out int width);
            int.TryParse(theCursorHeight.Text, out int height);
            theCutArea.Width = width;
            theCutArea.Height = height;
            cutting = true;
        }

    }
}
