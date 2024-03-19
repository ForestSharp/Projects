using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;


namespace VMW.Model
{
    internal class LasFile
    {

        /// <summary>
        /// Чтение файла по заданному пути
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        public static DataLas GetDataLas(string path)
        {           
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("Необходимо выбрать файл");

            try
            {
                DataLas dataLas = GetData(File.ReadAllText(path));
                return dataLas;
            }
            catch
            {
                throw new DirectoryNotFoundException($"Файл по данному пути {path} не обнаружен");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file">Считанный файл</param>
        private static DataLas GetData(string file)
        {
            if (string.IsNullOrEmpty(file))
                throw new ArgumentException("Файл пустой");

            DataLas dataLas = new();
            LasProcessing(file, ref dataLas);
            return dataLas;
        }

        private static void LasProcessing(string file, ref DataLas dataLas)
        {
            List<string> informationWell = new(file[..file.LastIndexOf('~')].Split("\r\n"));

            while (informationWell.IndexOf("") != -1)
                informationWell.Remove("");

            string currentBlock = "";

            foreach (string line in informationWell)
            {
                if (line[0] != '#')
                {
                    if (line[0] == '~')
                    {
                        currentBlock = line.Substring(1, line.IndexOf(' ')).Trim().ToUpper();
                        continue;
                    }

                    switch (currentBlock)
                    {
                        case "VERSION":
                            FillingInformation(ref dataLas.Version, line.Trim());
                            break;
                        case "WELL":
                            FillingInformation(ref dataLas.Well, line.Trim());
                            break;
                        case "CURVE":
                            FillingInformation(ref dataLas.Curve, line.Trim());
                            break;
                        case "PARAMETER":
                            FillingInformation(ref dataLas.Parameter, line.Trim());
                            break;
                        case "OTHER":
                            FillingInformation(ref dataLas.Other, line.Trim());
                            break;
                    }
                }      
            }

            List<string> logData = new(Regex.Replace(file[file.LastIndexOf('~')..], "[ ]+", "|").Replace("\r\n", "").Split('|'));

            //Удаляет из начала списка, которые не являются числами
            while (!float.TryParse(logData[0].Replace('.', ','), out _))
                logData.Remove(logData[0]);


            float.TryParse(dataLas.Well.Find(x => x.GetName() == "STRT.M").GetValue().Replace('.', ','), out float valueStart);
            float.TryParse(dataLas.Well.Find(x => x.GetName() == "STOP.M").GetValue().Replace('.', ','), out float valueStop);
            float.TryParse(dataLas.Well.Find(x => x.GetName() == "STEP.M").GetValue().Replace('.', ','), out float valueStep);

            var countStringMeasurement = 0.0;

            if (valueStep < 0)
                valueStep *= -1;

            if (valueStart > valueStop)
                countStringMeasurement = (valueStart - valueStop) / valueStep + 1;
            else
                countStringMeasurement = (valueStop - valueStart) / valueStep + 1;

            List<float> measurement = new();
            for (int i = 0, k = 0; i < countStringMeasurement; i++)
            {
                measurement.Clear();
                for (int j = 0; j < dataLas.Curve.Count; j++, k++)
                {
                    measurement.Add(float.Parse(logData[k].Replace('.', ',')));
                }
                dataLas.Measurements.Add(new(measurement));    
            }
            
        }

        static void FillingInformation(ref List<Information> information, string lineInformation)
        {
            string name = lineInformation[..lineInformation.IndexOf("  ")].Trim();
            string value = lineInformation[name.Length..lineInformation.IndexOf(':')].Trim();
            string description = lineInformation[(lineInformation.IndexOf(':') + 1)..].Trim();
            
            information.Add(new(name, description, value));
        }
    }
}
