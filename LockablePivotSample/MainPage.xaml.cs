using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Linq.Expressions;
using System.ComponentModel;

namespace LockablePivotSample
{
	public partial class MainPage : PhoneApplicationPage
	{
		private Point currentPoint;
		private Point oldPoint;
		
		public MainPage()
		{
			InitializeComponent();
            Touch.FrameReported += Touch_FrameReported;
            this.Loaded += MainPage_Loaded;
		}

        void Touch_FrameReported(object sender, TouchFrameEventArgs e)
        {
            if (e.GetPrimaryTouchPoint(this.FirstPivot_PV).Action == TouchAction.Up)
            {
                //this.pivot.IsHitTestVisible = true;
                //this.pivot.IsLocked = false;
            }   
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            Events();
        }

		private void ContentPanelCanvas_MouseMove(object sender, MouseEventArgs e)
		{
			currentPoint = e.GetPosition(this.ContentPanelCanvas);

			Line line = new Line() { X1 = currentPoint.X, Y1 = currentPoint.Y, X2 = oldPoint.X, Y2 = oldPoint.Y };
			line.Stroke = new SolidColorBrush(Colors.Red);
			line.StrokeThickness = 15;

			this.ContentPanelCanvas.Children.Add(line);
			oldPoint = currentPoint;
		}

		private void ContentPanelCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			currentPoint = e.GetPosition(ContentPanelCanvas);
			oldPoint = currentPoint;
		}

		private void btnLock_Click(object sender, RoutedEventArgs e)
		{
			//this.pivot.IsLocked = true;
		}

		private void btnUnLock_Click(object sender, RoutedEventArgs e)
		{
			//this.pivot.IsLocked = false;
		}

        public void Events()
        {
            this.FirstPivot_PV.UseOptimizedManipulationRouting = false;
            this.FirstPivot_PV.AddHandler(PivotItem.ManipulationStartedEvent, new EventHandler<ManipulationStartedEventArgs>(pivot_ManipulationStarted), true);
            this.FirstPivot_PV.AddHandler(PivotItem.ManipulationDeltaEvent, new EventHandler<ManipulationDeltaEventArgs>(pivot_ManipulationCompleted), true);
        }

        Point startPoint;
        private void pivot_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            startPoint = e.ManipulationOrigin;
        }

        private void pivot_ManipulationCompleted(object sender, ManipulationDeltaEventArgs e)
        {
           Point endPoint = e.ManipulationOrigin;
           if (endPoint.X - startPoint.X >= 0)
           {
               #region Control Right Side
               e.Complete();
               e.Handled = true;
               #endregion
           }

           if (endPoint.X - startPoint.X < 0)
           {
               #region Control Left Side
               e.Complete();
               e.Handled = true;
               #endregion
           }
        }


	}


    public static class ExtensionMethods
    {
        public static FrameworkElement FindVisualChild(this FrameworkElement root, string name)
        {
            FrameworkElement temp = root.FindName(name) as FrameworkElement;
            if (temp != null)
                return temp;

            foreach (FrameworkElement element in root.GetVisualDescendents())
            {
                temp = element.FindName(name) as FrameworkElement;
                if (temp != null)
                    return temp;
            }

            return null;
        }

        public static IEnumerable<FrameworkElement> GetVisualDescendents(this FrameworkElement root)
        {
            Queue<IEnumerable<FrameworkElement>> toDo = new Queue<IEnumerable<FrameworkElement>>();

            toDo.Enqueue(root.GetVisualChildren());
            while (toDo.Count > 0)
            {
                IEnumerable<FrameworkElement> children = toDo.Dequeue();
                foreach (FrameworkElement child in children)
                {
                    yield return child;
                    toDo.Enqueue(child.GetVisualChildren());
                }
            }
        }

        public static IEnumerable<FrameworkElement> GetVisualChildren(this FrameworkElement root)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(root); i++)
                yield return VisualTreeHelper.GetChild(root, i) as FrameworkElement;
        }
    }
}