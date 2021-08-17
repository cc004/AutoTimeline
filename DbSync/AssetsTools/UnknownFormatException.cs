using System;

namespace AssetsTools
{
	public class UnknownFormatException : Exception
	{
		public UnknownFormatException()
		{
		}

		public UnknownFormatException(string message)
			: base(message)
		{
		}
	}
}
