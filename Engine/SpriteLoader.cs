
using System;
using System.Xml;
using System.IO;

namespace Engine
{
	
	/// <summary>
	/// Class for loading sprite information into the resource manager
	/// </summary>
	public class SpriteLoader : IResourceLoader<SpriteDescriptor>
	{
		
		public SpriteLoader()
		{
		}
		
		public SpriteDescriptor LoadResource(string filename)
		{
			XmlTextReader reader = new XmlTextReader(filename);
			SpriteDescriptor sprite = new SpriteDescriptor();
		
			//Read XML tag and turn off whitespace handling (that is, don't read whitespaces as elements)
			reader.Read();
			reader.WhitespaceHandling = WhitespaceHandling.None;
			
			//Read the "sprite" tag
			if (!reader.Read())
			{
				throw new EndOfStreamException("Error loading resources: Could not read <resources> tag.");
			}
			if (reader.Name != "sprite")
			{
				throw new FormatException("Error loading resources: Unexpected tag: <" + reader.Name + ">");
			}
			
			while (reader.Read())
			{
				reader.MoveToElement();
				
				//Encountered end tag?
				if (reader.Name == "sprite")
				{
					if (reader.IsStartElement())
					{
						throw new FormatException("Unexpected tag: <sprite>");
					}
					break;
				}
				
				switch (reader.Name)
				{
				case "texture":
					sprite.TextureName = reader.GetAttribute("name");
					break;
				case "animation":
					string animationName = reader.GetAttribute("name");
					
					//Read frames
					while (reader.Read())
					{
						//Did we encounter the end tag for animation?
						if (reader.Name == "animation")
						{
							if (reader.IsStartElement())
							{
								throw new FormatException("Unexpected tag: <animation>");
							}
							break;
						}
						
						if (reader.Name != "frame")
						{
							throw new FormatException("Unexpected tag: " + reader.Name);
						}
						int x = Int32.Parse(reader.GetAttribute("x"));
						int y = Int32.Parse(reader.GetAttribute("y"));
						int delay = Int32.Parse(reader.GetAttribute("delay"));
						int nextframe = Int32.Parse(reader.GetAttribute("next"));
						int w = Int32.Parse(reader.GetAttribute("w"));
						int h = Int32.Parse(reader.GetAttribute("h"));
						
						sprite.AddFrame(animationName, x, y, w, h, delay, nextframe);
					}
					
					break;
				default:
					throw new FormatException("Unknown tag " + reader.Name);
				}
				
			}

			return sprite;
		}
	}
}
