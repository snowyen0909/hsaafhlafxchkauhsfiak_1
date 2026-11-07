using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Xna.Framework.Content;

namespace Sexy.TodLib;

internal class ReanimXmlReader : ContentTypeReader<ReanimatorDefinition>
{
	internal enum ReanimScaleType
	{
		NoScale = 0,
		InvertAndScale = 1,
		ScaleFromPC = 255
	}

	private static ReanimatorTransform previous;

	public static float DEG_TO_RAD = (float)Math.PI / 180f;

	protected override ReanimatorDefinition Read(ContentReader input, ReanimatorDefinition existingInstance)
	{
		ReanimatorDefinition reanimatorDefinition = new ReanimatorDefinition();
		CustomContentReader customContentReader = new CustomContentReader(input);
		string path = customContentReader.ReadString();
		string empty = string.Empty;
		XDocument xDocument;
		using (Stream stream = GlobalStaticVars.gPvZActivity.Assets.Open(Path.Combine(input.ContentManager.RootDirectory, path)))
		{
			StreamReader streamReader = new StreamReader(stream);
			xDocument = XDocument.Parse("<reanim>" + streamReader.ReadToEnd() + "</reanim>");
		}
		string text = xDocument.Root.Element("doScale")?.Value;
		ReanimScaleType doScale = ((text != null) ? ((ReanimScaleType)int.Parse(text)) : ReanimScaleType.ScaleFromPC);
		reanimatorDefinition.mFPS = float.Parse(xDocument.Root.Element("fps").Value);
		IEnumerable<XElement> enumerable = xDocument.Root.Elements("track");
		reanimatorDefinition.mTrackCount = (short)enumerable.Count();
		reanimatorDefinition.mTracks = new ReanimatorTrack[reanimatorDefinition.mTrackCount];
		IEnumerator<XElement> enumerator = enumerable.GetEnumerator();
		for (int i = 0; i < reanimatorDefinition.mTrackCount; i++)
		{
			enumerator.MoveNext();
			ReadReanimTrack(enumerator.Current, doScale, out var track);
			reanimatorDefinition.mTracks[i] = track;
		}
		return reanimatorDefinition;
	}

	private void ReadReanimTrack(XElement input, ReanimScaleType doScale, out ReanimatorTrack track)
	{
		string value = input.Element("name").Value;
		IEnumerable<XElement> enumerable = input.Elements("t");
		int transformCount = enumerable.Count();
		track = new ReanimatorTrack(value, transformCount);
		IEnumerator<XElement> enumerator = enumerable.GetEnumerator();
		previous = GetDefault();
		for (int i = 0; i < track.mTransformCount; i++)
		{
			enumerator.MoveNext();
			ReadReanimTransform(enumerator.Current, doScale, out var transform);
			track.mTransforms[i] = transform;
		}
	}

	private static ReanimatorTransform GetDefault()
	{
		ReanimatorTransform reanimatorTransformForLoadingThread = ReanimatorTransform.GetReanimatorTransformForLoadingThread();
		reanimatorTransformForLoadingThread.mFontName = string.Empty;
		reanimatorTransformForLoadingThread.mImageName = string.Empty;
		reanimatorTransformForLoadingThread.mText = string.Empty;
		reanimatorTransformForLoadingThread.mAlpha = ReanimatorXnaHelpers.DEFAULT_FIELD_PLACEHOLDER;
		reanimatorTransformForLoadingThread.mFrame = ReanimatorXnaHelpers.DEFAULT_FIELD_PLACEHOLDER;
		reanimatorTransformForLoadingThread.mScaleX = ReanimatorXnaHelpers.DEFAULT_FIELD_PLACEHOLDER;
		reanimatorTransformForLoadingThread.mScaleY = ReanimatorXnaHelpers.DEFAULT_FIELD_PLACEHOLDER;
		reanimatorTransformForLoadingThread.mSkewX = ReanimatorXnaHelpers.DEFAULT_FIELD_PLACEHOLDER;
		reanimatorTransformForLoadingThread.mSkewY = ReanimatorXnaHelpers.DEFAULT_FIELD_PLACEHOLDER;
		reanimatorTransformForLoadingThread.mSkewXCos = ReanimatorXnaHelpers.DEFAULT_FIELD_PLACEHOLDER;
		reanimatorTransformForLoadingThread.mSkewXSin = ReanimatorXnaHelpers.DEFAULT_FIELD_PLACEHOLDER;
		reanimatorTransformForLoadingThread.mSkewYCos = ReanimatorXnaHelpers.DEFAULT_FIELD_PLACEHOLDER;
		reanimatorTransformForLoadingThread.mSkewYSin = ReanimatorXnaHelpers.DEFAULT_FIELD_PLACEHOLDER;
		reanimatorTransformForLoadingThread.mTransX = ReanimatorXnaHelpers.DEFAULT_FIELD_PLACEHOLDER;
		reanimatorTransformForLoadingThread.mTransY = ReanimatorXnaHelpers.DEFAULT_FIELD_PLACEHOLDER;
		return reanimatorTransformForLoadingThread;
	}

	private void ReadReanimTransform(XElement input, ReanimScaleType doScale, out ReanimatorTransform transform)
	{
		previous = GetDefault();
		transform = GetDefault();
		transform.mFontName = input.Element("font")?.Value ?? previous.mFontName;
		transform.mImageName = input.Element("i")?.Value ?? previous.mImageName;
		transform.mText = input.Element("text")?.Value ?? previous.mText;
		transform.mAlpha = (float.TryParse(input.Element("a")?.Value, out transform.mAlpha) ? transform.mAlpha : previous.mAlpha);
		transform.mFrame = (float.TryParse(input.Element("f")?.Value, out transform.mFrame) ? transform.mFrame : previous.mFrame);
		transform.mScaleX = (float.TryParse(input.Element("sx")?.Value, out transform.mScaleX) ? transform.mScaleX : previous.mScaleX);
		transform.mScaleY = (float.TryParse(input.Element("sy")?.Value, out transform.mScaleY) ? transform.mScaleY : previous.mScaleY);
		transform.mSkewX = (float.TryParse(input.Element("kx")?.Value, out transform.mSkewX) ? transform.mSkewX : previous.mSkewX);
		transform.mSkewY = (float.TryParse(input.Element("ky")?.Value, out transform.mSkewY) ? transform.mSkewY : previous.mSkewY);
		float result = ReanimatorXnaHelpers.DEFAULT_FIELD_PLACEHOLDER;
		float result2 = ReanimatorXnaHelpers.DEFAULT_FIELD_PLACEHOLDER;
		bool flag = float.TryParse(input.Element("x")?.Value, out result);
		bool flag2 = float.TryParse(input.Element("y")?.Value, out result2);
		switch (doScale)
		{
		case ReanimScaleType.InvertAndScale:
			transform.mTransX = ((result == ReanimatorXnaHelpers.DEFAULT_FIELD_PLACEHOLDER) ? result : Constants.InvertAndScale(result));
			transform.mTransY = ((result2 == ReanimatorXnaHelpers.DEFAULT_FIELD_PLACEHOLDER) ? result2 : Constants.InvertAndScale(result2));
			break;
		case ReanimScaleType.ScaleFromPC:
			transform.mTransX = ((result == ReanimatorXnaHelpers.DEFAULT_FIELD_PLACEHOLDER) ? result : (Constants.S * result));
			transform.mTransY = ((result2 == ReanimatorXnaHelpers.DEFAULT_FIELD_PLACEHOLDER) ? result2 : (Constants.S * result2));
			break;
		default:
			transform.mTransX = result;
			transform.mTransY = result2;
			break;
		}
		if (!flag)
		{
			transform.mTransX = previous.mTransX;
		}
		if (!flag2)
		{
			transform.mTransY = previous.mTransY;
		}
		previous = transform;
	}
}
