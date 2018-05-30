using System;
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
            return new CustomAttributeBuilder(
                attribute.GetType().GetConstructor(Type.EmptyTypes),
                // ReSharper disable once CoVariantArrayConversion
                Type.EmptyTypes,
                new FieldInfo[0],
                new object[0]);
        }
    }
}