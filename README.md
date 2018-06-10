# CustomTypeBuilder
Custom type builder in C#

[Nuget](https://www.nuget.org/packages/CustomTypeBuilder/)

Create a type on the fly, either by extending an existing type or starting from ground-up. The idea behind this project is to dynamically extend a basic POCO type (or record type), add new properties to it quickly and instantiate it as cast it back to base type. Hence underneath that type is a POCO type but with some extra properties that can be accessible by reflection.

```csharp
var type = Builders.CustomTypeBuilder.New()
    // Useful while generating nested types, you can re-use `moduleBuilder`
    .GetModuleBuilder(out var moduleBuilder)
    .SetModuleBuilder(moduleBuilder)
    // Extend an existing type
    .FinalizeOptionsBuilder().Extend<DummyClass>()
    // Add properties
    .AddProperty<string>("Name")
    .AddProperty<int>("Age")
    // Class level attribute
    .AddAttribute(new RequiredAttribute())
    .Compile();
    // Or instantiate this type and cast to DummyClass, or use `Activator.CreateInstance`
    // .Instantiate<DummyClass>()
```
