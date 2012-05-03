CONFIG=DEBUG_X86

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

RELEASE=release
ASSEMBLY=$(RELEASE)/MapEditor.exe

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
	@rm -f $(RELEASE)/data
	@ln -s ../../Mario/data $(RELEASE)/data
	@echo "All done! Execute $(ASSEMBLY) or type 'make run' to run the map editor!"

run:all
	./$(RELEASE)/MapEditor.exe

../Engine/release/last_build_tag hiage:
	@mkdir -p $(RELEASE)
	@$(MAKE) -C ../Engine
	@cp ../Engine/release/* $(RELEASE)

$(ASSEMBLY): $(SOURCES) ../Engine/release/last_build_tag
	@mkdir -p $(RELEASE)
	@echo "Building $@..."
	$(CSC) $(CSFLAGS) $(SOURCES) $(ASSEMBLIES) $(PACKAGES) /target:exe /unsafe /out:$@ 

clean:
	rm -rf $(BIN) *.pidb $(RELEASE)

