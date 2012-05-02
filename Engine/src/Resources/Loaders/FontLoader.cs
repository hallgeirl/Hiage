
using System;

namespace Engine
{
	
	
	public class FontLoader : IResourceLoader<ISE.FTFont>
	{
		public ISE.FTFont LoadResource(string filename, string name)
		{
			int errors;
			ISE.FTFont font = new ISE.FTFont(filename, out errors);
			font.ftRenderToTexture(Renderer.BASE_FONT_SIZE, 196);
			
			return font;
		}
	}
}
