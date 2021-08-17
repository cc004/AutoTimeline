namespace AssetsTools
{
	public static class Extensions
	{
		public static AssetsFile ToAssetsFile(this AssetBundleFile.FileType file)
		{
			AssetsFile assetsFile = new AssetsFile();
			assetsFile.Read(new UnityBinaryReader(file.Data));
			return assetsFile;
		}

		public static void LoadAssetsFile(this AssetBundleFile.FileType file, AssetsFile assets)
		{
			UnityBinaryWriter unityBinaryWriter = new UnityBinaryWriter();
			assets.Write(unityBinaryWriter);
			file.Data = unityBinaryWriter.ToBytes();
		}
	}
}
