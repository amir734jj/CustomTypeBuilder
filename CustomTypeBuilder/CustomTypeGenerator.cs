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

        /// <summary>
        /// Initialize custom type builder
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parentType"></param>
        public CustomTypeGenerator(string name = null, Type parentType = null)
        {
            var typeSignature = name ?? RandomSafeString("DynamicType");
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(typeSignature), AssemblyBuilderAccess.Run);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule(RandomSafeString("Module"));
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
            // type.GetProperties().ForEach(x => AddProperty(x.Name, x.PropertyType));

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