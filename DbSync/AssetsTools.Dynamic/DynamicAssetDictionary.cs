using System.Collections.Generic;

namespace AssetsTools.Dynamic
{
	public class DynamicAssetDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IDynamicAssetBase
	{
		private string key_proto_name;

		private string value_proto_name;

		public string TypeName => "Dictionary<" + key_proto_name + "," + value_proto_name + ">";

		public DynamicAssetDictionary()
		{
			key_proto_name = typeof(TKey).Name;
			value_proto_name = typeof(TValue).Name;
		}

		public DynamicAssetDictionary(int capacity)
			: base(capacity)
		{
			key_proto_name = typeof(TKey).Name;
			value_proto_name = typeof(TValue).Name;
		}

		public DynamicAssetDictionary(Dictionary<TKey, TValue> dic)
			: base((IDictionary<TKey, TValue>)dic)
		{
			key_proto_name = typeof(TKey).Name;
			value_proto_name = typeof(TValue).Name;
		}

		internal DynamicAssetDictionary(int count, string keytype, string valuetype)
		{
			key_proto_name = keytype;
			value_proto_name = valuetype;
		}

		public DynamicAsset GetKeyPrototype()
		{
			return DynamicAsset.PrototypeDic[key_proto_name];
		}

		public DynamicAsset GetValuePrototype()
		{
			return DynamicAsset.PrototypeDic[value_proto_name];
		}

		public DynamicAssetDictionary<TKey, TValue> GetPrototype()
		{
			return new DynamicAssetDictionary<TKey, TValue>(0, key_proto_name, value_proto_name);
		}
	}
}
