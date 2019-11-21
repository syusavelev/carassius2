using System.Collections.Generic;
using System.Windows;
using System.Xml;

namespace PNEditorEditView
{
    /// <summary>
    /// Author: Natalia Nikitina
    /// Work with PNML format here
    /// PAIS Lab, 2014
    /// </summary>
    class Pnml
    {
        /// <summary>
        /// Read model from PNML
        /// </summary>
        /// <param name="pnmlDoc">pnml document</param>
        /// <param name="exc">shows whether any mistakes has been detected</param>
        public static List<VArc> OpenPnml(XmlDocument pnmlDoc, out bool exc)
        {
            var returnArcs = new List<VArc>();

            exc = false;
            XmlNode net = null;
            if (pnmlDoc.DocumentElement.FirstChild.Name == "net")
                net = pnmlDoc.DocumentElement.FirstChild;

            if (net == null)
            {
                MessageBox.Show("Node 'net' is missed, check the file");
                exc = true;
                return returnArcs;
            }

            XmlNode page = null;

            for (var i = 0; i < net.ChildNodes.Count; i++)
            {
                if (net.ChildNodes[i].Name == "page")
                    page = net.ChildNodes[i];
            }

            if (page == null)
            {
                MessageBox.Show("Node 'page' is missed, check the file");
                exc = true;
                return returnArcs;
            }

            var petriNetObjects = page.ChildNodes;

            if (petriNetObjects.Count == 0)
            {
                MessageBox.Show("There are no nodes in the net, check the file");
                exc = true;
                return returnArcs;
            }

            foreach (XmlNode figure in petriNetObjects)
            {
                string id = "", name = "";
                int numberOfTokens = 0;
                double x = -1, y = -1;

                if (figure.Name == "place" || figure.Name == "transition")
                {
                    for (var i = 0; i < figure.Attributes.Count; i++)
                    {
                        if (figure.Attributes[i].Name == "id")
                            id = figure.Attributes[i].Value;
                    }

                    var figureNodes = figure.ChildNodes;

                    //todo Какой-то очень стремный кусок кода. Нужно рефакторить

                    for (int i = 0; i < figureNodes.Count; i++)
                        if (figureNodes[i].Name == "name")
                            name = figureNodes[i].FirstChild.InnerText;
                        else if (figureNodes[i].Name == "graphics")
                        {
                            for (int j = 0; j < figureNodes[i].ChildNodes.Count; j++)
                                if (figureNodes[i].ChildNodes[j].Name == "position")
                                    for (int k = 0; k < figureNodes[i].ChildNodes[j].Attributes.Count; k++)
                                        if (figureNodes[i].ChildNodes[j].Attributes[k].Name == "x")
                                        {
                                            if (double.TryParse(figureNodes[i].ChildNodes[j].Attributes[k].Value.Replace('.', ','), out x) == false || x < 0)
                                            {
                                                x = 0;
                                                MessageBox.Show("Node " + id + " has incorrect x-coordinate(it will be assumed as 0)");
                                            }
                                        }
                                        else if (figureNodes[i].ChildNodes[j].Attributes[k].Name == "y")
                                            if (double.TryParse(figureNodes[i].ChildNodes[j].Attributes[k].Value.Replace('.', ','), out y) == false || y < 0)
                                            {
                                                y = 0;
                                                MessageBox.Show("Node " + id + " has incorrect y-coordinate(it will be assumed as 0)");
                                            }
                        }
                        else if (figureNodes[i].Name == "initialMarking")
                            if (int.TryParse(figureNodes[i].FirstChild.InnerText, out numberOfTokens) == false)
                            {
                                numberOfTokens = 0;
                                MessageBox.Show("Place " + id + " has incorrect number of tokens(it will be assumed as 0)");
                            }

                    if (figure.Name == "place")
                    {
                        var place = VPlace.Create(x, y);
                        place.Id = id;
                        place.Label = name;
                        place.NumberOfTokens = numberOfTokens;
                        PNEditorControl.Net.places.Add(place);
                        //SetOfFigures.Figures.Add(place);
                    }
                    else
                    {
                        var transition = VTransition.Create(x, y);
                        transition.Id = id;
                        transition.Label = name;
                        PNEditorControl.Net.transitions.Add(transition);
                        //SetOfFigures.Figures.Add(transition);
                    }
                }
                else if (figure.Name == "arc")
                {
                }
                else
                    MessageBox.Show("The file contains element with foreign name, it will be ignored, check the file");
            }
            if (PNEditorControl.Net.Nodes.Count == 0)
            {
                MessageBox.Show("There are no nodes in the net, check the file");
                exc = true;
                return returnArcs;
            }
            else
            {
                foreach (XmlNode figure in petriNetObjects)
                {
                    if (figure.Name != "arc") continue;

                    PetriNetNode from = null, to = null;
                    string fromId = null, toId = null, id = null;

                    for (var i = 0; i < figure.Attributes.Count; i++)
                    {
                        switch (figure.Attributes[i].Name)
                        {
                            case "source":
                                fromId = figure.Attributes[i].Value;
                                break;
                            case "target":
                                toId = figure.Attributes[i].Value;
                                break;
                            case "id":
                                id = figure.Attributes[i].Value;
                                break;
                        }
                    }

                    var arcWeight = 1;
                    if (figure.FirstChild != null)
                        if (figure.FirstChild.Name == "name")
                            int.TryParse(figure.FirstChild.FirstChild.FirstChild.InnerText, out arcWeight);

                    foreach (var f in PNEditorControl.Net.Nodes)
                        if (f.Id == fromId)
                            from = f;
                        else if (f.Id == toId)
                            to = f;

                    var arc = new VArc(from, to)
                    {
                        Weight = arcWeight.ToString(),
                        Id = id,
                        IsDirected = true
                    };
                    returnArcs.Add(arc);
                }
            }
            return returnArcs;
        }

        /// <summary>
        /// Save model in PNML
        /// </summary>
        /// <param name="fileName">name of the file</param>
        /// 
        public static XmlDocument SavePnml(string fileName, IList<VArc> arcs, bool saveLayout)
        {
            XmlDocument newPnml = new XmlDocument();

            newPnml.LoadXml("<?xml version=\"1.0\" encoding=\"UTF-8\"?> <pnml> </pnml>");
            
            XmlNode netNode = newPnml.CreateElement("net");
            
            XmlAttribute id = newPnml.CreateAttribute("id");
            id.Value = "net1";
            netNode.Attributes.Append(id);

            XmlAttribute type = newPnml.CreateAttribute("type");
            type.Value = "http://www.pnml.org/version-2009/grammar/pnmlcoremodel";
            netNode.Attributes.Append(type);

            newPnml.DocumentElement.AppendChild(netNode);

            XmlNode nameNetNode = newPnml.CreateElement("name");
            XmlNode nameNetText = newPnml.CreateElement("text");
            XmlNode pageNode = newPnml.CreateElement("page");
            
            XmlAttribute pageId = newPnml.CreateAttribute("id");
            pageId.Value = "n0";
            pageNode.Attributes.Append(pageId);
            
            nameNetText.InnerText = fileName;
            PNEditorControl.ShowMainWindowTitleDelegate("Carassius - Petri Net Editor | " + fileName);
            nameNetNode.AppendChild(nameNetText);
            netNode.AppendChild(nameNetNode);

            foreach (PetriNetNode f in PNEditorControl.Net.Nodes)
            {
                XmlNode figure;
                if (f is VPlace)
                    figure = newPnml.CreateElement("place");
                else
                    figure = newPnml.CreateElement("transition");
                
                XmlAttribute figureId = newPnml.CreateAttribute("id");
                figureId.Value = f.Id;
                figure.Attributes.Append(figureId);
                XmlNode figureName = newPnml.CreateElement("name");
                XmlNode figureNameText = newPnml.CreateElement("text");
                if (!string.IsNullOrEmpty(f.Label))
                {
                    figureNameText.InnerText = f.Label;
                    figureName.AppendChild(figureNameText);
                    figure.AppendChild(figureName);
                }
                if(saveLayout){
                    XmlNode figureGraphics = newPnml.CreateElement("graphics");
                    XmlNode figurePosition = newPnml.CreateElement("position");
                    XmlNode figureDimension = newPnml.CreateElement("dimension");
                    XmlAttribute figureX = newPnml.CreateAttribute("x");
                    XmlAttribute figureY = newPnml.CreateAttribute("y");
                    figureX.Value = f.CoordX.ToString().Replace(",", ".");
                    figureY.Value = f.CoordY.ToString().Replace(",", ".");
                    figurePosition.Attributes.Append(figureX);
                    figurePosition.Attributes.Append(figureY);
                    figureGraphics.AppendChild(figurePosition);
                    XmlAttribute sizeX = newPnml.CreateAttribute("x");
                    XmlAttribute sizeY = newPnml.CreateAttribute("y");
                    if (f is VPlace)
                    {
                        sizeX.Value = "30";
                        sizeY.Value = "30";
                    }
                    else
                    {
                        sizeX.Value = "35";
                        sizeY.Value = "35";
                    }
                    figureDimension.Attributes.Append(sizeX);
                    figureDimension.Attributes.Append(sizeY);
                    figureGraphics.AppendChild(figureDimension);
                    figure.AppendChild(figureGraphics);
                }

                if (f is VPlace && (f as VPlace).NumberOfTokens != 0)
                {
                    XmlNode marking = newPnml.CreateElement("initialMarking");
                    XmlNode markingText = newPnml.CreateElement("text");
                    markingText.InnerText = (f as VPlace).NumberOfTokens.ToString();
                    marking.AppendChild(markingText);
                    figure.AppendChild(marking);
                }
                pageNode.AppendChild(figure);
            }

            foreach (var arc in arcs)
            {
                XmlNode arcNode = newPnml.CreateElement("arc");
                var arcId = newPnml.CreateAttribute("id");
                arcId.Value = arc.Id;

                arcNode.Attributes.Append(arcId);
                var arcSource = newPnml.CreateAttribute("source");
                var arcTarget = newPnml.CreateAttribute("target");
                arcSource.Value = arc.From.Id;
                arcTarget.Value = arc.To.Id;
                arcNode.Attributes.Append(arcSource);
                arcNode.Attributes.Append(arcTarget);
                XmlNode arcName = newPnml.CreateElement("name");
                XmlNode arcType = newPnml.CreateElement("arctype");
                XmlNode arcNameText = newPnml.CreateElement("text");
                XmlNode arcTypeText = newPnml.CreateElement("text");
                arcNameText.InnerText = arc.Weight;
                arcTypeText.InnerText = "normal";
                arcName.AppendChild(arcNameText);
                arcType.AppendChild(arcTypeText);
                arcNode.AppendChild(arcName);
                arcNode.AppendChild(arcType);
                pageNode.AppendChild(arcNode);
            }
            netNode.AppendChild(pageNode);
            return newPnml;
        }

    }
}