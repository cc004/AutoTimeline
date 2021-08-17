using System;
using System.Collections.Generic;

namespace AssetsTools.Dynamic
{
	public static class Extensions
	{
		public class ClassNotFoundException : Exception
		{
			public ClassNotFoundException(ClassIDType id)
				: base("Class " + id.ToString() + " is not available in this file.")
			{
			}
		}

		public static DynamicAsset ToDynamicAsset(this AssetsFile.ObjectType obj)
		{
			if (!obj.parent.TryGetTarget(out var target))
			{
				throw new NullReferenceException("parent");
			}
			return DynamicAsset.GetDeserializer(target.Types[obj.TypeID])(new UnityBinaryReader(obj.Data));
		}

		public static void LoadDynamicAsset(this AssetsFile.ObjectType obj, DynamicAsset asset)
		{
			if (!obj.parent.TryGetTarget(out var target))
			{
				throw new NullReferenceException("parent");
			}
			UnityBinaryWriter unityBinaryWriter = new UnityBinaryWriter();
			DynamicAsset.GetSerializer(target.Types[obj.TypeID])(unityBinaryWriter, asset);
			obj.Data = unityBinaryWriter.ToBytes();
		}

		public static IEnumerable<AssetsFile.ObjectType> ObjectsWithClass(this AssetsFile assets, ClassIDType id)
		{
			int typeid;
			for (typeid = 0; typeid < assets.Types.Length && assets.Types[typeid].ClassID != (int)id; typeid++)
			{
			}
			if (typeid == assets.Types.Length)
			{
				throw new ClassNotFoundException(id);
			}
			AssetsFile.ObjectType[] objects = assets.Objects;
			foreach (AssetsFile.ObjectType objectType in objects)
			{
				if (objectType.TypeID == typeid)
				{
					yield return objectType;
				}
			}
		}
	}
}
