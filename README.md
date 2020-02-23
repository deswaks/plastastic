# Plastastic
Plastastic is a Revit plugin for exporting a building model to a 3D printable file.
Currently the only implemented functionality is direct export of building walls to .stl

# Goals
We expect to develop the program to be able to modify and simplify the building geometry to make 3D printing easier.

# Installation
To install the program, follow the steps below
1. Clone this repository with git
2. Place the .addin file from the resources folder in your Revit add-ins folder usually located in C:\ProgramData\Autodesk\Revit\Addins\2020
3. Place the build .dll from the resources folder anywhere you like
4. replace the path enclosed by <Assembly></Assembly> tags in the .addin file by the path to where you placed the .dll in step 3.

# Dependencies
The export functionality is based upon the official open source plugin by Autodesk found in the following link. The source from the official plugin has been included in this repository.
https://github.com/Autodesk/revit-stl-extension
