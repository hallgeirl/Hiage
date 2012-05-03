using System;
using System.Collections.Generic;

using Engine;

namespace Mario
{
	public static class Helpers
	{
		static Dictionary<string, Sprite> SpriteToDictionary(string key, Sprite s)
		{
			Dictionary<string, Sprite> dict = new Dictionary<string, Sprite>();
			dict[key] = s;
			return dict;
		}
	}
}

