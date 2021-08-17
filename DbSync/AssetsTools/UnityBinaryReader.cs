using System;
using System.IO;
using System.Text;
using AssetsTools.LZ4;

namespace AssetsTools
{
	public class UnityBinaryReader
	{
		private byte[] file;

		private int offset;

		private int bound;

		private int start;

		public int Position
		{
			get
			{
				return offset - start;
			}
			set
			{
				int num = value + start;
				if (bound < num)
				{
					throw new IndexOutOfRangeException();
				}
				offset = num;
			}
		}

		public UnityBinaryReader(string filename)
		{
			CheckEndianness();
			start = 0;
            using (FileStream fileStream = new FileStream(filename, FileMode.Open))
			{
				file = new byte[new FileInfo(filename).Length];
                bound = fileStream.Read(file, 0, file.Length);
                offset = 0;
			}
		}

		public UnityBinaryReader(byte[] bin)
		{
			CheckEndianness();
			file = bin ?? throw new NullReferenceException("bin");
			offset = 0;
			start = 0;
			bound = bin.Length;
		}

		public UnityBinaryReader(byte[] bin, int offset, int length)
		{
			CheckEndianness();
			file = bin ?? throw new NullReferenceException("bin");
			int num = bin.Length;
			if (num <= offset || offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (num < length + offset || length < 0)
			{
				throw new ArgumentOutOfRangeException("length");
			}
			this.offset = offset;
			start = offset;
			bound = length + offset;
		}

		private static void CheckEndianness()
		{
			if (!BitConverter.IsLittleEndian)
			{
				throw new NotSupportedException("BigEndian platform is not supported");
			}
		}

		public UnityBinaryReader Slice(int offset, int length)
		{
			return new UnityBinaryReader(file, offset, length);
		}

		public UnityBinaryReader Slice(int offset)
		{
			return new UnityBinaryReader(file, offset, bound - offset);
		}

		public byte ReadByte()
		{
			byte result = file[offset];
			offset++;
			return result;
		}

		public sbyte ReadSByte()
		{
			byte num = file[offset];
			offset++;
			return (sbyte)num;
		}

		public bool ReadBool()
		{
			bool result = file[offset] != 0;
			offset++;
			return result;
		}

		public unsafe short ReadShort()
		{
			if (bound < offset + 2)
			{
				throw new IndexOutOfRangeException();
			}
			fixed (byte* ptr = &file[offset])
			{
				offset += 2;
				return *(short*)ptr;
			}
		}

		public unsafe int ReadInt()
		{
			if (bound < offset + 4)
			{
				throw new IndexOutOfRangeException();
			}
			fixed (byte* ptr = &file[offset])
			{
				offset += 4;
				return *(int*)ptr;
			}
		}

		public unsafe long ReadLong()
		{
			if (bound < offset + 8)
			{
				throw new IndexOutOfRangeException();
			}
			fixed (byte* ptr = &file[offset])
			{
				offset += 8;
				return *(long*)ptr;
			}
		}

		public unsafe float ReadFloat()
		{
			if (bound < offset + 4)
			{
				throw new IndexOutOfRangeException();
			}
			fixed (byte* ptr = &file[offset])
			{
				offset += 4;
				return *(float*)ptr;
			}
		}

		public unsafe double ReadDouble()
		{
			if (bound < offset + 8)
			{
				throw new IndexOutOfRangeException();
			}
			fixed (byte* ptr = &file[offset])
			{
				offset += 8;
				return *(double*)ptr;
			}
		}

		public unsafe ushort ReadUShort()
		{
			if (bound < offset + 2)
			{
				throw new IndexOutOfRangeException();
			}
			fixed (byte* ptr = &file[offset])
			{
				offset += 2;
				return *(ushort*)ptr;
			}
		}

		public unsafe uint ReadUInt()
		{
			if (bound < offset + 4)
			{
				throw new IndexOutOfRangeException();
			}
			fixed (byte* ptr = &file[offset])
			{
				offset += 4;
				return *(uint*)ptr;
			}
		}

		public unsafe ulong ReadULong()
		{
			if (bound < offset + 8)
			{
				throw new IndexOutOfRangeException();
			}
			fixed (byte* ptr = &file[offset])
			{
				offset += 8;
				return (ulong)(*(long*)ptr);
			}
		}

		public unsafe short ReadShortBE()
		{
			if (bound < offset + 2)
			{
				throw new IndexOutOfRangeException();
			}
			short result = 0;
			fixed (byte* ptr2 = &file[offset])
			{
				byte* ptr = (byte*)(&result);
				*ptr = ptr2[1];
				ptr[1] = *ptr2;
				offset += 2;
				return result;
			}
		}

		public unsafe int ReadIntBE()
		{
			if (bound < offset + 4)
			{
				throw new IndexOutOfRangeException();
			}
			int result = 0;
			fixed (byte* ptr2 = &file[offset])
			{
				byte* ptr = (byte*)(&result);
				*ptr = ptr2[3];
				ptr[1] = ptr2[2];
				ptr[2] = ptr2[1];
				ptr[3] = *ptr2;
				offset += 4;
				return result;
			}
		}

		public unsafe long ReadLongBE()
		{
			if (bound < offset + 8)
			{
				throw new IndexOutOfRangeException();
			}
			long result = 0L;
			fixed (byte* ptr2 = &file[offset])
			{
				byte* ptr = (byte*)(&result);
				*ptr = ptr2[7];
				ptr[1] = ptr2[6];
				ptr[2] = ptr2[5];
				ptr[3] = ptr2[4];
				ptr[4] = ptr2[3];
				ptr[5] = ptr2[2];
				ptr[6] = ptr2[1];
				ptr[7] = *ptr2;
				offset += 8;
				return result;
			}
		}

		public unsafe float ReadFloatBE()
		{
			if (bound < offset + 4)
			{
				throw new IndexOutOfRangeException();
			}
			float result = 0f;
			fixed (byte* ptr2 = &file[offset])
			{
				byte* ptr = (byte*)(&result);
				*ptr = ptr2[3];
				ptr[1] = ptr2[2];
				ptr[2] = ptr2[1];
				ptr[3] = *ptr2;
				offset += 4;
				return result;
			}
		}

		public unsafe double ReadDoubleBE()
		{
			if (bound < offset + 8)
			{
				throw new IndexOutOfRangeException();
			}
			double result = 0.0;
			fixed (byte* ptr2 = &file[offset])
			{
				byte* ptr = (byte*)(&result);
				*ptr = ptr2[7];
				ptr[1] = ptr2[6];
				ptr[2] = ptr2[5];
				ptr[3] = ptr2[4];
				ptr[4] = ptr2[3];
				ptr[5] = ptr2[2];
				ptr[6] = ptr2[1];
				ptr[7] = *ptr2;
				offset += 8;
				return result;
			}
		}

		public unsafe ushort ReadUShortBE()
		{
			if (bound < offset + 2)
			{
				throw new IndexOutOfRangeException();
			}
			ushort result = 0;
			fixed (byte* ptr2 = &file[offset])
			{
				byte* ptr = (byte*)(&result);
				*ptr = ptr2[1];
				ptr[1] = *ptr2;
				offset += 2;
				return result;
			}
		}

		public unsafe uint ReadUIntBE()
		{
			if (bound < offset + 4)
			{
				throw new IndexOutOfRangeException();
			}
			uint result = 0u;
			fixed (byte* ptr2 = &file[offset])
			{
				byte* ptr = (byte*)(&result);
				*ptr = ptr2[3];
				ptr[1] = ptr2[2];
				ptr[2] = ptr2[1];
				ptr[3] = *ptr2;
				offset += 4;
				return result;
			}
		}

		public unsafe ulong ReadULongBE()
		{
			if (bound < offset + 8)
			{
				throw new IndexOutOfRangeException();
			}
			fixed (byte* ptr2 = &file[offset])
			{
				ulong result = default(ulong);
				byte* ptr = (byte*)(&result);
				*ptr = ptr2[7];
				ptr[1] = ptr2[6];
				ptr[2] = ptr2[5];
				ptr[3] = ptr2[4];
				ptr[4] = ptr2[3];
				ptr[5] = ptr2[2];
				ptr[6] = ptr2[1];
				ptr[7] = *ptr2;
				offset += 8;
				return result;
			}
		}

		public unsafe void ReadBytes(byte[] dest, int offset, int length)
		{
			if (dest == null)
			{
				throw new NullReferenceException("dest");
			}
			if (offset < 0 || offset >= dest.Length)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (length < 0 || length > dest.Length - offset)
			{
				throw new ArgumentOutOfRangeException("length");
			}
			if (length + this.offset > bound)
			{
				throw new IndexOutOfRangeException();
			}
			fixed (byte* source = &file[this.offset])
			{
				fixed (byte* destination = &dest[offset])
				{
					Buffer.MemoryCopy(source, destination, length, length);
				}
				this.offset += length;
			}
		}

		public byte[] ReadBytes(int length)
		{
			byte[] array = new byte[length];
			ReadBytes(array, 0, length);
			return array;
		}

		public string ReadString(int length)
		{
			if (length < 0)
			{
				throw new ArgumentOutOfRangeException("length");
			}
			if (offset + length > bound)
			{
				throw new IndexOutOfRangeException();
			}
			if (length != 0)
			{
				string @string = Encoding.UTF8.GetString(file, offset, length);
				offset += length;
				return @string;
			}
			return "";
		}

		public unsafe string ReadStringToNull()
		{
			if (offset >= bound)
			{
				throw new IndexOutOfRangeException();
			}
			fixed (byte* ptr = &file[0])
			{
				byte* ptr2 = ptr + offset;
				byte* ptr3 = ptr + bound;
				while (*ptr2 != 0)
				{
					ptr2++;
					if (ptr2 >= ptr3)
					{
						throw new IndexOutOfRangeException();
					}
				}
				int num = (int)(ptr2 - ptr) - offset;
				string @string = Encoding.UTF8.GetString(file, offset, num);
				offset = num + 1 + offset;
				return @string;
			}
		}

		public unsafe T[] ReadValueArray<T>() where T : unmanaged
		{
			int num = ReadInt();
			if (num != 0)
			{
				T[] array = new T[num];
				fixed (byte* source = &file[offset])
				{
					fixed (T* ptr = &array[0])
					{
						void* destination = ptr;
						int num2 = sizeof(T) * array.Length;
						if (num2 + offset > bound)
						{
							throw new IndexOutOfRangeException();
						}
						Buffer.MemoryCopy(source, destination, num2, num2);
						offset += num2;
						return array;
					}
				}
			}
			return new T[0];
		}

		public int ReadLZ4Data(int compressed_size, int uncompressed_size, byte[] dest, int dest_offset)
		{
			int result = LZ4Codec.Decode(file, offset, compressed_size, dest, dest_offset, uncompressed_size);
			offset += compressed_size;
			return result;
		}
	}
}
