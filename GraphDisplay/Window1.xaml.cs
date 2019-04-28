using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Markup;
using System.Timers;
using System.IO;
using Microsoft.Win32;

namespace GraphDisplay
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        Timer timer = new Timer();
        List<Node> nodes = new List<Node>();
        static Graph graph;

        public class Node 
        {
            Ellipse shape = new Ellipse();
            List<Node> neighbours = new List<Node>();
            //List<double> edgeAngles = new List<double>();
            Vector position = new Vector(0, 0);
            public Vector speed = new Vector(0, 0);
            public readonly double radius;
            //public double angle = 0;
            //public double angularSpeed = 0;
			public bool isDragged = false;
            public static Hashtable shapeToNode = new Hashtable();

            public Node(double radius)
            {
                Node.shapeToNode.Add(this.shape, this);
                this.shape.Height = 2 * radius;
                this.shape.Width = 2 * radius;
                this.radius = radius;
                this.Position = new Vector(0, 0);
                this.shape.Fill = new SolidColorBrush(Colors.Silver);
                this.shape.Stroke = new SolidColorBrush(Colors.Black);
				this.shape.MouseDown += new MouseButtonEventHandler(Node_MouseDown);
				this.shape.MouseUp += new MouseButtonEventHandler(Node_MouseUp);
                this.shape.MouseMove += new MouseEventHandler(Node_MouseMove);                
            }

            public Vector Position
            {
                get
                {
                    return this.position;
                }
                set
                {
                    this.position = value;
                    Canvas.SetLeft(this.shape, position.y - this.radius);
                    Canvas.SetTop(this.shape, position.x - this.radius);
                }
            }

            public List<Node> Neighbours
            {
                get
                {
                    return this.neighbours;
                }
            }

            public Ellipse Shape
            {
                get
                {
                    return this.shape;
                }
            }

            public void Move(Vector force)
            {
                this.speed = this.speed + force;
                this.Position = this.position + this.speed;                
            }
			
            public void Place(Vector position)
            {
                this.speed = new Vector(0, 0);
                this.Position = position;                
            }			

            public void AddNeighbour(Node node)
            {
                if (!this.neighbours.Contains(node))
                {                    
                    this.neighbours.Add(node);
                    //this.edgeAngles.Add(0);
                    //for (int n = 0; n < this.edgeAngles.Count; n++)
                    //{
                    //    this.edgeAngles[n] = 2 * n * Math.PI / this.edgeAngles.Count;
                    //}
                }
            }

            //public Vector GetAttachPoint(Node node)
            //{
            //    int index = this.neighbours.IndexOf(node);
            //    double x = this.position.x + this.radius * Math.Cos(this.angle + this.edgeAngles[index]);
            //    double y = this.position.y + this.radius * Math.Sin(this.angle + this.edgeAngles[index]);
            //    return new Vector(x, y);
            //}

            public static double Distance(Node node1, Node node2)
            {
                return (node1.Position - node2.Position).Length - node1.radius - node2.radius;
            }
        }

        public class Graph
        {
            List<Node> nodes = new List<Node>();
            List<Node> startNodes = new List<Node>();
            List<Node> endNodes = new List<Node>();
            List<Line> lines = new List<Line>();
            public readonly double distance;
            public Vector center = new Vector(100, 100);
			public bool isDragged = false;
            public Canvas canvas;
            

            public List<Node> Nodes
            {
                get
                {
                    return nodes;
                }
            }

            public Graph(List<Node> nodes, double distance)
            {
                this.nodes = nodes;
                this.distance = distance;
                int m = (int) Math.Sqrt(this.nodes.Count);
                for (int n = 0; n < this.nodes.Count; n++)
                {
                    this.nodes[n].Position = new Vector((n / m) * this.distance, (n % m) * this.distance);
                }
            }

            public void Link(int nodeIndex1, int nodeIndex2)
            {
                this.nodes[nodeIndex1].AddNeighbour(this.nodes[nodeIndex2]);
                this.nodes[nodeIndex2].AddNeighbour(this.nodes[nodeIndex1]);
                this.startNodes.Add(this.nodes[nodeIndex1]);
                this.endNodes.Add(this.nodes[nodeIndex2]);
            }

            public void Refresh()
            {                
				if (!this.isDragged)
				{
					Vector massCenter = new Vector(0, 0);
					foreach (Node node in this.nodes)
					{
						massCenter = massCenter + node.Position;
					}
				    massCenter = (1 / (double) this.nodes.Count) * massCenter;

					foreach (Node node in this.nodes)
					{
						node.Position = node.Position - massCenter + this.center;
					}
				}

                for (int n = 0; n < this.startNodes.Count; n++)
                {
                    this.lines[n].Stroke = new SolidColorBrush(Colors.Black);

                    Vector r = this.endNodes[n].Position - this.startNodes[n].Position;
                    r = r.GetNormalized();
                    Vector attachPoint1 = this.startNodes[n].Position + this.startNodes[n].radius * r;
                    Vector attachPoint2 = this.endNodes[n].Position - this.endNodes[n].radius * r;

                    this.lines[n].Y1 = attachPoint1.x;
                    this.lines[n].Y2 = attachPoint2.x;
                    this.lines[n].X1 = attachPoint1.y;
                    this.lines[n].X2 = attachPoint2.y;
                }
            }

            public void Draw(Canvas canvas)
            {
                this.canvas = canvas;
                center.y = canvas.ActualWidth / 2;
                center.x = canvas.ActualHeight / 2;
                for (int n = 0; n < this.startNodes.Count; n++)
                {
                    this.lines.Add(new Line());
                }
                this.Refresh();
                foreach (Node node in this.nodes)
                {
                    canvas.Children.Add(node.Shape);
                }
                foreach (Line line in this.lines)
                {
                    canvas.Children.Add(line);
                }
            }
        }

        public Window1()
        {
            InitializeComponent();
            nodes.Add(new Node(20));
            nodes.Add(new Node(20));
            nodes.Add(new Node(20));
            nodes.Add(new Node(20));
            nodes.Add(new Node(20));
            nodes.Add(new Node(20));
            nodes.Add(new Node(20));
            nodes.Add(new Node(20));
            nodes.Add(new Node(20));
            nodes.Add(new Node(20));
            graph = new Graph(nodes, 80);
            graph.Link(0, 1);
            graph.Link(0, 2);
            graph.Link(1, 3);
            graph.Link(1, 4);
            graph.Link(1, 2);
            graph.Link(5, 2);
            graph.Link(1, 7);
            graph.Link(7, 6);
            graph.Link(8, 2);
            graph.Link(8, 3);
            graph.Link(9, 7);
            graph.Draw(canvas1);
            InitializeTimer();
        }

        void InitializeTimer()
        {
            timer.Interval = 0.01;
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            timer.Start();
        }
		
		public static void Node_MouseDown(object sender, MouseButtonEventArgs e)
		{
			graph.isDragged = true;
			Node node = (Node)Node.shapeToNode[sender];
            node.isDragged = true;
		}
        public static void Node_MouseMove(object sender, MouseEventArgs e)
        {            
            Node node = (Node)Node.shapeToNode[sender];
            if (node.isDragged)
            {
                Vector position = new Vector(0, 0);
                position.x = e.GetPosition(graph.canvas).Y;
                position.y = e.GetPosition(graph.canvas).X;
                node.Place(position);
            }
        }
		
		public static void Node_MouseUp(object sender, MouseButtonEventArgs e)
		{
			graph.isDragged = false;
            Node node = (Node) Node.shapeToNode[sender];
			node.isDragged = false;
		}

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            canvas1.Dispatcher.Invoke(new Action(
            delegate()
            {
                Move();
            }));
        }

        void Move()
        {
            double totalForceMag = 0;
            List<Vector> totalForces = new List<Vector>();
            for (int n = 0; n < graph.Nodes.Count; n++)
            {
                totalForces.Add(new Vector(0, 0));


                Vector force = new Vector(0, 0);
                Vector r = new Vector(0, 0);
                double l = 0;
                foreach (Node neighbour in graph.Nodes[n].Neighbours)
                {

                    r = neighbour.Position - graph.Nodes[n].Position;
                    l = r.Length;
                    r = r.GetNormalized();

                    force = 0.01 * (l - graph.distance) * r;
                    totalForces[n] = totalForces[n] + force;

                }

                foreach (Node node in graph.Nodes)
                {
                    force = new Vector(0, 0);
                    if (node != graph.Nodes[n])
                    {
                        r = node.Position - graph.Nodes[n].Position;
                        l = r.Length;
                        r = r.GetNormalized();

                        force = -0.1 * (1 / Math.Sqrt(l + 1)) * r;

                    }
                    totalForces[n] = totalForces[n] + force;
                }

                totalForces[n] = totalForces[n] - 0.01 * graph.Nodes[n].speed;
            }
            for (int n = 0; n < graph.Nodes.Count; n++)
            {
                if (!graph.Nodes[n].isDragged)
                {
                    totalForceMag = totalForceMag + totalForces[n].Length;
                    graph.Nodes[n].Move(totalForces[n]);
                }
            }
            graph.Refresh();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            graph.center.x = canvas1.ActualHeight / 2;
            graph.center.y = canvas1.ActualWidth / 2;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string xaml = XamlWriter.Save(canvas1);
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.DefaultExt = ".xaml";
            dialog.Filter = "Windows Markup File (.xaml)|*.xaml";
            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                File.WriteAllText(dialog.FileName, xaml);
            }
        }
    }
}
