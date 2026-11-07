using System;
using System.Collections.Concurrent;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Sexy;

public static class DiscreteResources
{
	public static ConcurrentDictionary<string, Image> mImageResources = new ConcurrentDictionary<string, Image>(8, 1000);

	public static void UnpackAtlases()
	{
		string text = "IMAGE_REANIM_";
		int num = 0;
		while (true)
		{
			string text2 = Path.GetDirectoryName("images_reanim/atlases/") + Constants.ImageSubPath + "reanim" + num;
			string text3 = "xnb";
			string text4 = "Content/" + text2.ToLower() + "." + text3.ToString().ToLower();
			string text5 = "Content/" + text2.ToLower() + ".json";
			Texture2D texture2D;
			JObject jObject;
			try
			{
				texture2D = GlobalStaticVars.gSexyAppBase.mContentManager.Load<Texture2D>(GlobalStaticVars.GetResourceDir() + text2.ToLower());
				using Stream stream = GlobalStaticVars.gPvZActivity.Assets.Open(text5.Replace('\\', '/'));
				jObject = (JObject)JToken.ReadFrom(new JsonTextReader(new StreamReader(stream)));
			}
			catch (Exception)
			{
				break;
			}
			Image theParent = new Image(texture2D, 0, 0, texture2D.Width, texture2D.Height);
			foreach (JProperty item in jObject["frames"].AsJEnumerable())
			{
				JToken value = item.Value;
				string name = item.Name;
				int s = value["frame"]["x"].Value<int>();
				int t = value["frame"]["y"].Value<int>();
				int theWidth = value["frame"]["w"].Value<int>();
				int theHeight = value["frame"]["h"].Value<int>();
				if (name.ToLower().EndsWith(".png"))
				{
					ConcurrentDictionary<string, Image> concurrentDictionary = mImageResources;
					string text6 = name;
					concurrentDictionary[text + text6.Substring(0, text6.Length - 4).ToUpper()] = new Image(theParent, s, t, theWidth, theHeight);
				}
			}
			num++;
		}
	}
}
