using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GameOfLife
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static List<GridNode> gridNodes = new List<GridNode>();
        int generationValue = 0;
         int cellCount = 0;
        public event PropertyChangedEventHandler PropertyChanged;
        public int CellCount { get { return cellCount; } set { cellCount = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CellCount")); } }
        bool isGridSet = false;
        public int GenerationValue { get { return generationValue; } set { generationValue = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("GenerationValue")); } }
        private bool runGame = false;
        public MainWindow()
        {
            InitializeComponent();
            Binding b = new Binding(), b2 = new Binding();
            b.Path = new PropertyPath("GenerationValue");
            b.Source = GenerationValue;
            generationLbl.SetBinding(Label.ContentProperty, b);
            b2.Path = new PropertyPath("CellCount");
            b2.Source = CellCount;
            livingCellLbl.SetBinding(Label.ContentProperty, b2);
        }

        private void setGridBttn_Click(object sender, RoutedEventArgs e)
        {
            string lengthText = lengthTxt.Text, widthText = widthTxt.Text;
            try
            {
                int length = int.Parse(lengthText), width = int.Parse(widthText), max = length * width, top = 20, left = 20;
                int leftmove = 0, topMove = 0;
                gridCanvas.Children.Clear();
                gridNodes.Clear();
                for (int i = 0; i < max; i++)
                {
                    GridNode n = new GridNode(length, width, false);
                    if (i != 0)
                    {
                        if (i % width != 0)
                        {
                            leftmove += 10;
                        }
                        else if (i % width == 0)
                        {
                            leftmove = 0;
                            topMove += 10;
                        }
                    }
                    gridCanvas.Children.Add(n.node);
                    Canvas.SetTop(n.node, top + topMove);
                    Canvas.SetLeft(n.node, left + leftmove);
                    gridNodes.Add(n);
                }
                ConnectNodes();
                isGridSet = true;
            }
            catch (Exception) { }
        }

        private void playBttn_Click(object sender, RoutedEventArgs e)
        {
            if (isGridSet)
            {
                runGame = true;
                generationLbl.Content = GenerationValue;
                for (int x = 0; x < gridNodes.Count; x++)
                {
                    if (gridNodes.ElementAt(x).isAlive)
                    {
                        CellCount++;
                        livingCellLbl.Content = CellCount;
                    }
                }
                RunLife();
            }

        }

        private void updateGeneration(object sender = null, ElapsedEventArgs e = null)
        {
            ++GenerationValue;
        }

        private void nextBttn_Click(object sender, RoutedEventArgs e)
        {
            updateGeneration();
            for (int x = 0; x < gridNodes.Count; x++)
            {
                if (gridNodes.ElementAt(x).isAlive)
                {
                    CellCount++;
                    livingCellLbl.Content = CellCount;
                }
            }
            LifeTransitions();
        }

        private void stopBttn_Click(object sender, RoutedEventArgs e)
        {
            runGame = false;
        }

        private void RunLife()
        {
            while (runGame)
            {
                LifeTransitions();
                
            }
        }

        private void LifeTransitions()
        {
            int length = int.Parse(lengthTxt.Text), width = int.Parse(widthTxt.Text), max = length * width, top = 20, left = 20;
            int leftmove = 0, topMove = 0;
            for (int i = 0; i < gridNodes.Count; i++)
            {
                GridNode n = gridNodes.ElementAt(i);
                List<int> neighborsLiving = new List<int>();
                for (int x = 0; x < n.neighbors.Count; x++)
                {
                    if (n.neighbors.ElementAt(x).isAlive)
                    {
                        neighborsLiving.Add(x);
                    }
                }
                bool stillCell = (neighborsLiving.Count == 2 || neighborsLiving.Count == 3) && n.isAlive;
                if (!stillCell)
                {
                    if (!n.isAlive && neighborsLiving.Count == 3)
                    {
                        gridNodes.ElementAt(i).changeNode();
                        
                    }
                    else if (n.isAlive && (neighborsLiving.Count > 3 || neighborsLiving.Count < 2))
                    {
                        gridNodes.ElementAt(i).changeNode();
                    }
                }
            }
            try
            {
                gridCanvas.Children.Clear();
                for (int i = 0; i < max; i++)
                {
                    if (i != 0)
                    {
                        if (i % width != 0)
                        {
                            leftmove += 10;
                        }
                        else if (i % width == 0)
                        {
                            leftmove = 0;
                            topMove += 10;
                        }
                    }
                    if (gridNodes.ElementAt(i).isAlive)
                    {
                        CellCount++;
                        livingCellLbl.Content = CellCount;
                    }
                    gridCanvas.Children.Add(gridNodes.ElementAt(i).node);
                    Canvas.SetTop(gridNodes.ElementAt(i).node, top + topMove);
                    Canvas.SetLeft(gridNodes.ElementAt(i).node, left + leftmove);

                }
                ConnectNodes();
                GenerationValue++;
                generationLbl.Content = GenerationValue;
            }
            catch (Exception) { }
        }

        private void ConnectNodes()
        {
            for (int i = 0; i < gridNodes.Count; i++)
            {
                if (i + 1 < gridNodes.Count)
                {
                    gridNodes.ElementAt(i).neighbors.Add(gridNodes.ElementAt(i + 1));
                }
                if (i - 1 > -1)
                {
                    gridNodes.ElementAt(i).neighbors.Add(gridNodes.ElementAt(i - 1));
                }
                if (i + 9 < gridNodes.Count)
                {
                    gridNodes.ElementAt(i).neighbors.Add(gridNodes.ElementAt(i + 9));
                }
                if (i - 9 > -1)
                {
                    gridNodes.ElementAt(i).neighbors.Add(gridNodes.ElementAt(i - 9));
                }
                if (i + 10 < gridNodes.Count)
                {
                    gridNodes.ElementAt(i).neighbors.Add(gridNodes.ElementAt(i + 10));
                }
                if (i - 10 > -1)
                {
                    gridNodes.ElementAt(i).neighbors.Add(gridNodes.ElementAt(i - 10));
                }
                if (i + 11 < gridNodes.Count)
                {
                    gridNodes.ElementAt(i).neighbors.Add(gridNodes.ElementAt(i + 11));
                }
                if (i - 11 > -1)
                {
                    gridNodes.ElementAt(i).neighbors.Add(gridNodes.ElementAt(i - 11));
                }
            }
        }

        private class GridNode
        {
            public bool isAlive { get => _isAlive; set { _isAlive = value; } }

            public Button node { get => _node; set => _node = value; }
            public List<GridNode> neighbors { get => _neighbors; set => _neighbors = value; }
            bool _isAlive = false;
            Button _node;
            List<GridNode> _neighbors = new List<GridNode>();
            public GridNode(int width, int length, bool isAlive)
            {
                this.isAlive = isAlive;
                setButton(length, width);
            }

            public void setButton(int width, int length)
            {
                SolidColorBrush color = new SolidColorBrush();
                color = Brushes.White;
                if (isAlive)
                {
                    color = Brushes.Red;
                }
                node = new Button()
                {
                    Width = width,
                    Height = length,
                    Background = color,
                };
                node.Click += (s, e) =>
                {
                    changeNode();
                }; ;
            }

            public void changeNode()
            {
                SolidColorBrush color = new SolidColorBrush();
                color = Brushes.White;
                if (!isAlive)
                {
                    color = Brushes.Red;
                    isAlive = true;

                }
                else
                {
                    isAlive = false;
                }
                node.Background = color;
            }
        }


    }

}
