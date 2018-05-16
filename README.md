# CustomTypeBuilder
Custom type builder in C#

Create a type on the fly, either by extending an exisitng type of starting from ground-up
```csharp
var type = Builders
    .CustomTypeBuilder.NewExtend<DummyClass>()  // create a type by extending an existing type
    .AddProperty<string>("PropertyNameIsHere")  // add new property of type string with 
    .Compile();                                 // to build and get Type object
    // .Instantiate<DummyClass>()               // or instantiate this type and cast to DummyClass
```
