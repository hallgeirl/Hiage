
using System;
using System.Xml;
using System.IO;
using System.Collections.Generic;


namespace Engine
{

	/// <summary>
	/// Class for loading / managing resources, like textures
	/// </summary>
	public class ResourceManager
	{
		Dictionary<string, Resource<Texture>> textures = new Dictionary<string, Resource<Texture>>();
		Dictionary<string, Resource<SpriteDescriptor>> sprites = new Dictionary<string, Resource<SpriteDescriptor>>();
		
		/// <summary>
		/// Loadable resource. Loads when it's used.
		/// </summary>
		private class Resource<T>
		{
			private bool isLoaded = false;
			private string filename;
			private T resource;
			
			public Resource(string filename)
			{
				this.filename = filename;
			}
			
			//// <value>
			/// Has the resource been loaded?
			/// </value>
			public bool IsLoaded
			{
				get
				{
					return isLoaded;
				}
			}
			
			/// <summary>
			/// Load the resource
			/// </summary>
			/// <param name="resourceLoader">
			/// A <see cref="IResourceLoader"/>
			/// </param>
			public void Load(IResourceLoader<T> resourceLoader)
			{
				resource = resourceLoader.LoadResource(filename);
				isLoaded = true;
			}
			
			//// <value>
			/// Retrieve the actual resource (content)
			/// </value>
			public T Content
			{
				get
				{
					return resource;
				}
			}
		}
		
		
		/// <summary>
		/// Construct a ResourceManager object
		/// </summary>
		public ResourceManager()
		{
			
		}
		
		/// <summary>
		/// Load resources from a resource XML file.
		/// </summary>
		/// <param name="resourceXML">
		/// A <see cref="System.String"/>
		/// </param>
		public void LoadResources(string resourceXML)
		{
			XmlTextReader reader = new XmlTextReader(resourceXML);
			string directory = Path.GetDirectoryName(resourceXML);
			
			Log.Write("Loading resources from \"" + resourceXML + "\"");

			//Read XML tag and turn off whitespace handling (that is, don't read whitespaces as elements)
			reader.Read();
			reader.WhitespaceHandling = WhitespaceHandling.None;
			
			//Read the "resources" tag
			if (!reader.Read())
			{
				throw new EndOfStreamException("Error loading resources: Could not read <resources> tag.");
			}
			if (reader.Name != "resources")
			{
				throw new FormatException("Error loading resources: Unexpected tag: <" + reader.Name + ">");
			}

			//Read resource entries
			while (reader.Read())
			{
				reader.MoveToElement();
				
				if (reader.Name == "resources" && reader.IsStartElement())
				{
					throw new FormatException("Error loading resources: Duplicate <resources> start tag.");
				}
				else if (reader.Name == "resources" && !reader.IsStartElement())
				{
					break;
				}

				//Read resource tag
				string type = reader.GetAttribute("type");
				string name = reader.GetAttribute("name");
				string file = Path.Combine(directory, reader.GetAttribute("path"));
				
				switch (type)
				{
				case "texture":
					if (textures.ContainsKey(name))
					{
						Log.Write("Texture with name \"" + name + "\" already added. Resource ignored.", Log.WARNING);
					}
					else 
					{
						textures.Add(name, new Resource<Texture>(file));
					}
					break;
					
				case "sprite":
					if (sprites.ContainsKey(name))
					{
						Log.Write("Sprite with name \"" + name + "\" already added. Resource ignored.", Log.WARNING);
					}
					else
					{
						sprites.Add(name, new Resource<SpriteDescriptor>(file));
					}
					break;
					
				default:
					Log.Write("Unknown resource type: \"" + type + "\". Resource ignored.", Log.WARNING);
					break;
				}
				
				Log.Write("Added " + type + " resource with path \"" + file + "\"");
			}
			
		}
		
		/// <summary>
		/// Retrieve a texture. Load it if neccesary.
		/// </summary>
		/// <param name="name">
		/// A <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// A <see cref="Texture"/>
		/// </returns>
		public Texture GetTexture(string name)
		{
			if (textures.ContainsKey(name))
			{
				if (!textures[name].IsLoaded)
				{
					Log.Write("Texture \"" + name + "\" was not loaded. Loading it.");
					textures[name].Load(new TextureLoader());
				}
				return textures[name].Content;
			}
			throw new KeyNotFoundException("Texture with name " + name + " does not exist.");
		}
		
		public SpriteDescriptor GetSpriteDescriptor(string name)
		{
			if (sprites.ContainsKey(name))
			{
				if (!sprites[name].IsLoaded)
				{
					Log.Write("Sprite \"" + name + "\" was not loaded. Loading it.");
					sprites[name].Load(new SpriteLoader());
				}
				return sprites[name].Content;
			}
			throw new KeyNotFoundException("Sprite with name " + name + " does not exist.");
		}
	}
}
