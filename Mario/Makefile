CONFIG=DEBUG_X86

# Source files
SOURCES =  \
	src/Camera.cs \
	src/CollisionHandlers/InAirCollisionResponseComponent.cs \
	src/CollisionHandlers/PhysicalObjectCollisionResponseComponent.cs \
	src/Components/ControllerInterfaceComponent.cs \
	src/Components/GroundEnemyInterfaceComponent.cs \
	src/Components/MarioInterfaceComponent.cs \
	src/Components/ObjectPropertiesComponent.cs \
	src/Components/ObjectStates/InAirState.cs \
	src/Components/ObjectStates/MovementAnimationState.cs \
	src/Components/ObjectStates/ObjectState.cs \
	src/Components/ObjectStates/RunState.cs \
	src/Components/ObjectStates/StandState.cs \
	src/Components/ObjectStates/WalkState.cs \
	src/Components/SpeedLimitComponent.cs \
	src/Components/StateMachineComponent.cs \
	src/Controllers/ControllerComponent.cs \
	src/Controllers/DebugController.cs \
	src/Controllers/DumbGroundAIComponent.cs \
	src/Controllers/MarioDiesController.cs \
	src/Controllers/PlayerController.cs \
	src/GameStates/DeadState.cs \
	src/GameStates/LevelState.cs \
	src/GameStates/MarioGameState.cs \
	src/Helpers.cs \
	src/Main.cs \
	src/Messages/CollidedWithGroundMessage.cs \
	src/Messages/ControlIssuedMessage.cs \
	src/Messages/InAirMessage.cs \
	src/Messages/LandedMessage.cs \
	src/Messages/SetStateMessage.cs \
	src/ObjectFactory.cs \
	src/Objects/BasicGroundEnemy.cs \
	src/Objects/Character.cs \
	src/Objects/Coin.cs \
	src/Objects/GameObjectComponent.cs \
	src/Objects/Icon.cs \
	src/Objects/Mushroom.cs \
	src/Objects/PhysicalObject.cs \
	src/Objects/Player.cs \
	src/Physics.cs \
	src/PlayerState.cs 

# Referenced assemblies
ASSEMBLIES =

PACKAGES =

PROJECTS = /r:../Engine/release/Hiage.dll 

RELEASE=release
ASSEMBLY=$(RELEASE)/Mario.exe

.PHONY: hiage

# Compilers and compiler options
CSC=dmcs
CSFLAGS+=/noconfig /codepage:utf8 /warn:4

ifeq ($(CONFIG), DEBUG_X86)
CSFLAGS+=/optimize- /debug /define:"DEBUG"
endif

ifeq (CONFIG, RELEASE_X86)
CSFLAGS+=/optimize+
endif

SHELL=/bin/bash

all: hiage $(ASSEMBLY)
	@mkdir -p $(RELEASE)
	rm -f $(RELEASE)/data
	ln -s ../data $(RELEASE)/data
	@echo "All done! Execute $(ASSEMBLY) or type 'make run' to run Mario!"

run:all
	./$(RELEASE)/Mario.exe

../Engine/release/last_build_tag hiage:
	@mkdir -p $(RELEASE)
	@$(MAKE) -C ../Engine
	@cp ../Engine/release/* $(RELEASE)

$(ASSEMBLY): $(SOURCES) ../Engine/release/last_build_tag
	@mkdir -p $(RELEASE)
	@echo "Building $@..."
	$(CSC) $(CSFLAGS) $(SOURCES) $(ASSEMBLIES) $(PACKAGES) $(PROJECTS) /target:exe /out:$@ 

clean:
	rm -rf $(BIN) *.pidb $(RELEASE)

