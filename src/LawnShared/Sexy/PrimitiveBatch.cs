using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Sexy;

public class PrimitiveBatch : IDisposable
{
	private const float farZPlane = 1f;

	private VertexPositionColor[] vertices = new VertexPositionColor[1024];

	private VertexPositionColorTexture[] texturedVertices = new VertexPositionColorTexture[16384];

	private short[] indices = new short[16384];

	private int positionInBuffer;

	private int positionInIndexBuffer;

	public int primitiveCount;

	private BasicEffect basicEffect;

	private GraphicsDevice device;

	private PrimitiveType primitiveType;

	private int numVertsPerPrimitive;

	private bool hasBegun;

	private bool isDisposed;

	private Image texture;

	public int OffsetX;

	public int OffsetY;

	public Matrix Transform;

	private int screenWidth;

	private int screenHeight;

	private bool mHasTransform = true;

	private SamplerState lastUsedSamplerState;

	private VertexPositionColorTexture vertex = new VertexPositionColorTexture(default, default, default);

	private Matrix t;

	private RasterizerState rasterizerState = new RasterizerState
	{
		CullMode = CullMode.None
	};

	private int VerticesLength
	{
		get
		{
			if (texture != null)
			{
				return texturedVertices.Length;
			}
			return vertices.Length;
		}
	}

	public bool HasBegun => hasBegun;

	public Image Texture
	{
		get
		{
			return texture;
		}
		set
		{
			if ((texture != null || value != null) && (texture == null || value == null || texture.Texture != value.Texture))
			{
				Flush();
				texture = value;
				if (texture != null)
				{
					basicEffect.TextureEnabled = true;
					basicEffect.Texture = texture.Texture;
				}
				else
				{
					basicEffect.TextureEnabled = false;
					basicEffect.Texture = null;
				}
				basicEffect.CurrentTechnique.Passes[0].Apply();
			}
		}
	}

	public PrimitiveBatch(GraphicsDevice graphicsDevice)
	{
		if (graphicsDevice == null)
		{
			throw new ArgumentNullException("graphicsDevice");
		}
		device = graphicsDevice;
		basicEffect = new BasicEffect(graphicsDevice);
		basicEffect.VertexColorEnabled = true;
		basicEffect.LightingEnabled = false;
		basicEffect.FogEnabled = false;
	}

	public void SetupMatrices()
	{
		ScreenScales mScreenScales = GlobalStaticVars.gSexyAppBase.mScreenScales;
		float num = (float)mScreenScales.mTransX * (float)Constants.BackBufferSize.X / (float)device.PresentationParameters.BackBufferHeight;
		float num2 = (float)mScreenScales.mTransY * (float)Constants.BackBufferSize.Y / (float)device.PresentationParameters.BackBufferWidth;
		float num3 = (float)device.PresentationParameters.BackBufferWidth / mScreenScales.mScaleFactor - (float)Constants.BackBufferSize.Y - num;
		float num4 = (float)device.PresentationParameters.BackBufferHeight / mScreenScales.mScaleFactor - (float)Constants.BackBufferSize.X - num2;
		basicEffect.View = Matrix.CreateOrthographicOffCenter(0f - num, (float)Constants.BackBufferSize.Y + num3, (float)Constants.BackBufferSize.X + num4, 0f - num2, 0f, 1f);
		screenWidth = device.PresentationParameters.BackBufferWidth;
		screenHeight = device.PresentationParameters.BackBufferHeight;
	}

	public void SetupMatrices(int width, int height, float virtualWidth, float virtualHeight)
	{
		basicEffect.View = Matrix.CreateOrthographicOffCenter(0f, virtualWidth, virtualHeight, 0f, 0f, 1f);
		screenWidth = width;
		screenHeight = height;
	}

	public void Draw(Image img, TRect destination, TRect source, Color colour, bool extraOffset, bool sourceOffsetsUsed)
	{
		Matrix? transform = Transform;
		Draw(img, destination, source, ref transform, Vector2.Zero, colour, extraOffset, sourceOffsetsUsed, PrimitiveBatchEffects.None);
	}

	public void Draw(Image img, TRect destination, TRect source, Color colour, bool extraOffset, bool sourceOffsetsUsed, PrimitiveBatchEffects effects)
	{
		Matrix? transform = Transform;
		Draw(img, destination, source, ref transform, Vector2.Zero, colour, extraOffset, sourceOffsetsUsed, effects);
	}

	public void Draw(Image img, TRect destination, TRect source, ref Matrix transform, Vector2 center, Color colour, bool extraOffset, bool sourceOffsetsUsed)
	{
		Matrix? transform2 = (Matrix?)transform * (Matrix?)Transform;
		Draw(img, destination, source, ref transform2, center, colour, extraOffset, sourceOffsetsUsed, PrimitiveBatchEffects.None);
	}

	public void DrawRotatedScaled(Image img, TRect destination, TRect source, Vector2 center, float rotation, Vector2 scale, Color colour, bool extraOffset, bool sourceOffsetsUsed, PrimitiveBatchEffects effects)
	{
		Matrix? matrix = Matrix.CreateTranslation(-OffsetX, -OffsetY, 0f) * Matrix.CreateScale(scale.X, scale.Y, 1f) * Matrix.CreateRotationZ(rotation) * Matrix.CreateTranslation(destination.mX, destination.mY, 0f);
		destination.mX = 0;
		destination.mY = 0;
		destination.mWidth = source.mWidth;
		destination.mHeight = source.mHeight;
		matrix *= Transform;
		Draw(img, destination, source, ref matrix, center, colour, extraOffset: false, sourceOffsetsUsed, effects);
	}

	public void Draw(Image img, TRect destination, TRect source, ref Matrix? transform, Vector2 center, Color colour, bool extraOffset, bool sourceOffsetsUsed, PrimitiveBatchEffects effects)
	{
		if (!destination.IsEmpty)
		{
			if (transform.HasValue)
			{
				Matrix value = transform.Value;
				value.M41 += value.M11 * (0f - center.X) + value.M21 * (0f - center.Y);
				value.M42 += value.M12 * (0f - center.X) + value.M22 * (0f - center.Y);
				Transform = value;
				mHasTransform = true;
			}
			else
			{
				mHasTransform = false;
			}
			Texture = img;
			vertex.Color = colour;
			if (extraOffset)
			{
				destination.mX -= OffsetX;
				destination.mY -= OffsetY;
			}
			float num = source.mX;
			float num2 = source.mY;
			if (sourceOffsetsUsed)
			{
				num = (float)source.mX + (float)img.mS / img.mInvScale;
				num2 = (float)source.mY + (float)img.mT / img.mInvScale;
			}
			bool flag = effects == PrimitiveBatchEffects.MirrorHorizontally;
			bool flag2 = effects == PrimitiveBatchEffects.MirrorVertically;
			float y = (num2 + (float)(flag2 ? source.mHeight : 0)) / (float)img.Texture.Height * img.mInvScale;
			float y2 = (num2 + (float)((!flag2) ? source.mHeight : 0)) / (float)img.Texture.Height * img.mInvScale;
			float x = (num + (float)(flag ? source.mWidth : 0)) / (float)img.Texture.Width * img.mInvScale;
			float x2 = (num + (float)((!flag) ? source.mWidth : 0)) / (float)img.Texture.Width * img.mInvScale;
			float z = 0f;
			vertex.Position = new Vector3(destination.mX, destination.mY, z);
			vertex.TextureCoordinate = new Vector2(x, y);
			AddVertex(ref vertex);
			short num3 = (short)(positionInBuffer - 1);
			vertex.Position = new Vector3(destination.mX + destination.mWidth, destination.mY, z);
			vertex.TextureCoordinate = new Vector2(x2, y);
			AddVertex(ref vertex);
			short num4 = (short)(positionInBuffer - 1);
			vertex.Position = new Vector3(destination.mX, destination.mY + destination.mHeight, z);
			vertex.TextureCoordinate = new Vector2(x, y2);
			AddVertex(ref vertex);
			short num5 = (short)(positionInBuffer - 1);
			vertex.Position = new Vector3(destination.mX + destination.mWidth, destination.mY + destination.mHeight, z);
			vertex.TextureCoordinate = new Vector2(x2, y2);
			AddVertex(ref vertex);
			short num6 = (short)(positionInBuffer - 1);
			if (positionInIndexBuffer + 6 >= indices.Length)
			{
				Flush();
			}
			indices[positionInIndexBuffer++] = num3;
			indices[positionInIndexBuffer++] = num4;
			indices[positionInIndexBuffer++] = num5;
			indices[positionInIndexBuffer++] = num4;
			indices[positionInIndexBuffer++] = num6;
			indices[positionInIndexBuffer++] = num5;
			primitiveCount += 2;
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (disposing && !isDisposed)
		{
			if (basicEffect != null)
			{
				basicEffect.Dispose();
			}
			isDisposed = true;
		}
	}

	public void Restart()
	{
		Begin(primitiveType, OffsetX, OffsetY, Transform, Texture, lastUsedSamplerState);
	}

	public void Begin(PrimitiveType primitiveType, int offsetX, int offsetY, Matrix? transform, Image texture, SamplerState st)
	{
		GlobalStaticVars.g.GraphicsDevice.RasterizerState = rasterizerState;
		if (hasBegun)
		{
			throw new InvalidOperationException("End must be called before Begin can be called again.");
		}
		if (screenWidth == device.PresentationParameters.BackBufferWidth)
		{
			_ = screenHeight;
			_ = device.PresentationParameters.BackBufferHeight;
		}
		if (primitiveType == PrimitiveType.LineStrip || primitiveType == PrimitiveType.TriangleStrip)
		{
			throw new NotSupportedException("The specified primitiveType is not supported by PrimitiveBatch.");
		}
		hasBegun = true;
		lastUsedSamplerState = st;
		if (st != null)
		{
			device.SamplerStates[0] = st;
		}
		this.primitiveType = primitiveType;
		numVertsPerPrimitive = NumVertsPerPrimitive(primitiveType);
		Texture = texture;
		OffsetX = offsetX;
		OffsetY = offsetY;
		if (transform.HasValue)
		{
			Transform = transform.Value;
		}
		else
		{
			Transform = Matrix.Identity;
		}
		basicEffect.CurrentTechnique.Passes[0].Apply();
	}

	public void AddTriVertices(TriVertex[,] vertices, int triangleCount, Color? theColor)
	{
		for (int i = 0; i < triangleCount; i++)
		{
			for (int j = 0; j < 3; j++)
			{
				if (theColor.HasValue)
				{
					vertices[i, j].color = theColor.Value;
				}
				AddVertex(ref vertices[i, j].mVert);
				indices[positionInIndexBuffer++] = (short)(positionInBuffer - 1);
			}
			primitiveCount++;
		}
	}

	public void AddVertex(Vector2 vertex, Color color)
	{
		if (!hasBegun)
		{
			throw new InvalidOperationException("Begin must be called before AddVertex can be called.");
		}
		if (positionInBuffer % numVertsPerPrimitive == 0 && positionInBuffer + numVertsPerPrimitive >= VerticesLength)
		{
			Flush();
		}
		vertex.X += OffsetX;
		vertex.Y += OffsetY;
		if (mHasTransform)
		{
			Vector2.Transform(ref vertex, ref Transform, out vertex);
		}
		if (texture == null)
		{
			vertices[positionInBuffer].Position = new Vector3(vertex.X, vertex.Y, 0f);
			vertices[positionInBuffer].Color = color;
		}
		else
		{
			texturedVertices[positionInBuffer].Position = new Vector3(vertex.X, vertex.Y, 0f);
			texturedVertices[positionInBuffer].Color = color;
			texturedVertices[positionInBuffer].TextureCoordinate = Vector2.Zero;
		}
		positionInBuffer++;
	}

	public void AddVertex(ref VertexPositionColorTexture vertex)
	{
		if (!hasBegun)
		{
			throw new InvalidOperationException("Begin must be called before AddVertex can be called.");
		}
		if (positionInBuffer % numVertsPerPrimitive == 0 && positionInBuffer + numVertsPerPrimitive >= VerticesLength)
		{
			Flush();
		}
		Vector3 position = vertex.Position;
		position.X += OffsetX;
		position.Y += OffsetY;
		if (mHasTransform)
		{
			Vector3.Transform(ref position, ref Transform, out position);
		}
		position.X -= 0.5f;
		position.Y -= 0.5f;
		vertex.Position = position;
		if (texture == null)
		{
			vertices[positionInBuffer].Position = vertex.Position;
			vertices[positionInBuffer].Color = vertex.Color;
		}
		else
		{
			texturedVertices[positionInBuffer] = vertex;
		}
		positionInBuffer++;
	}

	public void End()
	{
		if (!hasBegun)
		{
			throw new InvalidOperationException("Begin must be called before End can be called.");
		}
		Flush();
		hasBegun = false;
	}

	private void Flush()
	{
		if (!hasBegun)
		{
			throw new InvalidOperationException("Begin must be called before Flush can be called.");
		}
		if (positionInBuffer != 0)
		{
			if (texture == null)
			{
				device.DrawUserIndexedPrimitives(primitiveType, vertices, 0, positionInBuffer, indices, 0, primitiveCount);
			}
			else
			{
				device.DrawUserIndexedPrimitives(primitiveType, texturedVertices, 0, positionInBuffer, indices, 0, primitiveCount);
			}
			positionInBuffer = 0;
			positionInIndexBuffer = 0;
			primitiveCount = 0;
		}
	}

	private static int NumVertsPerPrimitive(PrimitiveType primitive)
	{
		return primitive switch
		{
			PrimitiveType.TriangleList => 3, 
			PrimitiveType.LineList => 2, 
			_ => throw new InvalidOperationException("primitive is not valid"), 
		};
	}
}
