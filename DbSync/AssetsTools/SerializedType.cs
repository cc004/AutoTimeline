namespace AssetsTools
{
	public struct SerializedType : ISerializable
	{
		public int ClassID;

		public bool IsStrippedType;

		public short ScriptTypeIndex;

		public byte[] ScriptID;

		public byte[] OldTypeHash;

		public TypeTree TypeTree;

		public void Read(UnityBinaryReader reader)
		{
			ClassID = reader.ReadInt();
			IsStrippedType = reader.ReadByte() != 0;
			ScriptTypeIndex = reader.ReadShort();
			if (ClassID == 114)
			{
				ScriptID = reader.ReadBytes(16);
			}
			OldTypeHash = reader.ReadBytes(16);
		}

		public void Write(UnityBinaryWriter writer)
		{
			writer.WriteInt(ClassID);
			writer.WriteByte((byte)(IsStrippedType ? 1u : 0u));
			writer.WriteShort(ScriptTypeIndex);
			if (ClassID == 114)
			{
				writer.WriteBytes(ScriptID, 0, 16);
			}
			writer.WriteBytes(OldTypeHash, 0, 16);
		}
	}
}
