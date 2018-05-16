# CustomTypeBuilder
Custom type builder in C#

Create a type on the fly, either by extending an existing type or starting from ground-up. The idea behind this project is to dynamically extend a basic POCO type (or record type), add new properties to it quickly and instantiate it as cast it back to base type. Hence underneath that type is a POCO type but with some extra properties that can be accessible by reflection.

```csharp
var type = Builders
    .CustomTypeBuilder.New()                    // Start the builder
    .Extend<DummyClass>                         // extend an existing type
    .AddProperty<string>("PropertyNameIsHere")  // add new property of type string with 
    .Compile();                                 // to build and get Type object
    // .Instantiate<DummyClass>()               // or instantiate this type and cast to DummyClass
```
