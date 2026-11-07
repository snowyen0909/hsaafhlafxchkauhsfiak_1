using System.IO;

namespace Microsoft.Xna.Framework.Net;

public class PacketReader : BinaryReader
{
	internal byte[] Data
	{
		get
		{
			MemoryStream memoryStream = (MemoryStream)BaseStream;
			return memoryStream.GetBuffer();
		}
		set
		{
			MemoryStream memoryStream = (MemoryStream)BaseStream;
			memoryStream.Write(value, 0, value.Length);
		}
	}

	public int Length => (int)BaseStream.Length;

	public int Position
	{
		get
		{
			return (int)BaseStream.Position;
		}
		set
		{
			if (BaseStream.Position != value)
			{
				BaseStream.Position = value;
			}
		}
	}

	public PacketReader()
		: this(0)
	{
	}

	public PacketReader(int capacity)
		: base(new MemoryStream(0))
	{
	}

	public Color ReadColor()
	{
		Color transparent = Color.Transparent;
		transparent.PackedValue = ReadUInt32();
		return transparent;
	}

	public override double ReadDouble()
	{
		return ReadDouble();
	}

	public Matrix ReadMatrix()
	{
		return new Matrix
		{
			M11 = ReadSingle(),
			M12 = ReadSingle(),
			M13 = ReadSingle(),
			M14 = ReadSingle(),
			M21 = ReadSingle(),
			M22 = ReadSingle(),
			M23 = ReadSingle(),
			M24 = ReadSingle(),
			M31 = ReadSingle(),
			M32 = ReadSingle(),
			M33 = ReadSingle(),
			M34 = ReadSingle(),
			M41 = ReadSingle(),
			M42 = ReadSingle(),
			M43 = ReadSingle(),
			M44 = ReadSingle()
		};
	}

	public Quaternion ReadQuaternion()
	{
		return new Quaternion
		{
			X = ReadSingle(),
			Y = ReadSingle(),
			Z = ReadSingle(),
			W = ReadSingle()
		};
	}

	public Vector2 ReadVector2()
	{
		return new Vector2
		{
			X = ReadSingle(),
			Y = ReadSingle()
		};
	}

	public Vector3 ReadVector3()
	{
		return new Vector3
		{
			X = ReadSingle(),
			Y = ReadSingle(),
			Z = ReadSingle()
		};
	}

	public Vector4 ReadVector4()
	{
		return new Vector4
		{
			X = ReadSingle(),
			Y = ReadSingle(),
			Z = ReadSingle(),
			W = ReadSingle()
		};
	}

	internal void Reset(int size)
	{
		MemoryStream memoryStream = (MemoryStream)BaseStream;
		memoryStream.SetLength(size);
		memoryStream.Position = 0L;
	}
}
