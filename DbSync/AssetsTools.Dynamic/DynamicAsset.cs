using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace AssetsTools.Dynamic
{
	public class DynamicAsset : DynamicObject, IDynamicAssetBase
	{
		private struct NodeTree
		{
			public string Name;

			public string Type;

			public bool IsAligned;

			public List<NodeTree> Children;

			public bool HasChildren => Children != null;

			public static NodeTree FromNodes(TypeTree.Node[] nodes)
			{
				int i = 0;
				return readNodes(nodes, ref i);
			}

			private static NodeTree readNodes(TypeTree.Node[] nodes, ref int i)
			{
				NodeTree result = default(NodeTree);
				result.Name = nodes[i].Name;
				result.Type = nodes[i].Type;
				result.IsAligned = (nodes[i].MetaFlag & 0x4000) != 0;
				result.Children = null;
				if (i < nodes.Length - 1)
				{
					if (nodes[i].Level < nodes[i + 1].Level)
					{
						List<NodeTree> list = (result.Children = new List<NodeTree>());
						i++;
						byte level = nodes[i].Level;
						while (i < nodes.Length && nodes[i].Level >= level)
						{
							list.Add(readNodes(nodes, ref i));
						}
					}
					else
					{
						i++;
					}
				}
				else
				{
					i++;
				}
				return result;
			}
		}

		private class LocalManager
		{
			private ILGenerator il;

			private Dictionary<Type, List<int>> local_table = new Dictionary<Type, List<int>>();

			private int local_count;

			private int ret_local = -1;

			public LocalManager(ILGenerator il)
			{
				this.il = il;
			}

			public int AllocLocal(Type type)
			{
				if (!local_table.TryGetValue(type, out var value))
				{
					value = new List<int>(16);
					local_table[type] = value;
					value.Add(0);
				}
				int num = value[0];
				if (num < value.Count - 1)
				{
					value[0] = num + 1;
					return value[num + 1];
				}
				il.DeclareLocal(type);
				value.Add(local_count);
				value[0] = num + 1;
				return local_count++;
			}

			public void ReleaseLocal(Type type)
			{
				local_table[type][0]--;
			}

			public void ReturnLocal(Type type)
			{
				List<int> list = local_table[type];
				ret_local = list[list[0]];
				list[0]--;
			}

			public int GetRetLocal()
			{
				return ret_local;
			}
		}

		private class ProtoNameManager
		{
			private List<string> types = new List<string>(32);

			public void PushType(string name)
			{
				types.Add(name);
			}

			public void PopType()
			{
				types.RemoveAt(types.Count - 1);
			}

			public string GetFQN(string name)
			{
				return types.Aggregate((string a, string b) => a + "." + b) + "." + name;
			}

			public string GetFQN()
			{
				return types.Aggregate((string a, string b) => a + "." + b);
			}
		}

		public class TypeMismatchException : Exception
		{
			public TypeMismatchException()
			{
			}

			public TypeMismatchException(string message)
				: base(message)
			{
			}
		}

		private class DeserializerBuilder
		{
			private ILGenerator il;

			private NodeTree root;

			private LocalManager locman;

			private ProtoNameManager protoman;

			public DeserializerBuilder(TypeTree.Node[] nodes)
			{
				il = null;
				root = NodeTree.FromNodes(nodes);
			}

			public void Build(ILGenerator il)
			{
				this.il = il;
				locman = new LocalManager(il);
				protoman = new ProtoNameManager();
				GenReadObject(root);
				il.Emit(OpCodes.Ret);
			}

			private object GenReadObject(NodeTree node)
			{
				protoman.PushType(node.Type);
				string fQN = protoman.GetFQN();
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				il.Emit(OpCodes.Ldc_I4, node.Children.Count);
				il.Emit(OpCodes.Newobj, DicStrObjCtor);
				List<NodeTree> children = node.Children;
				for (int i = 0; i < children.Count; i++)
				{
					string text = PrettifyName(children[i].Name);
					il.Emit(OpCodes.Dup);
					il.Emit(OpCodes.Ldstr, text);
					dictionary.Add(text, GenReadUnknownType(children[i], requireBoxing: true));
					il.Emit(OpCodes.Callvirt, DicStrObjAdd);
				}
				il.Emit(OpCodes.Ldstr, fQN);
				il.Emit(OpCodes.Newobj, DynamicAssetCtor);
				protoman.PopType();
				DynamicAsset dynamicAsset = new DynamicAsset(dictionary, fQN);
				PrototypeDic[fQN] = dynamicAsset;
				return dynamicAsset;
			}

			private object GenReadUnknownType(NodeTree node, bool requireBoxing)
			{
				if (!TryGenKnownType(node, requireBoxing, out var prototype))
				{
					if (!(node.Type == "TypelessData"))
					{
						prototype = ((node.Type == "map") ? GenReadDic(node) : ((!node.HasChildren || !(node.Children[0].Type == "Array")) ? GenReadObject(node) : GenReadArray(node.Children[0])));
					}
					else
					{
						MethodInfo meth = ReadValueArray.MakeGenericMethod(typeof(byte));
						il.Emit(OpCodes.Ldarg_0);
						il.Emit(OpCodes.Call, meth);
						prototype = new byte[0];
					}
				}
				if (node.IsAligned)
				{
					GenAlign();
				}
				return prototype;
			}

			private bool TryGenKnownType(NodeTree node, bool requireBoxing, out object prototype)
			{
				if (PrimitiveTypeDic.TryGetValue(node.Type, out var value))
				{
					il.Emit(OpCodes.Ldarg_0);
					il.Emit(OpCodes.Call, PrimitiveReaderDic[value]);
					if (requireBoxing)
					{
						il.Emit(OpCodes.Box, value);
					}
					prototype = Activator.CreateInstance(value);
					return true;
				}
				if (node.Type == "string")
				{
					GenReadString();
					prototype = "";
					return true;
				}
				prototype = null;
				return false;
			}

			private void GenReadString()
			{
				il.Emit(OpCodes.Ldarg_0);
				il.Emit(OpCodes.Call, ReadAlignedString);
			}

			private object GenReadArray(NodeTree node)
			{
				NodeTree elem = node.Children[1];
				object result;
				if (PrimitiveTypeDic.TryGetValue(elem.Type, out var value))
				{
					MethodInfo meth = ReadValueArray.MakeGenericMethod(value);
					il.Emit(OpCodes.Ldarg_0);
					il.Emit(OpCodes.Call, meth);
					result = Activator.CreateInstance(value.MakeArrayType(), 0);
				}
				else if (elem.Type == "string")
				{
					il.Emit(OpCodes.Ldarg_0);
					il.Emit(OpCodes.Call, ReadInt);
					il.Emit(OpCodes.Newarr, typeof(string));
					int j = locman.AllocLocal(typeof(int));
					il.EmitFor(j, delegate
					{
						il.Emit(OpCodes.Dup);
						il.Emit(OpCodes.Ldlen);
						il.Emit(OpCodes.Conv_I4);
						il.EmitLdloc(j);
						return OpCodes.Ble_S;
					}, delegate
					{
						il.Emit(OpCodes.Dup);
						il.EmitLdloc(j);
						GenReadString();
						il.Emit(OpCodes.Stelem, typeof(string));
					});
					locman.ReleaseLocal(typeof(int));
					result = new string[0];
				}
				else
				{
					if (elem.Type == "map")
					{
						throw new NotImplementedException("Array of map is not supported");
					}
					string fQN = protoman.GetFQN(elem.Type);
					il.Emit(OpCodes.Ldarg_0);
					il.Emit(OpCodes.Call, ReadInt);
					il.Emit(OpCodes.Ldstr, fQN);
					il.Emit(OpCodes.Newobj, DynamicAssetArrayCtor);
					il.Emit(OpCodes.Dup);
					il.Emit(OpCodes.Ldfld, DynamicAssetArrayelems);
					int i = locman.AllocLocal(typeof(int));
					il.EmitFor(i, delegate
					{
						il.Emit(OpCodes.Dup);
						il.Emit(OpCodes.Ldlen);
						il.Emit(OpCodes.Conv_I4);
						il.EmitLdloc(i);
						return OpCodes.Ble;
					}, delegate
					{
						il.Emit(OpCodes.Dup);
						il.EmitLdloc(i);
						GenReadUnknownType(elem, requireBoxing: false);
						il.Emit(OpCodes.Stelem_Ref);
					});
					il.Emit(OpCodes.Pop);
					result = new DynamicAssetArray(0, fQN);
				}
				if (node.IsAligned)
				{
					GenAlign();
				}
				return result;
			}

			private object GenReadDic(NodeTree node)
			{
				NodeTree pair = node.Children[0].Children[1];
				if (!PrimitiveTypeDic.TryGetValue(pair.Children[0].Type, out var value))
				{
					value = ((!(pair.Children[0].Type == "string")) ? typeof(object) : typeof(string));
				}
				if (!PrimitiveTypeDic.TryGetValue(pair.Children[1].Type, out var value2))
				{
					value2 = ((!(pair.Children[1].Type == "string")) ? typeof(object) : typeof(string));
				}
				string text = ((value == typeof(object)) ? protoman.GetFQN(pair.Children[0].Type) : value.GetCSharpName());
				string text2 = ((value2 == typeof(object)) ? protoman.GetFQN(pair.Children[1].Type) : value2.GetCSharpName());
				int cnt = locman.AllocLocal(typeof(int));
				il.Emit(OpCodes.Ldarg_0);
				il.Emit(OpCodes.Call, ReadInt);
				il.EmitStloc(cnt);
				Type type = typeof(DynamicAssetDictionary<, >).MakeGenericType(value, value2);
				MethodInfo add = type.GetMethod("Add", new Type[2]
				{
					value,
					value2
				});
				il.EmitLdloc(cnt);
				il.Emit(OpCodes.Ldstr, text);
				il.Emit(OpCodes.Ldstr, text2);
				il.Emit(OpCodes.Newobj, type.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod, null, new Type[3]
				{
					typeof(int),
					typeof(string),
					typeof(string)
				}, null));
				int i = locman.AllocLocal(typeof(int));
				il.EmitFor(i, delegate
				{
					il.EmitLdloc(i);
					il.EmitLdloc(cnt);
					return OpCodes.Bge;
				}, delegate
				{
					il.Emit(OpCodes.Dup);
					GenReadUnknownType(pair.Children[0], requireBoxing: false);
					GenReadUnknownType(pair.Children[1], requireBoxing: false);
					il.Emit(OpCodes.Callvirt, add);
				});
				locman.ReleaseLocal(typeof(int));
				locman.ReleaseLocal(typeof(int));
				if (node.Children[0].IsAligned)
				{
					il.Emit(OpCodes.Ldarg_0);
					il.Emit(OpCodes.Ldc_I4_4);
					il.Emit(OpCodes.Call, AlignReader);
				}
				return Activator.CreateInstance(type, BindingFlags.Instance | BindingFlags.NonPublic, null, new object[3]
				{
					0,
					text,
					text2
				}, null);
			}

			private void GenAlign()
			{
				il.Emit(OpCodes.Ldarg_0);
				il.Emit(OpCodes.Ldc_I4_4);
				il.Emit(OpCodes.Call, AlignReader);
			}
		}

		private class SerializerBuilder
		{
			private ILGenerator il;

			private NodeTree root;

			private LocalManager locman;

			private ProtoNameManager protoman;

			public SerializerBuilder(TypeTree.Node[] nodes)
			{
				il = null;
				root = NodeTree.FromNodes(nodes);
			}

			public void Build(ILGenerator il)
			{
				this.il = il;
				locman = new LocalManager(il);
				protoman = new ProtoNameManager();
				il.Emit(OpCodes.Ldarg_1);
				GenWriteObject(root);
				il.Emit(OpCodes.Ret);
			}

			private void GenWriteObject(NodeTree node)
			{
				List<NodeTree> children = node.Children;
				int obj = locman.AllocLocal(DicStrObjType);
				il.Emit(OpCodes.Castclass, typeof(DynamicAsset));
				il.Emit(OpCodes.Ldfld, DynamicAsset_objects);
				il.EmitStloc(obj);
				for (int i = 0; i < children.Count; i++)
				{
					string membername = PrettifyName(children[i].Name);
					GenWriteUnknownType(children[i], requireUnboxing: true, delegate(ILGenerator cil)
					{
						cil.EmitLdloc(obj);
						cil.Emit(OpCodes.Ldstr, membername);
						cil.Emit(OpCodes.Callvirt, DicStrObjGetter);
					});
				}
			}

			private void GenWriteUnknownType(NodeTree node, bool requireUnboxing, Action<ILGenerator> loader)
			{
				if (!TryGenKnownType(node, requireUnboxing, loader))
				{
					if (node.Type == "TypelessData")
					{
						MethodInfo meth = WriteValueArray.MakeGenericMethod(typeof(byte));
						il.Emit(OpCodes.Ldarg_0);
						loader(il);
						il.Emit(OpCodes.Castclass, typeof(byte).MakeArrayType());
						il.Emit(OpCodes.Call, meth);
					}
					else if (node.Type == "map")
					{
						GenWriteDic(node, loader);
					}
					else if (node.HasChildren && node.Children[0].Type == "Array")
					{
						GenWriteArray(node.Children[0], loader);
					}
					else
					{
						loader(il);
						GenWriteObject(node);
					}
				}
				if (node.IsAligned)
				{
					GenAlign();
				}
			}

			private bool TryGenKnownType(NodeTree node, bool requireUnboxing, Action<ILGenerator> loader)
			{
				if (PrimitiveTypeDic.TryGetValue(node.Type, out var value))
				{
					il.Emit(OpCodes.Ldarg_0);
					loader(il);
					if (requireUnboxing)
					{
						il.Emit(OpCodes.Unbox_Any, value);
					}
					il.Emit(OpCodes.Call, PrimitiveWriterDic[value]);
					return true;
				}
				if (node.Type == "string")
				{
					il.Emit(OpCodes.Ldarg_0);
					loader(il);
					if (requireUnboxing)
					{
						il.Emit(OpCodes.Castclass, typeof(string));
					}
					il.Emit(OpCodes.Call, WriteAlignedString);
					return true;
				}
				return false;
			}

			private void GenWriteArray(NodeTree node, Action<ILGenerator> loader)
			{
				NodeTree elem = node.Children[1];
				if (PrimitiveTypeDic.TryGetValue(elem.Type, out var value))
				{
					MethodInfo meth = WriteValueArray.MakeGenericMethod(value);
					il.Emit(OpCodes.Ldarg_0);
					loader(il);
					il.Emit(OpCodes.Castclass, value.MakeArrayType());
					il.Emit(OpCodes.Call, meth);
				}
				else if (elem.Type == "string")
				{
					Type type = typeof(string).MakeArrayType();
					int ary2 = locman.AllocLocal(type);
					loader(il);
					il.Emit(OpCodes.Castclass, type);
					il.EmitStloc(ary2);
					il.Emit(OpCodes.Ldarg_0);
					il.EmitLdloc(ary2);
					il.Emit(OpCodes.Ldlen);
					il.Emit(OpCodes.Conv_I4);
					il.Emit(OpCodes.Call, WriteInt);
					int j = locman.AllocLocal(typeof(int));
					il.EmitFor(j, delegate
					{
						il.EmitLdloc(ary2);
						il.Emit(OpCodes.Ldlen);
						il.Emit(OpCodes.Conv_I4);
						il.EmitLdloc(j);
						return OpCodes.Ble_S;
					}, delegate
					{
						il.Emit(OpCodes.Ldarg_0);
						il.EmitLdloc(ary2);
						il.EmitLdloc(j);
						il.Emit(OpCodes.Ldelem_Ref);
						il.Emit(OpCodes.Call, WriteAlignedString);
					});
					locman.ReleaseLocal(typeof(int));
					locman.ReleaseLocal(type);
				}
				else
				{
					if (elem.Type == "map")
					{
						throw new NotImplementedException("Array of map is not supported");
					}
					Type type2 = typeof(IDynamicAssetBase).MakeArrayType();
					int ary = locman.AllocLocal(type2);
					loader(il);
					il.Emit(OpCodes.Castclass, typeof(DynamicAssetArray));
					il.Emit(OpCodes.Ldfld, DynamicAssetArrayelems);
					il.EmitStloc(ary);
					il.Emit(OpCodes.Ldarg_0);
					il.EmitLdloc(ary);
					il.Emit(OpCodes.Ldlen);
					il.Emit(OpCodes.Conv_I4);
					il.Emit(OpCodes.Call, WriteInt);
					int i = locman.AllocLocal(typeof(int));
					il.EmitFor(i, delegate
					{
						il.EmitLdloc(ary);
						il.Emit(OpCodes.Ldlen);
						il.Emit(OpCodes.Conv_I4);
						il.EmitLdloc(i);
						return OpCodes.Ble;
					}, delegate
					{
						GenWriteUnknownType(elem, requireUnboxing: false, delegate(ILGenerator ccil)
						{
							ccil.EmitLdloc(ary);
							ccil.EmitLdloc(i);
							ccil.Emit(OpCodes.Ldelem_Ref);
						});
					});
					locman.ReleaseLocal(typeof(int));
					locman.ReleaseLocal(type2);
				}
				if (node.IsAligned)
				{
					GenAlign();
				}
			}

			private void GenWriteDic(NodeTree node, Action<ILGenerator> loader)
			{
				NodeTree nodeTree = node.Children[0].Children[1];
				if (!PrimitiveTypeDic.TryGetValue(nodeTree.Children[0].Type, out var value))
				{
					value = ((!(nodeTree.Children[0].Type == "string")) ? typeof(object) : typeof(string));
				}
				if (!PrimitiveTypeDic.TryGetValue(nodeTree.Children[1].Type, out var value2))
				{
					value2 = ((!(nodeTree.Children[1].Type == "string")) ? typeof(object) : typeof(string));
				}
				Type type = typeof(DynamicAssetDictionary<, >).MakeGenericType(value, value2);
				type.GetMethod("Item", new Type[2]
				{
					value,
					value2
				});
				MethodInfo getMethod = type.GetProperty("Count")?.GetMethod;
				int i = locman.AllocLocal(type);
				loader(il);
				il.Emit(OpCodes.Castclass, type);
				il.EmitStloc(i);
				il.Emit(OpCodes.Ldarg_0);
				il.EmitLdloc(i);
				il.Emit(OpCodes.Callvirt, getMethod);
				il.Emit(OpCodes.Call, WriteInt);
				Type type2 = typeof(Dictionary<, >.Enumerator).MakeGenericType(value, value2);
				int i2 = locman.AllocLocal(type2);
				Type type3 = typeof(KeyValuePair<, >).MakeGenericType(value, value2);
				int kv = locman.AllocLocal(type3);
				MethodInfo method = type.GetMethod("GetEnumerator");
				MethodInfo method2 = type2.GetMethod("MoveNext");
				MethodInfo getMethod2 = type2.GetProperty("Current")?.GetMethod;
				MethodInfo getkey = type3.GetProperty("Key")?.GetMethod;
				MethodInfo getvalue = type3.GetProperty("Value")?.GetMethod;
				Label label = il.DefineLabel();
				Label label2 = il.DefineLabel();
				loader(il);
				il.Emit(OpCodes.Castclass, type);
				il.Emit(OpCodes.Callvirt, method);
				il.EmitStloc(i2);
				il.BeginExceptionBlock();
				il.Emit(OpCodes.Br, label2);
				il.MarkLabel(label);
				il.EmitLdloca(i2);
				il.Emit(OpCodes.Call, getMethod2);
				il.EmitStloc(kv);
				GenWriteUnknownType(nodeTree.Children[0], requireUnboxing: false, delegate(ILGenerator cil)
				{
					cil.EmitLdloca(kv);
					cil.Emit(OpCodes.Call, getkey);
				});
				GenWriteUnknownType(nodeTree.Children[1], requireUnboxing: false, delegate(ILGenerator cil)
				{
					cil.EmitLdloca(kv);
					cil.Emit(OpCodes.Call, getvalue);
				});
				il.MarkLabel(label2);
				il.EmitLdloca(i2);
				il.Emit(OpCodes.Call, method2);
				il.Emit(OpCodes.Brtrue, label);
				il.BeginFinallyBlock();
				il.EmitLdloca(i2);
				il.Emit(OpCodes.Constrained, type2);
				il.Emit(OpCodes.Callvirt, Dispose);
				il.EndExceptionBlock();
				if (node.Children[0].IsAligned)
				{
					GenAlign();
				}
				locman.ReleaseLocal(type2);
				locman.ReleaseLocal(type3);
				locman.ReleaseLocal(type);
			}

			private void GenAlign()
			{
				il.Emit(OpCodes.Ldarg_0);
				il.Emit(OpCodes.Ldc_I4_4);
				il.Emit(OpCodes.Call, AlignWriter);
			}
		}

		internal static Dictionary<string, DynamicAsset> PrototypeDic = new Dictionary<string, DynamicAsset>();

		private static Dictionary<int, Func<UnityBinaryReader, DynamicAsset>> _deserializerCache = new Dictionary<int, Func<UnityBinaryReader, DynamicAsset>>();

		private static Dictionary<int, Func<UnityBinaryReader, DynamicAsset>> _monodeserializerCache = new Dictionary<int, Func<UnityBinaryReader, DynamicAsset>>();

		private static Dictionary<int, Action<UnityBinaryWriter, DynamicAsset>> _serializerCache = new Dictionary<int, Action<UnityBinaryWriter, DynamicAsset>>();

		private static Dictionary<int, Action<UnityBinaryWriter, DynamicAsset>> _monoserializerCache = new Dictionary<int, Action<UnityBinaryWriter, DynamicAsset>>();

		private static Type DicStrObjType = typeof(Dictionary<string, object>);

		private static ConstructorInfo DicStrObjCtor = typeof(Dictionary<string, object>).GetConstructor(new Type[1]
		{
			typeof(int)
		});

		private static MethodInfo DicStrObjAdd = typeof(Dictionary<string, object>).GetMethod("Add", new Type[2]
		{
			typeof(string),
			typeof(object)
		});

		private static MethodInfo DicStrObjGetter = typeof(Dictionary<string, object>).GetProperty("Item")?.GetMethod;

		private static ConstructorInfo DynamicAssetCtor = typeof(DynamicAsset).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod, null, new Type[2]
		{
			typeof(Dictionary<string, object>),
			typeof(string)
		}, null);

		private static FieldInfo DynamicAsset_objects = typeof(DynamicAsset).GetField("objects", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod);

		private static ConstructorInfo DynamicAssetArrayCtor = typeof(DynamicAssetArray).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod, null, new Type[2]
		{
			typeof(int),
			typeof(string)
		}, null);

		private static FieldInfo DynamicAssetArrayelems = typeof(DynamicAssetArray).GetField("elems", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod);

		private static MethodInfo Dispose = typeof(IDisposable).GetMethod("Dispose");

		private static MethodInfo ReadInt = typeof(UnityBinaryReader).GetMethod("ReadInt", Type.EmptyTypes);

		private static MethodInfo ReadString = typeof(UnityBinaryReader).GetMethod("ReadString", new Type[1]
		{
			typeof(int)
		});

		private static MethodInfo ReadAlignedString = typeof(IOLibExtensions).GetMethod("ReadAlignedString");

		private static MethodInfo ReadValueArray = typeof(UnityBinaryReader).GetMethod("ReadValueArray", Type.EmptyTypes);

		private static MethodInfo AlignReader = typeof(IOLibExtensions).GetMethod("Align", new Type[2]
		{
			typeof(UnityBinaryReader),
			typeof(int)
		});

		private static MethodInfo WriteInt = typeof(UnityBinaryWriter).GetMethod("WriteInt", new Type[1]
		{
			typeof(int)
		});

		private static MethodInfo WriteString = typeof(UnityBinaryWriter).GetMethod("WriteString", new Type[1]
		{
			typeof(string)
		});

		private static MethodInfo WriteAlignedString = typeof(IOLibExtensions).GetMethod("WriteAlignedString");

		private static MethodInfo WriteValueArray = typeof(UnityBinaryWriter).GetMethod("WriteValueArray");

		private static MethodInfo AlignWriter = typeof(IOLibExtensions).GetMethod("Align", new Type[2]
		{
			typeof(UnityBinaryWriter),
			typeof(int)
		});

		private static Dictionary<string, Type> PrimitiveTypeDic = new Dictionary<string, Type>
		{
			{
				"SInt8",
				typeof(sbyte)
			},
			{
				"UInt8",
				typeof(byte)
			},
			{
				"short",
				typeof(short)
			},
			{
				"SInt16",
				typeof(short)
			},
			{
				"UInt16",
				typeof(ushort)
			},
			{
				"unsigned short",
				typeof(ushort)
			},
			{
				"int",
				typeof(int)
			},
			{
				"SInt32",
				typeof(int)
			},
			{
				"UInt32",
				typeof(uint)
			},
			{
				"unsigned int",
				typeof(uint)
			},
			{
				"Type*",
				typeof(uint)
			},
			{
				"long long",
				typeof(long)
			},
			{
				"SInt64",
				typeof(long)
			},
			{
				"UInt64",
				typeof(ulong)
			},
			{
				"unsigned long long",
				typeof(long)
			},
			{
				"float",
				typeof(float)
			},
			{
				"double",
				typeof(double)
			},
			{
				"bool",
				typeof(bool)
			}
		};

		private static Dictionary<Type, MethodInfo> PrimitiveReaderDic = new Dictionary<Type, MethodInfo>
		{
			{
				typeof(sbyte),
				typeof(UnityBinaryReader).GetMethod("ReadSByte", Type.EmptyTypes)
			},
			{
				typeof(byte),
				typeof(UnityBinaryReader).GetMethod("ReadByte", Type.EmptyTypes)
			},
			{
				typeof(short),
				typeof(UnityBinaryReader).GetMethod("ReadShort", Type.EmptyTypes)
			},
			{
				typeof(ushort),
				typeof(UnityBinaryReader).GetMethod("ReadUShort", Type.EmptyTypes)
			},
			{
				typeof(int),
				typeof(UnityBinaryReader).GetMethod("ReadInt", Type.EmptyTypes)
			},
			{
				typeof(uint),
				typeof(UnityBinaryReader).GetMethod("ReadUInt", Type.EmptyTypes)
			},
			{
				typeof(long),
				typeof(UnityBinaryReader).GetMethod("ReadLong", Type.EmptyTypes)
			},
			{
				typeof(ulong),
				typeof(UnityBinaryReader).GetMethod("ReadULong", Type.EmptyTypes)
			},
			{
				typeof(bool),
				typeof(UnityBinaryReader).GetMethod("ReadBool", Type.EmptyTypes)
			},
			{
				typeof(float),
				typeof(UnityBinaryReader).GetMethod("ReadFloat", Type.EmptyTypes)
			},
			{
				typeof(double),
				typeof(UnityBinaryReader).GetMethod("ReadDouble", Type.EmptyTypes)
			}
		};

		private static Dictionary<Type, MethodInfo> PrimitiveWriterDic = new Dictionary<Type, MethodInfo>
		{
			{
				typeof(sbyte),
				typeof(UnityBinaryWriter).GetMethod("WriteSByte", new Type[1]
				{
					typeof(sbyte)
				})
			},
			{
				typeof(byte),
				typeof(UnityBinaryWriter).GetMethod("WriteByte", new Type[1]
				{
					typeof(byte)
				})
			},
			{
				typeof(short),
				typeof(UnityBinaryWriter).GetMethod("WriteShort", new Type[1]
				{
					typeof(short)
				})
			},
			{
				typeof(ushort),
				typeof(UnityBinaryWriter).GetMethod("WriteUShort", new Type[1]
				{
					typeof(ushort)
				})
			},
			{
				typeof(int),
				typeof(UnityBinaryWriter).GetMethod("WriteInt", new Type[1]
				{
					typeof(int)
				})
			},
			{
				typeof(uint),
				typeof(UnityBinaryWriter).GetMethod("WriteUInt", new Type[1]
				{
					typeof(uint)
				})
			},
			{
				typeof(long),
				typeof(UnityBinaryWriter).GetMethod("WriteLong", new Type[1]
				{
					typeof(long)
				})
			},
			{
				typeof(ulong),
				typeof(UnityBinaryWriter).GetMethod("WriteULong", new Type[1]
				{
					typeof(ulong)
				})
			},
			{
				typeof(bool),
				typeof(UnityBinaryWriter).GetMethod("WriteBool", new Type[1]
				{
					typeof(bool)
				})
			},
			{
				typeof(float),
				typeof(UnityBinaryWriter).GetMethod("WriteFloat", new Type[1]
				{
					typeof(float)
				})
			},
			{
				typeof(double),
				typeof(UnityBinaryWriter).GetMethod("WriteDouble", new Type[1]
				{
					typeof(double)
				})
			}
		};

		private static Dictionary<string, Type> KnownTypeDic = new Dictionary<string, Type>();

		internal Dictionary<string, object> objects;

		private string proto_name;

		public string TypeName => proto_name;

		public static Func<UnityBinaryReader, DynamicAsset> GetDeserializer(SerializedType type)
		{
			Dictionary<int, Func<UnityBinaryReader, DynamicAsset>> dictionary = _deserializerCache;
			int key = type.ClassID;
			if (type.ClassID == 114)
			{
				dictionary = _monodeserializerCache;
				key = GetHashOfMonoBehaviour(type.ScriptID);
			}
			if (dictionary.TryGetValue(key, out var value))
			{
				return value;
			}
			Func<UnityBinaryReader, DynamicAsset> func = GenDeserializer(type.TypeTree.Nodes);
			dictionary.Add(key, func);
			return func;
		}

		public static Action<UnityBinaryWriter, DynamicAsset> GetSerializer(SerializedType type)
		{
			Dictionary<int, Action<UnityBinaryWriter, DynamicAsset>> dictionary = _serializerCache;
			int key = type.ClassID;
			if (type.ClassID == 114)
			{
				dictionary = _monoserializerCache;
				key = GetHashOfMonoBehaviour(type.ScriptID);
			}
			if (dictionary.TryGetValue(key, out var value))
			{
				return value;
			}
			Action<UnityBinaryWriter, DynamicAsset> action = GenSerializer(type.TypeTree.Nodes);
			dictionary.Add(key, action);
			return action;
		}

		private static int GetHashOfMonoBehaviour(byte[] scriptID)
		{
			return BitConverter.ToInt32(scriptID, 0) ^ BitConverter.ToInt32(scriptID, 4) ^ BitConverter.ToInt32(scriptID, 8) ^ BitConverter.ToInt32(scriptID, 12);
		}

		private static string PrettifyName(string name)
		{
			return name.Replace(' ', '_').Replace("[", "").Replace("]", "");
		}

		public DynamicAsset()
		{
		}

		internal DynamicAsset(Dictionary<string, object> dic, string proto_name)
		{
			objects = dic;
			this.proto_name = proto_name;
		}

		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			return objects.TryGetValue(binder.Name, out result);
		}

		public override bool TrySetMember(SetMemberBinder binder, object value)
		{
			IDynamicAssetBase dynamicAssetBase = value as IDynamicAssetBase;
			object value2 = null;
			if (!objects.TryGetValue(binder.Name, out value2))
			{
				return false;
			}
			if (value2.GetType() != value.GetType())
			{
				throw new TypeMismatchException("The type of `" + binder.Name + "` is `" + value2.GetType().GetCSharpName() + "` but got `" + value.GetType().GetCSharpName() + "`");
			}
			if (dynamicAssetBase != null && dynamicAssetBase.TypeName != ((IDynamicAssetBase)value2).TypeName)
			{
				throw new TypeMismatchException("The type of `" + binder.Name + "` is `" + ((IDynamicAssetBase)value2).TypeName + "` but got `" + dynamicAssetBase.TypeName + "`");
			}
			objects[binder.Name] = value;
			return true;
		}

		public bool HasMember(string name)
		{
			return objects.ContainsKey(name);
		}

		public dynamic AsDynamic()
		{
			return this;
		}

		public override int GetHashCode()
		{
			int num = proto_name.GetHashCode();
			foreach (KeyValuePair<string, object> @object in objects)
			{
				num ^= @object.Key.GetHashCode() ^ @object.Value.GetHashCode();
			}
			return num;
		}

		public DynamicAsset GetPrototype()
		{
			return PrototypeDic[proto_name];
		}

		public static Func<UnityBinaryReader, DynamicAsset> GenDeserializer(TypeTree.Node[] nodes)
		{
			DynamicMethod dynamicMethod = new DynamicMethod(nodes[0].Type, typeof(DynamicAsset), new Type[1]
			{
				typeof(UnityBinaryReader)
			}, typeof(DynamicAssetArray).Module, skipVisibility: true);
			new DeserializerBuilder(nodes).Build(dynamicMethod.GetILGenerator());
			return (Func<UnityBinaryReader, DynamicAsset>)dynamicMethod.CreateDelegate(typeof(Func<UnityBinaryReader, DynamicAsset>));
		}

		public static Action<UnityBinaryWriter, DynamicAsset> GenSerializer(TypeTree.Node[] nodes)
		{
			DynamicMethod dynamicMethod = new DynamicMethod(nodes[0].Type, null, new Type[2]
			{
				typeof(UnityBinaryWriter),
				typeof(DynamicAsset)
			}, typeof(DynamicAssetArray).Module, skipVisibility: true);
			new SerializerBuilder(nodes).Build(dynamicMethod.GetILGenerator());
			return (Action<UnityBinaryWriter, DynamicAsset>)dynamicMethod.CreateDelegate(typeof(Action<UnityBinaryWriter, DynamicAsset>));
		}
	}
}
