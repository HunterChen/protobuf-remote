# Building the C++ version #

## Dependencies ##
  * [Google's official protocol buffer implementation](http://code.google.com/p/protobuf)
  * [Boost](http://www.boost.org), needed for the thread library, the unordered container implementations, and the header-only Preprocessor library.
To build tests:
  * [Google Mock](http://code.google.com/p/googlemock), this comes bundled with a version of Google Test which is also needed.

## Building ##

Currently only Windows and Visual Studio 10 are supported. [Visual C++ 2010 Express](http://www.microsoft.com/express/Windows/) can be used and is freely available.

Porting should be simple, and contributions are welcome - the only real platform specific code is in SocketRpcChannel, as it uses WinSock heavily. A vanilla Berkeley sookets implementation would be required for non-Windows platforms.

### Windows ###
Visual Studio 2010 project files are provided. Several macros are used to specify the location of dependencies, these can be either set as environment variables or set as User Macros in Visual Studio. (To add a User Macro in Visual Studio go to View/Other Windows/Property Manager to open the Property Manager, then edit the properties on the Microsoft.Cpp.Win32.user property sheet, and then select User Macros)

The required macros are as follows:
| $(ProtoBufCompiler) | The full path for protoc.exe |
|:--------------------|:-----------------------------|
| $(ProtoBufIncludePath) | Include path for Protocol buffer library |
| $(ProtoBufDebugLibPath) | Location of protobuf debug .lib files |
| $(ProtoBufReleaseLibPath) | Location of protobuf release .lib files |
| $(BoostIncludePath) | Include path for Boost |
| $(BoostLibPath) | Location of Boost .lib files |
| $(GtestIncludePath) | Include path for google test, included in google mock |
| $(GmockIncludePath) | Include path for google mock library |
| $(GmockDebugLibPath) | Location of google mock debug .lib files |
| $(GmockReleaseLibPath) | Location of google mock release .lib files |