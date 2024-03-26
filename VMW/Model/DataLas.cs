using System;
using System.Collections.Generic;
using System.Linq;

namespace VMW.Model
{
    internal class DataLas
    {
        public List<Information> Version = new();
        public List<Information> Well = new();
        public List<Information> Curve = new();
        public List<Information> Parameter = new();
        public List<Information> Other = new();

        public List<Measurements> Measurements = new();

        #region методы Get
        public void GetMeasurements(out float[] depth, out float[][] measurements)
        {
            string[] nameOfDepthCurve = new[] { "DEPT" };
            string[] nameOfLeverCurve = new[] { "R1", "FING", "RA" };
            
            var measurementsArray = Measurements.ToArray();
            depth = new float[measurementsArray.Length];
            measurements = new float[measurementsArray.Length][];

            if (Curve.IndexOf(Curve.Find(x => x.GetName() == "DEPT.M")) == -1)
                throw new ArgumentException("Не найдена строка с наименованием \"DEPT.M\", необходимо проверить файл и указать наименование указанное в ошибке");
            
            if (Curve.IndexOf(Curve.Find(x => nameOfLeverCurve.Any(c => x.GetName().Contains(c)))) == -1)
                throw new ArgumentException($"Не найдена строка с наименованием \"{nameOfLeverCurve[0]} или {nameOfLeverCurve[1]} или {nameOfLeverCurve[2]}\", необходимо проверить файл и указать наименование указанное в ошибке");

            var indexOfDepthCurve = Curve.IndexOf(Curve.Find(x => nameOfDepthCurve.Any(c => x.GetName().Contains(c))));
            var indexFirstOfLeverCurve = Curve.IndexOf(Curve.Find(x => nameOfLeverCurve.Any(c => x.GetName().Contains(c))));

            for (int i = 0; i < measurementsArray.Length; i++)
            {
                depth[i] = measurementsArray[i]._measurements[Curve.IndexOf(Curve.Find(x => x.GetName() == "DEPT.M"))];
                measurements[i] = new float[GetNumberOfCurvedLevers()];

                for (int j = 0, k = indexFirstOfLeverCurve; j < measurements[i].Length; j++, k++)
                    measurements[i][j] = measurementsArray[i]._measurements[k];
            }
        }

        public int GetNumberOfCurvedLevers()
        {
            string[] nameOfLeverCurve = new[] { "R1", "FING", "RA" };
            var indexFirstOfLeverCurve = Curve.IndexOf(Curve.Find(x => nameOfLeverCurve.Any(c => x.GetName().Contains(c))));

            return Curve.Count - indexFirstOfLeverCurve;
        }

        public string GetValueByName(string name, Information[] informations)
        {
            return informations.First(x => x.Name == name).GetValue();
        }

        public double GetDoubleValueByName(string name, Information[] informations)
        {
            double.TryParse(informations.First(x => x.Name == name).GetValue().Replace('.', ','), out double result);
            return result;
        }
    }
    #endregion

    #region Структуры
    public struct Information
    {
        private readonly string _name;
        private readonly string _value;
        private readonly string _description;

        public string Name => _name; 
        public string Description => _description; 
        public string Value => _value;

        public Information(string name, string description, string value)
        {
            _name = name;
            _description = description;
            _value = value;
        }

        public override string ToString()
        {
            return $"{_name}: {_value} описание: {_description}";
        }

        #region методы Get
        public string GetValue() => _value;

        public string GetDescription() => _description;

        public string GetName() => _name;
        #endregion
    }

    public struct Measurements
    {
        public readonly float[] _measurements;

        public Measurements(List<float> measurement)
        {
            _measurements = new float[measurement.Count];
            measurement.CopyTo(_measurements);
        }

        public override string ToString()
        {
            return $"Глубина: {_measurements[0]}";
        }
    }
    #endregion
}

