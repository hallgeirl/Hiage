
# Source files
SOURCES =  \
	src/Arena.cs  \
    src/Component.cs  \
    src/ComponentFactory.cs  \
    src/Container.cs  \
    src/FlyingState.cs  \
    src/IntroState.cs  \
    src/MainApp.cs  \
    src/MenuState.cs  \
    src/PlayerShipBody.cs  \
    src/Ship.cs  \
    src/SimpleBullet.cs  \
    src/SimpleGunTurret.cs

# Referenced assemblies
ASSEMBLIES = /r:release/Hiage.dll

PACKAGES =

PROJECTS = 

BIN=bin
ASSEMBLY=$(BIN)/Battleships.exe
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
	./$(RELEASE)/Battleships.exe

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

