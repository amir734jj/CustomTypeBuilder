using System;
using System.Reflection.Emit;
using static CustomTypeBuilder.Utilities.LambdaHelper;

namespace CustomTypeBuilder.Builders
{
    /// <summary>
    /// Dynamic type builder builder
    /// </summary>
    public class CustomTypeBuilder
    {   
        public static CustomTypeBuilderInitialization New() => new CustomTypeBuilderInitialization();

        /// <summary>
        /// Initialization step
        /// </summary>
        public class CustomTypeBuilderInitialization
        {
            private string _name, _namespace;

            private Type _parentType;

            private ModuleBuilder _moduleBuilder;
            
            /// <summary>
            /// Add name
            /// </summary>
            /// <param name="name"></param>
            /// <returns></returns>
            public CustomTypeBuilderInitialization SetName(string name) => Run(() => _name = name, this);
            
            /// <summary>
            /// Add namespace
            /// </summary>
            /// <param name="namespace"></param>
            /// <returns></returns>
            public CustomTypeBuilderInitialization SetNameSpace(string @namespace) => Run(() => _namespace = @namespace, this);

            /// <summary>
            /// Add parentType dynamically
            /// </summary>
            /// <param name="parentType"></param>
            /// <returns></returns>
            public CustomTypeBuilderInitialization SetParentType(Type parentType) => Run(() => _parentType = parentType, this);
            
            /// <summary>
            /// Add parentType statically
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <returns></returns>
            public CustomTypeBuilderInitialization SetParentType<T>() => Run(() => _parentType = typeof(T), this);

            /// <summary>
            /// Add module builder
            /// </summary>
            /// <param name="moduleBuilder"></param>
            /// <returns></returns>
            public CustomTypeBuilderInitialization SetModuleBuilder(ModuleBuilder moduleBuilder) => Run(() => _moduleBuilder = moduleBuilder, this);
            
            /// <summary>
            /// Compile the type to building builder step
            /// </summary>
            /// <returns></returns>
            public CustomTypeBuilderBuilding FinalizeOptionsBuilder() => new CustomTypeBuilderBuilding(new CustomTypeGenerator(_name, _parentType, _namespace));            
        }

        /// <summary>
        /// Building step
        /// </summary>
        public class CustomTypeBuilderBuilding
        {
            private readonly CustomTypeGenerator _dynamicTypeObject;
            
            public CustomTypeBuilderBuilding(CustomTypeGenerator dynamicTypeObject) => _dynamicTypeObject = dynamicTypeObject;
            
            /// <summary>
            /// Extend a type
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <returns></returns>
            public CustomTypeBuilderBuilding Extend<T>() => Extend(typeof(T));

            /// <summary>
            /// Extend a type dynamically
            /// </summary>
            /// <param name="type"></param>
            /// <returns></returns>
            public CustomTypeBuilderBuilding Extend(Type type) => Run(() => _dynamicTypeObject.ExtendType(type), this);

            /// <summary>
            /// Add interface statically
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <returns></returns>
            public CustomTypeBuilderBuilding AddInterface<T>() => AddInterface(typeof(T));

            /// <summary>
            /// Add interface dynamically
            /// </summary>
            /// <param name="type"></param>
            /// <returns></returns>
            public CustomTypeBuilderBuilding AddInterface(Type type) => Run(() => _dynamicTypeObject.ExtendType(type), this);
            
            /// <summary>
            /// Add property statically
            /// </summary>
            /// <param name="name"></param>
            /// <typeparam name="T"></typeparam>
            /// <returns></returns>
            public CustomTypeBuilderBuilding AddProperty<T>(string name) => AddProperty(name, typeof(T));

            /// <summary>
            /// Add property dynamically
            /// </summary>
            /// <param name="name"></param>
            /// <param name="type"></param>
            /// <returns></returns>
            public CustomTypeBuilderBuilding AddProperty(string name, Type type) => Run(() => _dynamicTypeObject.AddProperty(name, type), this);

            /// <summary>
            /// Add class level attribute
            /// </summary>
            /// <param name="attribute"></param>
            /// <returns></returns>
            public CustomTypeBuilderBuilding AddAttribute(Attribute attribute) => Run(() => _dynamicTypeObject.AddAttribute(attribute), this);

            /// <summary>
            /// Returns the module builder
            /// </summary>
            /// <param name="moduleBuilder"></param>
            /// <returns></returns>
            public CustomTypeBuilderBuilding GetModuleBuilder(out ModuleBuilder moduleBuilder)
            {
                moduleBuilder = _dynamicTypeObject.GetModuleBuilder();
                return this;
            }

            /// <summary>
            /// Compile builder into a type
            /// </summary>
            /// <returns></returns>
            public Type Compile() => _dynamicTypeObject.CompileResultType();
            
            /// <summary>
            /// Instantiate a type
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <returns></returns>
            public T Instantiate<T>() where T : class => Activator.CreateInstance(_dynamicTypeObject.CompileResultType()) as T;     
        }
    }
}