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

        public PetriNet MyClone()
        {
            PetriNet temp = new PetriNet();
            temp.Places = new List<Place>();
            temp.Transitions = new List<Transition>();
            temp.Arcs = new List<Arc>();

            for(int i = 0; i < Arcs.Count; ++i)
            {
                temp.Arcs.Add(Arcs[i].MyClone());

                Place p = temp.Arcs[i].NodeFrom as Place;
                Transition t = temp.Arcs[i].NodeFrom as Transition;
                if (p != null)
                {
                    if(temp.Places.FindIndex(a => a.Id == p.Id) == -1)
                        temp.Places.Add(p);
                }
                else
                {
                    if(temp.Transitions.FindIndex(a => a.Id == t.Id) == -1)
                        temp.Transitions.Add(t);
                }


                p = temp.Arcs[i].NodeTo as Place;
                t = temp.Arcs[i].NodeTo as Transition;
                if (p != null)
                {
                    if (temp.Places.FindIndex(a => a.Id == p.Id) == -1)
                        temp.Places.Add(p);
                }
                else
                {
                    if (temp.Transitions.FindIndex(a => a.Id == t.Id) == -1)
                        temp.Transitions.Add(t);
                }
            }

            return temp;
        }
    }
}
