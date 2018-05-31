using CustomTypeBuilder.Extensions;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Xunit;

namespace CustomTypeBuilder.Tests
{
    public class CustomTypeBuilderTest
    {
        [Fact]
        public void Test__ExtraProperty()
        {
            // Arrange
            const string propertyName = "NewProperty";

            // Act
            var obj = Builders.CustomTypeBuilder.New()
                .FinalizeOptionsBuilder()
                .Extend<DummyClass>()
                .AddProperty<string>(propertyName)
                .Instantiate<DummyClass>();

            // Assert
            Assert.Equal(typeof(string), obj.GetType().GetProperty(propertyName).PropertyType);
        }

        [Fact]
        public void Test__TypeCheck()
        {
            // Arrange
            const string propertyName = "NewProperty";

            // Act
            var type = Builders.CustomTypeBuilder.New()
                .FinalizeOptionsBuilder()
                .Extend<DummyClass>()
                .AddProperty<string>(propertyName)
                .Compile();

            // Assert
            Assert.True(typeof(DummyClass).IsAssignableFrom(type));
        }

        [Fact]
        public void Test__DupliatePropertyName()
        {
            // Arrange
            const string propertyName = "NewProperty";

            // Act
            var action = new Action(() => Builders.CustomTypeBuilder.New()
                .FinalizeOptionsBuilder()
                .Extend<DummyClass>()
                .AddProperty<string>(propertyName)
                .AddProperty<int>(propertyName)
                .Compile());

            // Assert
            Assert.Throws<ArgumentException>(action);
        }

        [Fact]
        public void Test__CompletelyNewType()
        {
            // Arrange
            var builder = Builders.CustomTypeBuilder.New().FinalizeOptionsBuilder();

            // Act
            typeof(DummyClass).GetProperties().ForEach(x => builder.AddProperty(x.Name, x.PropertyType));
            var type = builder.Compile();

            // Assert
            Assert.True(typeof(DummyClass).GetProperties()
                .Zip(type.GetProperties(), (x, y) => x.PropertyType == y.PropertyType && x.Name == y.Name)
                .All(x => x));
        }
        
        [Fact]
        public void Test__CustomAttribute()
        {
            // Arrange
            const string propertyName = "NewProperty";

            // Act
            var obj = Builders.CustomTypeBuilder.New().FinalizeOptionsBuilder().Extend<DummyClass>()
                .AddProperty<string>(propertyName)
                .AddAttribute(new RequiredAttribute())
                .Instantiate<DummyClass>();
            
            // Assert
            Assert.True(obj.GetType().GetCustomAttributes().Any(x => x is RequiredAttribute));
        }
        
        [Fact]
        public void Test__NestedTypes()
        {
            // Arrange
            var nestedType = Builders.CustomTypeBuilder.New()
                .FinalizeOptionsBuilder()
                .AddProperty<string>("NestedProp")
                .GetModuleBuilder(out var moduleBuilder)
                .Compile();
            
            const string propertyName = "NestedProp";

            // Act
            var obj = Builders.CustomTypeBuilder.New()
                .SetModuleBuilder(moduleBuilder)
                .FinalizeOptionsBuilder()
                .Extend<DummyClass>()
                .AddProperty(propertyName, nestedType)
                .Instantiate<DummyClass>();
            
            // Assert
            Assert.Equal(nestedType, obj.GetType().GetProperty(propertyName).PropertyType);
        }
    }
}
