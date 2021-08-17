using System;
using System.Collections.Generic;

namespace AssetsTools
{
	internal static class TypeExtensions
	{
		private static Dictionary<Type, string> PrimitiveNames = new Dictionary<Type, string>
		{
			{
				typeof(sbyte),
				"sbyte"
			},
			{
				typeof(byte),
				"byte"
			},
			{
				typeof(short),
				"short"
			},
			{
				typeof(ushort),
				"ushort"
			},
			{
				typeof(int),
				"int"
			},
			{
				typeof(uint),
				"uint"
			},
			{
				typeof(long),
				"long"
			},
			{
				typeof(ulong),
				"ulong"
			},
			{
				typeof(bool),
				"bool"
			},
			{
				typeof(string),
				"string"
			}
		};

		public static string GetCSharpName(this Type type)
		{
			if (PrimitiveNames.TryGetValue(type, out var value))
			{
				return value;
			}
			if (type.GenericTypeArguments.Length != 0)
			{
				string text = type.GenericTypeArguments[0].GetCSharpName();
				for (int i = 1; i < type.GenericTypeArguments.Length; i++)
				{
					text = text + ", " + type.GenericTypeArguments[i].GetCSharpName();
				}
				return type.Name.Substring(0, type.Name.LastIndexOf('`')) + "<" + text + ">";
			}
			return type.Name;
		}
	}
}
