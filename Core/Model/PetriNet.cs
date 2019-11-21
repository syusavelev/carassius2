using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Model
{
    /// <summary>
    /// The petri net
    /// </summary>
    public class PetriNet
    {
        /// <summary>
        /// Places in net
        /// </summary>
        public List<Place> Places { get; set; } = new List<Place>();
        /// <summary>
        /// Transitions in net
        /// </summary>
        public List<Transition> Transitions { get; set; } = new List<Transition>();
        /// <summary>
        /// Arcs in net
        /// </summary>
        public List<Arc> Arcs { get; set; } = new List<Arc>();
        /// <summary>
        /// Makes clone of petri net
        /// </summary>
        /// <returns></returns>
        public PetriNet Clone()
        {
            return new PetriNet()
            {
                Places = Places.ToList(),
                Transitions = Transitions.ToList(),
                Arcs = Arcs.ToList()
            };
        }
    }
}
