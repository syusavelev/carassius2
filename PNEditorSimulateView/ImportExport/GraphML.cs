using System;
using System.Windows;
using System.Xml;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace PNEditorSimulateView
{
    /// <summary>
    /// Author: Natalia Nikitina
    /// Work with graphML format here
    /// PAIS Lab, 2014
    /// </summary>
    class GraphML
    {
        public static Dictionary<string, string> keys = new Dictionary<string, string>();

        public static string GetKeyByValueForStrings(string value)
        {
            return (from key in keys where key.Value.Equals(value) select key.Key).FirstOrDefault();
        }

        public static List<VArc> OpenGraphMl(XmlDocument graphmlDoc, out bool exc)
        {
            var returnArcs = new List<VArc>();

            keys.Clear();
            exc = false;
            XmlNode graph = null;
            if (graphmlDoc.DocumentElement.LastChild.Name == "graph")
                graph = graphmlDoc.DocumentElement.LastChild;

            for (var i = 0; i < graphmlDoc.DocumentElement.ChildNodes.Count; i++)
            {
                if (graphmlDoc.DocumentElement.ChildNodes[i].Name != "key") continue;
                string keyId = null, keyValue = null;
                for (var j = 0; j < graphmlDoc.DocumentElement.ChildNodes[i].Attributes.Count; j++)
                {
                    switch (graphmlDoc.DocumentElement.ChildNodes[i].Attributes[j].Name)
                    {
                        case "id":
                            keyId = graphmlDoc.DocumentElement.ChildNodes[i].Attributes[j].Value;
                            break;
                        case "attr.name":
                            keyValue = graphmlDoc.DocumentElement.ChildNodes[i].Attributes[j].Value;
                            break;
                    }
                }
                if (!keys.ContainsKey(keyId)) keys.Add(keyId, keyValue);
            }

            bool isArcsDirectedByDefault = false;
  
            if (graph == null)
            {
                MessageBox.Show("Node 'graph' is missed, check the file");
                exc = true;
                return returnArcs;
            }
            else if (graph.LastChild.Name == "edgedefault")
            {
                if (graph.LastChild.Value == "directed")
                    isArcsDirectedByDefault = true;
            }
            XmlNodeList graphObjects = graph.ChildNodes;

            if (graphObjects.Count == 0)
            {
                MessageBox.Show("There are no nodes in the graph, check the file");
                exc = true;
                return returnArcs;
            }

            foreach (XmlNode node in graphObjects)
            {
                var id = "";

                if (node.Name == "node")
                {
                    for (int i = 0; i < node.Attributes.Count; i++)
                    {
                        if (node.Attributes[i].Name == "id")
                            id = node.Attributes[i].Value;
                    }
                    
                    VPlace place = VPlace.Create(0, 0);
                    place.Id = id;
                    PNEditorControl.Net.places.Add(place);
                    //SetOfFigures.Figures.Add(place);
                }
                else if (node.Name == "edge")
                {
                }
                else
                    MessageBox.Show("The file contains element with foreign name, it will be ignored, check the file");
            }

            if (PNEditorControl.Net.Nodes.Count == 0)
            {
                MessageBox.Show("There are no nodes in the graph, check the file");
                exc = true;
                return returnArcs;
            }
            else
            {
                foreach (XmlNode node in graphObjects)
                {
                    if (node.Name != "edge") continue;
                    PetriNetNode from = null, to = null;
                    string fromId = null, toId = null, id = null, directed = null;

                    for (int i = 0; i < node.Attributes.Count; i++)
                    {
                        switch (node.Attributes[i].Name)
                        {
                            case "source":
                                fromId = node.Attributes[i].Value;
                                break;
                            case "target":
                                toId = node.Attributes[i].Value;
                                break;
                            case "id":
                                id = node.Attributes[i].Value;
                                break;
                            case "directed":
                                directed = node.Attributes[i].Value;
                                break;
                        }
                    }
                    double weight = 0;
                    if (node.FirstChild != null)
                    {
                        var data = node.FirstChild;
                        for (var i = 0; i < data.Attributes.Count; i++)
                        {
                            if (data.Attributes[i].Name != "key") continue;
                            string weightValue;
                            keys.TryGetValue(data.Attributes[i].Value, out weightValue);
                            if (weightValue == "weight")
                            {
                                double.TryParse(data.InnerText.Replace('.',','), out weight);
                            }
                        }
                    }

                    foreach (var figure in PNEditorControl.Net.Nodes)
                        if (figure.Id == fromId)
                            from = figure;
                        else if (figure.Id == toId)
                            to = figure;

                    var arc = new VArc(from, to)
                    {
                        Weight = weight > 1 ? weight.ToString(CultureInfo.InvariantCulture) : "1",
                        Id = id
                    };
                    if (directed != null)
                    {
                        arc.IsDirected = directed == "true";
                    }
                    else arc.IsDirected = isArcsDirectedByDefault;
                        
                    returnArcs.Add(arc);
                }
            }
            return returnArcs;
        }

        public static XmlDocument SaveGraphMl(String fileName, IList<VArc> arcs)
        {
            XmlDocument newGraphML = new XmlDocument();
            newGraphML.LoadXml("<?xml version=\"1.0\" encoding=\"UTF-8\"?> <graphml> </graphml>");
            XmlNode graphML = newGraphML.LastChild;
            XmlAttribute xmlns = newGraphML.CreateAttribute("xmlns");
            xmlns.Value = "http://graphml.graphdrawing.org/xmlns";
            graphML.Attributes.Append(xmlns);
            XmlAttribute xsi = newGraphML.CreateAttribute("xmlns:xsi");
            xsi.Value = "http://www.w3.org/2001/XMLSchema-instance";
            graphML.Attributes.Append(xsi);
            XmlAttribute schemaLocation = newGraphML.CreateAttribute("xsi:schemaLocation");
            schemaLocation.Value = "http://graphml.graphdrawing.org/xmlns/1.0/graphml.xsd";
            graphML.Attributes.Append(schemaLocation);

            XmlNode keyWeightNode = newGraphML.CreateElement("key");
            XmlAttribute keyId = newGraphML.CreateAttribute("id");
            keyId.Value = "d1";
            keyWeightNode.Attributes.Append(keyId);
            XmlAttribute keyFor = newGraphML.CreateAttribute("for");
            keyFor.Value = "edge";
            keyWeightNode.Attributes.Append(keyFor);
            XmlAttribute keyName = newGraphML.CreateAttribute("attr.name");
            keyName.Value = "weight";
            keyWeightNode.Attributes.Append(keyName);
            var keyType = newGraphML.CreateAttribute("attr.type");
            keyType.Value = "double";
            keyWeightNode.Attributes.Append(keyType);
            graphML.AppendChild(keyWeightNode);

            XmlNode graphNode = newGraphML.CreateElement("graph");
            var id = newGraphML.CreateAttribute("id");
            id.Value = fileName;
            graphNode.Attributes.Append(id);

            var isArcsDirectedByDefault = true;
            var first = arcs[0].IsDirected;
            for (var i = 1; i < arcs.Count; i++)
            {
                if (arcs[i].IsDirected != first)
                    isArcsDirectedByDefault = false;
            }
            if (isArcsDirectedByDefault)
            {
                var edgeDefault = newGraphML.CreateAttribute("edgedefault");
                edgeDefault.Value = arcs[0].IsDirected ? "directed" : "undirected";
                graphNode.Attributes.Append(edgeDefault);
            }
            graphML.AppendChild(graphNode);

            foreach (var place in PNEditorControl.Net.places)
            {
                XmlNode node = newGraphML.CreateElement("node");
                var nodeId = newGraphML.CreateAttribute("id");
                nodeId.Value = place.Id;
                node.Attributes.Append(nodeId);
                graphNode.AppendChild(node);
            }


            foreach (var arc in arcs)
            {
                XmlNode edgeNode = newGraphML.CreateElement("edge");
                var edgeId = newGraphML.CreateAttribute("id");
                edgeId.Value = arc.Id;
                edgeNode.Attributes.Append(edgeId);

                if (isArcsDirectedByDefault == false)
                {
                    var isEdgeDirected = newGraphML.CreateAttribute("directed");
                    isEdgeDirected.Value = arc.IsDirected ? "true" : "false";
                    edgeNode.Attributes.Append(isEdgeDirected);
                }

                var edgeSource = newGraphML.CreateAttribute("source");
                var edgeTarget = newGraphML.CreateAttribute("target");
                edgeSource.Value = arc.From.Id;
                edgeTarget.Value = arc.To.Id;
                edgeNode.Attributes.Append(edgeSource);
                edgeNode.Attributes.Append(edgeTarget);
                if (arc.Weight != "1")
                {
                    XmlNode edgeData = newGraphML.CreateElement("data");
                    var key = newGraphML.CreateAttribute("key");
                    key.Value = "d1";
                    edgeData.Attributes.Append(key);
                    edgeData.InnerText = arc.Weight.Replace(',','.');
                    edgeNode.AppendChild(edgeData);
                }
                graphNode.AppendChild(edgeNode);
            }
            newGraphML.AppendChild(graphML);
            return newGraphML;
        }
    }
}
