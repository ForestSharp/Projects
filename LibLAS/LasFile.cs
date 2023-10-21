using System.Text.RegularExpressions;

namespace LibLAS
{
    public class LasFile
    {      
        public string FilePath { get; }

        public BlockData[] BlockDataLAS;
        public Measurement[] ASCII_LogInfo;

        internal readonly string[] NameBlocks = new string[5]
        {
            "~version information",
            "~well information",
            "~curve information",
            "~parameter information block",
            "~other information"
        };

        

        public LasFile(string path) => FilePath = path;

        /// <summary>
        /// Чтение файла по заданному пути
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        public void ReadFile()
        {
            if (string.IsNullOrEmpty(FilePath))
                throw new ArgumentException("Выберете файл");

            string FileLAS;

            try
            {
                FileLAS  = File.ReadAllText(FilePath);                
            }
            catch
            {
                throw new DirectoryNotFoundException($"Файл по данному пути {FilePath} не обнаружен");
            }

            if (string.IsNullOrEmpty(FileLAS))
                throw new ArgumentException("Файл пустой");

            FileProcessing(FileLAS);
        }

        /// <summary>
        /// Обрабока считанного файла
        /// </summary>
        /// <param name="file">Считанный файл</param>
        private void FileProcessing(string file)
        {
            SplitTheFileIntoInformationAndMeasurementRecords(file, out string stringInformationData, out string stringLogData);

            CheckTheAvailabilityOfTheNecessaryInformationBlocks(stringInformationData);

            DistributeAndFillInTheInformationBlock(stringInformationData);

            InitializationMeasurements();

            FillingMeasurements(stringLogData);

            FillingInInformationAboutTheRecord();
        }

        /// <summary>
        /// Разделяет считанный файл на блок с информацией и блок с записью кривых
        /// </summary>
        /// <param name="file">Считанный файл</param>
        /// <param name="stringInformationData">Возвращает блок с информацией</param>
        /// <param name="stringLogData">Возвращает блок с записями кривых</param>
        /// <exception cref="ArgumentException">В случае если невозможно разделить файл на блоки</exception>
        private static void SplitTheFileIntoInformationAndMeasurementRecords(string file, out string stringInformationData, out string stringLogData)
        {
            string tempString = file;

            int indexLogData = tempString.ToLower().IndexOf("~ascii log data");

            if (indexLogData == -1)
                throw new ArgumentException("Блок информации с записями кривых не обнаружен");

            stringInformationData = tempString[..indexLogData];

            stringLogData = tempString[(indexLogData + 15)..];

        }

        /// <summary>
        /// Получить определенный блок с информацией
        /// </summary>
        /// <param name="strings">Блоки содержащие информацию</param>
        /// <param name="blockIndex">Индекс необходимого блока</param>
        /// <returns>Возвращает массив необходимого блока с информацией</returns>
        private static InformationWell[] GetBlockInformation(string[] strings, int blockIndex)
        {
            var blockInformation = strings.Skip(blockIndex + 1).SkipWhile(character => character[0] == '#').TakeWhile(character => Check(character, strings[^1])).ToArray();

            InformationWell[] informationWells = new InformationWell[blockInformation.Length];

            for (int i=0; i<blockInformation.Length; i++)
                informationWells[i] = FillInformationWell(blockInformation[i]);

            return informationWells;
        }

        /// <summary>
        /// Обработать информацию
        /// </summary>
        /// <param name="blockInformation">строка с информацией из блока</param>
        /// <returns>Возвращает обработанную информацию</returns>
        private static InformationWell FillInformationWell(string blockInformation)
        {
            blockInformation = blockInformation.Remove(blockInformation.IndexOf(':'));

            string name = blockInformation[..(int)(blockInformation.Length / 2)].Trim();
            string value = blockInformation[(int)(blockInformation.Length / 2)..].Trim();

            InformationWell informationWell = new InformationWell(name, value);
            return informationWell;
        }

        /// <summary>
        /// Проверяет на конец необходимого блока или на окончание массива строк 
        /// </summary>
        /// <param name="strings">Строка для сравнения</param>
        /// <param name="endLine">Строка обозначающая конец массива строк</param>
        /// <returns></returns>
        private static bool Check(string strings, string endLine)
        {
            if (strings == endLine)
                return false;

            if (strings[0] == '~')
                return false;
        
            return true;
        }

        /// <summary>
        /// Проверяет на наличие необходимых блоков с информацией
        /// </summary>
        /// <param name="informationData">блок с информацией</param>
        /// <exception cref="ArgumentException">Исключение если не обнаружен блок с информацией о скважине или блок с наименованием кривых</exception>
        private static void CheckTheAvailabilityOfTheNecessaryInformationBlocks(string informationData)
        {
            if (!informationData.ToLower().Contains("~well information", StringComparison.CurrentCulture))
                throw new ArgumentException("Блок информации о скважине не обнаружен");

            if (!informationData.ToLower().Contains("~curve information", StringComparison.CurrentCulture))
                throw new ArgumentException("Блок информации с наименованием и количеством кривых не обнаружен");
        }

        /// <summary>
        /// Распределить и заполнить блок с информацией
        /// </summary>
        private void DistributeAndFillInTheInformationBlock(string stringInformationData)
        {
            string[] stringsInformationLAS = stringInformationData.Split("\r\n");

            string[] tempLowerStrings = new string[stringsInformationLAS.Length];

            for (int i = 0; i < stringsInformationLAS.Length; i++)
            {
                stringsInformationLAS[i] = stringsInformationLAS[i].Trim();
                tempLowerStrings[i] = stringsInformationLAS[i].ToLower().Trim();
            }

            BlockDataLAS = new BlockData[NameBlocks.Length];

            for (int i = 0; i < NameBlocks.Length; i++)
            {
                BlockDataLAS[i] = new BlockData(NameBlocks[i])
                {
                    Data = GetBlockInformation(stringsInformationLAS, Array.IndexOf(tempLowerStrings, NameBlocks[i]))
                };
            }
        }

        /// <summary>
        /// Заполнение информации о записи
        /// </summary>
        private void FillingInInformationAboutTheRecord()
        {
            if (CheckingTheCorrectnessOfTheDepthRecording())
                ExpandValuesInAnArray();
        }

        /// <summary>
        /// Развернуть значения в массиве
        /// </summary>
        private void ExpandValuesInAnArray()
        {
            foreach (Measurement LogMeasurement in ASCII_LogInfo)
                LogMeasurement.ValueOfMeasurements = LogMeasurement.ValueOfMeasurements.Reverse().ToArray();
        }

        /// <summary>
        /// Проверяет значения первого элемента и последнего элемента в массиве с записями кривых
        /// </summary>
        /// <returns>true, если начальная глубина больше конечной глубины</returns>
        private bool CheckingTheCorrectnessOfTheDepthRecording()
        {
            if (ASCII_LogInfo[0].Name == "DEPT.M")
                if (ASCII_LogInfo[0].ValueOfMeasurements[0] > ASCII_LogInfo[0].ValueOfMeasurements[^1])
                    return true;

            return false;
        }
        
        /// <summary>
        /// Инициализация измерений для последующей записи
        /// </summary>
        private void InitializationMeasurements()
        {
            ASCII_LogInfo = new Measurement[BlockDataLAS[2].Data.Length];

            for (int i = 0; i < ASCII_LogInfo.Length; i++)
                ASCII_LogInfo[i] = new Measurement(BlockDataLAS[2].Data[i].Name);
        }

        /// <summary>
        /// Заполнение данных об измерениях кривых
        /// </summary>
        /// <param name="stringLogData"></param>
        private void FillingMeasurements(string stringLogData)
        {
            var measurements = SplitMeasurements(stringLogData);

            for(int i = 0; i< ASCII_LogInfo.Length; i++)
                ASCII_LogInfo[i].ValueOfMeasurements = new double[measurements.Length/ BlockDataLAS[2].Data.Length];

            for (int i = 0, m = 0, s = 0; i < measurements.Length; i++, m++)
            {
                if (m == BlockDataLAS[2].Data.Length)
                {
                    s++;
                    m = 0;
                }

                if(double.TryParse(measurements[i], out double result))
                    ASCII_LogInfo[m].ValueOfMeasurements[s] = result;
            }
        }

        /// <summary>
        /// Обрабатывает измерения кривых
        /// </summary>
        /// <param name="logData"></param>
        /// <returns>Возвращает массив обработанных измерений для дальнейшего заполнения </returns>
        private static string[] SplitMeasurements(string logData)
        {

            string patternFindingExtraSpaces = @"\s+";
            string characterToReplaceExtraSpaces = " ";

            var FileLAS_split = logData.ToLower().Replace("\r\n", "");

            string measurements = FileLAS_split.Replace('.', ',');

            Regex regex = new(patternFindingExtraSpaces);
            string result = regex.Replace(measurements, characterToReplaceExtraSpaces);

            result = result.Trim();

            var arrayMeasurementsr = result.Split(" ");

            return arrayMeasurementsr;
        }

        /// <summary>
        /// Для хранения наименования кривой и ее измерениях 
        /// </summary>
        public class Measurement
        {
            public readonly string Name;

            public double[] ValueOfMeasurements;

            /// <summary>
            /// Присваивание наименования кривой
            /// </summary>
            /// <param name="name">Наименования кривой</param>
            public Measurement(string name) => Name = name;
        }

        /// <summary>
        ///  Блок для хранения обработанной информацией
        /// </summary>
        public class BlockData
        {
            public readonly string Name;
            public InformationWell[] Data;

            public BlockData(string name) => Name = name;
        }

        public class InformationWell 
        {
            public string Name { get; }
            public string Value { get; }

            public InformationWell(string name, string value)
            {
                Name = name;
                Value = value;
            }
        }
    }
}