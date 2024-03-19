using System.Collections.Generic;

namespace VMW.Model
{
    internal class DataModelWell
    {
        List<Vertex> Measurements;

        Vertex[] GetVertices() => Measurements.ToArray();

    }
}
