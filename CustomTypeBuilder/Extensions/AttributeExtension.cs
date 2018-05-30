using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace CustomTypeBuilder.Extensions
{
    public static class AttributeExtension
    {
        /// <summary>
        /// Creates a CustomAttributeBuilder given an attribute
        /// </summary>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static CustomAttributeBuilder BuildCustomAttribute(this Attribute attribute)
        {
            var type = attribute.GetType();
            var constructor = type.GetConstructor(Type.EmptyTypes);
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);

            var propertyValues = properties.Select(p => p.GetValue(attribute, null));
            var fieldValues = fields.Select(f => f.GetValue(attribute));
            
            return new CustomAttributeBuilder(constructor,
                // ReSharper disable once CoVariantArrayConversion
                Type.EmptyTypes,
                properties,
                propertyValues.ToArray(),
                fields,
                fieldValues.ToArray());
        }
    }
}