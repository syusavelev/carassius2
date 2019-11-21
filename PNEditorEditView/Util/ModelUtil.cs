using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Model;

namespace PNEditorEditView.Util
{
    class ModelUtil
    {
        public static (List<VPlace>, List<VTransition>, List<VArc>) FromOriginalModel(PetriNet net)
        {
            Dictionary<string, VPlace> places = new Dictionary<string, VPlace>();
            foreach (var netPlace in net.Places)
            {
                var vplace = new VPlace(netPlace.Position.x, netPlace.Position.y, netPlace.Tokens, netPlace.Id)
                {
                    Label = netPlace.Label,
                };
                places[netPlace.Id] = vplace;
                vplace.BindSyncPlace(netPlace);
            }
            VPlace.Counter = places.Count + 1;

            Dictionary<string, VTransition> transitions = new Dictionary<string, VTransition>();
            foreach (var netTransition in net.Transitions)
            {
                var vtransition = new VTransition(netTransition.Position.x, netTransition.Position.y, netTransition.Id)
                {
                    Label = netTransition.Label,
                    Priority = netTransition.Priority
                };
                transitions[netTransition.Id] = vtransition;
                vtransition.BindSyncTransition(netTransition);
            }

            VTransition.Counter = transitions.Count + 1;

            Dictionary<string, PetriNetNode> nodes = new Dictionary<string, PetriNetNode>();
            foreach (var kv in places)
            {
                nodes[kv.Key] = kv.Value;
            }

            foreach (var kv in transitions)
            {
                nodes[kv.Key] = kv.Value;
            }
            List<VArc> arcs = new List<VArc>();
            foreach (var netArc in net.Arcs)
            {
                var varc = new VArc();
                varc.IsDirected = true;
                varc.Id = netArc.Id;
                varc.Weight = netArc.Weight.ToString();
                varc.From = nodes[netArc.NodeFrom.Id];
                varc.To = nodes[netArc.NodeTo.Id];
                varc.From.ThisArcs.Add(varc);
                varc.To.ThisArcs.Add(varc);
                arcs.Add(varc);
                varc.BindSyncArc(netArc);
            }

            return (places.Select(t => t.Value).ToList(), transitions.Select(t => t.Value).ToList(), arcs);
        }
    }
}
