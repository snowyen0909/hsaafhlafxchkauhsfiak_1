using System;

namespace Microsoft.Xna.Framework.GamerServices;

public class SignedInEventArgs : EventArgs
{
	public SignedInGamer Gamer;

	public SignedInEventArgs(SignedInGamer gamer)
	{
	}
}
