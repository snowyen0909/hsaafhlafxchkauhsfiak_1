namespace Sexy;

public enum WidgetFlags
{
	Update = 1,
	MarkDirty = 2,
	Draw = 4,
	Clip = 8,
	AllowMouse = 0x10,
	AllowFocus = 0x20
}
