namespace AssetsTools
{
	public static class IOLibExtensions
	{
		public static void Align(this UnityBinaryReader reader, int align)
		{
			int num = reader.Position % align;
			if (num != 0)
			{
				reader.Position += align - num;
			}
		}

		public static void Align(this UnityBinaryWriter writer, int align)
		{
			int num = writer.Position % align;
			if (num != 0)
			{
				writer.Position += align - num;
			}
		}

		public static string ReadAlignedString(this UnityBinaryReader reader)
		{
			string result = reader.ReadString(reader.ReadInt());
			reader.Align(4);
			return result;
		}

		public static void WriteAlignedString(this UnityBinaryWriter writer, string str)
		{
			writer.WriteString(str);
			writer.Align(4);
		}
	}
}
