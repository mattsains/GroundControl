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
        public string GateTag;
        public Vector2 position;
        public NodeType nodeType;
        public bool canHold; //whether this is somewhere a plane can stop, or just a kink in a taxiway or something

        /// <summary>
        /// Dummy constructors that make sure that the right data is given. All the real work happens in Initialize
        /// </summary>
        public TaxiNode(string id, Vector2 position, NodeType nodeType, string GateTag)
        {
            if (nodeType != NodeType.Gate) throw new Exception("Tried to use the wrong initialization for non-Gate nodetype");
            else this._Initialize(id, position, nodeType, true, GateTag);
        }
        public TaxiNode(string id, Vector2 position, NodeType nodeType, bool canHold)
        {
            if (nodeType == NodeType.Gate) throw new Exception("Tried to use the wrong initialization for Gate nodetype");
            else this._Initialize(id, position, nodeType, canHold, "");
        }
        public TaxiNode(string id, Vector2 position, NodeType nodeType, bool canHold, string runwayTag)
        {
            if (nodeType != NodeType.Runway) throw new Exception("I think this is the wrong initialization for runway");
            else this._Initialize(id, position, nodeType, canHold, runwayTag);
        }
        private void _Initialize(string id, Vector2 position, NodeType nodeType, bool canHold, string GateTag)
        {
            this.id = id;
            this.nodeType = nodeType;
            this.position = position;
            if (nodeType != NodeType.Gate && nodeType!=NodeType.Runway)
                this.canHold = canHold;
            else this.GateTag = GateTag;
        }
        public override string ToString()
        {
            return id;
        }
    }
    
}
