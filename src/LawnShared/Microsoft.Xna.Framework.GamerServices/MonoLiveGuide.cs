using System;
using Microsoft.Xna.Framework.Graphics;

namespace Microsoft.Xna.Framework.GamerServices;

internal class MonoLiveGuide : DrawableGameComponent
{
	private SpriteBatch spriteBatch;

	private Texture2D signInProgress;

	private Color alphaColor = new Color(128, 128, 128, 0);

	private byte startalpha;

	private TimeSpan gt = TimeSpan.Zero;

	private TimeSpan last = TimeSpan.Zero;

	private int delay = 2;

	public MonoLiveGuide(Game game)
		: base(game)
	{
		Enabled = false;
		Visible = false;
		DrawOrder = int.MaxValue;
	}

	public override void Initialize()
	{
		base.Initialize();
	}

	private Texture2D Circle(GraphicsDevice graphics, int radius)
	{
		int num = radius * 2;
		Vector2 vector = new Vector2(radius, radius);
		Texture2D texture2D = new Texture2D(graphics, num, num, mipmap: false, SurfaceFormat.Color);
		Color[] array = new Color[num * num];
		for (int i = 0; i < array.Length; i++)
		{
			int num2 = (i + 1) % num;
			int num3 = (i + 1) / num;
			Vector2 vector2 = new Vector2(Math.Abs(vector.X - (float)num2), Math.Abs(vector.Y - (float)num3));
			if (Math.Sqrt(vector2.X * vector2.X + vector2.Y * vector2.Y) > (double)radius)
			{
				array[i] = Color.Transparent;
			}
			else
			{
				array[i] = Color.White;
			}
		}
		texture2D.SetData(array);
		return texture2D;
	}

	protected override void LoadContent()
	{
		spriteBatch = new SpriteBatch(Game.GraphicsDevice);
		signInProgress = Circle(Game.GraphicsDevice, 10);
		base.LoadContent();
	}

	protected override void UnloadContent()
	{
		base.UnloadContent();
	}

	public override void Draw(GameTime gameTime)
	{
		spriteBatch.Begin();
		Vector2 vector = new Vector2(Game.GraphicsDevice.Viewport.Width / 2, Game.GraphicsDevice.Viewport.Height - 100);
		Vector2 zero = Vector2.Zero;
		alphaColor.A = startalpha;
		for (int i = 0; i < 12; i++)
		{
			float num = (float)((double)i / 12.0 * Math.PI * 2.0);
			zero = new Vector2(vector.X + (float)Math.Cos(num) * 50f, vector.Y + (float)Math.Sin(num) * 50f);
			spriteBatch.Draw(signInProgress, zero, alphaColor);
			alphaColor.A += 21;
			if (alphaColor.A > byte.MaxValue)
			{
				alphaColor.A = 0;
			}
		}
		spriteBatch.End();
		base.Draw(gameTime);
	}

	public override void Update(GameTime gameTime)
	{
		if (gt == TimeSpan.Zero)
		{
			gt = (last = gameTime.TotalGameTime);
		}
		if ((gameTime.TotalGameTime - last).Milliseconds > 100)
		{
			last = gameTime.TotalGameTime;
			startalpha += 21;
		}
		if ((gameTime.TotalGameTime - gt).TotalSeconds > (double)delay)
		{
			string text = "iosuser";
			SignedInGamer signedInGamer = new SignedInGamer();
			signedInGamer.DisplayName = text;
			signedInGamer.Gamertag = text;
			signedInGamer.IsSignedInToLive = false;
			Gamer.SignedInGamers.Add(signedInGamer);
			Visible = false;
			Enabled = false;
			gt = TimeSpan.Zero;
		}
		base.Update(gameTime);
	}
}
