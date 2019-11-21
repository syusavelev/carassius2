using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Model
{
    /// <summary>
    /// Represents place in petri net
    /// </summary>
    public class Place : NodeBase
    {
        /// <summary>
        /// Number of tokens in a place
        /// </summary>
        public int Tokens { get; set; }

    }
}
