using System;
using UnitFrameData;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace MonitorDataGenerator
{
    class Program
    {
        static void Main(string[] args)
        {

            DirectoryInfo folder = new DirectoryInfo("Data/unitPrefabDatas");
            var unitList = new List<UnitData>();
            foreach (FileInfo file in folder.GetFiles("*.json"))
            {
                var targetUnit = new UnitData { prefab = int.Parse(file.Name.Substring(5, 6)) };
                try
                {
                    targetUnit.Initialize();
                    unitList.Add(targetUnit);
                }

                catch 
                {
                    Console.WriteLine(targetUnit.prefab);
                    continue;
                }



            }
            ConvertToJson(unitList, "unit_auto_prefab.json");
            Console.WriteLine();
        }


        public static void ConvertToJson(object obj, string savePath)
        {
            string str = JsonConvert.SerializeObject(obj);

            //json格式化
            JsonSerializer jsonSerializer = new JsonSerializer();
            TextReader textReader = new StringReader(str);
            JsonTextReader jsonTextReader = new JsonTextReader(textReader);
            object _object = jsonSerializer.Deserialize(jsonTextReader);
            if (_object != null)
            {
                StringWriter stringWriter = new StringWriter();
                JsonTextWriter jsonWriter = new JsonTextWriter(stringWriter)
                {
                    Formatting = Formatting.Indented,
                    Indentation = 4,
                    IndentChar = ' '
                };
                jsonSerializer.Serialize(jsonWriter, _object);
                File.WriteAllText(savePath, stringWriter.ToString());
            }

        }
    }
}