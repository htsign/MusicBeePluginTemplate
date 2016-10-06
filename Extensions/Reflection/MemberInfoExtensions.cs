using System;
using System.Reflection;

namespace MusicBeePlugin.Extensions.Reflection
{
    public static class MemberInfoExtension
    {
        public static bool HasAttribute<T>(this MemberInfo mi)
            where T : Attribute
        {
            Attribute attribute = Attribute.GetCustomAttribute(mi, typeof(T));
            return (attribute != null);
        }
    }
}
