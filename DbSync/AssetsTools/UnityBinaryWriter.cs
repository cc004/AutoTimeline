using System;
using System.Text;
using AssetsTools.LZ4;

namespace AssetsTools
{
	public class UnityBinaryWriter
	{
		private byte[] file = new byte[256];

		private int offset;

		private int bound;

		private int capacity = 256;

		public int Position
		{
			get
			{
				return offset;
			}
			set
			{
				if (offset > bound)
				{
					bound = offset;
				}
				if (capacity < value)
				{
					EnsureCapacity(value);
				}
				offset = value;
				if (offset > bound)
				{
					bound = offset;
				}
			}
		}

		public int Length
		{
			get
			{
				if (bound <= offset)
				{
					return offset;
				}
				return bound;
			}
		}

		public UnityBinaryWriter()
		{
			if (!BitConverter.IsLittleEndian)
			{
				throw new NotSupportedException("BigEndian platform is not supported");
			}
		}

		public unsafe void EnsureCapacity(int value)
		{
			int num = value;
			if (num < capacity * 2)
			{
				num = capacity * 2;
			}
			fixed (byte* source = &file[0])
			{
				byte[] array = new byte[num];
				fixed (byte* destination = &array[0])
				{
					Buffer.MemoryCopy(source, destination, num, file.LongLength);
					file = array;
				}
			}
			capacity = num;
		}

		public unsafe byte[] ToBytes()
		{
			if (offset > bound)
			{
				bound = offset;
			}
			byte[] array = new byte[bound];
			if (bound == 0)
			{
				return array;
			}
			fixed (byte* destination = &array[0])
			{
				fixed (byte* source = &file[0])
				{
					Buffer.MemoryCopy(source, destination, bound, bound);
				}
			}
			return array;
		}

		public void WriteByte(byte value)
		{
			if (capacity < offset + 1)
			{
				EnsureCapacity(offset + 1);
			}
			file[offset] = value;
			offset++;
		}

		public void WriteSByte(sbyte value)
		{
			if (capacity < offset + 1)
			{
				EnsureCapacity(offset + 1);
			}
			file[offset] = (byte)value;
			offset++;
		}

		public void WriteBool(bool value)
		{
			if (capacity < offset + 1)
			{
				EnsureCapacity(offset + 1);
			}
			file[offset] = (byte)(value ? 1 : 0);
			offset++;
		}

		public unsafe void WriteShort(short value)
		{
			if (capacity < offset + 2)
			{
				EnsureCapacity(offset + 2);
			}
			fixed (byte* ptr = &file[offset])
			{
				*(short*)ptr = value;
				offset += 2;
			}
		}

		public unsafe void WriteInt(int value)
		{
			if (capacity < offset + 4)
			{
				EnsureCapacity(offset + 4);
			}
			fixed (byte* ptr = &file[offset])
			{
				*(int*)ptr = value;
				offset += 4;
			}
		}

		public unsafe void WriteLong(long value)
		{
			if (capacity < offset + 8)
			{
				EnsureCapacity(offset + 8);
			}
			fixed (byte* ptr = &file[offset])
			{
				*(long*)ptr = value;
				offset += 8;
			}
		}

		public unsafe void WriteFloat(float value)
		{
			if (capacity < offset + 4)
			{
				EnsureCapacity(offset + 4);
			}
			fixed (byte* ptr = &file[offset])
			{
				*(float*)ptr = value;
				offset += 4;
			}
		}

		public unsafe void WriteDouble(double value)
		{
			if (capacity < offset + 8)
			{
				EnsureCapacity(offset + 8);
			}
			fixed (byte* ptr = &file[offset])
			{
				*(double*)ptr = value;
				offset += 8;
			}
		}

		public unsafe void WriteUShort(ushort value)
		{
			if (capacity < offset + 2)
			{
				EnsureCapacity(offset + 2);
			}
			fixed (byte* ptr = &file[offset])
			{
				*(ushort*)ptr = value;
				offset += 2;
			}
		}

		public unsafe void WriteUInt(uint value)
		{
			if (capacity < offset + 4)
			{
				EnsureCapacity(offset + 4);
			}
			fixed (byte* ptr = &file[offset])
			{
				*(uint*)ptr = value;
				offset += 4;
			}
		}

		public unsafe void WriteULong(ulong value)
		{
			if (capacity < offset + 8)
			{
				EnsureCapacity(offset + 8);
			}
			fixed (byte* ptr = &file[offset])
			{
				*(ulong*)ptr = value;
				offset += 8;
			}
		}

		public unsafe void WriteShortBE(short value)
		{
			if (capacity < offset + 2)
			{
				EnsureCapacity(offset + 2);
			}
			fixed (byte* ptr2 = &file[offset])
			{
				byte* ptr = (byte*)(&value);
				*ptr2 = ptr[1];
				ptr2[1] = *ptr;
				offset += 2;
			}
		}

		public unsafe void WriteIntBE(int value)
		{
			if (capacity < offset + 4)
			{
				EnsureCapacity(offset + 4);
			}
			fixed (byte* ptr2 = &file[offset])
			{
				byte* ptr = (byte*)(&value);
				*ptr2 = ptr[3];
				ptr2[1] = ptr[2];
				ptr2[2] = ptr[1];
				ptr2[3] = *ptr;
				offset += 4;
			}
		}

		public unsafe void WriteLongBE(long value)
		{
			if (capacity < offset + 8)
			{
				EnsureCapacity(offset + 8);
			}
			fixed (byte* ptr2 = &file[offset])
			{
				byte* ptr = (byte*)(&value);
				*ptr2 = ptr[7];
				ptr2[1] = ptr[6];
				ptr2[2] = ptr[5];
				ptr2[3] = ptr[4];
				ptr2[4] = ptr[3];
				ptr2[5] = ptr[2];
				ptr2[6] = ptr[1];
				ptr2[7] = *ptr;
				offset += 8;
			}
		}

		public unsafe void WriteFloatBE(float value)
		{
			if (capacity < offset + 4)
			{
				EnsureCapacity(offset + 4);
			}
			fixed (byte* ptr2 = &file[offset])
			{
				byte* ptr = (byte*)(&value);
				*ptr2 = ptr[3];
				ptr2[1] = ptr[2];
				ptr2[2] = ptr[1];
				ptr2[3] = *ptr;
				offset += 4;
			}
		}

		public unsafe void WriteDoubleBE(double value)
		{
			if (capacity < offset + 8)
			{
				EnsureCapacity(offset + 8);
			}
			fixed (byte* ptr2 = &file[offset])
			{
				byte* ptr = (byte*)(&value);
				*ptr2 = ptr[7];
				ptr2[1] = ptr[6];
				ptr2[2] = ptr[5];
				ptr2[3] = ptr[4];
				ptr2[4] = ptr[3];
				ptr2[5] = ptr[2];
				ptr2[6] = ptr[1];
				ptr2[7] = *ptr;
				offset += 8;
			}
		}

		public unsafe void WriteUShortBE(ushort value)
		{
			if (capacity < offset + 2)
			{
				EnsureCapacity(offset + 2);
			}
			fixed (byte* ptr2 = &file[offset])
			{
				byte* ptr = (byte*)(&value);
				*ptr2 = ptr[1];
				ptr2[1] = *ptr;
				offset += 2;
			}
		}

		public unsafe void WriteUIntBE(uint value)
		{
			if (capacity < offset + 4)
			{
				EnsureCapacity(offset + 4);
			}
			fixed (byte* ptr2 = &file[offset])
			{
				byte* ptr = (byte*)(&value);
				*ptr2 = ptr[3];
				ptr2[1] = ptr[2];
				ptr2[2] = ptr[1];
				ptr2[3] = *ptr;
				offset += 4;
			}
		}

		public unsafe void WriteULongBE(ulong value)
		{
			if (capacity < offset + 8)
			{
				EnsureCapacity(offset + 8);
			}
			fixed (byte* ptr2 = &file[offset])
			{
				byte* ptr = (byte*)(&value);
				*ptr2 = ptr[7];
				ptr2[1] = ptr[6];
				ptr2[2] = ptr[5];
				ptr2[3] = ptr[4];
				ptr2[4] = ptr[3];
				ptr2[5] = ptr[2];
				ptr2[6] = ptr[1];
				ptr2[7] = *ptr;
				offset += 8;
			}
		}

		public unsafe void WriteBytes(byte[] src, int offset, int length)
		{
			if (length == 0)
			{
				return;
			}
			if (src == null)
			{
				throw new NullReferenceException("src");
			}
			if (offset < 0 || offset >= src.Length)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (length < 0 || length > src.Length - offset)
			{
				throw new ArgumentOutOfRangeException("length");
			}
			if (length + this.offset > capacity)
			{
				EnsureCapacity(length + this.offset);
			}
			fixed (byte* destination = &file[this.offset])
			{
				fixed (byte* source = &src[offset])
				{
					Buffer.MemoryCopy(source, destination, length, length);
					this.offset += length;
				}
			}
		}

		public void WriteBytes(byte[] src)
		{
			WriteBytes(src, 0, src.Length);
		}

		public unsafe void WriteString(string value)
		{
			if (value == null)
			{
				WriteInt(0);
				return;
			}
			if (value.Length == 0)
			{
				WriteInt(0);
				return;
			}
			int num;
			int num2 = Encoding.UTF8.GetMaxByteCount(value.Length) + (num = offset + 4);
			if (num2 > capacity)
			{
				EnsureCapacity(num2);
			}
			int bytes = Encoding.UTF8.GetBytes(value, 0, value.Length, file, num);
			fixed (byte* ptr = &file[offset])
			{
				*(int*)ptr = bytes;
				offset = num + bytes;
			}
		}

		public void WriteStringToNull(string value)
		{
			if (value == null)
			{
				WriteByte(0);
				return;
			}
			if (value.Length == 0)
			{
				WriteByte(0);
				return;
			}
			int num;
			int num2 = Encoding.UTF8.GetMaxByteCount(value.Length + 1) + (num = offset);
			if (num2 > capacity)
			{
				EnsureCapacity(num2);
			}
			num = Encoding.UTF8.GetBytes(value, 0, value.Length, file, num) + num;
			file[num] = 0;
			offset = num + 1;
		}

		public unsafe void WriteValueArray<T>(T[] array) where T : unmanaged
		{
			if (array == null || array.Length == 0)
			{
				WriteInt(0);
				return;
			}
			long num = sizeof(T) * array.LongLength;
			if (num + offset + 4 > capacity)
			{
				EnsureCapacity((int)num + offset + 4);
			}
			fixed (byte* ptr = &file[offset])
			{
				fixed (T* source = &array[0])
				{
					*(int*)ptr = array.Length;
					Buffer.MemoryCopy(source, ptr + 4, num, num);
					offset += (int)num + 4;
				}
			}
		}

		public int WriteLZ4Data(byte[] bin, int offset, int length)
		{
			if (this.offset + length > capacity)
			{
				EnsureCapacity(this.offset + length);
			}
			int num = LZ4Codec.Encode(bin, offset, length, file, this.offset, capacity - this.offset);
			this.offset += num;
			return num;
		}

		public int WriteLZ4Data(byte[] bin)
		{
			return WriteLZ4Data(bin, 0, bin.Length);
		}
	}
}
