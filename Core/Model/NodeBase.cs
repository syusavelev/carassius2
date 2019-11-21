using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Model
{
    /// <summary>
    /// Base class for nodes in model
    /// </summary>
    public abstract class NodeBase
    {
        /// <summary>
        /// Position of node in document (pixels)
        /// </summary>
        public (double x,double y) Position { get; set; }
        /// <summary>
        /// Textual label of node
        /// </summary>
        public string Label { get; set; }
        /// <summary>
        /// Unique identificator of node
        /// </summary>
        public string Id { get; set; }
    }
}
