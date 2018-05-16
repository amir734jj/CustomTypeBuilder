using System;

namespace CustomTypeBuilder.Extensions
{
    /// <summary>
    /// type extensions
    /// </summary>
    public static class TypeExtension
    {
        /// <summary>
        /// Returns true if type if system type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsSystemType(this Type type) => type.Namespace.StartsWith("System");
    }
}