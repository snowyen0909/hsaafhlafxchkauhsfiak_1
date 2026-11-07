using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Sexy;

internal static class TexturePixelReader
{
	public static Color[] ReadRegion(Texture2D texture, Rectangle region)
	{
		Color[] array = new Color[region.Width * region.Height];
		if (texture.Format == SurfaceFormat.Color)
		{
			texture.GetData(0, region, array, 0, array.Length);
			return array;
		}
		lock (ResourceManager.DrawLocker)
		{
			GraphicsDevice graphicsDevice = texture.GraphicsDevice;
			RenderTargetBinding[] renderTargets = graphicsDevice.GetRenderTargets();
			Viewport viewport = graphicsDevice.Viewport;
			Rectangle scissorRectangle = graphicsDevice.ScissorRectangle;
			BlendState blendState = graphicsDevice.BlendState;
			SamplerState value = graphicsDevice.SamplerStates[0];
			DepthStencilState depthStencilState = graphicsDevice.DepthStencilState;
			RasterizerState rasterizerState = graphicsDevice.RasterizerState;
			using RenderTarget2D renderTarget2D = new RenderTarget2D(graphicsDevice, region.Width, region.Height, mipMap: false, SurfaceFormat.Color, DepthFormat.None);
			using SpriteBatch spriteBatch = new SpriteBatch(graphicsDevice);
			try
			{
				graphicsDevice.SetRenderTarget(renderTarget2D);
				spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Opaque, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);
				spriteBatch.Draw(texture, renderTarget2D.Bounds, region, Color.White);
				spriteBatch.End();
				renderTarget2D.GetData(array);
				return array;
			}
			finally
			{
				graphicsDevice.SetRenderTargets(renderTargets);
				graphicsDevice.Viewport = viewport;
				graphicsDevice.ScissorRectangle = scissorRectangle;
				graphicsDevice.BlendState = blendState;
				graphicsDevice.SamplerStates[0] = value;
				graphicsDevice.DepthStencilState = depthStencilState;
				graphicsDevice.RasterizerState = rasterizerState;
			}
		}
	}
}
