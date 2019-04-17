using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ReflectionTest
{
    public class DefaultObjectReflectionProvider<T> : IObjectReflection, IObjectReflection<T>
    {
        readonly Type type;
        Dictionary<string, PropertyInfo> propertyCache;

        public DefaultObjectReflectionProvider()
        {
            type = typeof(T);

            var properties = type.GetProperties();
            var capacity = Math.Max(1, properties.Length / 2);
            propertyCache = new Dictionary<string, PropertyInfo>(capacity);

            foreach (var property in type.GetProperties())
            {
                propertyCache.Add(property.Name, property);
            }
        }

        static object ConvertTo(object value, Type convertType)
        {
            if (convertType == typeof(Guid))
                return Guid.Parse(value.ToString());

            return Convert.ChangeType(value, convertType);
        }

        public object GetProperty(object @object, string propertyName)
        {
            return propertyCache[propertyName].GetValue((T)@object);
        }

        public void SetProperty(object @object, string propertyName, object value)
        {
            propertyCache[propertyName].SetValue((T)@object, value);
        }

        public void ConvertSetProperty(object @object, string propertyName, object value)
        {
            var property = propertyCache[propertyName];
            property.SetValue((T)@object, ConvertTo(value, property.PropertyType));
        }
    }
}
