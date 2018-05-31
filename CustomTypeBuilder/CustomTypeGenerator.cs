using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using CustomTypeBuilder.Extensions;
using static CustomTypeBuilder.Utilities.TypeUtility;

namespace CustomTypeBuilder
{
    /// <summary>
    /// Creates a new type dynamically
    /// </summary>
    public class CustomTypeGenerator
    {
        private readonly TypeBuilder _typeBuilder;

        private readonly Dictionary<string, Type> _properties;
        
        private readonly ModuleBuilder _moduleBuilder;

        /// <summary>
        /// Initialize custom type builder
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parentType"></param>
        /// <param name="namespace"></param>
        /// <param name="moduleBuilder"></param>
        public CustomTypeGenerator(string name = null, Type parentType = null, string @namespace = null, ModuleBuilder moduleBuilder = null)
        {
            var assemblyName = RandomSafeString("DynamicAseembly");
            var typeSignature = name ?? RandomSafeString("DynamicType");

            // add namespace
            if (@namespace != name)
            {
                typeSignature = $"{@namespace}.{typeSignature}";
            }

            // set module builder if any
            if (moduleBuilder == null)
            {
                var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(assemblyName), AssemblyBuilderAccess.Run);
                moduleBuilder = assemblyBuilder.DefineDynamicModule(RandomSafeString("Module"));
            }

            _moduleBuilder = moduleBuilder;

            _typeBuilder = moduleBuilder.DefineType(typeSignature,
                TypeAttributes.Public |
                TypeAttributes.Class |
                TypeAttributes.AutoClass |
                TypeAttributes.AnsiClass |
                TypeAttributes.BeforeFieldInit |
                TypeAttributes.AutoLayout,
                parentType);
                        
            _typeBuilder.DefineDefaultConstructor(MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);
            
            _properties = new Dictionary<string, Type>();
        }

        /// <summary>
        /// Get ModuleBuilder
        /// </summary>
        /// <returns></returns>
        public ModuleBuilder GetModuleBuilder() => _moduleBuilder;
        
        /// <summary>
        /// Add attribute to the class
        /// </summary>
        /// <param name="attribute"></param>
        public void AddAttribute(Attribute attribute)
        {
            _typeBuilder.SetCustomAttribute(attribute.BuildCustomAttribute());
        }
        
        /// <summary>
        /// Compile the type builder to a type
        /// </summary>
        /// <returns></returns>
        public Type CompileResultType()
        {            
            return _typeBuilder.CreateType();
        }

        /// <summary>
        /// Add interfaces to a type
        /// </summary>
        /// <param name="type"></param>
        public void AddInterface(Type type)
        {
            if (!type.IsInterface)
            {
                throw new ArgumentException("Type was expected to be an interface");
            }

            _typeBuilder.AddInterfaceImplementation(type);

            // add types in interface
            type.GetProperties().ForEach(x => AddProperty(x.Name, x.PropertyType));
        }

        public void ExtendType(Type type)
        {
            _typeBuilder.SetParent(type);
        }

        public void AddProperty(string propertyName, Type propertyType)
        {
            if (!IsValidName(propertyName)) throw new ArgumentException("Property name does not follow to C# type system");

            if (_properties.Keys.Any(x => x == propertyName)) throw new ArgumentException("Duplicate property name");

            // add property to dictionary
            _properties.Add(propertyName, propertyType);
            
            var fieldBuilder = _typeBuilder.DefineField("_" + propertyName, propertyType, FieldAttributes.Private);

            var propertyBuilder = _typeBuilder.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);
            var getPropMthdBldr = _typeBuilder.DefineMethod("get_" + propertyName,
                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, propertyType,
                Type.EmptyTypes);
            var getIl = getPropMthdBldr.GetILGenerator();

            getIl.Emit(OpCodes.Ldarg_0);
            getIl.Emit(OpCodes.Ldfld, fieldBuilder);
            getIl.Emit(OpCodes.Ret);

            var setPropMthdBldr = _typeBuilder.DefineMethod("set_" + propertyName,
                    MethodAttributes.Public |
                    MethodAttributes.SpecialName |
                    MethodAttributes.HideBySig,
                    null, new[] {propertyType});

            var setIl = setPropMthdBldr.GetILGenerator();
            var modifyProperty = setIl.DefineLabel();
            var exitSet = setIl.DefineLabel();

            setIl.MarkLabel(modifyProperty);
            setIl.Emit(OpCodes.Ldarg_0);
            setIl.Emit(OpCodes.Ldarg_1);
            setIl.Emit(OpCodes.Stfld, fieldBuilder);

            setIl.Emit(OpCodes.Nop);
            setIl.MarkLabel(exitSet);
            setIl.Emit(OpCodes.Ret);

            propertyBuilder.SetGetMethod(getPropMthdBldr);
            propertyBuilder.SetSetMethod(setPropMthdBldr);
        }
    }
}