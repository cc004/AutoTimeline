using System;

namespace AssetsTools
{
	public class AssetsFile : ISerializable
	{
		public struct ExternalFileType : ISerializable
		{
			public Guid Guid;

			public int Type;

			public string PathName;

			public void Read(UnityBinaryReader reader)
			{
				reader.ReadStringToNull();
				Guid = new Guid(reader.ReadBytes(16));
				Type = reader.ReadInt();
				PathName = reader.ReadStringToNull();
			}

			public void Write(UnityBinaryWriter writer)
			{
				writer.WriteStringToNull("");
				writer.WriteBytes(Guid.ToByteArray());
				writer.WriteInt(Type);
				writer.WriteStringToNull(PathName);
			}
		}

		public struct HeaderType : ISerializable
		{
			public int MetadataSize;

			public int FileSize;

			public int Version;

			public int DataOffset;

			public bool IsBigEndian;

			public byte[] Reserved;

			public void Read(UnityBinaryReader reader)
			{
				MetadataSize = reader.ReadIntBE();
				FileSize = reader.ReadIntBE();
				Version = reader.ReadIntBE();
				DataOffset = reader.ReadIntBE();
				IsBigEndian = ((reader.ReadByte() != 0) ? true : false);
				Reserved = reader.ReadBytes(3);
			}

			public void Write(UnityBinaryWriter writer)
			{
				writer.WriteIntBE(MetadataSize);
				writer.WriteIntBE(FileSize);
				writer.WriteIntBE(Version);
				writer.WriteIntBE(DataOffset);
				writer.WriteByte((byte)(IsBigEndian ? 1u : 0u));
				writer.WriteBytes(Reserved, 0, 3);
			}

			internal int CalcSize()
			{
				return 20;
			}
		}

		public struct MetadataHeaderType : ISerializable
		{
			public string UnityVersion;

			public int TargetPlatform;

			public bool EnableTypeTree;

			public void Read(UnityBinaryReader reader)
			{
				UnityVersion = reader.ReadStringToNull();
				TargetPlatform = reader.ReadInt();
				EnableTypeTree = reader.ReadByte() != 0;
			}

			public void Write(UnityBinaryWriter writer)
			{
				writer.WriteStringToNull(UnityVersion);
				writer.WriteInt(TargetPlatform);
				writer.WriteByte((byte)(EnableTypeTree ? 1u : 0u));
			}
		}

		public class ObjectType
		{
			internal WeakReference<AssetsFile> parent;

			public long PathID;

			public byte[] Data;

			public int TypeID;
		}

		public struct ScriptIdentifierType : ISerializable
		{
			public int Index;

			public long Identifier;

			public void Read(UnityBinaryReader reader)
			{
				Index = reader.ReadInt();
				reader.Align(4);
				Identifier = reader.ReadLong();
			}

			public void Write(UnityBinaryWriter writer)
			{
				writer.WriteInt(Index);
				writer.Align(4);
				writer.WriteLong(Identifier);
			}
		}

		public HeaderType Header;

		public MetadataHeaderType MetadataHeader;

		public SerializedType[] Types;

		public ObjectType[] Objects;

		public ScriptIdentifierType[] Scripts;

		public ExternalFileType[] Externals;

		public string UserInformation;

		public void Read(UnityBinaryReader reader)
		{
			Header.Read(reader);
			if (Header.Version != 17)
			{
				throw new NotSupportedException("Version " + Header.Version + " is not supported");
			}
			if (Header.IsBigEndian)
			{
				throw new NotSupportedException("BigEndian file is not supported");
			}
			readMetadata(reader);
			readObjects(reader);
			readScripts(reader);
			readExternals(reader);
			UserInformation = reader.ReadStringToNull();
		}

		public void Write(UnityBinaryWriter writer)
		{
			int position = writer.Position;
			writer.Position += Header.CalcSize();
			writeMetadata(writer);
			byte[] src = writeObjects(writer);
			writeScripts(writer);
			writeExternals(writer);
			writer.WriteStringToNull(UserInformation);
			Header.MetadataSize = writer.Position - Header.CalcSize();
			if (writer.Position < 4096)
			{
				writer.Position = 4096;
			}
			else
			{
				writer.Align(16);
			}
			Header.DataOffset = writer.Position;
			writer.WriteBytes(src);
			Header.FileSize = writer.Position;
			writer.Position = position;
			Header.Write(writer);
		}

		private void readExternals(UnityBinaryReader reader)
		{
			int num = reader.ReadInt();
			Externals = new ExternalFileType[num];
			Externals.Read(reader);
		}

		private void writeExternals(UnityBinaryWriter writer)
		{
			writer.WriteInt(Externals.Length);
			Externals.Write(writer);
		}

		private void readMetadata(UnityBinaryReader reader)
		{
			MetadataHeader.Read(reader);
			int num = reader.ReadInt();
			Types = new SerializedType[num];
			for (int i = 0; i < num; i++)
			{
				Types[i].Read(reader);
				if (MetadataHeader.EnableTypeTree)
				{
					Types[i].TypeTree = new TypeTree();
					Types[i].TypeTree.Read(reader);
				}
			}
		}

		private void writeMetadata(UnityBinaryWriter writer)
		{
			MetadataHeader.Write(writer);
			writer.WriteInt(Types.Length);
			for (int i = 0; i < Types.Length; i++)
			{
				Types[i].Write(writer);
				if (MetadataHeader.EnableTypeTree)
				{
					Types[i].TypeTree.Write(writer);
				}
			}
		}

		private void readObjects(UnityBinaryReader reader)
		{
			int num = reader.ReadInt();
			Objects = new ObjectType[num];
			for (int i = 0; i < num; i++)
			{
				Objects[i] = new ObjectType();
				Objects[i].parent = new WeakReference<AssetsFile>(this);
				reader.Align(4);
				Objects[i].PathID = reader.ReadLong();
				uint num2 = reader.ReadUInt();
				uint length = reader.ReadUInt();
				Objects[i].TypeID = reader.ReadInt();
				int position = reader.Position;
				reader.Position = Header.DataOffset + (int)num2;
				Objects[i].Data = reader.ReadBytes((int)length);
				reader.Position = position;
			}
		}

		private byte[] writeObjects(UnityBinaryWriter writer)
		{
			writer.WriteInt(Objects.Length);
			UnityBinaryWriter unityBinaryWriter = new UnityBinaryWriter();
			for (int i = 0; i < Objects.Length; i++)
			{
				writer.Align(4);
				writer.WriteLong(Objects[i].PathID);
				unityBinaryWriter.Align(8);
				writer.WriteInt(unityBinaryWriter.Position);
				writer.WriteInt(Objects[i].Data.Length);
				writer.WriteInt(Objects[i].TypeID);
				unityBinaryWriter.WriteBytes(Objects[i].Data);
			}
			return unityBinaryWriter.ToBytes();
		}

		private void readScripts(UnityBinaryReader reader)
		{
			int num = reader.ReadInt();
			Scripts = new ScriptIdentifierType[num];
			Scripts.Read(reader);
		}

		private void writeScripts(UnityBinaryWriter writer)
		{
			writer.WriteInt(Scripts.Length);
			Scripts.Write(writer);
		}
	}
}
