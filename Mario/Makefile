# Source files
SOURCES =  \
	src/Camera.cs \
	src/Controllers/DumbGroundAI.cs \
	src/Controllers/IController.cs \
	src/Controllers/PlayerController.cs \
	src/LevelState.cs \
	src/Main.cs \
	src/MarioGameState.cs \
	src/ObjectFactory.cs \
	src/Objects/BasicGroundEnemy.cs \
	src/Objects/Character.cs \
	src/Objects/Coin.cs \
	src/Objects/GameObject.cs \
	src/Objects/PhysicalObject.cs \
	src/Objects/Player.cs \
	src/Physics.cs \
	src/PlayerInfo.cs \
	src/PlayerState.cs 

# Referenced assemblies
ASSEMBLIES = /r:release/Hiage.dll

PACKAGES =

PROJECTS = 

BIN=bin
ASSEMBLY=$(BIN)/Mario.exe
RELEASE=release

.PHONY: hiage

# Compilers and compiler options
CSC=mono-csc
CSFLAGS=-o+

all:$(ASSEMBLY)
	@mkdir -p $(RELEASE)
	@cp $(ASSEMBLY) $(RELEASE) 
	@cp -r data $(RELEASE)

run:all
	./$(RELEASE)/Mario.exe

hiage:
	@mkdir -p $(RELEASE)
	$(MAKE) -C ../Engine
	@cp ../Engine/release/* $(RELEASE)

$(ASSEMBLY):
	$(MAKE) hiage
	@mkdir -p $(BIN)
	@echo "Building $@..."
	@$(CSC) $(SOURCES) $(ASSEMBLIES) $(PACKAGES) /target:exe /out:$@ 

clean:
	rm -rf $(BIN) *.pidb $(RELEASE)

