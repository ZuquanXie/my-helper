using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private readonly BitmapImage sourceImage;

        private readonly Collection<UserOperation> operationLogs = new Collection<UserOperation>();

        static readonly int CutAreaBorderSize = 2;
        static readonly Brush CutAreaBorderFill = Brushes.Black;

        private bool cutAreaDrag = false;
        private Point cutAreaDragOffset = new Point(0, 0);
        private Point cutAreaDragBeginPosition = new Point(0, 0);

        private int cutAreaResizeMoveThrottling = 0;
        private bool cutAreaResizeLeft = false;
        private bool cutAreaResizeRight = false;
        private bool cutAreaResizeTop = false;
        private bool cutAreaResizeBottom = false;
        private Point cutAreaResizeBeginPoint = new Point(0, 0);
        private Point cutAreaResizeBeginPosition = new Point(0, 0);
        private Size cutAreaResizeBeginSize = new Size(0, 0);

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
            InitCutArea();
        }

        private void WindowMouseLeave(object sender, MouseEventArgs e)
        {
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
            theImage.Width = sourceImage.PixelWidth;
            theImage.Height = sourceImage.PixelHeight;
            theImage.Fill = drawingBrush;
            Canvas.SetTop(theImage, 0);
            Canvas.SetLeft(theImage, 0);
            Canvas.SetZIndex(theImage, 0);
        }

        private void InitCutArea()
        {
            var initWidth = 100;
            var initHeight = 100;
            var groupWidth = CutAreaBorderSize * 2 + initWidth;
            var groupHeight = CutAreaBorderSize * 2 + initHeight;

            /// group
            theCutAreaGroup.Width = groupWidth;
            theCutAreaGroup.Height = groupHeight;
            Canvas.SetZIndex(theCutAreaGroup, 2);
            Canvas.SetLeft(theCutAreaGroup, 0);
            Canvas.SetTop(theCutAreaGroup, 0);

            /// area
            theCutArea.Width = initWidth;
            theCutArea.Height = initHeight;
            theCutArea.Fill = Brushes.Transparent;
            Canvas.SetLeft(theCutArea, CutAreaBorderSize);
            Canvas.SetTop(theCutArea, CutAreaBorderSize);

            /// left
            theCutAreaLeft.Width = CutAreaBorderSize;
            theCutAreaLeft.Height = groupHeight;
            //theCutAreaLeft.Fill = CutAreaBorderFill;
            theCutAreaLeft.Fill = Brushes.Red;
            Canvas.SetLeft(theCutAreaLeft, 0);
            Canvas.SetTop(theCutAreaLeft, 0);

            /// top
            theCutAreaTop.Width = initWidth;
            theCutAreaTop.Height = CutAreaBorderSize;
            //theCutAreaTop.Fill = CutAreaBorderFill;
            theCutAreaTop.Fill = Brushes.Green;
            Canvas.SetLeft(theCutAreaTop, CutAreaBorderSize);
            Canvas.SetTop(theCutAreaTop, 0);

            /// right
            theCutAreaRight.Width = CutAreaBorderSize;
            theCutAreaRight.Height = groupHeight;
            //theCutAreaRight.Fill = CutAreaBorderFill;
            theCutAreaRight.Fill = Brushes.Red;
            Canvas.SetRight(theCutAreaRight, 0);
            Canvas.SetTop(theCutAreaRight, 0);

            /// bottom
            theCutAreaBottom.Width = initWidth;
            theCutAreaBottom.Height = CutAreaBorderSize;
            //theCutAreaBottom.Fill = CutAreaBorderFill;
            theCutAreaBottom.Fill = Brushes.Green;
            Canvas.SetLeft(theCutAreaBottom, CutAreaBorderSize);
            Canvas.SetBottom(theCutAreaBottom, 0);
        }

        private void UpdatePositionCutArea(Point to)
        {
            Canvas.SetLeft(theCutAreaGroup, to.X);
            Canvas.SetTop(theCutAreaGroup, to.Y);
        }

        private void UpdatePositionCutArea(Point to, Point from)
        {
            operationLogs.Add(new UserOperation(UserOperationType.MoveCutArea, new Point(to.X, to.Y), new Point(from.X, from.Y)));
            UpdatePositionCutArea(to);
        }

        private void UpdateSizeCutArea(Size to, Point positionTo)
        {
            UpdatePositionCutArea(positionTo);
            theCutAreaGroup.Width = to.Width;
            theCutAreaGroup.Height = to.Height;

            theCutArea.Width = theCutAreaTop.Width = theCutAreaBottom.Width = to.Width - CutAreaBorderSize * 2;
            theCutArea.Height = to.Height - CutAreaBorderSize * 2;
            theCutAreaLeft.Height = theCutAreaRight.Height = to.Height;
        }

        private void UpdateSizeCutArea(Size to, Point positionTo, Size from, Point positionFrom)
        {
            operationLogs.Add(new UserOperation(UserOperationType.ResizeCutArea, new DragResize(to, positionTo), new DragResize(from, positionFrom)));
            UpdateSizeCutArea(to, positionTo);
        }

        private void DragMoveBeginCutArea(Point position)
        {
            cutAreaDrag = true;
            cutAreaDragOffset = position;
            cutAreaDragBeginPosition = new Point(Canvas.GetLeft(theCutAreaGroup), Canvas.GetTop(theCutAreaGroup));
        }

        private void DragMoveCutArea(Point position)
        {
            var to = new Point();
            var x = position.X - cutAreaDragOffset.X;
            var y = position.Y - cutAreaDragOffset.Y;
            if (x < 0)
            {
                to.X = 0;
            } else if (x + theCutAreaGroup.Width > theCanvas.Width)
            {
                to.X = theCanvas.Width - theCutAreaGroup.Width;
            } else
            {
                to.X = x;
            }
            if (y < 0)
            {
                to.Y = 0;
            } else if (y + theCutAreaGroup.Height > theCanvas.Height)
            {
                to.Y = theCanvas.Height - theCutAreaGroup.Height;
            } else
            {
                to.Y = y;
            }
            UpdatePositionCutArea(to);
        }

        private void DragMoveEndCutArea()
        {
            cutAreaDrag = false;
            UpdatePositionCutArea(new Point(Canvas.GetLeft(theCutAreaGroup), Canvas.GetTop(theCutAreaGroup)), cutAreaDragBeginPosition);
        }

        private void DragResizeCutArea(Point position, bool needLog = false)
        {
            double xChanged = 0;
            double yChanged = 0;
            double widthChanged = 0;
            double heightChanged = 0;
            if (cutAreaResizeLeft)
            {
                widthChanged = cutAreaResizeBeginPoint.X - position.X;
                xChanged = -widthChanged;
            } else if (cutAreaResizeRight)
            {
                widthChanged = position.X - cutAreaResizeBeginPoint.X;
            } else if (cutAreaResizeTop)
            {
                heightChanged = cutAreaResizeBeginPoint.Y - position.Y;
                yChanged = -heightChanged;
            } else
            {
                heightChanged = position.Y - cutAreaResizeBeginPoint.Y;
            }
            if (needLog)
            {
                UpdateSizeCutArea(
                    new Size(cutAreaResizeBeginSize.Width + widthChanged, cutAreaResizeBeginSize.Height + heightChanged),
                    new Point(cutAreaResizeBeginPosition.X + xChanged, cutAreaResizeBeginPosition.Y + yChanged),
                    cutAreaResizeBeginSize,
                    cutAreaResizeBeginPosition
                    );
            } else
            {
                UpdateSizeCutArea(
                    new Size(cutAreaResizeBeginSize.Width + widthChanged, cutAreaResizeBeginSize.Height + heightChanged),
                    new Point(cutAreaResizeBeginPosition.X + xChanged, cutAreaResizeBeginPosition.Y + yChanged)
                    );
            }
        }

        private void DragResizeBeginCutArea(Rectangle handler, Point position)
        {
            cutAreaResizeLeft = handler == theCutAreaLeft;
            cutAreaResizeRight = handler == theCutAreaRight;
            cutAreaResizeTop = handler == theCutAreaTop;
            cutAreaResizeBottom = handler == theCutAreaBottom;
            cutAreaResizeBeginPoint = position;
            cutAreaResizeBeginPosition = new Point(Canvas.GetLeft(theCutAreaGroup), Canvas.GetTop(theCutAreaGroup));
            cutAreaResizeBeginSize = new Size(theCutAreaGroup.Width, theCutAreaGroup.Height);
        }

        /// todo: change the element's property will create wrong result when drag move
        private void DragResizeMoveCutArea(MouseEventArgs e)
        {
            if (cutAreaResizeMoveThrottling < 5)
            {
                cutAreaResizeMoveThrottling += 1;
                return;
            }
            DragResizeCutArea(e.GetPosition(theCutAreaGroup));
            cutAreaResizeMoveThrottling = 0;
        }

        private void DragResizeEndCutArea(MouseEventArgs e)
        {
            DragResizeCutArea(e.GetPosition(theCutAreaGroup), true);
            cutAreaResizeLeft = cutAreaResizeRight = cutAreaResizeTop = cutAreaResizeBottom = false;
        }

        private void CreatePreview(Point position, Size size)
        {
            var visual = new VisualBrush
            {
                ViewboxUnits = BrushMappingMode.Absolute,
                Viewbox = new Rect(position.X, position.Y, size.Width, size.Height),
                Visual = theImage
            };
            var rectangle = new Rectangle
            {
                Width = size.Width,
                Height = size.Height,
                Fill = visual
            };
            thePreview.Children.Add(rectangle);
        }

        private void ButtonCut_Click(object sender, RoutedEventArgs e)
        {
            CreatePreview(new Point(Canvas.GetLeft(theCutAreaGroup), Canvas.GetTop(theCutAreaGroup)), new Size(theCutAreaGroup.Width, theCutAreaGroup.Height));
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (cutAreaDrag == true)
            {
                DragMoveCutArea(e.GetPosition(theImage));
                return;
            }
            if (cutAreaResizeLeft || cutAreaResizeRight || cutAreaResizeTop || cutAreaResizeBottom) {
                DragResizeMoveCutArea(e);
                return;
            }
        }

        private void Canvas_MouseLeave(object sender, MouseEventArgs e)
        {
            if (cutAreaDrag)
            {
                DragMoveEndCutArea();
                return;
            }
            if (cutAreaResizeLeft || cutAreaResizeRight || cutAreaResizeTop || cutAreaResizeBottom) {
                DragResizeEndCutArea(e);
                return;
            }
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            /// drag move cutArea
            if (e.Source == theCutArea)
            {
                DragMoveBeginCutArea(e.GetPosition(theCutAreaGroup));
                return;
            }

            /// drag resize cutArea
            if (e.Source == theCutAreaLeft || e.Source == theCutAreaRight || e.Source == theCutAreaTop || e.Source == theCutAreaBottom)
            {
                DragResizeBeginCutArea((Rectangle)e.Source, e.GetPosition(theCutAreaGroup));
                return;
            }
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (cutAreaDrag)
            {
                DragMoveEndCutArea();
                return;
            }
            if (cutAreaResizeLeft || cutAreaResizeRight || cutAreaResizeTop || cutAreaResizeBottom) {
                DragResizeEndCutArea(e);
                return;
            }
        }
    }

    public enum UserOperationType
    {
        MoveCutArea,
        ResizeCutArea
    }

    public class DragResize
    {
        public Size Size;
        public Point Position;
        public DragResize(Size size, Point position)
        {
            Size = size;
            Position = position;
        }

        override
        public string ToString() {
            return "Size: " + Size.ToString() + " Position: " + Position.ToString();
        }
    }

    public class UserOperation
    {
        public UserOperationType Type;
        public Object From;
        public Object To;

        public UserOperation(UserOperationType type, object to, object from)
        {
            Type = type;
            From = from;
            To = to;
        }
        override
        public string ToString()
        {
            return "(From: " + From.ToString() + ") To: " + To.ToString();
        }
    }
}
