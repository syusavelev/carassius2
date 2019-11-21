
using System.Windows;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.WpfGraphControl;
using System.Collections.Generic;

namespace GraphVisualizationModule
{
    /// <summary>
    /// Логика взаимодействия для GraphVisualization.xaml
    /// </summary>
    public partial class GraphVisualization : Window
    {
        GraphViewer viewer;
        Graph graph;
        Tree _tree;
        HashSet<string> fire_nodes;

        public GraphVisualization()
        {
            InitializeComponent();
        }

        void ProcessTree(Tree tree)
        {
            if (tree.fire)
            {
                fire_nodes.Add(tree.StringState);
            }
            foreach (Tree t in tree.subtrees)
            {
                graph.AddEdge(tree.StringState, t.StringState);
                ProcessTree(t);
            }
        }

        void ColorNodes(Tree tr)
        {
            foreach (Node n in graph.Nodes)
            {
                if (fire_nodes.Contains(n.LabelText))
                {
                    n.Attr.FillColor = Color.Red;
                }
            }
        }

        public Tree tree
        {
            set
            {
                _tree = value;
                Graph g = new Graph();
                graph = g;
                fire_nodes = new HashSet<string>();
                ProcessTree(_tree);
                ColorNodes(_tree);
                viewer.Graph = graph;
            }
            get
            {
                return _tree;
            }
        }



        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Initialized(object sender, System.EventArgs e)
        {
            Graph graph = new Graph();
            viewer = new GraphViewer();
            viewer.BindToPanel(dock);
            graph.Attr.LayerDirection = LayerDirection.TB;
            viewer.Graph = graph;
        }
    }
}
