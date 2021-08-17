namespace AssetsTools
{
	internal static class MiniMemoryPool<T>
	{
		private const int DEFAULT_SIZE = 255;

		private static byte[] buf;

		private static byte[] buf2;

		public static byte[] GetBuffer(int size)
		{
			if (buf == null)
			{
				buf = new byte[(size > 255) ? size : 255];
				return buf;
			}
			if (buf.Length < size)
			{
				buf = new byte[size];
			}
			return buf;
		}

		public static byte[] GetBuffer2(int size)
		{
			if (buf == null)
			{
				buf = new byte[(size > 255) ? size : 255];
				return buf;
			}
			if (buf2.Length < size)
			{
				buf2 = new byte[size];
			}
			return buf2;
		}
	}
}
