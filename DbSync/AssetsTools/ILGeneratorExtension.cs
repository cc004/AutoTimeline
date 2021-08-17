using System;
using System.Reflection.Emit;

namespace AssetsTools
{
	internal static class ILGeneratorExtension
	{
		public static void EmitLdloc(this ILGenerator il, int i)
		{
			switch (i)
			{
			case 0:
				il.Emit(OpCodes.Ldloc_0);
				return;
			case 1:
				il.Emit(OpCodes.Ldloc_1);
				return;
			case 2:
				il.Emit(OpCodes.Ldloc_2);
				return;
			case 3:
				il.Emit(OpCodes.Ldloc_3);
				return;
			}
			if (i <= 255)
			{
				il.Emit(OpCodes.Ldloc_S, (byte)i);
			}
			else
			{
				il.Emit(OpCodes.Ldloc, i);
			}
		}

		public static void EmitStloc(this ILGenerator il, int i)
		{
			switch (i)
			{
			case 0:
				il.Emit(OpCodes.Stloc_0);
				return;
			case 1:
				il.Emit(OpCodes.Stloc_1);
				return;
			case 2:
				il.Emit(OpCodes.Stloc_2);
				return;
			case 3:
				il.Emit(OpCodes.Stloc_3);
				return;
			}
			if (i <= 255)
			{
				il.Emit(OpCodes.Stloc_S, (byte)i);
			}
			else
			{
				il.Emit(OpCodes.Stloc, i);
			}
		}

		public static void EmitLdloca(this ILGenerator il, int i)
		{
			if (i <= 255)
			{
				il.Emit(OpCodes.Ldloca_S, (byte)i);
			}
			else
			{
				il.Emit(OpCodes.Ldloca, i);
			}
		}

		public static void EmitFor(this ILGenerator il, int i, Func<ILGenerator, OpCode> cond, Action<ILGenerator> block)
		{
			il.Emit(OpCodes.Ldc_I4_0);
			il.EmitStloc(i);
			Label label = il.DefineLabel();
			Label label2 = il.DefineLabel();
			il.MarkLabel(label);
			il.Emit(cond(il), label2);
			block(il);
			il.EmitLdloc(i);
			il.Emit(OpCodes.Ldc_I4_1);
			il.Emit(OpCodes.Add);
			il.EmitStloc(i);
			il.Emit(OpCodes.Br, label);
			il.MarkLabel(label2);
		}
	}
}
