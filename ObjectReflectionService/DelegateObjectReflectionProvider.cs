using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ReflectionTest
{
    public class DelegateObjectReflectionProvider<T> : IObjectReflection, IObjectReflection<T>
    {
        readonly Type type;
        Dictionary<string, Func<T, object>> delegateGetCache;
        Dictionary<string, Action<T, object>> delegateSetCache;
        Dictionary<string, Action<T, object>> delegateConvertSetCache;

        public DelegateObjectReflectionProvider()
        {
            type = typeof(T);

            var properties = type.GetProperties();
            var capacity = Math.Max(1, properties.Length / 2);
            delegateGetCache = new Dictionary<string, Func<T, object>>(capacity);
            delegateSetCache = new Dictionary<string, Action<T, object>>(capacity);
            delegateConvertSetCache = new Dictionary<string, Action<T, object>>(capacity);

            foreach (var property in type.GetProperties())
            {
                CacheGetSetMethods(property);
            }
        }

        void CacheGetSetMethods(PropertyInfo property)
        {
            var propertyType = property.PropertyType;

            var genericGetHelper = typeof(DelegateObjectReflectionProvider<T>).GetMethod("GetMethodHelper",
                    BindingFlags.Static | BindingFlags.NonPublic);
            var constructedGetHelper = genericGetHelper.MakeGenericMethod(propertyType);

            delegateGetCache.Add(property.Name, (Func<T, object>)constructedGetHelper.Invoke(null, new object[] { property }));

            var genericSetHelper = typeof(DelegateObjectReflectionProvider<T>).GetMethod("SetMethodHelper",
                    BindingFlags.Static | BindingFlags.NonPublic);
            var constructedSetHelper = genericSetHelper.MakeGenericMethod(propertyType);

            delegateSetCache.Add(property.Name, (Action<T, object>)constructedSetHelper.Invoke(null, new object[] { property }));

            var genericConvertSetHelper = typeof(DelegateObjectReflectionProvider<T>).GetMethod("ConvertSetMethodHelper",
                    BindingFlags.Static | BindingFlags.NonPublic);
            var constructedConvertSetHelper = genericConvertSetHelper.MakeGenericMethod(propertyType);

            delegateConvertSetCache.Add(property.Name, (Action<T, object>)constructedConvertSetHelper.Invoke(null, new object[] { property }));
        }

        static Func<T, object> GetMethodHelper<TOut>(PropertyInfo property)
        {
            var func = (Func<T, TOut>)Delegate.CreateDelegate(typeof(Func<T, TOut>), property.GetGetMethod(nonPublic: true));
            return (T target) => func(target);
        }
        static Action<T, object> SetMethodHelper<TValue>(PropertyInfo property)
        {
            var func = (Action<T, TValue>)Delegate.CreateDelegate(typeof(Action<T, TValue>), property.GetSetMethod(nonPublic: true));
            return (T target, object value) => func(target, (TValue)value);
        }

        static Action<T, object> ConvertSetMethodHelper<TValue>(PropertyInfo property)
        {
            var func = (Action<T, TValue>)Delegate.CreateDelegate(typeof(Action<T, TValue>), property.GetSetMethod(nonPublic: true));

            if (typeof(TValue) == typeof(Guid))
                return (T target, object value) => func(target, (TValue)(Guid.Parse(value.ToString()) as object));
            else if (typeof(TValue) == typeof(string))
                return (T target, object value) => func(target, (TValue)(value.ToString() as object));
            else if (typeof(TValue) == typeof(int))
                return (T target, object value) => func(target, (TValue)(Convert.ToInt32(value) as object));
            else if (typeof(TValue) == typeof(decimal))
                return (T target, object value) => func(target, (TValue)(Convert.ToDecimal(value) as object));
            else if (typeof(TValue) == typeof(bool))
                return (T target, object value) => func(target, (TValue)(Convert.ToBoolean(value) as object));
            else if (typeof(TValue) == typeof(DateTime))
                return (T target, object value) => func(target, (TValue)(Convert.ToDateTime(value) as object));

            return (T target, object value) => func(target, (TValue)Convert.ChangeType(value, typeof(TValue)));
        }

        public object GetProperty(object @object, string propertyName)
        {
            return delegateGetCache[propertyName]((T)@object);
        }

        public void SetProperty(object @object, string propertyName, object value)
        {
            delegateSetCache[propertyName]((T)@object, value);
        }

        public void ConvertSetProperty(object @object, string propertyName, object value)
        {
            delegateConvertSetCache[propertyName]((T)@object, value);
        }
    }
}
