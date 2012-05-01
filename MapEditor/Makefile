# Source files
SOURCES =  \
	AssemblyInfo.cs \
	gtk-gui/generated.cs \
	gtk-gui/MainWindow.cs \
	gtk-gui/MapEditor.NewMapDialog.cs \
	gtk-gui/MapEditor.ObjectChooser.cs \
	gtk-gui/MapEditor.TileChooser.cs \
	src/EditorModel.cs \
	src/InteractThread.cs \
	src/Main.cs \
	src/MainWindow.cs \
	src/MapEditorState.cs \
	src/MapObject.cs \
	src/NewMapDialog.cs \
	src/ObjectChooser.cs \
	src/TileChooser.cs 

# Referenced assemblies
ASSEMBLIES =  \
	/r:../Engine/release/Hiage.dll \
	/r:Mono.Posix \
	/r:System \
	/r:System.Xml \
	-pkg:glade-sharp-2.0 \
	-pkg:glib-sharp-2.0 \
	-pkg:gtk-sharp-2.0

PACKAGES =

PROJECTS = 

HIAGE = lib/Hiage.dll

BIN=bin
ASSEMBLY=$(BIN)/MapEditor.exe
RELEASE=release

.PHONY: hiage

# Compilers and compiler options
CSC=mono-csc
CSFLAGS=-o+

all:$(ASSEMBLY)
	@mkdir -p $(RELEASE)
	@cp $(ASSEMBLY) $(RELEASE) 
	@cp -r ../Mario/data $(RELEASE)

run:all
	./$(RELEASE)/MapEditor.exe

hiage:
	mkdir -p $(RELEASE)
	$(MAKE) -C ../Engine
	@cp ../Engine/release/* $(RELEASE)

$(ASSEMBLY):
	$(MAKE) hiage
	@mkdir -p $(BIN)
	@echo "Building $@..."
	@$(CSC) $(SOURCES) $(ASSEMBLIES) $(PACKAGES) /target:exe /unsafe /out:$@ 

clean:
	rm -rf $(BIN) *.pidb $(RELEASE)

