namespace Sexy;

public static class KeyCodes
{
	public class KeyNameEntry
	{
		public string mKeyName = new string(new char[12]);

		public KeyCode mKeyCode;

		public KeyNameEntry(string theKeyName, KeyCode theKeyCode)
		{
			mKeyName = theKeyName;
			mKeyCode = theKeyCode;
		}
	}

	public const int MAX_KEYNAME_LEN = 12;

	public static KeyNameEntry[] aKeyCodeArray = new KeyNameEntry[87]
	{
		new KeyNameEntry("UNKNOWN", KeyCode.Unknown),
		new KeyNameEntry("LBUTTON", KeyCode.Lbutton),
		new KeyNameEntry("RBUTTON", KeyCode.Rbutton),
		new KeyNameEntry("CANCEL", KeyCode.Cancel),
		new KeyNameEntry("MBUTTON", KeyCode.Mbutton),
		new KeyNameEntry("BACK", KeyCode.Back),
		new KeyNameEntry("TAB", KeyCode.Tab),
		new KeyNameEntry("CLEAR", KeyCode.Clear),
		new KeyNameEntry("RETURN", KeyCode.Return),
		new KeyNameEntry("SHIFT", KeyCode.Shift),
		new KeyNameEntry("CONTROL", KeyCode.Control),
		new KeyNameEntry("MENU", KeyCode.Menu),
		new KeyNameEntry("PAUSE", KeyCode.Pause),
		new KeyNameEntry("CAPITAL", KeyCode.Capital),
		new KeyNameEntry("KANA", KeyCode.Kana),
		new KeyNameEntry("HANGEUL", KeyCode.Kana),
		new KeyNameEntry("HANGUL", KeyCode.Kana),
		new KeyNameEntry("JUNJA", KeyCode.Junja),
		new KeyNameEntry("FINAL", KeyCode.Final),
		new KeyNameEntry("HANJA", KeyCode.Hanja),
		new KeyNameEntry("KANJI", KeyCode.Hanja),
		new KeyNameEntry("ESCAPE", KeyCode.Escape),
		new KeyNameEntry("CONVERT", KeyCode.Convert),
		new KeyNameEntry("NONCONVERT", KeyCode.Nonconvert),
		new KeyNameEntry("ACCEPT", KeyCode.Accept),
		new KeyNameEntry("MODECHANGE", KeyCode.Modechange),
		new KeyNameEntry("SPACE", KeyCode.Space),
		new KeyNameEntry("PRIOR", KeyCode.Prior),
		new KeyNameEntry("NEXT", KeyCode.Next),
		new KeyNameEntry("END", KeyCode.End),
		new KeyNameEntry("HOME", KeyCode.Home),
		new KeyNameEntry("LEFT", KeyCode.Left),
		new KeyNameEntry("UP", KeyCode.Up),
		new KeyNameEntry("RIGHT", KeyCode.Right),
		new KeyNameEntry("DOWN", KeyCode.Down),
		new KeyNameEntry("SELECT", KeyCode.Select),
		new KeyNameEntry("PRINT", KeyCode.Print),
		new KeyNameEntry("EXECUTE", KeyCode.Execute),
		new KeyNameEntry("SNAPSHOT", KeyCode.Snapshot),
		new KeyNameEntry("INSERT", KeyCode.Insert),
		new KeyNameEntry("DELETE", KeyCode.Delete),
		new KeyNameEntry("HELP", KeyCode.Help),
		new KeyNameEntry("LWIN", KeyCode.Lwin),
		new KeyNameEntry("RWIN", KeyCode.Rwin),
		new KeyNameEntry("APPS", KeyCode.Apps),
		new KeyNameEntry("NUMPAD0", KeyCode.Numpad0),
		new KeyNameEntry("NUMPAD1", KeyCode.Numpad1),
		new KeyNameEntry("NUMPAD2", KeyCode.Numpad2),
		new KeyNameEntry("NUMPAD3", KeyCode.Numpad3),
		new KeyNameEntry("NUMPAD4", KeyCode.Numpad4),
		new KeyNameEntry("NUMPAD5", KeyCode.Numpad5),
		new KeyNameEntry("NUMPAD6", KeyCode.Numpad6),
		new KeyNameEntry("NUMPAD7", KeyCode.Numpad7),
		new KeyNameEntry("NUMPAD8", KeyCode.Numpad8),
		new KeyNameEntry("NUMPAD9", KeyCode.Numpad9),
		new KeyNameEntry("MULTIPLY", KeyCode.Multiply),
		new KeyNameEntry("ADD", KeyCode.Add),
		new KeyNameEntry("SEPARATOR", KeyCode.Separator),
		new KeyNameEntry("SUBTRACT", KeyCode.Subtract),
		new KeyNameEntry("DECIMAL", KeyCode.Decimal),
		new KeyNameEntry("DIVIDE", KeyCode.Divide),
		new KeyNameEntry("F1", KeyCode.F1),
		new KeyNameEntry("F2", KeyCode.F2),
		new KeyNameEntry("F3", KeyCode.F3),
		new KeyNameEntry("F4", KeyCode.F4),
		new KeyNameEntry("F5", KeyCode.F5),
		new KeyNameEntry("F6", KeyCode.F6),
		new KeyNameEntry("F7", KeyCode.F7),
		new KeyNameEntry("F8", KeyCode.F8),
		new KeyNameEntry("F9", KeyCode.F9),
		new KeyNameEntry("F10", KeyCode.F10),
		new KeyNameEntry("F11", KeyCode.F11),
		new KeyNameEntry("F12", KeyCode.F12),
		new KeyNameEntry("F13", KeyCode.F13),
		new KeyNameEntry("F14", KeyCode.F14),
		new KeyNameEntry("F15", KeyCode.F15),
		new KeyNameEntry("F16", KeyCode.F16),
		new KeyNameEntry("F17", KeyCode.F17),
		new KeyNameEntry("F18", KeyCode.F18),
		new KeyNameEntry("F19", KeyCode.F19),
		new KeyNameEntry("F20", KeyCode.F20),
		new KeyNameEntry("F21", KeyCode.F21),
		new KeyNameEntry("F22", KeyCode.F22),
		new KeyNameEntry("F23", KeyCode.F23),
		new KeyNameEntry("F24", KeyCode.F24),
		new KeyNameEntry("NUMLOCK", KeyCode.Numlock),
		new KeyNameEntry("SCROLL", KeyCode.Scroll)
	};

	public static KeyCode GetKeyCodeFromName(string theKeyName)
	{
		if (theKeyName.Length >= 11)
		{
			return KeyCode.Unknown;
		}
		string text = theKeyName.ToUpper();
		if (theKeyName.Length == 1)
		{
			byte b = (byte)text[0];
			if (b >= 48 && b <= 90)
			{
				return (KeyCode)b;
			}
			if (b >= 51 && b <= 96)
			{
				return (KeyCode)(b + 128);
			}
		}
		for (int i = 0; i < aKeyCodeArray.Length; i++)
		{
			if (string.Compare(text, aKeyCodeArray[i].mKeyName) == 0)
			{
				return aKeyCodeArray[i].mKeyCode;
			}
		}
		return KeyCode.Unknown;
	}

	public static string GetKeyNameFromCode(KeyCode theKeyCode)
	{
		if (theKeyCode >= KeyCode.Asciibegin && theKeyCode <= KeyCode.Asciiend)
		{
			char[] array = new char[2]
			{
				(char)theKeyCode,
				'\0'
			};
			return array.ToString();
		}
		if (theKeyCode >= KeyCode.Asciibegin2 && theKeyCode <= KeyCode.Asciiend2)
		{
			char[] array2 = new char[2]
			{
				(char)((byte)theKeyCode - 128),
				'\0'
			};
			return array2.ToString();
		}
		for (int i = 0; i < aKeyCodeArray.Length; i++)
		{
			if (theKeyCode == aKeyCodeArray[i].mKeyCode)
			{
				return aKeyCodeArray[i].mKeyName;
			}
		}
		return "UNKNOWN";
	}
}
