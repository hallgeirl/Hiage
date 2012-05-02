Hiage Game Engine
=================

This is my 2D game engine written in C#. It should run in both Linux (with Mono) and Windows. 

Currently supported features:

- OpenGL 2D rendering
- Animated sprites
- Tilemaps
- Lazy resource loading using XMLs
- Keyboard/mouse I/O
- Easy to use map editor
- Framerate independent collision detection
- (probably more; will update this section later ;))

Installation
------------

Required libraries and software:

- C# compiler (Mono or Visual C# should both work; tested on Mono on Linux, with the "dmcs" compiler)
- Mono or .NET runtime (tested 3.5 and above)
- libdevil-dev: Image loading and manipulation toolkit.

Compiling:

    cd <hiage root>
    make

This builds all projects. To compile each project separately, simply "cd" into the project's directory and run "make" from there. 

Usage
-----
To run any of the projects using the Hiage engine, e.g. MapEditor or Mario:

    cd <project>
    make run

Example:

    cd MapEditor
    make run

Alternatively:

    cd MapEditor
    make && ./release/MapEditor


Contributing
------------
Since this is a spare time project which I don't have much time to work on, I am not actively looking for contributors. However, if you really do want to contribute with anything (eg. a better collision detection algorithm, audio support, etc.) let me know.
