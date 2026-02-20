using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace ModemMergerDesktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DateTime _lastClickedTime;
        private bool _isFirstClick = true;
        private bool _doubleClickDetected = false;
        private const int DoubleClickTimeLimit = 2000;

        public MainWindow()
        {
            InitializeComponent();
        }

        // Close the window when the close button is clicked
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // Minimize the window when the minimize button is clicked
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        // Maximize/Restore the window when the maximize button is clicked
        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
            }
        }

        //// Allow dragging the window by holding down the left mouse button on the top panel
        //protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        //{
           
        //}

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //base.OnMouseLeftButtonDown(e);

            DateTime currentClickTime = DateTime.Now;

            // Initiate dragging the window
            
            if (_isFirstClick)
            {
                //First Click
                _isFirstClick = false;
                _doubleClickDetected = false;
                _lastClickedTime = currentClickTime;

                // Start a timer to reset if no second click happens
                DispatcherTimer clickTimer = new DispatcherTimer();
                clickTimer.Interval = TimeSpan.FromMilliseconds(DoubleClickTimeLimit);
                clickTimer.Tick += (s, args) =>
                {
                    clickTimer.Stop();
                    _isFirstClick = true;
                };
                clickTimer.Start();

                if (e.ButtonState == MouseButtonState.Pressed)
                {
                    DragMove();
                    
                }
            }
            else
            {
                //Second click, check if it is within the time limit
                TimeSpan timeBetweenClicks = currentClickTime - _lastClickedTime;
                if (timeBetweenClicks.TotalMilliseconds <= DoubleClickTimeLimit)
                {
                    _doubleClickDetected = true;
                }
                _isFirstClick = true;
            }
            
        }

        

        private void Grid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_doubleClickDetected)
            {
                //maximize the window or make it normal
                if (this.WindowState == WindowState.Maximized)
                {
                    this.WindowState = WindowState.Normal;
                }
                else
                {
                    this.WindowState = WindowState.Maximized;
                }
            }
        }
    }
}
