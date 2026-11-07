namespace Sexy;

public struct SexyChar(char c)
{
	public char value_type = c;

	public static bool operator ==(SexyChar a, KeyCode b)
	{
		return a.Equals(b);
	}

	public static bool operator !=(SexyChar a, KeyCode b)
	{
		return !(a == b);
	}

	public static bool operator ==(SexyChar a, uint b)
	{
		return a.Equals(b);
	}

	public static bool operator !=(SexyChar a, uint b)
	{
		return !(a == b);
	}

	public override bool Equals(object obj)
	{
		if (obj is SexyChar)
		{
			return value_type == ((SexyChar)obj).value_type;
		}
		if (obj is KeyCode)
		{
			return false;
		}
		if (obj is char obj2)
		{
			return value_type.Equals(obj2);
		}
		if (obj is uint num)
		{
			return value_type.Equals((char)num);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return value_type.GetHashCode();
	}

	public override string ToString()
	{
		return value_type.ToString();
	}

	public static implicit operator char(SexyChar value)
	{
		return value.value_type;
	}
}
