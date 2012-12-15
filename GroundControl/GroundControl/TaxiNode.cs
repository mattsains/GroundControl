using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GroundControl
{
    enum NodeType { Taxiway, Gate, Runway }

    class TaxiNode
    {
        public string id;
        public Vector2 position;
        public NodeType nodeType;
        public bool canHold; //whether this is somewhere a plane can stop, or just a kink in a taxiway or something
        
        public TaxiNode(string id, Vector2 position, NodeType nodeType, bool canHold)
        {
            this.id = id;
            this.nodeType = nodeType;
            this.position = position;
            this.canHold = canHold;
        }
    }
    
}
