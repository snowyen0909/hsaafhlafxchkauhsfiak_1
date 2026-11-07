namespace Microsoft.Xna.Framework.Net;

public delegate NetworkSession NetworkSessionAsynchronousCreate(NetworkSessionType sessionType, int maxLocalGamers, int maxGamers, int privateGamerSlots, NetworkSessionProperties sessionProperties, int hostGamer, bool isHost);
