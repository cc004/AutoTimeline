using System;

namespace AssetsTools.Dynamic
{
	public class DynamicAssetArray : IDynamicAssetBase
	{
		private string proto_name;

		internal IDynamicAssetBase[] elems;

		public string TypeName => "Array<" + proto_name + ">";

		public IDynamicAssetBase this[int index]
		{
			get
			{
				return elems[index];
			}
			set
			{
				if (value.TypeName != proto_name)
				{
					throw new ArrayTypeMismatchException("The element type is `" + proto_name + "` but got `" + value.TypeName + "`");
				}
				elems[index] = value;
			}
		}

		internal DynamicAssetArray(int count, string protoname)
		{
			elems = new IDynamicAssetBase[count];
			proto_name = protoname;
		}

		public DynamicAssetArray GetPrototype()
		{
			return new DynamicAssetArray(0, proto_name);
		}

		public DynamicAsset GetElementPrototype()
		{
			return DynamicAsset.PrototypeDic[proto_name];
		}

		public void Resize(int length)
		{
			DynamicAsset[] destinationArray = new DynamicAsset[length];
			Array.Copy(elems, destinationArray, (length > elems.Length) ? length : elems.Length);
			IDynamicAssetBase[] array = (elems = destinationArray);
		}
	}
}
