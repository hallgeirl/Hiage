CONFIG=DEBUG_X86

# Source files
SOURCES =  \
	AssemblyInfo.cs \
	lib/FTclass.cs \
	src/Audio/AudioManager.cs \
	src/Common/Box.cs \
	src/Common/Constants.cs \
	src/Common/Exceptions.cs \
	src/Common/Log.cs \
	src/Common/RandomGenerator.cs \
	src/Common/Timer.cs \
	src/Common/Vector.cs \
	src/EntitySystem/Components/CollidableComponent.cs \
	src/EntitySystem/Components/CollisionResponseComponent.cs \
	src/EntitySystem/Components/DrawableComponent.cs \
	src/EntitySystem/Components/FrictionComponent.cs \
	src/EntitySystem/Components/GravityComponent.cs \
	src/EntitySystem/Components/MotionComponent.cs \
	src/EntitySystem/Components/RendererComponent.cs \
	src/EntitySystem/Components/SpriteComponent.cs \
	src/EntitySystem/Components/TransformComponent.cs \
	src/EntitySystem/GameObject.cs \
	src/EntitySystem/GOComponent.cs \
	src/Exceptions/LoggedException.cs \
	src/GameLogic/CollisionManager.cs \
	src/GameLogic/Game.cs \
	src/GameLogic/GameState.cs \
	src/GameLogic/ICollidable.cs \
	src/GameLogic/ParallaxBackground.cs \
	src/GameLogic/ParticleEngine.cs \
	src/GameLogic/SpatialGrid.cs \
	src/GameLogic/TileMap.cs \
	src/Input/InputManager.cs \
	src/MessagePassing/IMessageRecipient.cs \
	src/MessagePassing/Message.cs \
	src/MessagePassing/Messages/AccellerationChangedMessage.cs \
	src/MessagePassing/Messages/CollisionEventMessage.cs \
	src/MessagePassing/Messages/ObjectObjectCollisionEventMessage.cs \
	src/MessagePassing/Messages/PlayAnimationMessage.cs \
	src/MessagePassing/Messages/PositionChangedMessage.cs \
	src/MessagePassing/Messages/RenderableChangedMessage.cs \
	src/MessagePassing/Messages/RequestMessage.cs \
	src/MessagePassing/Messages/ResponseMessage.cs \
	src/MessagePassing/Messages/ScalingChangedMessage.cs \
	src/MessagePassing/Messages/VelocityChangedMessage.cs \
	src/MessagePassing/Property.cs \
	src/MessagePassing/VelocityProperty.cs \
	src/Rendering/Display.cs \
	src/Rendering/IRenderable.cs \
	src/Rendering/Renderer.cs \
	src/Rendering/Sprite.cs \
	src/Rendering/Texture.cs \
	src/Resources/IResourceLoader.cs \
	src/Resources/Loaders/AudioClipLoader.cs \
	src/Resources/Loaders/FontLoader.cs \
	src/Resources/Loaders/MapLoader.cs \
	src/Resources/Loaders/MusicLoader.cs \
	src/Resources/Loaders/ObjectLoader.cs \
	src/Resources/Loaders/SpriteLoader.cs \
	src/Resources/Loaders/TextureLoader.cs \
	src/Resources/Loaders/TilesetLoader.cs \
	src/Resources/MapDescriptor.cs \
	src/Resources/ObjectDescriptor.cs \
	src/Resources/ResourceManager.cs \
	src/Resources/SpriteDescriptor.cs

# Referenced assemblies
ASSEMBLIES =  \
	/r:lib/SdlDotNet.dll \
	/r:lib/Tao.DevIl.dll \
	/r:lib/Tao.FreeType.dll \
	/r:lib/Tao.OpenGl.dll \
	/r:lib/Tao.Sdl.dll

PACKAGES =  \
	/r:System \
	/r:System.Drawing \
	/r:System.Xml

PROJECTS =

# Output
DLL=Hiage.dll
RELEASE=release
ASSEMBLY=$(RELEASE)/$(DLL)

# Compilers and compiler options
CSC=dmcs
CSFLAGS+=/noconfig /codepage:utf8 /warn:4

ifeq ($(CONFIG), DEBUG_X86)
CSFLAGS+=/optimize- /debug /define:"DEBUG"
endif

ifeq (CONFIG, RELEASE_X86)
CSFLAGS+=/optimize+
endif

all:$(ASSEMBLY)
	@mkdir -p $(RELEASE)
	@cp lib/*.dll lib/*.config $(RELEASE)
	@echo "Built $(ASSEMBLY)."

$(ASSEMBLY): $(SOURCES)
	@mkdir -p $(RELEASE)
	@echo "Building $@..."
	$(CSC) $(CSFLAGS) $(SOURCES) $(ASSEMBLIES) $(PACKAGES) /target:library /out:$@ 
	@touch $(RELEASE)/last_build_tag

clean:
	rm -fr $(BIN) *.pidb $(RELEASE)


