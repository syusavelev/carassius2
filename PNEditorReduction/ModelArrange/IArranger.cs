using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PNEditorReduction.Model;

namespace PNEditorReduction.ModelArrange
{
    interface IArranger
    {
        void ArrangePetriNetModel(List<PetriNetNode> allFigures, IList<VArc> arcs, PNEditorControl control);

        void ArrangeMulticlusterGraph(int numberOfClusters, List<PetriNetNode> figuresInOneCluster,
            List<PetriNetNode> figuresInAllClusters, IList<VArc> arcs);

        void DepthFirstSearch(PetriNetNode figure, int modelNum);
    }
}
