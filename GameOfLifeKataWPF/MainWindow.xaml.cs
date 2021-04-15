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

namespace GameOfLifeKataWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        public MainWindow()
        {
            InitializeComponent();

            AliveDead.CreateDictionary();

            List<Button> allButtons = new List<Button>
            {
                button1, button2, button3, button4, button5,
                button6, button7, button8, button9, button10,
                button11, button12, button13, button14, button15,
                button16, button17, button18, button19, button20,
                button21, button22, button23, button24, button25,
            };

            ButtonList buttonList = new ButtonList(allButtons);


        }
        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            AliveDead.ButtonValues();
            AliveDead.Next();
        }


        //private void button1_Click(object sender, RoutedEventArgs e)
        //{

        //}

        //private void button1_Click(object sender, RoutedEventArgs e)
        //{
        //    var button = (Button)sender;

        //}





        private void button_Click(object sender, RoutedEventArgs e)
        {

            var button = (Button)sender;

            if (((SolidColorBrush)button.Background).Color == Colors.White)
            {
                button.Background = Brushes.ForestGreen;
            }
            else if (((SolidColorBrush)button.Background).Color == Colors.ForestGreen)
            {
                button.Background = Brushes.White;
            }
        }





    }

    public class ButtonList
    {
        static public List<Button> buttons { get; set; }

        public ButtonList(List<Button> pButtons)
        {
            buttons = pButtons;
        }
    }
    public class AliveDead
    {
        // live with <2 live neighbors dies
        // live with >3 live neighbors dies
        // live with 2 or 3 live neighbors lives
        // if dead, with 3 neighbors becomes alive

        static private int width = 5;
        static private int height = 5;

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

        static Dictionary<Point, bool> squares = new Dictionary<Point, bool>();


        static public void CreateDictionary()
        {
            

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
            squares = forLoop;
        }

        static public bool StatusCheck(int liveNeighbors, bool currentStatus)
        {
            if (currentStatus == true)
            {
                if (liveNeighbors < 2 || liveNeighbors > 3)
                {
                    return false;
                }
                return true;

            }
            else
            {
                if (liveNeighbors == 3)
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

        static public void Next()
        {
            
            squares = NewStatus(squares);
            int counter = 0;
            foreach (KeyValuePair<Point, bool> keyValuePair in squares)
            {
                if (keyValuePair.Value == true)
                {
                    ButtonList.buttons[counter].Background = Brushes.ForestGreen;
                }
                else
                {
                    ButtonList.buttons[counter].Background = Brushes.White;
                }
                counter += 1;
            }
        }

        public void PrintMap(Dictionary<Point, bool> grid)
        {
            Console.Clear();

            string top = " ";
            string line = "|"; // will still need to print this out first

            for (int i = 0; i < width; i++)
            {
                top += "_ ";
                // line += "_|";
            }

            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine();
            }

            int formatSpacing = (Console.WindowWidth / 2 + top.Length / 2);
            top = String.Format("{0," + formatSpacing + "}", top);
            Console.WriteLine(top); // this can stay the same


            for (int i = 0; i < height; i++)
            {
                line = "|";
                for (int n = 0; n < width; n++)
                {
                    Point loopPoint = new Point(n, i);
                    if (grid[loopPoint])
                    {
                        line += "0|";
                    }
                    else
                    {
                        line += "_|";
                    }


                }
                //Console.ForegroundColor = ConsoleColor.DarkRed;
                line = String.Format("{0, " + formatSpacing + "}", line);
                Console.WriteLine(line);
            }
        }

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
