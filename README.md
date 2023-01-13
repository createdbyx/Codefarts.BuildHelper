# BuildHelper Lib

The build helper library project started out as a utility that I could use for doing build commands both pre and post build.
The process is a bit complicated and messy at the moment and I'm hoping to simplify that eventually.  

It currently uses an XML file to contain the commands you want to execute. The name of the XML element refers to the plug in command, and the XML attributes refer to the parameters for that command. 

The project is currently transitioning and becoming more of a generic command system.  
 
The code tries to use best practices as much as possible in terms of dependency injection and interfaces.

This repo includes a number of command plugins that are helpful during a build process.  

There is still a lot of work to be done and I will eventually get around to creating an IDE that you can visually create and edit commands.

There are 288 unit tests that are all passing but there's still more work to be done.