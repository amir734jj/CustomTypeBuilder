using System;

namespace CustomTypeBuilder.Builders
{
    /// <summary>
    /// Dynamic type builder builder
    /// </summary>
    public class CustomTypeBuilder
    {
        private readonly CustomTypeGenerator _dynamicTypeObject;

        private CustomTypeBuilder(string name = null, Type type = null) => _dynamicTypeObject = new CustomTypeGenerator(name, type);

        public CustomTypeBuilder Extend<T>() => Extend(typeof(T));

        public CustomTypeBuilder AddInterface<T>() => AddInterface(typeof(T));

        public CustomTypeBuilder AddProperty<T>(string name) => AddProperty(name, typeof(T));

        public CustomTypeBuilder AddProperty(string name, Type type)
        {
            _dynamicTypeObject.AddProperty(name, type);

            return this;
        }

        public CustomTypeBuilder Extend(Type type)
        {
            _dynamicTypeObject.ExtendType(type);

            return this;
        }

        public CustomTypeBuilder AddInterface(Type type)
        {
            _dynamicTypeObject.ExtendType(type);

            return this;
        }

        public CustomTypeBuilder AddAttribute(Attribute attribute)
        {
            _dynamicTypeObject.AddAttribute(attribute);
            
            return this;
        }

        public static CustomTypeBuilder NewExtend(Type type, string name = null) => new CustomTypeBuilder(name, type);

        public static CustomTypeBuilder NewExtend<T>(string name = null) => new CustomTypeBuilder(name, typeof(T));

        public Type Compile() => _dynamicTypeObject.CompileResultType();

        public T Instantiate<T>() where T : class => Activator.CreateInstance(_dynamicTypeObject.CompileResultType()) as T;

        public static CustomTypeBuilder New(string name = null) => new CustomTypeBuilder(name);
    }
}