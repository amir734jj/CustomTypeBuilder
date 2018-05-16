using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CustomTypeBuilder.Utilities
{
    /// <summary>
    /// Dynamic type utility
    /// </summary>
    public static class TypeUtility
    {
        // ReSharper disable once InconsistentNaming
        private static readonly HashSet<string> _typeNames = new HashSet<string>();
        
        // ReSharper disable once InconsistentNaming
        private static readonly Random _random = new Random();
        
        /// <summary>
        /// Returns random safe string to be used for type name or property names
        /// </summary>
        /// <param name="suffix"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string RandomSafeString(string suffix = null, int length = 10)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToArray();
            return _typeNames.Add(Enumerable.Range(0, length)
                       .Aggregate(
                           new StringBuilder(),
                           (sb, n) => sb.Append(chars[_random.Next(chars.Length)]),
                           // ReSharper disable once TailRecursiveCall
                           sb => sb.ToString()) + (suffix ?? string.Empty)) ? _typeNames.Last() : RandomSafeString();
        }

        /// <summary>
        /// Validates name is valid, either property name or type name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool IsValidName(string name) => Regex.IsMatch(name, @"[A-Z]\w+");
    }
}