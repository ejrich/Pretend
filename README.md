# Pretend

Building my own game engine with .Net Core and ***Pretend***ing I know what I'm doing

### Design Philosophies

Make a cross-platform engine that respects the developer and uses modern C# design patterns and architecture.

* Native dependency injection without having to configure a container
* Minimal overhead for fast execution
* No public classes, only a factory creating an instance of an interface

### Project Setup

Required Downloads:

* [.Net Core 3.1](https://dotnet.microsoft.com/download/dotnet-core/3.1) is used to build and run the project (once .Net 5 is out of preview, this will be updated)
* [SDL2](https://www.libsdl.org/download-2.0.php) is used for windowing (this will be bundled eventually)

To build, simply run `dotnet build`.  I recommend using Visual Studio or Rider as an IDE, or VS Code if you don't have access to either of these.  The main project is a class library so you cannot run it, but for this project, you can run Sandbox to verify your development environment is configured correctly.

### Project Roadmap

I am working on this project in my own time, but I will create projects in GitHub and will regularly update progress
