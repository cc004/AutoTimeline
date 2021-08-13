using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PCRAutoTimeline.Interaction
{
    class UnitAutoData
    {
		private class Unit_auto
		{
			public string unit_id { get; set; }
			public string unit_name { get; set; }
			public string atk_type { get; set; }
			public string atk_cast_time { get; set; }
			public string ub_type { get; set; }
			public string atk_prefab_data { get; set; }
		}

		private class RootObject
		{
			public List<Unit_auto> data_list { get; set; }
		}

		private static RootObject unit_auto_data;

		public static void Init() 
		{
			FileStream fs = new FileStream("UnitAutoData.json", FileMode.Open);
			StreamReader fileStream = new StreamReader(fs);
			string str = "";
			string line;
			while ((line = fileStream.ReadLine()) != null)
			{
				str += line;
			}
			unit_auto_data = JsonConvert.DeserializeObject<RootObject>(str);

		}


		public static int GetUbTypeFromId(long unit_id)
        {
			foreach (var unit_data in unit_auto_data.data_list)
            {
				if (long.Parse(unit_data.unit_id) == unit_id)
                {

					var out_data = Int32.Parse(unit_data.ub_type);
					return out_data;

				}
            }
			return -1;
        }
		public static int GetAtkType(long unit_id)
		{
			foreach (var unit_data in unit_auto_data.data_list)
			{
				if (long.Parse(unit_data.unit_id) == unit_id)
				{

					var out_data = Int32.Parse(unit_data.atk_type);
					return out_data;

				}
			}
			return -1;
		}
		public static int GetAtkPrefabFrame(long unit_id)//如果找不到数据返回-1（可能是有弹道角色）
		{
			foreach (var unit_data in unit_auto_data.data_list)
			{
				if (long.Parse(unit_data.unit_id) == unit_id)
				{

					var out_data = float.Parse(unit_data.atk_prefab_data);
					return (int)Math.Ceiling(60*out_data);

				}
			}
			return -1;
		}

	}
}
