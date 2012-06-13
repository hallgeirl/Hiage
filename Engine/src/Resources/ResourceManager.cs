
using System;
using System.Xml;
using System.IO;
using System.Collections.Generic;
using SdlDotNet.Audio;


namespace Engine
{

	/// <summary>
	/// Class for loading / managing resources, like textures
	/// </summary>
	public class ResourceManager
	{
		//The file that should be loaded first when loading resources
		public const string MainResourceFile = "resources.xml";
		
		//Resource references
		Dictionary<string, Resource<Texture>> 			textures = new Dictionary<string, Resource<Texture>>();
		Dictionary<string, Resource<SpriteDescriptor>> 	sprites  = new Dictionary<string, Resource<SpriteDescriptor>>();
		Dictionary<string, Resource<ISE.FTFont>>  		fonts 	 = new Dictionary<string, Resource<ISE.FTFont>>();
		Dictionary<string, Resource<Tileset>> 			tilesets = new Dictionary<string, Resource<Tileset>>();
		Dictionary<string, Resource<Sound>> 		    sounds   = new Dictionary<string, Resource<Sound>>();
		Dictionary<string, Resource<Music>> 		    music    = new Dictionary<string, Resource<Music>>();
		Dictionary<string, Resource<MapDescriptor>> 	tilemaps = new Dictionary<string, Resource<MapDescriptor>>();
		Dictionary<string, Resource<ObjectDescriptor>> 	objects  = new Dictionary<string, Resource<ObjectDescriptor>>();
		
		/// <summary>
		/// Loadable resource. Loads when it's used (Lazy loading).
		/// </summary>
		private class Resource<T>
		{
			private bool isLoaded = false;
			private string filename;
			private T resource;
			private string name;
			
			public Resource(string filename, string name)
			{
				this.filename = filename;
				this.name = name;
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
				resource = resourceLoader.LoadResource(filename, name);
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
		/// Recursively load all resources under the given path
		/// </summary>
		public void LoadResources(string path)
		{
			DirectoryInfo root = new DirectoryInfo(path);
			
			DirectoryInfo[] dirs = root.GetDirectories();
			
			FileInfo[] files = root.GetFiles("*.xml");
			
			foreach (var f in files)
			{
				//Check what kind of resource this is
				XmlDocument doc = new XmlDocument();
				doc.Load(f.FullName);
				
				foreach (XmlNode c in doc.ChildNodes)
				{
					if (c.NodeType == XmlNodeType.Element)
					{
						string name = "";
						try
						{
							name = c.Attributes["id"].Value;
						}
						catch (Exception)
						{
							if (c.Name != "resources")
							{
								Log.Write("Could not find id of resource in " + f.FullName, Log.WARNING);
								break;
							}
						}
						
						
						switch (c.Name)
						{
							case "sprite":
								if (sprites.ContainsKey(name))
									Log.Write("Sprite with name \"" + name + "\" already added. Resource ignored.", Log.WARNING);
								else
									sprites.Add(name, new Resource<SpriteDescriptor>(f.FullName, name));
								break;
								
							case "resources":
								LoadResourceXML(f.FullName);
								break;
								
							case "tileset":
							 	if (tilesets.ContainsKey(name))
									Log.Write("Tileset with name \"" + name + "\" already added. Resource ignored.", Log.WARNING);
								else
									tilesets.Add(name, new Resource<Tileset>(f.FullName, name));
								break;
			
							case "map":
							 	if (tilemaps.ContainsKey(name))
									Log.Write("Map with name \"" + name + "\" already added. Resource ignored.", Log.WARNING);
								else
									tilemaps.Add(name, new Resource<MapDescriptor>(f.FullName, name));
								break;
							
							case "object":
							 	if (objects.ContainsKey(name))
									Log.Write("Object with name \"" + name + "\" already added. Resource ignored.", Log.WARNING);
								else
									objects.Add(name, new Resource<ObjectDescriptor>(f.FullName, name));
								break;
							
							default:
								Log.Write("Unknown resource type: \"" + c.Name + "\". Resource ignored.", Log.WARNING);
								break;						}
					}
				}
			}
			
			//Load sub directories
			foreach (var dir in dirs)
			{
				LoadResources(Path.Combine(path, dir.Name));
			}
		}
		
		/// <summary>
		/// Load resources from a resource XML file.
		/// </summary>
		public void LoadResourceXML(string resourceXML)
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
				if (reader.NodeType == XmlNodeType.Comment)
				{
					continue;
				}
				
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
					case "sound":
						if (sounds.ContainsKey(name))
							Log.Write("Sound with name \"" + name + "\" already added. Resource ignored.", Log.WARNING);
						else
							sounds.Add(name, new Resource<Sound>(file, name));
						break;
					
					case "music":
						if (music.ContainsKey(name))
							Log.Write("Music with name \"" + name + "\" already added. Resource ignored.", Log.WARNING);
						else
							music.Add(name, new Resource<Music>(file, name));
						break;
					
					case "texture":
						if (textures.ContainsKey(name))
							Log.Write("Texture with name \"" + name + "\" already added. Resource ignored.", Log.WARNING);
						else 
							textures.Add(name, new Resource<Texture>(file, name));
						break;
						
					case "font":
						if (fonts.ContainsKey(name))
							Log.Write("Font with name \"" + name + "\" already added. Resource ignored.", Log.WARNING);
						else
							fonts.Add(name, new Resource<ISE.FTFont>(file, name));
						break;
	
					default:
						Log.Write("Invalid resource type for resource library file: \"" + type + "\". Resource ignored.", Log.WARNING);
						break;
				}
				
				Log.Write("Added " + type + " resource with path \"" + file + "\"");
			}
			
		}
		
		#region Resource retrieval
		
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
		
		public ISE.FTFont GetFont(string name)
		{
			if (fonts.ContainsKey(name))
			{
				if (!fonts[name].IsLoaded)
				{
					fonts[name].Load(new FontLoader());
				}
				return fonts[name].Content;
			}
			throw new KeyNotFoundException("Font with name " + name + " does not exist.");
		}
		
		public Tileset GetTileset(string name)
		{
			if (tilesets.ContainsKey(name))
			{
				if (!tilesets[name].IsLoaded)
				{
					Log.Write("Tileset \"" + name + "\" was not loaded. Loading it.");
					tilesets[name].Load(new TilesetLoader());
				}
				return tilesets[name].Content;
			}
			throw new KeyNotFoundException("Tileset with name " + name + " does not exist.");
		}
		
		public MapDescriptor GetMapDescriptor(string name)
		{
			if (tilemaps.ContainsKey(name))
			{
				if (!tilemaps[name].IsLoaded)
				{
					Log.Write("Tilemap \"" + name + "\" was not loaded. Loading it.");
					tilemaps[name].Load(new MapLoader());
				}
				return tilemaps[name].Content;
			}
			throw new KeyNotFoundException("Tilemap with name " + name + " does not exist.");
		}
		
		public ObjectDescriptor GetObjectDescriptor(string name)
		{
			if (objects.ContainsKey(name))
			{
				if (!objects[name].IsLoaded)
				{
					Log.Write("Object \"" + name + "\" was not loaded. Loading it.");
					objects[name].Load(new ObjectLoader());
				}
				return objects[name].Content;
			}
			throw new KeyNotFoundException("Object with name " + name + " does not exist.");
		}
		
		public Sound GetAudioClip(string name)
		{
			if (sounds.ContainsKey(name))
			{
				if (!sounds[name].IsLoaded)
				{
					Log.Write("Audio clip \"" + name + "\" was not loaded. Loading it.");
					sounds[name].Load(new AudioClipLoader());
				}
				return sounds[name].Content;
			}
			throw new KeyNotFoundException("Audio clip with name " + name + " does not exist.");
		}
		
		public Music GetMusic(string name)
		{
			if (music.ContainsKey(name))
			{
				if (!music[name].IsLoaded)
				{
					Log.Write("Music \"" + name + "\" was not loaded. Loading it.");
					music[name].Load(new MusicLoader());
				}
				return music[name].Content;
			}
			throw new KeyNotFoundException("Music with name " + name + " does not exist.");
		}
		
		//The list of all tileset names
		public IEnumerable<string> Tilesets
		{
			get 
			{ 
				return tilesets.Keys; 
			}
		}
		
		//List of all object names
		public IEnumerable<string> Objects
		{
			get
			{
				return objects.Keys;
			}
		}
		
		public IEnumerable<string> Textures
		{
			get
			{
				return textures.Keys;
			}
		}
		
		#endregion
	}
}
