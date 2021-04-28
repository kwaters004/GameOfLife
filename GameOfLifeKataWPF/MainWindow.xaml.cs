using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace GameOfLifeKataWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>



    public partial class MainWindow : Window
    {
        private readonly BackgroundWorker _backgroundWorker;
        public List<Point> shape = new List<Point>();
        public int speed = 1500;


        public MainWindow()
        {

            InitializeComponent();
            // this is where i would take the input

            AliveDead.CreateDictionary(5, 5);

            _backgroundWorker = new BackgroundWorker
            {
                WorkerSupportsCancellation = true
            };
            _backgroundWorker.DoWork += backgroundWorker_DoWork;
            _backgroundWorker.RunWorkerCompleted += backgroundWorker_RunWorkerCompleted;


            nextButton.Focus();
            CreateButtonsAndGrid(5, 5);
            underPop.Text = "<" + Convert.ToString(AliveDead.underPopulated);
            overPop.Text = ">" + Convert.ToString(AliveDead.overPopulated);
            growthS.Text = ">=" + Convert.ToString(AliveDead.growthStart);
            growthE.Text = "<=" + Convert.ToString(AliveDead.growthEnd);

            slider.Value = 50;

        }

        private void CreateButtonsAndGrid(int width, int height)
        {

            List<Button> allButtons = new List<Button>();
            List<ColumnDefinition> cols = new List<ColumnDefinition>();
            List<RowDefinition> ros = new List<RowDefinition>();

            for (int j = 0; j < width; j++)
            {
                ColumnDefinition c = new ColumnDefinition();
                c.Width = new GridLength(playArea.Width / width);
                playArea.ColumnDefinitions.Add(c);
                cols.Add(c);
            }
            for (int i = 0; i < height; i++)
            {
                RowDefinition r = new RowDefinition();
                r.Height = new GridLength(playArea.Height / height);
                playArea.RowDefinitions.Add(r);
                ros.Add(r);
            }


            int buttonwidth = 0;
            int buttonheight = 0;


            buttonwidth = (Int32)playArea.Width / width;
            buttonheight = (Int32)playArea.Height / height;


            //int extra = 0;
            //if ((Int32)playArea.Width % width == 0)
            //{
            //    buttonwidth = (Int32)playArea.Width / width;
            //    //firstRow.Height = new GridLength(20);

            //}
            //else
            //{
            //    extra = (Int32)playArea.Width % width;
            //    buttonwidth = ((Int32)playArea.Height - extra) / width;
            //    //firstRow.Height = new GridLength(20 + (extra / 2));
            //}

            //if ((Int32)playArea.Height % height == 0)
            //{
            //    buttonheight = (Int32)playArea.Height / height;
            //    //firstColumn.Width = new GridLength(20);
            //}
            //else
            //{
            //    extra = (Int32)playArea.Height % height;
            //    buttonheight = ((Int32)playArea.Height - extra) / height;
            //    //firstColumn.Width = new GridLength(20 + (extra / 2));

            //}

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {

                    Button button = new Button();

                    // need to add button functionality
                    button.Click += button_Click;
                    button.DragEnter += button_Click;
                    //button.Height = buttonheight;
                    //button.Width = buttonwidth;

                    button.Background = Brushes.White;

                    Grid.SetColumn(button, j);
                    Grid.SetRow(button, i);
                    playArea.Children.Add(button);
                    allButtons.Add(button);
                }
            }



            ButtonList.buttons = allButtons;
            ButtonList.columns = cols;
            ButtonList.rows = ros;

            playArea.HorizontalAlignment = HorizontalAlignment.Center;
            playArea.VerticalAlignment = VerticalAlignment.Center;



        }


        private void StartLoop()
        {
            if (!_backgroundWorker.IsBusy)
            {
                _backgroundWorker.RunWorkerAsync();
            }
        }

        private void StopLoop()
        {
            _backgroundWorker.CancelAsync();
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var backgroundWorker = (BackgroundWorker)sender;

            while (!e.Cancel)
                if (backgroundWorker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
                else
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        AliveDead.ButtonValues();
                        livesText.Text = "Alive: " + Convert.ToString(AliveDead.Next());
                        // could add an if statement to check if all boxes are false
                        // if true, call StopLoop();


                    });
                    Thread.Sleep(speed);
                }
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                // handle cancellation
            }
            if (e.Error != null)
            {
                // handle error
            }

            // completed without cancellation or exception
        }


        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            AliveDead.ButtonValues();
            livesText.Text = "Alive: " + Convert.ToString(AliveDead.Next());
        }



        private void button_Click(object sender, RoutedEventArgs e)
        {


            var button = (Button)sender;

            if (((SolidColorBrush)button.Background).Color == Colors.White)
            {
                //ButtonList.buttons[0].Background = Brushes.ForestGreen;

                button.Background = Brushes.ForestGreen;
                if (shape.Count != 0)
                {

                    int counter = 0;
                    // since Dictionaries don't have traditional indecies i need to iterate through them to get the correct index.
                    foreach (KeyValuePair<Point, bool> keyValuePair in AliveDead.squares)
                    {
                        if (counter == ButtonList.buttons.IndexOf(button))
                        {
                            // this needs to be replaced with a list of points
                            List<Point> allToClick = new List<Point>();
                            foreach (Point point in shape)
                            {
                                allToClick.Add(new Point(keyValuePair.Key.x + point.x, keyValuePair.Key.y + point.y));
                            }

                            foreach (Point buttonPoint in allToClick)
                            {

                                if (AliveDead.squares.ContainsKey(buttonPoint))
                                {
                                    int counter2 = 0;
                                    foreach (KeyValuePair<Point, bool> keyValuePair1 in AliveDead.squares)
                                    {
                                        if (keyValuePair1.Key.x == buttonPoint.x && keyValuePair1.Key.y == buttonPoint.y && counter2 < ButtonList.buttons.Count)
                                        {

                                            if (ButtonList.buttons[counter2].Background != Brushes.ForestGreen)
                                            {
                                                ButtonList.buttons[counter2].Background = Brushes.ForestGreen;
                                                //AliveDead.alive += 1;
                                            }


                                        }
                                        counter2 += 1;
                                    }

                                }
                            }

                            break;
                        }
                        counter += 1;

                    }
                }

            }
            else if (((SolidColorBrush)button.Background).Color == Colors.ForestGreen)
            {
                AliveDead.alive -= 1;
                button.Background = Brushes.White;
                if (shape.Count != 0)
                {

                    int counter = 0;
                    // since Dictionaries don't have traditional indecies i need to iterate through them to get the correct index.
                    foreach (KeyValuePair<Point, bool> keyValuePair in AliveDead.squares)
                    {
                        if (counter == ButtonList.buttons.IndexOf(button))
                        {
                            // this needs to be replaced with a list of points
                            List<Point> allToClick = new List<Point>();
                            foreach (Point point in shape)
                            {
                                allToClick.Add(new Point(keyValuePair.Key.x + point.x, keyValuePair.Key.y + point.y));
                            }

                            foreach (Point buttonPoint in allToClick)
                            {

                                if (AliveDead.squares.ContainsKey(buttonPoint))
                                {
                                    int counter2 = 0;
                                    foreach (KeyValuePair<Point, bool> keyValuePair1 in AliveDead.squares)
                                    {
                                        if (keyValuePair1.Key.x == buttonPoint.x && keyValuePair1.Key.y == buttonPoint.y && counter2 < ButtonList.buttons.Count)
                                        {

                                            if (ButtonList.buttons[counter2].Background != Brushes.White)
                                            {
                                                ButtonList.buttons[counter2].Background = Brushes.White;
                                                //AliveDead.alive += 1;
                                            }


                                        }
                                        counter2 += 1;
                                    }

                                }
                            }

                            break;
                        }
                        counter += 1;

                    }
                }
            }
            nextButton.Focus();



            AliveDead.ButtonValues();

            int countingAlive = 0;
            foreach (KeyValuePair<Point, bool> checking in AliveDead.squares)
            {
                if (checking.Value == true)
                {
                    countingAlive += 1;
                }
            }

            livesText.Text = "Alive: " + countingAlive;

            //MessageBox.Show(AliveDead.squares[ButtonList.buttons.IndexOf(button)].ToString());
            //MessageBox.Show(ButtonList.buttons.IndexOf(button).ToString());
        }

        private void play_Click(object sender, RoutedEventArgs e)
        {
            play.Background = Brushes.ForestGreen;
            StartLoop();
        }

        private void stop_Click()
        {
            var bc = new BrushConverter();
            play.Background = (Brush)bc.ConvertFrom("#FFDDDDDD");
            StopLoop();
        }

        private void stop_Click(object sender, RoutedEventArgs e)
        {
            var bc = new BrushConverter();
            play.Background = (Brush)bc.ConvertFrom("#FFDDDDDD");
            StopLoop();
        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {


            if (slider.Value >= 1)
            {
                string sValue = Convert.ToString(slider.Value);
                sizeText.Text = sValue + " x " + sValue;

                int width = Convert.ToInt32(slider.Value);
                int height = Convert.ToInt32(slider.Value);

                foreach (Button button in ButtonList.buttons)
                {
                    playArea.Children.Remove(button);

                }
                foreach (ColumnDefinition column in ButtonList.columns)
                {
                    playArea.ColumnDefinitions.Remove(column);
                }
                foreach (RowDefinition row in ButtonList.rows)
                {
                    playArea.RowDefinitions.Remove(row);
                }

                AliveDead.CreateDictionary(width, height);
                CreateButtonsAndGrid(width, height);
                //AliveDead.ApplyStatus();
            }



        }

        private void nineSquare_Click(object sender, RoutedEventArgs e)
        {
            //ineSquare.Background = Brushes.ForestGreen;
            nineSquare.Opacity = .5;
            var bc = new BrushConverter();
            //oneSquare.Background = (Brush)bc.ConvertFrom("#FFDDDDDD");
            oneSquare.Opacity = 1;
            //circle.Background = (Brush)bc.ConvertFrom("#FFDDDDDD");
            circle.Opacity = 1;




            shape = new List<Point>
            {
                new Point(-1, -1),
                new Point(0, -1),
                new Point(1, -1),
                new Point(-1, 0),
                new Point(1, 0),
                new Point(-1, 1),
                new Point(0, 1),
                new Point(1, 1),
            };
        }

        private void oneSquare_Click(object sender, RoutedEventArgs e)
        {
            shape = new List<Point>();
            //oneSquare.Background = Brushes.ForestGreen;
            oneSquare.Opacity = .5;
            var bc = new BrushConverter();
            //nineSquare.Background = (Brush)bc.ConvertFrom("#FFDDDDDD");
            nineSquare.Opacity = 1;
            //circle.Background = (Brush)bc.ConvertFrom("#FFDDDDDD");
            circle.Opacity = 1;
        }

        private void circle_Click(object sender, RoutedEventArgs e)
        {
            //circle.Background = Brushes.ForestGreen;
            circle.Opacity = .5;
            var bc = new BrushConverter();
            //nineSquare.Background = (Brush)bc.ConvertFrom("#FFDDDDDD");
            nineSquare.Opacity = 1;
            //oneSquare.Background = (Brush)bc.ConvertFrom("#FFDDDDDD");
            oneSquare.Opacity = 1;

            shape = new List<Point>
            {
                new Point(-1, -1),
                new Point(0, -1),
                new Point(1, -1),
                new Point(-1, 0),
                new Point(1, 0),
                new Point(-1, 1),
                new Point(0, 1),
                new Point(1, 1),
                new Point(-1, -2),
                new Point(0, -2),
                new Point(1, -2),
                new Point(-1, 2),
                new Point(0, 2),
                new Point(1, 2),
                new Point(-2, -1),
                new Point(-2, 0),
                new Point(-2, 1),
                new Point(2, -1),
                new Point(2, 0),
                new Point(2, 1)


            };
        }

        private void Fast_Click(object sender, RoutedEventArgs e)
        {
            fast.Opacity = .5;
            medium.Opacity = 1;
            slow.Opacity = 1;

            speed = 100;

        }

        private void medium_Click(object sender, RoutedEventArgs e)
        {
            fast.Opacity = 1;
            medium.Opacity = .5;
            slow.Opacity = 1;

            speed = 1000;
        }

        private void slow_Click(object sender, RoutedEventArgs e)
        {
            fast.Opacity = 1;
            medium.Opacity = 1;
            slow.Opacity = .5;

            speed = 1500;
        }

        private void underPopDown_Click(object sender, RoutedEventArgs e)
        {
            AliveDead.underPopulated -= 1;
            underPop.Text = "<" + Convert.ToString(AliveDead.underPopulated);
        }

        private void underPopUp_Click(object sender, RoutedEventArgs e)
        {
            AliveDead.underPopulated += 1;
            underPop.Text = "<" + Convert.ToString(AliveDead.underPopulated);
        }

        private void overPopDown_Click(object sender, RoutedEventArgs e)
        {
            AliveDead.overPopulated -= 1;
            overPop.Text = ">" + Convert.ToString(AliveDead.overPopulated);
        }

        private void overPopUp_Click(object sender, RoutedEventArgs e)
        {
            AliveDead.overPopulated += 1;
            overPop.Text = ">" + Convert.ToString(AliveDead.overPopulated);
        }

        private void growthStartDown_Click(object sender, RoutedEventArgs e)
        {
            AliveDead.growthStart -= 1;
            growthS.Text = ">=" + Convert.ToString(AliveDead.growthStart);
        }

        private void growthStartUp_Click(object sender, RoutedEventArgs e)
        {
            AliveDead.growthStart += 1;
            growthS.Text = ">=" + Convert.ToString(AliveDead.growthStart);
        }

        private void growthEndDown_Click(object sender, RoutedEventArgs e)
        {
            AliveDead.growthEnd -= 1;
            growthE.Text = "<=" + Convert.ToString(AliveDead.growthEnd);
        }

        private void growthEndUp_Click(object sender, RoutedEventArgs e)
        {
            AliveDead.growthEnd += 1;
            growthE.Text = "<=" + Convert.ToString(AliveDead.growthEnd);
        }

        private void clear_Click(object sender, RoutedEventArgs e)
        {
            if (slider.Value >= 1)
            {
                string sValue = Convert.ToString(slider.Value);
                sizeText.Text = sValue + " x " + sValue;

                int width = Convert.ToInt32(slider.Value);
                int height = Convert.ToInt32(slider.Value);

                foreach (Button button in ButtonList.buttons)
                {
                    playArea.Children.Remove(button);

                }
                foreach (ColumnDefinition column in ButtonList.columns)
                {
                    playArea.ColumnDefinitions.Remove(column);
                }
                foreach (RowDefinition row in ButtonList.rows)
                {
                    playArea.RowDefinitions.Remove(row);
                }

                AliveDead.CreateDictionary(width, height);
                CreateButtonsAndGrid(width, height);
            }
        }

        public class ButtonList
        {
            static public List<Button> buttons { get; set; }

            //public ButtonList(List<Button> pButtons)
            //{
            //    buttons = pButtons;
            //    AliveDead.run = false;
            //}

            static public List<ColumnDefinition> columns { get; set; }
            static public List<RowDefinition> rows { get; set; }



        }
        public class AliveDead
        {

            static public int underPopulated { get; set; } = 2;
            static public int overPopulated { get; set; } = 3;
            static public int growthStart { get; set; } = 3;
            static public int growthEnd { get; set; } = 3;



            // live with <2 live neighbors dies
            // live with >3 live neighbors dies
            // live with 2 or 3 live neighbors lives
            // if dead, with 3 neighbors becomes alive

            //static private int width = 5;
            //static private int height = 5;

            //static public Dictionary<Point, bool> grid;
            // i think i'm going to need another dictionary to make sure i don't overwrite the current status
            static public List<Point> toCheck = new List<Point>
        {
            new Point(-1, -1),
            new Point(0, -1),
            new Point(1, -1),
            new Point(-1, 0),
            new Point(1, 0),
            new Point(-1, 1),
            new Point(0, 1),
            new Point(1, 1),

        };

            static public Dictionary<Point, bool> squares = new Dictionary<Point, bool>();

            static public int alive { get; set; } = 0;


            //static public async void Continuous()
            //{

            //    while (run)
            //    {
            //        ButtonValues();
            //        Next();
            //        await Task.Delay(1500);
            //    }

            //}


            static public void CreateDictionary(int width, int height)
            {
                squares = new Dictionary<Point, bool>();

                for (int i = 0; i < height; i++)
                {
                    for (int n = 0; n < width; n++)
                    {
                        squares.Add(new Point(n, i), false);
                    }
                }


            }

            static public void ButtonValues()
            {
                Dictionary<Point, bool> forLoop = new Dictionary<Point, bool>();
                int counter = 0;
                foreach (KeyValuePair<Point, bool> bools in squares)
                {
                    if (counter < ButtonList.buttons.Count)
                    {

                        if (((SolidColorBrush)ButtonList.buttons[counter].Background).Color == Colors.White)
                        {
                            forLoop[bools.Key] = false;
                        }
                        else if (((SolidColorBrush)ButtonList.buttons[counter].Background).Color == Colors.ForestGreen)
                        {
                            forLoop[bools.Key] = true;
                        }
                        counter += 1;
                    }
                    else
                    {
                        forLoop[bools.Key] = false;
                    }
                }
                squares = forLoop;
            }

            static public void ApplyStatus()
            {
                int counter = 0;
                foreach (KeyValuePair<Point, bool> keyValuePair in squares)
                {
                    if (counter < ButtonList.buttons.Count)
                    {
                        if (keyValuePair.Value)
                        {
                            ButtonList.buttons[counter].Background = Brushes.ForestGreen;
                        }
                        else
                        {
                            ButtonList.buttons[counter].Background = Brushes.White;
                        }

                    }
                    counter += 1;
                }
            }


            static public bool StatusCheck(int liveNeighbors, bool currentStatus)
            {

                // check if they die
                if (currentStatus == true)
                {
                    if (liveNeighbors < underPopulated || liveNeighbors > overPopulated)
                    {
                        return false;
                    }
                    return true;

                }
                // check if they come alive
                else
                {
                    if (liveNeighbors >= growthStart && liveNeighbors <= growthEnd)
                    {
                        return true;
                    }
                }
                return false;
            }

            static public int NumAliveSurrounding(Point point, Dictionary<Point, bool> grid)
            {
                int liveCounter = 0;
                foreach (Point surrounding in toCheck)
                {
                    if (grid.ContainsKey(new Point(point.x + surrounding.x, point.y + surrounding.y)))
                    {
                        if (grid[new Point(point.x + surrounding.x, point.y + surrounding.y)])
                        {
                            liveCounter += 1;
                        }
                    }
                }
                return liveCounter;
            }

            static public int Next()
            {

                squares = NewStatus(squares);
                int counter = 0;
                alive = 0;
                foreach (KeyValuePair<Point, bool> keyValuePair in squares)
                {
                    if (counter < ButtonList.buttons.Count)
                    {

                        if (keyValuePair.Value == true)
                        {
                            alive += 1;
                            ButtonList.buttons[counter].Background = Brushes.ForestGreen;
                        }
                        else
                        {
                            ButtonList.buttons[counter].Background = Brushes.White;
                        }
                        counter += 1;
                    }
                }
                return alive;

            }

            //public void PrintMap(Dictionary<Point, bool> grid)
            //{
            //    Console.Clear();

            //    string top = " ";
            //    string line = "|"; // will still need to print this out first

            //    for (int i = 0; i < width; i++)
            //    {
            //        top += "_ ";
            //        // line += "_|";
            //    }

            //    for (int i = 0; i < 5; i++)
            //    {
            //        Console.WriteLine();
            //    }

            //    int formatSpacing = (Console.WindowWidth / 2 + top.Length / 2);
            //    top = String.Format("{0," + formatSpacing + "}", top);
            //    Console.WriteLine(top); // this can stay the same


            //    for (int i = 0; i < height; i++)
            //    {
            //        line = "|";
            //        for (int n = 0; n < width; n++)
            //        {
            //            Point loopPoint = new Point(n, i);
            //            if (grid[loopPoint])
            //            {
            //                line += "0|";
            //            }
            //            else
            //            {
            //                line += "_|";
            //            }


            //        }
            //        //Console.ForegroundColor = ConsoleColor.DarkRed;
            //        line = String.Format("{0, " + formatSpacing + "}", line);
            //        Console.WriteLine(line);
            //    }
            //}

            static public Dictionary<Point, bool> NewStatus(Dictionary<Point, bool> grid)
            {
                Dictionary<Point, bool> newGrid = new Dictionary<Point, bool>();
                foreach (KeyValuePair<Point, bool> points in grid)
                {
                    newGrid.Add(points.Key, StatusCheck(NumAliveSurrounding(points.Key, grid), grid[points.Key]));
                }

                grid = newGrid;
                return grid;
            }
        }

        public struct Point
        {
            public int x;
            public int y;
            public Point(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }


    }
}
