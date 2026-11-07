using System.Buffers;
using System.Buffers.Binary;
using System.IO;
using System.Text;

namespace Sexy;

public class BufferNew
{
	private const int BUFFER_SIZE = 256;

	public readonly MemoryStream InnerStream;

	private static readonly byte[] _buffer = new byte[256];

	public BufferNew()
	{
		InnerStream = new MemoryStream();
	}

	public BufferNew(MemoryStream stream)
	{
		InnerStream = stream;
	}

	public BufferNew(byte[] buffer)
	{
		InnerStream = new MemoryStream(buffer);
	}

	public bool ReadBoolean()
	{
		InnerStream.Read(_buffer, 0, 1);
		return (_buffer[0] & 1) != 0;
	}

	public void WriteBoolean(bool value)
	{
		_buffer[0] = (value ? ((byte)1) : ((byte)0));
		InnerStream.Write(_buffer, 0, 1);
	}

	public sbyte ReadInt8()
	{
		InnerStream.Read(_buffer, 0, 1);
		return (sbyte)_buffer[0];
	}

	public void WriteInt8(sbyte value)
	{
		_buffer[0] = (byte)value;
		InnerStream.Write(_buffer, 0, 1);
	}

	public byte ReadUInt8()
	{
		InnerStream.Read(_buffer, 0, 1);
		return _buffer[0];
	}

	public void WriteUInt8(byte value)
	{
		_buffer[0] = value;
		InnerStream.Write(_buffer, 0, 1);
	}

	public short ReadInt16LE()
	{
		InnerStream.Read(_buffer, 0, 2);
		return BinaryPrimitives.ReadInt16LittleEndian(_buffer);
	}

	public void WriteInt16LE(short value)
	{
		BinaryPrimitives.WriteInt16LittleEndian(_buffer, value);
		InnerStream.Write(_buffer, 0, 2);
	}

	public ushort ReadUInt16LE()
	{
		InnerStream.Read(_buffer, 0, 2);
		return BinaryPrimitives.ReadUInt16LittleEndian(_buffer);
	}

	public void WriteUInt16LE(ushort value)
	{
		BinaryPrimitives.WriteUInt16LittleEndian(_buffer, value);
		InnerStream.Write(_buffer, 0, 2);
	}

	public int ReadInt32LE()
	{
		InnerStream.Read(_buffer, 0, 4);
		return BinaryPrimitives.ReadInt32LittleEndian(_buffer);
	}

	public void WriteInt32LE(int value)
	{
		BinaryPrimitives.WriteInt32LittleEndian(_buffer, value);
		InnerStream.Write(_buffer, 0, 4);
	}

	public uint ReadUInt32LE()
	{
		InnerStream.Read(_buffer, 0, 4);
		return BinaryPrimitives.ReadUInt32LittleEndian(_buffer);
	}

	public void WriteUInt32LE(uint value)
	{
		BinaryPrimitives.WriteUInt32LittleEndian(_buffer, value);
		InnerStream.Write(_buffer, 0, 4);
	}

	public long ReadInt64LE()
	{
		InnerStream.Read(_buffer, 0, 8);
		return BinaryPrimitives.ReadInt64LittleEndian(_buffer);
	}

	public void WriteInt64LE(long value)
	{
		BinaryPrimitives.WriteInt64LittleEndian(_buffer, value);
		InnerStream.Write(_buffer, 0, 8);
	}

	public ulong ReadUInt64LE()
	{
		InnerStream.Read(_buffer, 0, 8);
		return BinaryPrimitives.ReadUInt64LittleEndian(_buffer);
	}

	public void WriteUInt64LE(ulong value)
	{
		BinaryPrimitives.WriteUInt64LittleEndian(_buffer, value);
		InnerStream.Write(_buffer, 0, 8);
	}

	public float ReadFloat32LE()
	{
		InnerStream.Read(_buffer, 0, 4);
		return BinaryPrimitives.ReadSingleLittleEndian(_buffer);
	}

	public void WriteFloat32LE(float value)
	{
		BinaryPrimitives.WriteSingleLittleEndian(_buffer, value);
		InnerStream.Write(_buffer, 0, 4);
	}

	public double ReadFloat64LE()
	{
		InnerStream.Read(_buffer, 0, 8);
		return BinaryPrimitives.ReadDoubleLittleEndian(_buffer);
	}

	public void WriteFloat64LE(double value)
	{
		BinaryPrimitives.WriteDoubleLittleEndian(_buffer, value);
		InnerStream.Write(_buffer, 0, 8);
	}

	public string ReadString()
	{
		int num = ReadInt32LE();
		if (num < 0)
		{
			return null;
		}
		if (num == 0)
		{
			return string.Empty;
		}
		if (num <= 256)
		{
			InnerStream.Read(_buffer, 0, num);
			return Encoding.Unicode.GetString(_buffer, 0, num);
		}
		byte[] array = null;
		try
		{
			array = ArrayPool<byte>.Shared.Rent(num);
			InnerStream.Read(array, 0, num);
			return Encoding.Unicode.GetString(array, 0, num);
		}
		finally
		{
			if (array != null)
			{
				ArrayPool<byte>.Shared.Return(array);
			}
		}
	}

	public void WriteString(string value)
	{
		if (value == null)
		{
			WriteInt32LE(-1);
			return;
		}
		if (value.Length <= 0)
		{
			WriteInt32LE(0);
			return;
		}
		int maxByteCount = Encoding.Unicode.GetMaxByteCount(value.Length);
		if (maxByteCount <= 252)
		{
			int bytes = Encoding.Unicode.GetBytes(value, 0, value.Length, _buffer, 4);
			WriteInt32LE(bytes);
			InnerStream.Write(_buffer, 4, bytes);
			return;
		}
		byte[] array = null;
		try
		{
			array = ArrayPool<byte>.Shared.Rent(maxByteCount);
			int bytes2 = Encoding.Unicode.GetBytes(value, 0, value.Length, array, 0);
			WriteInt32LE(bytes2);
			InnerStream.Write(array, 0, bytes2);
		}
		finally
		{
			if (array != null)
			{
				ArrayPool<byte>.Shared.Return(array);
			}
		}
	}
}
