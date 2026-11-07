using System;
using Microsoft.Xna.Framework.Graphics;

namespace Microsoft.Xna.Framework.GamerServices;

public sealed class GamerProfile : IDisposable
{
	~GamerProfile()
	{
		Dispose(disposing: false);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	private void Dispose(bool disposing)
	{
	}

	public Texture2D GetGamerPicture()
	{
		throw new NotImplementedException();
	}
}
