namespace AssetsTools
{
	internal static class ISerializableArrayExtensions
	{
		public static void Read<T>(this T[] ary, UnityBinaryReader reader) where T : ISerializable
		{
			for (int i = 0; i < ary.Length; i++)
			{
				ary[i].Read(reader);
			}
		}

		public static void Write<T>(this T[] ary, UnityBinaryWriter writer) where T : ISerializable
		{
			for (int i = 0; i < ary.Length; i++)
			{
				ary[i].Write(writer);
			}
		}
	}
}
