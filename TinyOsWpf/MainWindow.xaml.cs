using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls.Ribbon;
using System.Windows.Input;
using System.Windows.Media;

namespace TinyOsWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : RibbonWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            Main.Run(new string[] {
                @"C:\Users\abaku\Source\Repos\!GitHub\TinyOS\Sample Programs\prog1.txt",
                @"C:\Users\abaku\Source\Repos\!GitHub\TinyOS\Sample Programs\idle.txt"
            });
        }
    }

    public class VisualHost : FrameworkElement
    {
        // Create a collection of child visual objects. 
        private readonly VisualCollection _visuals;
        private readonly Dictionary<Guid, DrawingVisual> _visualDictionary;

        // Provide a required override for the VisualChildrenCount property. 
        protected override int VisualChildrenCount
        {
            get { return _visuals.Count; }
        }

        // Provide a required override for the GetVisualChild method.   
        protected override Visual GetVisualChild(int index)
        {
            if (index < 0 || index >= _visuals.Count)
            {
                throw new ArgumentOutOfRangeException();
            }

            return _visuals[index];
        }
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            System.Diagnostics.Debug.WriteLine(e.Property.Name);
            if (e.Property.Name == "ActualWidth" || e.Property.Name == "ActualHeight" || e.Property.Name == "Center")
            {
                DrawIt_Loaded(this, null);
            }
        }

        public VisualHost()
        {
            _visuals = new VisualCollection(this);
            _visualDictionary = new Dictionary<Guid, DrawingVisual>();

            FillVisuals();

            //this.Loaded += new RoutedEventHandler(DrawIt_Loaded);
            // Add the event handler for MouseLeftButtonUp.
            this.MouseLeftButtonUp += new MouseButtonEventHandler(MyVisualHost_MouseLeftButtonUp);
            this.MouseRightButtonUp += new MouseButtonEventHandler(MyVisualHost_MouseLeftButtonUp);
        }

        // Capture the mouse event and hit test the coordinate point value against
        // the child visual objects.
        void MyVisualHost_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // Retreive the coordinates of the mouse button event.
            Point pt = e.GetPosition((UIElement)sender);

            // Initiate the hit test by setting up a hit test result callback method.
            VisualTreeHelper.HitTest(this, null, new HitTestResultCallback(myCallback), new PointHitTestParameters(pt));
        }

        // If a child visual object is hit, toggle its opacity to visually indicate a hit.
        public HitTestResultBehavior myCallback(HitTestResult result)
        {
            if (result.VisualHit.GetType() == typeof(DrawingVisual))
            {
                var visual = (DrawingVisual)result.VisualHit;
                if (visual.Opacity == 1.0)
                {
                    using (var dc = visual.RenderOpen())
                    {
                        PutText(dc, visual.ContentBounds.Location);
                    }
                    visual.Opacity = 0.4;
                }
                else
                {
                    visual.Opacity = 1.0;
                }
            }

            // Stop the hit test enumeration of objects in the visual tree.
            return HitTestResultBehavior.Stop;
        }

        void FillVisuals()
        {
            for (int i = 0; i < 1000; i++)
            {
                var visual = new DrawingVisual();
                using (var dc = visual.RenderOpen())
                {
                    //dc.DrawRectangle(Brushes.Red, new Pen(Brushes.Black, 2),
                    //    new Rect(new Point(w, h), new Size(size, size)));
                    dc.DrawRectangle(Brushes.Red, null,
                        new Rect(new Point(i * 5, i * 5), new Size(5, 5)));
                }
                _visualDictionary.Add(Guid.NewGuid(), visual);
                _visuals.Add(visual);
            }
        }

        void DrawIt_Loaded(object sender, RoutedEventArgs e)
        {
            int w = 0;
            int h = 0;
            int size = 16;
            var lastDictionary = new Dictionary<Point, DrawingVisual>();
            foreach (var visualItem in _visualDictionary)
            {
                //System.Diagnostics.Debug.WriteLine(visualItem.Value.ToString());
                using (var dc = visualItem.Value.RenderOpen())
                {
                    if (visualItem.Value.Opacity < 1)
                    {
                        lastDictionary.Add(new Point(w, h), visualItem.Value);
                    }
                    else
                    {
                        dc.DrawRectangle(Brushes.Red, null,
                                                new Rect(new Point(w, h), new Size(size, size)));
                    }
                }
                w += size + 1;
                if (w >= ActualWidth - size)
                {
                    w = 0;
                    h += size + 1;
                }
            }
            foreach (var visualItem in lastDictionary)
            {
                using (var dc = visualItem.Value.RenderOpen())
                {
                    var pt = visualItem.Key;
                    dc.DrawRectangle(Brushes.Red, null,
                                                new Rect(pt, new Size(size, size)));
                    PutText(dc, pt);
                }
            }
            /*
            for (int h = 0; h <= ActualHeight - size; h += size + 1)
            {
                for (int w = 0; w <= ActualWidth - size; w += size + 1)
                {

                }
            }
            */
        }

        void PutText(DrawingContext dc, Point pt)
        {
            string testString = "Formatted MML Document is displayed here!\nPlease implement the user oriented layout logic.";

            FormattedText formattedText = new FormattedText(testString,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight, new Typeface("Verdana"),
                14, Brushes.Black);

            formattedText.MaxTextWidth = 80;
            formattedText.MaxTextHeight = 80;

            formattedText.SetForegroundBrush(new LinearGradientBrush(Colors.Blue, Colors.Teal, 90.0), 10, 12);

            formattedText.SetFontStyle(FontStyles.Italic, 36, 5);
            formattedText.SetForegroundBrush(new LinearGradientBrush(Colors.Pink, Colors.Crimson, 90.0), 36, 5);
            formattedText.SetFontSize(14, 36, 5);

            formattedText.SetFontWeight(FontWeights.Bold, 42, 48);

            dc.DrawRectangle(Brushes.White, null, new Rect(pt, new Point(100, 100)));

            dc.DrawText(formattedText, pt);
        }
    }
}
