ProtoBufRemote is an RPC implementation for C# and C++ using [Google's Protocol Buffers](http://code.google.com/p/protobuf) as the transport mechanism.

## Guides ##
C#:
  * [Getting started guide](GettingStartedCSharp.md)
  * [Manual](ManualCSharp.md)
  * [Building guide](BuildingCSharp.md)
C++:
  * [Getting started guide](GettingStartedCpp.md)
  * [Manual](ManualCpp.md)
  * [Building guide](BuildingCpp.md)

## Features ##
  * Using protocol buffers as the transport mechanism allows a multi-platform, multi-language implementation
  * Uses the official [protobuf](http://code.google.com/p/protobuf) implementation for C++, uses [protobuf-net](http://code.google.com/p/protobuf-net) for C#
  * Automatic handling of simple parameter types, no need to define protocol buffer messages for every RPC call
  * Protocol buffer messages can be used for complex parameters and return values
  * Both asynchronous and blocking programming models are available when making RPC calls
  * Full two way communication, both ends of the channel can make and receive RPC calls
  * C#: Reflection Emit implementation for maximum performance
  * C#: Dynamic proxy type support using the DLR
  * C++: Optional service and proxy code generation using Boost Preprocessor library