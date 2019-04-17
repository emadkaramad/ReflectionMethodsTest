using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ReflectionTest
{
    public class ObjectReflectionService
    {
        static Dictionary<string, IObjectReflection> objectMetadataCache = new Dictionary<string, IObjectReflection>();

        public static IObjectReflection GetInstace(Type objectType, Type cacheProviderType)
        {
            var key = string.Concat(cacheProviderType.FullName, "|", objectType.FullName);
            if (!objectMetadataCache.ContainsKey(key))
                lock (objectMetadataCache)
                {
                    if (!objectMetadataCache.ContainsKey(key))
                        objectMetadataCache.Add(key,
                        (IObjectReflection)Activator.CreateInstance(cacheProviderType.MakeGenericType(objectType)));
                }
            return objectMetadataCache[key];
        }
        public static IObjectReflection GetInstace<TObjectMemberCacheProvider, TObject>() 
            where TObjectMemberCacheProvider : IObjectReflection<TObject>, IObjectReflection, new()
        {
            var key = string.Concat(typeof(TObjectMemberCacheProvider).FullName, "|", typeof(TObject).FullName);
            if (!objectMetadataCache.ContainsKey(key))
                lock (objectMetadataCache)
                {
                    if (!objectMetadataCache.ContainsKey(key))
                        objectMetadataCache.Add(key, new TObjectMemberCacheProvider());
                }
            return objectMetadataCache[key];
        }
    }
}
