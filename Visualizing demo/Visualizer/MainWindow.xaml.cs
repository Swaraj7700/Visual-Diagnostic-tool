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
using System.Runtime.InteropServices;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;
using System.Windows.Media.Animation;
using System.Reflection.PortableExecutable;
using System.Reflection.Metadata;
using System.Windows.Media.Effects;

namespace Visualizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        /* [DllImport("ExportStackTrace.dll", CallingConvention = CallingConvention.StdCall)]
         static extern IntPtr getProcessHandle(); // Assuming the function returns a process handle as an IntPtr.

         [DllImport("ExportStackTrace.dll", CallingConvention = CallingConvention.StdCall)]
         static extern void Mult(int num1, int num2);
        */

        [return: MarshalAs(UnmanagedType.BStr)]
        [DllImport("ExportStackTrace.dll", CallingConvention = CallingConvention.StdCall)]
        static extern string GetFunctionName(int str_count);

        [DllImport("ExportStackTrace.dll", CallingConvention = CallingConvention.StdCall)]
        static extern int GetFunctionCount();
        private List<Expander> selectedExpanders = new List<Expander>();

        private List<string> selectedFunctionNames = new List<string>();

        public MainWindow()
        {
            InitializeComponent();

            // Calculate the desired proportion of the screen size
            double widthProportion = 0.95; // 70% of the screen width
            double heightProportion = 0.95; // 60% of the screen height

            // Get the screen dimensions
            double screenWidth = SystemParameters.PrimaryScreenWidth;
            double screenHeight = SystemParameters.PrimaryScreenHeight;

            // Calculate the window width and height based on the proportions
            double windowWidth = screenWidth * widthProportion;
            double windowHeight = screenHeight * heightProportion;

            // Set the window dimensions
            Width = windowWidth;
            Height = windowHeight;

            Title = "My Call Stack"; // Set the window title
            WindowStartupLocation = WindowStartupLocation.CenterScreen; // Set the window startup location
            WindowState = WindowState.Normal; // Set the initial window state
            WindowStyle = WindowStyle.SingleBorderWindow; // Set the window style
            ResizeMode = ResizeMode.NoResize; // Set the resize mode
                                              //Background = System.Windows.Media.Brushes.; // Set the background color
            SolidColorBrush backgroundBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#272537"));
            this.Background = backgroundBrush;
            // Create a Grid to host the button and stack
            Grid grid = new Grid();
            Content = grid;

            // Create a button
            LinearGradientBrush gradientBrush = new LinearGradientBrush();
            gradientBrush.StartPoint = new Point(0, 0);
            gradientBrush.EndPoint = new Point(1, 0);
            gradientBrush.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#5bc3ff"), 0));
            gradientBrush.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString("#3aa0ff"), 1));

            Button myButton = new Button
            {
                Content = "View Stack",
                Width = 200,
                Height = 60,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(0, 10, 10, 0),
                Background =gradientBrush, // Set the background color
                Foreground = Brushes.White, // Set the text color
                BorderThickness = new Thickness(2), // Add a border
                BorderBrush = Brushes.DarkSlateGray, // Set the border color
                FontSize = 16, // Set the font size
                FontWeight = FontWeights.Bold,
                FontFamily = new FontFamily("Open Sans") // Set the font family
                                                     // Set the font weight

            };

            

            Button myButton1 = new Button
            {
                Content = "Visualize", // Set the button text
                Width = 200,           // Set the button width
                Height = 60,            // Set the button height
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(10, 10, 0, 0) ,// Add left margin to shift it somewhat to the right
                Background = gradientBrush, // Set the background color
                Foreground = Brushes.White, // Set the text color
                BorderThickness = new Thickness(2), // Add a border
                BorderBrush = Brushes.DarkSlateGray, // Set the border color
                FontSize = 16, // Set the font size
                FontWeight = FontWeights.Bold,
                FontFamily = new FontFamily("Open Sans") // Set the font family// Set the font weight
            };
            myButton1.Click += (sender, e) =>
            {
                myButton1.Visibility = Visibility.Collapsed;
            };
            myButton.Click += (sender, e) =>
            {
                myButton1.Visibility = Visibility.Visible;
            };
            // Create a DropShadowEffect to be used on mouse enter
            DropShadowEffect shadowEffect = new DropShadowEffect
            {
                Color = Colors.White,
                ShadowDepth = 0,
                BlurRadius = 10,
                Opacity = 0.5
            };

            myButton.MouseEnter += (sender, e) =>
            {
                myButton.Background = Brushes.White;
                myButton.Foreground = Brushes.DarkBlue;
                myButton.Effect = shadowEffect;
            };

            myButton.MouseLeave += (sender, e) =>
            {
                myButton.Background =gradientBrush;
                myButton.Foreground =Brushes.White;
                //myButton.ClearValue(Button.BackgroundProperty);
                //myButton.ClearValue(Button.EffectProperty);
            };

            // Add a click event handler to the button
            myButton.Click += MyButton_Click;
            myButton1.Click += MyButton1_Click;

          
            // Add the button to the Grid
           grid.Children.Add(myButton);
            grid.Children.Add(myButton1);
        }
       

        private void MyButton_Click(object sender, RoutedEventArgs e)
        {
           
            int expanderCount = GetFunctionCount();
            Grid grid = Content as Grid;
            if (stackPanel != null)
            {
                grid.Children.Remove(stackPanel);
            }

            stackPanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            int defaultExpanderWidth = 300; // Set a default width for all expanders
            int defaultExpanderHeight = 60; // Set a default height for all expanders
            int expandedExpanderWidth = 400; // Set an expanded width
            int expandedExpanderHeight = 100; // Set an expanded height
            TimeSpan duration = TimeSpan.FromSeconds(0.3); // Duration for the transition animation

            // Clear the selected expanders list when the "View Stack" button is clicked
            selectedExpanders.Clear();

            for (int i = 0; i < expanderCount; i++)
            {
                Expander expander = new Expander
                {
                    IsExpanded = false,
                    Padding = new Thickness(10),
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Center,
                    Background = i == 0 ? new SolidColorBrush(Colors.Aquamarine) : new SolidColorBrush(Colors.White),
                    BorderBrush = new SolidColorBrush(Colors.Black),
                    BorderThickness = new Thickness(1), // Set the initial border thickness
                    Margin = new Thickness(10, 5, 10, 5),
                    Height = defaultExpanderHeight
                };

                TextBlock textBlock = new TextBlock
                {
                    Text = $"{i}",
                    Padding = new Thickness(10)
                };
                expander.Content = textBlock;

                TextBlock headerTextBlock = new TextBlock
                {
                    Text = GetFunctionName(i),
                    FontWeight = FontWeights.Bold,
                    FontSize = 16
                };
                expander.Header = headerTextBlock;

                // Smooth transition animation for expanding
                DoubleAnimation expandHeightAnimation = new DoubleAnimation(defaultExpanderHeight, expandedExpanderHeight, duration);

                // Smooth transition animation for collapsing
                DoubleAnimation collapseHeightAnimation = new DoubleAnimation(expandedExpanderHeight, defaultExpanderHeight, duration);

                expander.Expanded += (s, ev) =>
                {
                    expander.BeginAnimation(Expander.HeightProperty, expandHeightAnimation);
                };

                expander.Collapsed += (s, ev) =>
                {
                    expander.BeginAnimation(Expander.HeightProperty, collapseHeightAnimation);
                };

                // Attach a Click event handler to the Expander
                expander.MouseLeftButtonDown += (s, ev) =>
                {
                    if (selectedExpanders.Contains(expander))
                    {
                        // If the Expander is already selected, deselect it
                        selectedExpanders.Remove(expander);
                        expander.BorderThickness = new Thickness(1); // Reset border thickness
                        expander.BorderBrush= new SolidColorBrush(Colors.Salmon);
                    }
                    else
                    {
                        // If the Expander is not selected, select it
                        selectedExpanders.Add(expander);
                        expander.BorderThickness = new Thickness(3); // Increase border thickness
                        expander.BorderBrush= new SolidColorBrush(Colors.Salmon);
                    }
                };

                stackPanel.Children.Add(expander);
            }

            // Get the Grid that contains the button and add the stack to it
            grid.Children.Add(stackPanel);
        }

        private void MyButton1_Click(object sender, RoutedEventArgs e)
        {
            // Check if there are selected functions
            if (selectedExpanders.Count == 0)
            {
                MessageBox.Show("Please select functions first.");
                return;
            }

            Grid grid = Content as Grid;
            if (stackPanel != null)
            {
                grid.Children.Remove(stackPanel);
            }

            stackPanel = new StackPanel
            {
                Orientation = Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };

            int numPartitions = selectedExpanders.Count;

            stackPanel.SizeChanged += (s, ev) =>
            {
                for (int i = 0; i < numPartitions - 1; i++)
                {
                    Grid partition = new Grid();

                    // Calculate vertical positions to divide into equal parts
                    double y1 = (stackPanel.ActualHeight / numPartitions) * (i + 1);
                    double y2 = y1;

                    // Create a horizontal line as a divider
                    Line divider = new Line
                    {
                        X1 = 0,
                        X2 = stackPanel.ActualWidth,
                        Y1 = y1,
                        Y2 = y2,
                        Stroke = Brushes.Black,
                        StrokeThickness = 1,
                    };

                    partition.Children.Add(divider);
                    if (i==0)
                    {
                        Line vertical_divider = new Line
                        {
                            X1 = 450,
                            X2 = 450,
                            Y1 = 0,
                            Y2 = stackPanel.ActualHeight,
                            Stroke = Brushes.Black,
                            StrokeThickness = 1,
                        };

                        partition.Children.Add(vertical_divider);
                    }
                    stackPanel.Children.Add(partition);
                }
            };
            
            // Get the Grid that contains the button and add the stack to it
            grid.Children.Add(stackPanel);
        }
    }

}