using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Model
{
    /// <summary>
    /// Represents transition in petri net
    /// </summary>
    public class Transition : NodeBase
    {
        /// <summary>
        /// Priority of transition
        /// </summary>
        public int Priority { get; set; }
    }
}
