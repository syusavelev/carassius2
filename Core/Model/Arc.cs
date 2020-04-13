﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Model
{
    /// <summary>
    /// Represents arc in model
    /// </summary>
    public class Arc
    {
        /// <summary>
        /// Unique identificator of element
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Node where arc starts
        /// </summary>
        public NodeBase NodeFrom { get; set; }
        /// <summary>
        /// Node where arc ends
        /// </summary>
        public NodeBase NodeTo { get; set; }
        /// <summary>
        /// Weight of arc
        /// </summary>
        public int Weight { get; set; }

        
        public Arc MyClone()
        {
            Arc temp = new Arc();
            temp.Id = Id;
            temp.NodeFrom = NodeFrom.MyClone();
            temp.NodeTo = NodeTo.MyClone();
            temp.Weight = Weight;
            return temp;
        }
        
        //todo: validation if needed
    }
}
