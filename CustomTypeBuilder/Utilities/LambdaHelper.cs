using System;

namespace CustomTypeBuilder.Utilities
{
    public class LambdaHelper
    {
        /// <summary>
        /// Run the action and return this;
        /// </summary>
        /// <param name="action"></param>
        /// <param name="this"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Run<T>(Action action, T @this)
        {
            action();
            return @this;
        }
    }
}