using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GraphVisualizationModule
{

    public class Tree
    {
        public LinkedList<Tree> subtrees;
        public Tree parent;
        public bool fire;
        public string StringState;

        private int[] _data;
        public int[] data
        {
            set
            {
                _data = value;
                StringState = string.Join(",", _data);
            }
            get { return _data; }
        }

        public Tree()
        {
            subtrees = new LinkedList<Tree>();
        }
    }
}
