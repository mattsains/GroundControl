using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GroundControl
{
    class AircraftGroup:List<Aircraft>
    {
        public AircraftGroup() : base() { }
        
        public void Update(float dt)
        {
            this.ForEach(a => a.Update(dt));
        }
        public void Draw()
        {
            this.ForEach(a => a.Draw());
        }
    }
}
