using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMW.Model
{
    internal class DiametersWell
    {
        public double InnerDiameter {  get; set; }
        public double OuterDiameter { get; set;}

        internal DiametersWell(double innerDiameter, double outerDiameter) 
        { 
            InnerDiameter = innerDiameter;
            OuterDiameter = outerDiameter;
        }
    }
}
