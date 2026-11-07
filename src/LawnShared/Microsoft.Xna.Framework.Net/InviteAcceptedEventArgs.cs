using System;
using Microsoft.Xna.Framework.GamerServices;

namespace Microsoft.Xna.Framework.Net;

public class InviteAcceptedEventArgs : EventArgs
{
	private SignedInGamer gamer;

	public SignedInGamer Gamer => gamer;

	public bool IsCurrentSession => false;

	public InviteAcceptedEventArgs(SignedInGamer aGamer)
	{
		gamer = aGamer;
	}
}
