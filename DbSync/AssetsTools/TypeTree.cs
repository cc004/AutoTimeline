using System.Collections.Generic;
using System.Linq;

namespace AssetsTools
{
	public class TypeTree : ISerializable
	{
		public struct Node
		{
			public ushort Version;

			public byte Level;

			public bool IsArray;

			public string Type;

			public string Name;

			public int ByteSize;

			public int Index;

			public int MetaFlag;
		}

		private class StringTableBuilder
		{
			private UnityBinaryWriter _writer = new UnityBinaryWriter();

			private Dictionary<string, int> offset_table = new Dictionary<string, int>();

			public ushort AddString(string str)
			{
				if (offset_table.ContainsKey(str))
				{
					return (ushort)offset_table[str];
				}
				offset_table[str] = _writer.Position;
				_writer.WriteStringToNull(str);
				return (ushort)offset_table[str];
			}

			public byte[] ToBytes()
			{
				return _writer.ToBytes();
			}
		}

		private static readonly Dictionary<int, string> CommonStringDic = new Dictionary<int, string>
		{
			{
				0,
				"AABB"
			},
			{
				5,
				"AnimationClip"
			},
			{
				19,
				"AnimationCurve"
			},
			{
				34,
				"AnimationState"
			},
			{
				49,
				"Array"
			},
			{
				55,
				"Base"
			},
			{
				60,
				"BitField"
			},
			{
				69,
				"bitset"
			},
			{
				76,
				"bool"
			},
			{
				81,
				"char"
			},
			{
				86,
				"ColorRGBA"
			},
			{
				96,
				"Component"
			},
			{
				106,
				"data"
			},
			{
				111,
				"deque"
			},
			{
				117,
				"double"
			},
			{
				124,
				"dynamic_array"
			},
			{
				138,
				"FastPropertyName"
			},
			{
				155,
				"first"
			},
			{
				161,
				"float"
			},
			{
				167,
				"Font"
			},
			{
				172,
				"GameObject"
			},
			{
				183,
				"Generic Mono"
			},
			{
				196,
				"GradientNEW"
			},
			{
				208,
				"GUID"
			},
			{
				213,
				"GUIStyle"
			},
			{
				222,
				"int"
			},
			{
				226,
				"list"
			},
			{
				231,
				"long long"
			},
			{
				241,
				"map"
			},
			{
				245,
				"Matrix4x4f"
			},
			{
				256,
				"MdFour"
			},
			{
				263,
				"MonoBehaviour"
			},
			{
				277,
				"MonoScript"
			},
			{
				288,
				"m_ByteSize"
			},
			{
				299,
				"m_Curve"
			},
			{
				307,
				"m_EditorClassIdentifier"
			},
			{
				331,
				"m_EditorHideFlags"
			},
			{
				349,
				"m_Enabled"
			},
			{
				359,
				"m_ExtensionPtr"
			},
			{
				374,
				"m_GameObject"
			},
			{
				387,
				"m_Index"
			},
			{
				395,
				"m_IsArray"
			},
			{
				405,
				"m_IsStatic"
			},
			{
				416,
				"m_MetaFlag"
			},
			{
				427,
				"m_Name"
			},
			{
				434,
				"m_ObjectHideFlags"
			},
			{
				452,
				"m_PrefabInternal"
			},
			{
				469,
				"m_PrefabParentObject"
			},
			{
				490,
				"m_Script"
			},
			{
				499,
				"m_StaticEditorFlags"
			},
			{
				519,
				"m_Type"
			},
			{
				526,
				"m_Version"
			},
			{
				536,
				"Object"
			},
			{
				543,
				"pair"
			},
			{
				548,
				"PPtr<Component>"
			},
			{
				564,
				"PPtr<GameObject>"
			},
			{
				581,
				"PPtr<Material>"
			},
			{
				596,
				"PPtr<MonoBehaviour>"
			},
			{
				616,
				"PPtr<MonoScript>"
			},
			{
				633,
				"PPtr<Object>"
			},
			{
				646,
				"PPtr<Prefab>"
			},
			{
				659,
				"PPtr<Sprite>"
			},
			{
				672,
				"PPtr<TextAsset>"
			},
			{
				688,
				"PPtr<Texture>"
			},
			{
				702,
				"PPtr<Texture2D>"
			},
			{
				718,
				"PPtr<Transform>"
			},
			{
				734,
				"Prefab"
			},
			{
				741,
				"Quaternionf"
			},
			{
				753,
				"Rectf"
			},
			{
				759,
				"RectInt"
			},
			{
				767,
				"RectOffset"
			},
			{
				778,
				"second"
			},
			{
				785,
				"set"
			},
			{
				789,
				"short"
			},
			{
				795,
				"size"
			},
			{
				800,
				"SInt16"
			},
			{
				807,
				"SInt32"
			},
			{
				814,
				"SInt64"
			},
			{
				821,
				"SInt8"
			},
			{
				827,
				"staticvector"
			},
			{
				840,
				"string"
			},
			{
				847,
				"TextAsset"
			},
			{
				857,
				"TextMesh"
			},
			{
				866,
				"Texture"
			},
			{
				874,
				"Texture2D"
			},
			{
				884,
				"Transform"
			},
			{
				894,
				"TypelessData"
			},
			{
				907,
				"UInt16"
			},
			{
				914,
				"UInt32"
			},
			{
				921,
				"UInt64"
			},
			{
				928,
				"UInt8"
			},
			{
				934,
				"unsigned int"
			},
			{
				947,
				"unsigned long long"
			},
			{
				966,
				"unsigned short"
			},
			{
				981,
				"vector"
			},
			{
				988,
				"Vector2f"
			},
			{
				997,
				"Vector3f"
			},
			{
				1006,
				"Vector4f"
			},
			{
				1015,
				"m_ScriptingClassIdentifier"
			},
			{
				1042,
				"Gradient"
			},
			{
				1051,
				"Type*"
			},
			{
				1057,
				"int2_storage"
			},
			{
				1070,
				"int3_storage"
			},
			{
				1083,
				"BoundsInt"
			},
			{
				1093,
				"m_CorrespondingSourceObject"
			}
		};

		private static Dictionary<string, int> _revCommonStringDic = null;

		public Node[] Nodes;

		private static Dictionary<string, int> RevCommonStringDic
		{
			get
			{
				if (_revCommonStringDic == null)
				{
					_revCommonStringDic = CommonStringDic.ToDictionary((KeyValuePair<int, string> kv) => kv.Value, (KeyValuePair<int, string> kv) => kv.Key);
				}
				return _revCommonStringDic;
			}
		}

		private static string GetCommonString(int index)
		{
			if (CommonStringDic.ContainsKey(index))
			{
				return CommonStringDic[index];
			}
			return index.ToString();
		}

		private static int GetCommonStringID(string typename)
		{
			if (!RevCommonStringDic.ContainsKey(typename))
			{
				return -1;
			}
			return RevCommonStringDic[typename];
		}

		public void Read(UnityBinaryReader reader)
		{
			int num = reader.ReadInt();
			int num2 = reader.ReadInt();
			UnityBinaryReader unityBinaryReader = reader.Slice(reader.Position + 24 * num);
			Nodes = new Node[num];
			for (int i = 0; i < num; i++)
			{
				Nodes[i].Version = reader.ReadUShort();
				Nodes[i].Level = reader.ReadByte();
				Nodes[i].IsArray = reader.ReadByte() != 0;
				ushort num3 = reader.ReadUShort();
				if (reader.ReadUShort() == 0)
				{
					unityBinaryReader.Position = num3;
					Nodes[i].Type = unityBinaryReader.ReadStringToNull();
				}
				else
				{
					Nodes[i].Type = GetCommonString(num3);
				}
				ushort num4 = reader.ReadUShort();
				if (reader.ReadUShort() == 0)
				{
					unityBinaryReader.Position = num4;
					Nodes[i].Name = unityBinaryReader.ReadStringToNull();
				}
				else
				{
					Nodes[i].Name = GetCommonString(num4);
				}
				Nodes[i].ByteSize = reader.ReadInt();
				Nodes[i].Index = reader.ReadInt();
				Nodes[i].MetaFlag = reader.ReadInt();
			}
			reader.Position += num2;
		}

		public void Write(UnityBinaryWriter writer)
		{
			int position = writer.Position;
			writer.Position += 8;
			StringTableBuilder stringTableBuilder = new StringTableBuilder();
			for (int i = 0; i < Nodes.Length; i++)
			{
				writer.WriteUShort(Nodes[i].Version);
				writer.WriteByte(Nodes[i].Level);
				writer.WriteByte((byte)(Nodes[i].IsArray ? 1u : 0u));
				int commonStringID = GetCommonStringID(Nodes[i].Type);
				if (commonStringID == -1)
				{
					writer.WriteUShort(stringTableBuilder.AddString(Nodes[i].Type));
					writer.WriteUShort(0);
				}
				else
				{
					writer.WriteUShort((ushort)commonStringID);
					writer.WriteUShort(32768);
				}
				int commonStringID2 = GetCommonStringID(Nodes[i].Name);
				if (commonStringID2 == -1)
				{
					writer.WriteUShort(stringTableBuilder.AddString(Nodes[i].Name));
					writer.WriteUShort(0);
				}
				else
				{
					writer.WriteUShort((ushort)commonStringID2);
					writer.WriteUShort(32768);
				}
				writer.WriteInt(Nodes[i].ByteSize);
				writer.WriteInt(Nodes[i].Index);
				writer.WriteInt(Nodes[i].MetaFlag);
			}
			byte[] array = stringTableBuilder.ToBytes();
			writer.WriteBytes(array);
			int position2 = writer.Position;
			writer.Position = position;
			writer.WriteInt(Nodes.Length);
			writer.WriteInt(array.Length);
			writer.Position = position2;
		}
	}
}
