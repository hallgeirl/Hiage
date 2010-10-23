
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
		
		public SpriteDescriptor LoadResource(string filename, string name)
		{
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load(filename);
			SpriteDescriptor sprite;
			
			
			//Get the texture node
			XmlNode textureNode = xmlDoc.SelectSingleNode("/sprite/texture");
			sprite = new SpriteDescriptor(textureNode.InnerText);
			
			//And any default values
			XmlNode xmlTmp = xmlDoc.SelectSingleNode("/sprite/defaults/frame-width");
			int defaultFrameWidth = (xmlTmp != null ? int.Parse(xmlTmp.InnerText) : 0);
			xmlTmp = xmlDoc.SelectSingleNode("/sprite/defaults/frame-height");
			int defaultFrameHeight = (xmlTmp != null ? int.Parse(xmlTmp.InnerText) : 0);
			xmlTmp = xmlDoc.SelectSingleNode("/sprite/defaults/frame-delay");
			double defaultFrameDelay = (xmlTmp != null ? double.Parse(xmlTmp.InnerText) : 0);
			
			//And all animation nodes
			foreach (XmlNode animNode in xmlDoc.SelectNodes("/sprite/animation"))
			{
				string animationName = "";
				try
				{
					animationName = animNode.SelectSingleNode("name").InnerText;
				}
				catch (NullReferenceException e)
				{
					throw new NullReferenceException("Unable to find name for animation in " + filename, e);
				}
				
				//And finally all frame nodes
				foreach (XmlNode frameNode in animNode.SelectNodes("frame"))
				{
					int x = 0, y = 0, next = 0, width = defaultFrameWidth, height = defaultFrameHeight;
					double delay = defaultFrameDelay;
					x = int.Parse(frameNode.SelectSingleNode("@x").InnerText);
					y = int.Parse(frameNode.SelectSingleNode("@y").InnerText);
					try { delay = double.Parse(frameNode.SelectSingleNode("@delay").InnerText); } catch (NullReferenceException){}
					next = int.Parse(frameNode.SelectSingleNode("@next").InnerText);
					try { width = int.Parse(frameNode.SelectSingleNode("@w").InnerText); } catch (NullReferenceException){}
					try { height = int.Parse(frameNode.SelectSingleNode("@h").InnerText); } catch (NullReferenceException){}
					
					Log.Write("Added frame " + animationName + " " + x + " " + y + " " + width + " " + height);
					sprite.AddFrame(animationName, x, y, width, height, delay, next);
				}
				
			}

			return sprite;
		}
	}
}
