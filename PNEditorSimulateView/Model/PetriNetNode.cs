using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Shapes;
using Core.Model;

namespace PNEditorSimulateView
{

    public class PetriNetNode : Node
    {
        private NodeBase syncedNode;
        public NodeBase SyncedNode => syncedNode;

        protected void BindSyncBase(NodeBase nb)
        {
            syncedNode = nb;
            Sync();
        }

        public virtual void Sync()
        {
            if(syncedNode == null)
                return;
            syncedNode.Position = (_coordX, _coordY);
            syncedNode.Label = _label;
            syncedNode.Id = _id;
        }

        public double CoordX
        {
            get { return _coordX; }
            set
            {
                _coordX = value;
                Sync();
            }
        }

        public double CoordY
        {
            get { return _coordY; }
            set
            {
                _coordY = value;
                Sync();
            }
        }

        public string Label
        {
            get { return _label; }
            set
            {
                _label = value;
                Sync();
            }
        }

        public string Id
        {
            get { return _id; }
            set
            {
                _id = value;
                Sync();
            }
        }

        public List<VArc> ThisArcs = new List<VArc>();    //todo здесь сделать интерфейс нормальный, инкапсулировать    

        public void ConnectTo(VArc arc)
        {
            if (arc.TimesExistInList(ThisArcs) == 0)
                ThisArcs.Add(arc);
        }

        public bool IsChecked;
        public int ModelNumber;
        public bool IsSelect;

        public int Column; //todo Вот эти штуки тоже невнятные
        public int Row; //todo Здесь надо бы как-то поменять, убрать куда-то
        public double SpaceCoefficient = 1; // это лейаут ... тоже почти представление
        public double XDistance, YDistance;
        public double NetForceX, NetForceY, VelocityX, VelocityY;
        private double _coordX;
        private double _coordY;
        private string _label;
        private string _id;

        protected PetriNetNode(double x, double y, string id)
        {
            CoordX = x;
            CoordY = y;
            Id = id;
            Label = "";
        }

        //TODO Временная мера для рефакторинга. Потом убрать вообще! Фигуры создавать нельзя
        public static PetriNetNode Create()
        {
            return new PetriNetNode(0, 0, "");
        }

        public void DetectIdMatches(List<PetriNetNode> figures)
        {
            if (figures.Count < 1 || Id.Length < 5) return;
            if (InvalidFigureId(Id)) return;

            var numb = 0;
            foreach (var f in figures)
                if (Id == f.Id && f != this)
                {
                    numb++;
                    Id = f.Id + "-" + numb;
                }
        }

        private static bool InvalidFigureId(string id)
        {
            var tmp = id.Substring(0, 4);
            return tmp != "plac" || tmp != "tran" || tmp != "node" || tmp != "stat";
        }
    }

    public class VPlace : PetriNetNode
    {
        private Place syncPlace;
        public Place SyncPlace => syncPlace;
        public bool IsBound => syncPlace != null;
        public void BindSyncPlace(Place place)
        {
            syncPlace = place;
            BindSyncBase(place);
        }

        public override void Sync()
        {
            if (syncPlace == null)
                return;
            syncPlace.Tokens = _numberOfTokens;
            base.Sync();
        }
        public static int Counter { get; set; }

        public int NumberOfTokens
        {
            get { return _numberOfTokens; }
            set
            {
                _numberOfTokens = value;
                Sync();
            }
        }

        public VPlace(double x, double y, int numberOfTokens, string id)
            : base(x, y, id)
        {
            NumberOfTokens = numberOfTokens;
        }
        //todo: remove this counter, ужас
        public static VPlace Create(double x, double y)
        {
            Counter++;
            var idConstructionString = "";
            idConstructionString = "place" + Counter;

            return new VPlace(x, y, 0, idConstructionString);
        }

        public VPlace Copy()
        {
            return new VPlace(CoordX, CoordY, NumberOfTokens, Id);
        }

        public List<Ellipse> TokensList = new List<Ellipse>(); //todo И вот это тоже представление. Необходимо сделать отдельные классы для представления
        public Label NumberOfTokensLabel = new Label(); //TODO очень странная сущность здесь. убрать надо отсюда
        private int _numberOfTokens;
    }

    public class VTransition : PetriNetNode
    {
        private Transition syncTransition;
        public Transition SyncTransition => syncTransition;
        public bool IsBound => syncTransition != null;
        private int _priority;

        public void BindSyncTransition(Transition sync)
        {
            syncTransition = sync;
            syncTransition.Priority = Priority;
            BindSyncBase(sync);
        }

        public override void Sync()
        {
            if(syncTransition == null)
                return;
            syncTransition.Priority = _priority;
            base.Sync();
        }
        public int Priority
        {
            get { return _priority; }
            set
            {
                _priority = value;
                Sync();
            }
        } // added by Khavanskikh Vladimir

        public static int Counter { get; set; }

        public VTransition(double x, double y, string id)
            : base(x, y, id)
        {
            Priority = 0; // added by Khavanskikh Vladimir
        }
        //todo:remove counter
        public static VTransition Create(double x, double y)
        {
            Counter++;
            return new VTransition(x, y, "transition" + Counter);
        }

        public VTransition Copy()
        {
            return new VTransition(CoordX, CoordY, Id);
        }
    }

    public class VArc
    {
        private Arc syncArc;
        public Arc SyncArc => syncArc;
        public bool IsBound => syncArc != null;

        public void BindSyncArc(Arc sync)
        {
            syncArc = sync;
            Sync();
        }

        public void Sync()
        {
            if (syncArc == null)
                return;
            syncArc.Id = Id;
            syncArc.Weight = int.Parse(weight);
        }

        public string Id
        {
            get { return _id; }
            set
            {
                _id = value;
                Sync();
            }
        }

        public static int Counter;

        public PetriNetNode From { get; set; }
        public PetriNetNode To { get; set; }
        public VArc() { }
        public VArc(PetriNetNode first, PetriNetNode second)
        {
            //TODO Здесь очень большая опасность в случае сетей Петри!
            //TODO Можно засунуть неправильную сеть
            //TODO сейчас проверяю при создании сети вообще
            //Debug.Assert(((first is Place) && (second is Place)) || ((first is Transition) && (second is Transition)), "first and second must be of different types");

            From = first;
            To = second;
            Counter++;//какой ужас! в конструкторе!!!
            Id = "arc" + Counter;
            Weight = "1";
        }

        public bool DeleteOrNot;
        public bool IsSelect;
        private string weight;

        public string Weight
        {
            get { return weight; }
            set
            {
                weight = value;
                Sync();
            }
        }
        public Label WeightLabel = null; //todo Это на самом деле view, нужно убрать отсюда
        public bool IsDirected;
        private string _id;

        public void DetectIdMatches(IList<VArc> allArcs)
        {
            if (allArcs.Count < 1 || Id.Length < 3) return;
            if (InvalidArcId(Id)) return;

            var numb = 0;
            foreach (var a in allArcs)
                if (Id == a.Id)
                {
                    numb++;
                    Id = a.Id + "-" + numb;
                }
        }

        public void AddToThisArcsLists() //todo ЗАЧЕм ОНО вообще нужно?!
        {
            if (!From.ThisArcs.Contains(this))
                From.ThisArcs.Add(this);
            if (!To.ThisArcs.Contains(this))
                To.ThisArcs.Add(this);
        }

        public int TimesExistInList(List<VArc> arcs)
        {
            return arcs.Count(arc => Equals(arc));
        }

        private static bool InvalidArcId(string id)
        {
            var tmp = id.Substring(0, 3);
            return tmp != "arc" || tmp != "tra";
        }
    }
}