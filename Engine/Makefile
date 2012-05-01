# Source files
SOURCES =  \
	AssemblyInfo.cs \
	lib/FTclass.cs \
	src/Common/Box.cs \
	src/Common/Constants.cs \
	src/Common/Exceptions.cs \
	src/Common/Log.cs \
	src/Common/RandomGenerator.cs \
	src/Common/Timer.cs \
	src/Common/Vector.cs \
	src/GameLogic/CollisionManager.cs \
	src/GameLogic/Game.cs \
	src/GameLogic/ICollidable.cs \
	src/GameLogic/IGameState.cs \
	src/GameLogic/ParallaxBackground.cs \
	src/GameLogic/ParticleEngine.cs \
	src/GameLogic/SpatialGrid.cs \
	src/GameLogic/Sprite.cs \
	src/GameLogic/TileMap.cs \
	src/InputManager.cs \
	src/Rendering/Display.cs \
	src/Rendering/IRenderable.cs \
	src/Rendering/Renderer.cs \
	src/Rendering/Texture.cs \
	src/Resources/FontLoader.cs \
	src/Resources/IResourceLoader.cs \
	src/Resources/MapDescriptor.cs \
	src/Resources/MapLoader.cs \
	src/Resources/ObjectDescriptor.cs \
	src/Resources/ObjectLoader.cs \
	src/Resources/ResourceManager.cs \
	src/Resources/SpriteDescriptor.cs \
	src/Resources/SpriteLoader.cs \
	src/Resources/TextureLoader.cs \
	src/Resources/TilesetLoader.cs

# Referenced assemblies
ASSEMBLIES =  \
	/r:lib/SdlDotNet.dll \
	/r:lib/Tao.DevIl.dll \
	/r:lib/Tao.FreeType.dll \
	/r:lib/Tao.OpenGl.dll \
	/r:lib/Tao.Sdl.dll

PACKAGES = /r:System.Drawing

PROJECTS =

# Output
DLL=Hiage.dll
BIN=bin
ASSEMBLY=$(BIN)/$(DLL)
RELEASE=release




# Compilers and compiler options
CSC=mono-csc
CSFLAGS=-o+

all:$(ASSEMBLY) $(RELEASE)
	@echo "Copying $(ASSEMBLY) and dependencies to $(RELEASE)..."
	@cp $(ASSEMBLY) lib/*.dll lib/*.config $(RELEASE)

$(ASSEMBLY): $(BIN)
	@echo "Building $@..."
	@$(CSC) $(SOURCES) $(ASSEMBLIES) $(PACKAGES) /target:library /out:$@ 

$(BIN) $(RELEASE):
	mkdir -p $@

clean:
	rm -fr $(BIN) *.pidb $(RELEASE)


