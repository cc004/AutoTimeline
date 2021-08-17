namespace AssetsTools
{
	internal interface ISerializable
	{
		void Read(UnityBinaryReader reader);

		void Write(UnityBinaryWriter writer);
	}
}
