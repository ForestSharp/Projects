using System.Windows;
using System.Collections.Generic;
using OpenTK.Mathematics;
using System;
using System.Linq;

namespace VMW.Model
{
    internal class Borehole
    {
        
        private float[][] _centeredMeasurements;

        public Vertex[] Vertices { get; private set; }
        public uint[] Indices { get; private set; }

        private float _nullInformation;

        public Borehole(DataLas dataLas, bool centeringOfMeasurement = false)
        {

            float[] _depth;
            float[][] _measurements;

            try
            {
                dataLas.GetMeasurements(out _depth, out _measurements);
                int numberOfCurvedLevers = dataLas.GetNumberOfCurvedLevers();

                //float.TryParse(DataLas.GetValueByName("NULL.", dataLas.Well.ToArray()).Replace('.', ','), out float nullInformation);

                if (centeringOfMeasurement)
                    AlignmentMeasurements(_measurements, numberOfCurvedLevers);

                //ConvertingMeasurementstToCoordinates(numberOfCurvedLevers, _measurements, _depth, nullInformation);
            }
            catch(Exception e) 
            {
                MessageBox.Show(e.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void ConvertingMeasurementstToCoordinates(int numberOfCurvedLevers, float[][] measurements, float[] depth, float nullInformation)
        {

            double measurementAngle = 360 / numberOfCurvedLevers;
            
            List<Vertex> vertex = new();
            List<uint> indexVertices = new();

            for (int i = 0; i < depth.Length; i++)
            {
                for (int j = 0; j < numberOfCurvedLevers; j++)
                {
                    if (nullInformation == measurements[i][j])
                        continue;

                    int angle = (int)(j * measurementAngle);
                    vertex.Add(new Vertex(new Vector3((float)depth[i], (float)(measurements[i][j] * Math.Round(Math.Cos(MathHelper.DegreesToRadians(angle)), 4)), (float)(measurements[i][j] * Math.Round(Math.Sin(MathHelper.DegreesToRadians(angle)), 4))), Color4.DarkGreen,
                        new Vector3((float)depth[i], (float)(measurements[i][j] * Math.Round(Math.Cos(MathHelper.DegreesToRadians(angle)), 4)), (float)(measurements[i][j] * Math.Round(Math.Sin(MathHelper.DegreesToRadians(angle)), 4)))));
                }
            }
            

            uint numberLevers = (uint)numberOfCurvedLevers;

            for (uint i = 0; i < depth.Length - 35; i++)
            {
                for (uint j = 0, x = numberLevers * i, n; j < numberLevers; j++, x++)
                {
                    n = i > 0 ? numberLevers * (i + 1) : numberLevers;

                    uint h = (x + 1);
                    uint k = (x + numberLevers);
                    uint q = (k + 1);

                    if (j == numberLevers - 1)
                    {
                        h = h - numberLevers;
                        q = q - numberLevers;
                    }

                    indexVertices.Add(x);
                    indexVertices.Add(k);
                    indexVertices.Add(q);

                    indexVertices.Add(q);
                    indexVertices.Add(h);
                    indexVertices.Add(x);

                }
            }

            Indices = indexVertices.ToArray();

            Vertices = vertex.ToArray();
        }

        private float FormulaCenteringRadius(float firstNumber, float secondNumber, float thirdNumber) => ((float.Sqrt(float.Pow(secondNumber, 2) + float.Pow(firstNumber, 2))
                                                                                                               * float.Sqrt(float.Pow(thirdNumber, 2) + float.Pow(firstNumber, 2))) / (2 * firstNumber));

        private void AlignmentMeasurements(float[][] measurements, int numberLevers)
        {
            int ratioNumberLevers = numberLevers / 4;

            List<float> alignmentMeasurements = new();

            for (var i = 0; i < measurements.Length; i++)
                for (int j = 0; j < numberLevers; j++)
                    measurements[i][j] = FormulaCenteringRadius(measurements[i][j],
                                                                measurements[i][(j + ratioNumberLevers) % numberLevers],
                                                                measurements[i][(j + 3 * ratioNumberLevers) % numberLevers]);
        }
    }
}
