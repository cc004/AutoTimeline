using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AssetsTools.LZ4;

namespace AssetsTools
{
	public class AssetBundleFile : ISerializable
	{
		public class FileType
		{
			public string Name;

			public byte[] Data;

			internal int CalcInfoSize()
			{
				return 20 + Name.Length + 1;
			}
		}

		private struct CompressionInfo
		{
			public byte[] data;

			public int offset;

			public int length;
		}

		private struct BlockInfo : ISerializable
		{
			public int uncompressedSize;

			public int compressedSize;

			public short flag;

			public void Read(UnityBinaryReader reader)
			{
				uncompressedSize = reader.ReadIntBE();
				compressedSize = reader.ReadIntBE();
				flag = reader.ReadShortBE();
			}

			public void Write(UnityBinaryWriter writer)
			{
				writer.WriteIntBE(uncompressedSize);
				writer.WriteIntBE(compressedSize);
				writer.WriteShortBE(flag);
			}
		}

		private struct BlockReader
		{
			private UnityBinaryReader _reader;

			private BlockInfo[] _infos;

			private int idx;

			private byte[] buffer;

			private int begin;

			private int end;

			private int nextSize => _infos[idx].uncompressedSize;

			public BlockReader(BlockInfo[] infos, UnityBinaryReader reader)
			{
				_reader = reader;
				_infos = infos;
				idx = 0;
				buffer = null;
				begin = 0;
				end = 0;
			}

			public void ReadBytes(byte[] dest, int offset, int length)
			{
				if (length > end - begin)
				{
					int num = end - begin;
					if (num > 0)
					{
						Buffer.BlockCopy(buffer, begin, dest, offset, num);
						length -= num;
						offset += num;
					}
					while (nextSize <= length)
					{
						int num2 = readNextBlock(dest, offset);
						length -= num2;
						if (length == 0)
						{
							return;
						}
						offset += num2;
					}
					buffer = MemoryPool<BlockReader>.GetBuffer(nextSize);
					readNextBlock(buffer, 0);
					Buffer.BlockCopy(buffer, 0, dest, offset, length);
				}
				else
				{
					Buffer.BlockCopy(buffer, begin, dest, offset, length);
					begin += length;
				}
			}

			public void Seek(int offset)
			{
				int num = 0;
				for (int i = 0; i < _infos.Length; i++)
				{
					if (offset <= num + _infos[i].uncompressedSize)
					{
						if (offset == num)
						{
							begin = (end = 0);
							idx = i;
						}
						else
						{
							buffer = MemoryPool<BlockReader>.GetBuffer(_infos[i].uncompressedSize);
							begin = offset - num;
							end = buffer.Length;
						}
						return;
					}
					num += _infos[i].uncompressedSize;
				}
				throw new IndexOutOfRangeException();
			}

			private int readNextBlock(byte[] dest, int offset)
			{
				if (idx >= _infos.Length)
				{
					throw new IndexOutOfRangeException();
				}
				switch (_infos[idx].flag)
				{
				default:
					_reader.ReadBytes(dest, offset, _infos[idx].uncompressedSize);
					break;
				case 1:
					throw new NotSupportedException("LZMA is not supported");
				case 2:
				case 3:
					_reader.ReadLZ4Data(_infos[idx].compressedSize, _infos[idx].uncompressedSize, dest, offset);
					break;
				}
				return _infos[idx++].uncompressedSize;
			}
		}

		public struct HeaderType : ISerializable
		{
			public string signature;

			public int format;

			public string versionPlayer;

			public string versionEngine;

			public long bundleSize;

			internal long CalcSize()
			{
				return signature.Length + 1 + 4 + versionPlayer.Length + 1 + versionEngine.Length + 1 + 8;
			}

			public void Read(UnityBinaryReader reader)
			{
				signature = reader.ReadStringToNull();
				format = reader.ReadIntBE();
				versionPlayer = reader.ReadStringToNull();
				versionEngine = reader.ReadStringToNull();
				bundleSize = reader.ReadLongBE();
			}

			public void Write(UnityBinaryWriter writer)
			{
				writer.WriteStringToNull(signature);
				writer.WriteIntBE(format);
				writer.WriteStringToNull(versionPlayer);
				writer.WriteStringToNull(versionEngine);
				writer.WriteLongBE(bundleSize);
			}
		}

		public HeaderType Header;

		public FileType[] Files;

		public bool EnableCompression;

		private const int BLOCK_SIZE = 131072;

		public const int FORMAT = 6;

		public void Read(UnityBinaryReader reader)
		{
			Header.Read(reader);
			if (Header.signature != "UnityFS")
			{
				throw new UnknownFormatException("Signature " + Header.signature + " is not supported");
			}
			if (Header.format != 6)
			{
				throw new UnknownFormatException("Format " + Header.format + " is not supported");
			}
			readFiles(reader);
		}

		public void Write(UnityBinaryWriter writer)
		{
			int position = writer.Position;
			writer.Position += (int)Header.CalcSize();
			writeFiles(writer);
			Header.bundleSize = writer.Length;
			writer.Position = position;
			Header.Write(writer);
		}

		private void readFiles(UnityBinaryReader reader)
		{
			int compressed_size = reader.ReadIntBE();
			int num = reader.ReadIntBE();
			int num2 = reader.ReadIntBE();
			if (((uint)num2 & 0x80u) != 0)
			{
				throw new NotImplementedException("BlockInfos are at the end of file");
			}
			UnityBinaryReader unityBinaryReader;
			switch (num2 & 0x3F)
			{
			default:
				unityBinaryReader = reader;
				break;
			case 1:
				throw new NotSupportedException("LZMA is not supported");
			case 2:
			case 3:
			{
				byte[] buffer = MemoryPool<AssetBundleFile>.GetBuffer(num);
				reader.ReadLZ4Data(compressed_size, num, buffer, 0);
				unityBinaryReader = new UnityBinaryReader(buffer, 0, num);
				break;
			}
			}
			unityBinaryReader.Position += 16;
			BlockInfo[] array = new BlockInfo[unityBinaryReader.ReadIntBE()];
			array.Read(unityBinaryReader);
			int num3 = unityBinaryReader.ReadIntBE();
			Files = new FileType[num3];
			long[] array2 = new long[num3];
			for (int i = 0; i < num3; i++)
			{
				Files[i] = new FileType();
				array2[i] = unityBinaryReader.ReadLongBE();
				Files[i].Data = new byte[unityBinaryReader.ReadLongBE()];
				unityBinaryReader.ReadIntBE();
				Files[i].Name = unityBinaryReader.ReadStringToNull();
			}
			BlockReader blockReader = new BlockReader(array, reader);
			for (int j = 0; j < num3; j++)
			{
				blockReader.Seek((int)array2[j]);
				blockReader.ReadBytes(Files[j].Data, 0, Files[j].Data.Length);
			}
		}

		private void writeFiles(UnityBinaryWriter writer)
		{
			Files.Sum((FileType f) => f.Data.Length);
			int num = Files.Sum((FileType f) => f.Data.Length);
			int num2 = num / 131072 + ((num % 131072 != 0) ? 1 : 0);
			BlockInfo[] blockinfos = new BlockInfo[num2];
			short flag = (short)(EnableCompression ? 2 : 0);
			for (int j = 0; j < num2; j++)
			{
				blockinfos[j].uncompressedSize = 131072;
				blockinfos[j].compressedSize = 131072;
				blockinfos[j].flag = flag;
			}
			if (num % 131072 != 0)
			{
				blockinfos[num2 - 1].uncompressedSize = num % 131072;
				blockinfos[num2 - 1].compressedSize = num % 131072;
			}
			int num3 = 20 + 10 * blockinfos.Length;
			int num4 = 4 + Files.Sum((FileType f) => f.CalcInfoSize());
			int position = writer.Position;
			if (!EnableCompression)
			{
				writer.WriteIntBE(num3 + num4);
				writer.WriteIntBE(num3 + num4);
				writer.WriteIntBE(64);
				writer.Position += 16;
				writer.WriteIntBE(num2);
				blockinfos.Write(writer);
				writer.WriteIntBE(Files.Length);
				int num5 = 0;
				for (int k = 0; k < Files.Length; k++)
				{
					writer.WriteLongBE(num5);
					writer.WriteLongBE(Files[k].Data.LongLength);
					writer.WriteIntBE(4);
					writer.WriteStringToNull(Files[k].Name);
					num5 += Files[k].Data.Length;
				}
				for (int l = 0; l < Files.Length; l++)
				{
					writer.WriteBytes(Files[l].Data);
				}
				return;
			}
			byte[] compbuf = MemoryPool<AssetBundleFile>.GetBuffer(num2 * 131072);
			if (num2 < 128)
			{
				byte[] buffer = MiniMemoryPool<AssetBundleFile>.GetBuffer(131072);
				int num6 = 0;
				int num7 = 0;
				for (int m = 0; m < Files.Length; m++)
				{
					if (num6 > 0)
					{
						Buffer.BlockCopy(Files[m].Data, 0, buffer, num6, 131072 - num6);
						blockinfos[num7].compressedSize = TryLZ4Compress(buffer, 0, compbuf, num7 * 131072, 131072);
						if (blockinfos[num7].compressedSize == 131072)
						{
							blockinfos[num7].flag &= -64;
						}
						num7++;
					}
					int num8 = 0;
					if (num6 > 0)
					{
						num8 = 131072 - num6;
					}
					int num9 = (Files[m].Data.Length - num8) / 131072;
					int num10 = 0;
					while (num10 < num9)
					{
						blockinfos[num7].compressedSize = TryLZ4Compress(Files[m].Data, num8 + num10 * 131072, compbuf, num7 * 131072, 131072);
						if (blockinfos[num7].compressedSize == 131072)
						{
							blockinfos[num7].flag &= -64;
						}
						num10++;
						num7++;
					}
					num6 = (Files[m].Data.Length - num8) % 131072;
					if (num6 > 0)
					{
						Buffer.BlockCopy(Files[m].Data, Files[m].Data.Length - num6, buffer, 0, num6);
					}
				}
				if (num6 > 0)
				{
					blockinfos[num7].compressedSize = TryLZ4Compress(buffer, 0, compbuf, num7 * 131072, num6);
					if (blockinfos[num7].compressedSize == num6)
					{
						blockinfos[num7].flag &= -64;
					}
				}
			}
			else
			{
				CompressionInfo[] compinfos = new CompressionInfo[num2];
				byte[] buffer2 = MiniMemoryPool<AssetBundleFile>.GetBuffer(131072);
				int num11 = 0;
				int num12 = 0;
				for (int n = 0; n < Files.Length; n++)
				{
					if (num12 > 0)
					{
						Buffer.BlockCopy(Files[n].Data, 0, buffer2, num12, 131072 - num12);
						blockinfos[num11].compressedSize = TryLZ4Compress(buffer2, 0, compbuf, num11 * 131072, 131072);
						if (blockinfos[num11].compressedSize == 131072)
						{
							blockinfos[num11].flag &= -64;
						}
						num11++;
					}
					int num13 = 0;
					if (num12 > 0)
					{
						num13 = 131072 - num12;
					}
					int num14 = (Files[n].Data.Length - num13) / 131072;
					int num15 = 0;
					while (num15 < num14)
					{
						compinfos[num11].data = Files[n].Data;
						compinfos[num11].length = 131072;
						compinfos[num11].offset = num13 + num15 * 131072;
						num15++;
						num11++;
					}
					num12 = (Files[n].Data.Length - num13) % 131072;
					if (num12 > 0)
					{
						Buffer.BlockCopy(Files[n].Data, Files[n].Data.Length - num12, buffer2, 0, num12);
					}
				}
				if (num12 > 0)
				{
					blockinfos[num11].compressedSize = LZ4Codec.Encode(buffer2, 0, num12, compbuf, num11 * 131072, num12);
					if (blockinfos[num11].compressedSize == 0)
					{
						blockinfos[num11].compressedSize = num12;
						blockinfos[num11].flag &= -64;
						Buffer.BlockCopy(buffer2, 0, compbuf, num11 * 131072, 131072);
					}
				}
				Parallel.For(0, num2, delegate(int i)
				{
					if (compinfos[i].data != null)
					{
						blockinfos[i].compressedSize = TryLZ4Compress(compinfos[i].data, compinfos[i].offset, compbuf, i * 131072, compinfos[i].length);
						if (blockinfos[i].compressedSize == 131072)
						{
							blockinfos[i].flag &= -64;
						}
					}
				});
			}
			UnityBinaryWriter unityBinaryWriter = new UnityBinaryWriter();
			unityBinaryWriter.Position += 16;
			unityBinaryWriter.WriteIntBE(num2);
			blockinfos.Write(unityBinaryWriter);
			unityBinaryWriter.WriteIntBE(Files.Length);
			int num16 = 0;
			for (int num17 = 0; num17 < Files.Length; num17++)
			{
				unityBinaryWriter.WriteLongBE(num16);
				unityBinaryWriter.WriteLongBE(Files[num17].Data.LongLength);
				unityBinaryWriter.WriteIntBE(4);
				unityBinaryWriter.WriteStringToNull(Files[num17].Name);
				num16 += Files[num17].Data.Length;
			}
			writer.Position += 12;
			int value = writer.WriteLZ4Data(unityBinaryWriter.ToBytes());
			int position2 = writer.Position;
			writer.Position = position;
			writer.WriteIntBE(value);
			writer.WriteIntBE(num3 + num4);
			writer.WriteIntBE(66);
			writer.Position = position2;
			for (int num18 = 0; num18 < num2; num18++)
			{
				writer.WriteBytes(compbuf, num18 * 131072, blockinfos[num18].compressedSize);
			}
		}

		private int TryLZ4Compress(byte[] src, int srcOffset, byte[] dest, int destOffset, int length)
		{
			int num = LZ4Codec.Encode(src, srcOffset, length, dest, destOffset, length - 1);
			if (num == 0)
			{
				Buffer.BlockCopy(src, srcOffset, dest, destOffset, length);
				return length;
			}
			return num;
		}

		private void initHeader()
		{
			Header.signature = "UnityFS";
			Header.format = 6;
			Header.versionPlayer = "5.x.x";
			Header.versionEngine = "2017.3.1f1";
			Header.bundleSize = 0L;
		}

		public static AssetBundleFile LoadFromFile(string filename)
		{
			AssetBundleFile assetBundleFile = new AssetBundleFile();
			assetBundleFile.Read(new UnityBinaryReader(filename));
			return assetBundleFile;
		}

		public static AssetBundleFile LoadFromMemory(byte[] bin)
		{
			AssetBundleFile assetBundleFile = new AssetBundleFile();
			assetBundleFile.Read(new UnityBinaryReader(bin));
			return assetBundleFile;
		}

		public void SaveToFile(string filename)
		{
			UnityBinaryWriter unityBinaryWriter = new UnityBinaryWriter();
			Write(unityBinaryWriter);
            using (FileStream fileStream = new FileStream(filename, FileMode.Create))
			{
				byte[] array = unityBinaryWriter.ToBytes();
                fileStream.Write(array, 0, array.Length);
			}
		}
	}
}
