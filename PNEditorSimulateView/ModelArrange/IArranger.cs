using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PNEditorSimulateView.ModelArrange
{
    interface IArranger
    {
        void ArrangePetriNetModel(List<PetriNetNode> allFigures, IList<VArc> arcs, PNEditorControl control);

        void ArrangeMulticlusterGraph(int numberOfClusters, List<PetriNetNode> figuresInOneCluster,
            List<PetriNetNode> figuresInAllClusters, IList<VArc> arcs);

        void DepthFirstSearch(PetriNetNode figure, int modelNum);
    }
}
