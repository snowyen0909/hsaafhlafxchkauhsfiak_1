using Microsoft.Xna.Framework.Graphics;

namespace Sexy;

internal static class BlendStates
{
	public static BlendState imageLoadBlendAlpha = new BlendState
	{
		ColorWriteChannels = ColorWriteChannels.Alpha,
		AlphaDestinationBlend = Blend.Zero,
		ColorDestinationBlend = Blend.Zero,
		AlphaSourceBlend = Blend.One,
		ColorSourceBlend = Blend.One
	};

	public static BlendState blendColorLoadState = new BlendState
	{
		ColorWriteChannels = (ColorWriteChannels.Red | ColorWriteChannels.Green | ColorWriteChannels.Blue),
		AlphaDestinationBlend = Blend.Zero,
		ColorDestinationBlend = Blend.Zero,
		AlphaSourceBlend = Blend.SourceAlpha,
		ColorSourceBlend = Blend.SourceAlpha
	};
}
