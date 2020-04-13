using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Model;

namespace PNEditorReduction
{
    public class PNReductor
    {
        // Markers that determine whether or not to perform a particular type of reduction.
        public bool isFoSP { get; set; }  // Fusion of Series Places. 
        
        public bool isFoST { get; set; }  // Fusion of Series Transitions. 
        
        public bool isFoPP { get; set; }  // Fusion of Parallel Places. 
        
        public bool isFoPT { get; set; }  // Fusion of Parallel Transitions. 
        
        public bool isEoSLP { get; set; } // Elimination of Self-loop Places. 
        
        public bool isEoSLT { get; set; } // Elimination of Self-loop Transitions. 

        public PNReductor(PetriNet pn)
        {
            CurrentPN = pn;
            _prevStates = new Stack<PetriNet>();
        }

        public PetriNet CurrentPN { get; private set; }
        private Stack<PetriNet> _prevStates;

        //-------------------------------------------------------------------------------//
        public (PetriNet, bool) NextStep()
        {
            PetriNet netRes;
            bool res = true;
            _prevStates.Push(CurrentPN.MyClone());

            if (isFoSP)
            {
                (netRes, res) = performFoSP();
                if(res)
                {
                    CurrentPN = netRes;
                    return (CurrentPN, res);
                }
            }
            if (isFoST)
            {
                (netRes, res) = performFoST();
                if (res)
                {
                    CurrentPN = netRes;
                    return (CurrentPN, res);
                }
            }
            if (isFoPP)
            {
                (netRes, res) = performFoPP();
                if (res)
                {
                    CurrentPN = netRes;
                    return (CurrentPN, res);
                }
            }
            if (isFoPT)
            {
                (netRes, res) = performFoPT();
                if (res)
                {
                    CurrentPN = netRes;
                    return (CurrentPN, res);
                }
            }
            if (isEoSLP)
            {
                (netRes, res) = performEoSLP();
                if (res)
                {
                    CurrentPN = netRes;
                    return (CurrentPN, res);
                }
            }
            if (isEoSLT)
            {
                (netRes, res) = performEoSLT();
                if (res)
                {
                    CurrentPN = netRes;
                    return (CurrentPN, res);
                }
            }
            _prevStates.Pop();
            return (CurrentPN, false);
        }
        public (PetriNet, bool) PrevStep()
        {
            if(_prevStates.Count != 0)
            {
                CurrentPN = _prevStates.Pop();
                return (CurrentPN, true);
            }
            return (CurrentPN, false);
        }

        private (PetriNet, bool) performFoSP()
        {
            foreach (var transition in CurrentPN.Transitions)
            {

                if (numOfInputArc(transition) == 1 && numOfOutputArc(transition) == 1 && inputArcs(transition)[0].Weight == outputArcs(transition)[0].Weight &&
                    numOfOutputArc(inputArcs(transition)[0].NodeFrom) == 1)
                {
                    var Afrom = inputArcs(transition)[0];
                    var Pfrom = Afrom.NodeFrom;
                    var Ato = outputArcs(transition)[0];
                    var Pto = Ato.NodeTo;

                    ((Place)Pto).Tokens += ((Place)Pfrom).Tokens;
                    foreach (var arc in inputArcs(Pfrom))
                        arc.NodeTo = Pto;

                    remove(Afrom);
                    remove(Ato);
                    remove(Pfrom);
                    remove(transition);

                    return (CurrentPN, true);
                }
            }
            return (CurrentPN, false);
        } //done
        private (PetriNet, bool) performFoST()
        {
            foreach (var place in CurrentPN.Places)
            {
                if (numOfInputArc(place) == 1 && numOfOutputArc(place) == 1 && inputArcs(place)[0].Weight == outputArcs(place)[0].Weight &&
                    numOfInputArc(outputArcs(place)[0].NodeTo) == 1)
                {

                    var Afrom = inputArcs(place)[0];
                    var Tfrom = Afrom.NodeFrom;

                    var Ato = outputArcs(place)[0];
                    var Tto = Ato.NodeTo;
                    foreach (var arc in outputArcs(Tto))
                        arc.NodeFrom = Tfrom;

                    remove(Afrom);
                    remove(place);
                    remove(Ato);
                    remove(Tto);

                    return (CurrentPN, true);
                }
            }
            return (CurrentPN, false);
        } //done
        private (PetriNet, bool) performFoPP()
        {
            for(int i = 0; i < CurrentPN.Places.Count - 1; ++i)
            {
                var place1 = CurrentPN.Places[i];
                for (int j = i + 1; j < CurrentPN.Places.Count; ++j)
                {
                    var place2 = CurrentPN.Places[j];
                    if(numOfInputArc(place1) == 1 && numOfOutputArc(place1) == 1 &&
                        numOfInputArc(place2) == 1 && numOfOutputArc(place2) == 1)
                    {
                        var Afrom1 = inputArcs(place1)[0];
                        var Afrom2 = inputArcs(place2)[0];
                        var Ato1 = outputArcs(place1)[0];
                        var Ato2 = outputArcs(place2)[0];
                        if(Afrom1.NodeFrom.Id == Afrom2.NodeFrom.Id &&
                            Ato1.NodeTo.Id == Ato2.NodeTo.Id)
                        {
                            Afrom2.Weight += Afrom1.Weight;
                            Ato2.Weight += Ato1.Weight;
                            ((Place)place2).Tokens += ((Place)place1).Tokens;

                            remove(Afrom1);
                            remove(place1);
                            remove(Ato1);

                            return (CurrentPN, true);
                        }
                    }
                }
            }

            return (CurrentPN, false);
        } //done
        private (PetriNet, bool) performFoPT()
        {
            for(int i = 0; i < CurrentPN.Transitions.Count - 1; ++i)
            {
                var transition1 = CurrentPN.Transitions[i];
                
                for(int j = i + 1; j < CurrentPN.Transitions.Count; ++j)
                {
                    var transition2 = CurrentPN.Transitions[j];

                    if(numOfInputArc(transition1) == 1 && numOfOutputArc(transition1) == 1 &&
                        numOfInputArc(transition2) == 1 && numOfOutputArc(transition2) == 1)
                    {
                        var Afrom1 = inputArcs(transition1)[0];
                        var Afrom2 = inputArcs(transition2)[0];
                        var Ato1 = outputArcs(transition1)[0];
                        var Ato2 = outputArcs(transition2)[0];

                        if(Afrom1.NodeFrom.Id == Afrom2.NodeFrom.Id &&
                            Ato1.NodeTo.Id == Ato2.NodeTo.Id)
                        {
                            Ato2.Weight += Ato1.Weight;
                            Afrom2.Weight += Afrom1.Weight;

                            remove(Afrom1);
                            remove(transition1);
                            remove(Ato1);

                            return (CurrentPN, true);
                        }
                    }
                }
            }

            return (CurrentPN, false);
        } //done
        private (PetriNet, bool) performEoSLP()
        {
            for(int i = 0; i < CurrentPN.Places.Count; ++i)
            {
                var place = CurrentPN.Places[i];

                if(numOfInputArc(place) == 1 && numOfOutputArc(place) == 1)
                {
                    var Afrom = inputArcs(place)[0];
                    var Ato = outputArcs(place)[0];

                    if(Afrom.NodeFrom.Id == Ato.NodeTo.Id &&
                        Afrom.Weight >= Ato.Weight && place.Tokens >= Ato.Weight)
                    {
                        remove(Afrom);
                        remove(Ato);
                        remove(place);

                        return (CurrentPN, true);
                    }
                }

            }

            return (CurrentPN, false);
        } //done
        private (PetriNet, bool) performEoSLT()
        {
            for (int i = 0; i < CurrentPN.Transitions.Count; ++i)
            {
                var transition = CurrentPN.Transitions[i];

                if (numOfInputArc(transition) == 1 && numOfOutputArc(transition) == 1)
                {
                    var Afrom = inputArcs(transition)[0];
                    var Ato = outputArcs(transition)[0];

                    if (Afrom.NodeFrom.Id == Ato.NodeTo.Id)
                    {
                        remove(Afrom);
                        remove(Ato);
                        remove(transition);

                        return (CurrentPN, true);
                    }
                }
            }

            return (CurrentPN, false);
        } //done

        private void remove(Arc arc)
        {
            arc.NodeFrom = null;
            arc.NodeTo = null;
            CurrentPN.Arcs.RemoveAll(a => a.Id == arc.Id);
        }
        private void remove(NodeBase node)
        {
            if(node as Transition != null)
                CurrentPN.Transitions.RemoveAll(a => a.Id == node.Id);

            if (node as Place != null)
                CurrentPN.Places.RemoveAll(a => a.Id == node.Id);
        }
        private int numOfInputArc(NodeBase node)
        {
            int i = 0;
            foreach(var arc in CurrentPN.Arcs)
            {
                if (node.Id == arc.NodeTo.Id)
                    ++i;
            }
            return i;
        }
        private int numOfOutputArc(NodeBase node)
        {
            int i = 0;
            foreach (var arc in CurrentPN.Arcs)
            {
                if (node.Id == arc.NodeFrom.Id)
                    ++i;
            }
            return i;
        }
        private List<Arc> inputArcs(NodeBase node)
        {
            List<Arc> res = new List<Arc>();

            foreach (var arc in CurrentPN.Arcs)
            {
                if (node.Id == arc.NodeTo.Id)
                    res.Add(arc);
            }

            return res;
        }
        private List<Arc> outputArcs(NodeBase node)
        {
            List<Arc> res = new List<Arc>();

            foreach (var arc in CurrentPN.Arcs)
            {
                if (node.Id == arc.NodeFrom.Id)
                    res.Add(arc);
            }

            return res;
        }
    }
}
