namespace AssetsTools
{
	internal static class MemoryPool<T>
	{
		private const int DEFAULT_SIZE = 65535;

		private static byte[] buf;

		public static byte[] GetBuffer(int size)
		{
			if (buf == null)
			{
				buf = new byte[(size > 65535) ? size : 65535];
				return buf;
			}
			if (buf.Length < size)
			{
				if (size < buf.Length * 2)
				{
					buf = new byte[buf.Length * 2];
				}
				else
				{
					buf = new byte[size];
				}
			}
			return buf;
		}
	}
}
