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

RELEASE=release
ASSEMBLY=$(RELEASE)/Mario.exe

.PHONY: hiage

# Compilers and compiler options
CSC=mono-csc
CSFLAGS=/o+ -debug

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
	$(CSC) $(CSFLAGS) $(SOURCES) $(ASSEMBLIES) $(PACKAGES) /target:exe /out:$@ 

clean:
	rm -rf $(BIN) *.pidb $(RELEASE)

