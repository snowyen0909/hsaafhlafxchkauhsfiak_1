using System.Collections.Generic;

namespace Microsoft.Xna.Framework.GamerServices;

public class SignedInGamerCollection : List<SignedInGamer>
{
	public SignedInGamer this[PlayerIndex index]
	{
		get
		{
			if (Count == 0 || (int)index > Count - 1)
			{
				return null;
			}
			return base[(int)index];
		}
	}
}
