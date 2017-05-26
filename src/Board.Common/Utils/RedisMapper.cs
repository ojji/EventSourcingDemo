using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FastMember;
using StackExchange.Redis;

namespace Board.Common.Utils
{
    public static class RedisMapper
    {
        public static Dictionary<Type, Dictionary<string, Type>> TypeProperties = new Dictionary<Type, Dictionary<string, Type>>();
        private static readonly Dictionary<Type, Func<object, RedisValue>> ToRedisValueConverters = new Dictionary<Type, Func<object, RedisValue>>
        {
            { typeof(int), o => (int)o },
            { typeof(float), o => (float)o },
            { typeof(double), o => (double)o },
            { typeof(bool), o => (bool)o },
            { typeof(string), o => (string)o },
            { typeof(long), o => (long)o },
            { typeof(byte[]), o => (byte[])o },
            { typeof(DateTime), o => ((DateTime)o).ToString("O") },
            { typeof(Guid), o => o.ToString() }
        };
        private static readonly Dictionary<Type, Func<RedisValue, object>> FromRedisValueConverters = new Dictionary<Type, Func<RedisValue, object>>
        {
            { typeof(int), o => (int)o },
            { typeof(float), o => (float)o },
            { typeof(double), o => (double)o },
            { typeof(bool), o => (bool)o },
            { typeof(string), o => (string)o },
            { typeof(long), o => (long)o },
            { typeof(byte[]), o => (byte[])o },
            { typeof(DateTime), o => DateTime.Parse(o) },
            { typeof(Guid), o => new Guid(o.ToString()) }
        };

        public static void RegisterType<T>()
        {
            var getSetProperties = typeof(T).GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public |
                                                     BindingFlags.Instance |
                                                     BindingFlags.GetProperty | BindingFlags.SetProperty)
                                                     .Where(p => p.GetSetMethod() != null);
            RegisterType<T>(getSetProperties.Select(p => p.Name).ToArray());
        }

        public static void RegisterType<T>(string[] properties)
        {
            var typeProperties = new Dictionary<string, Type>();
            foreach (var property in properties)
            {
                typeProperties.Add(property, typeof(T).GetProperty(property).PropertyType);
            }

            TypeProperties.Add(typeof(T), typeProperties);
        }

        public static HashEntry[] ToRedis<T>(this T @object)
        {
            var objectType = typeof(T);
            if (TypeProperties.ContainsKey(objectType))
            {
                var objectAccessor = ObjectAccessor.Create(@object);
                var hashEntries = new List<HashEntry>();
                foreach (var properties in TypeProperties[objectType])
                {
                    var hashKey = properties.Key;
                    var hashValue = ToRedisValueConverters[properties.Value](objectAccessor[properties.Key]);
                    hashEntries.Add(new HashEntry(hashKey, hashValue));
                }
                return hashEntries.ToArray();
            }
            throw new ArgumentException("There is no mapping defined for this object. Use the RedisMapper.RegisterType() method to do so.");
        }

        public static T FromRedis<T>(this HashEntry[] hashEntries) where T : new()
        {
            var objectType = typeof(T);
            if (TypeProperties.ContainsKey(objectType))
            {
                var objectAccessor = TypeAccessor.Create(objectType);
                T objectRead = new T();

                foreach (var hashEntry in hashEntries)
                {
                    var propertyType = TypeProperties[typeof(T)][hashEntry.Name];
                    objectAccessor[objectRead, hashEntry.Name] = FromRedisValueConverters[propertyType](hashEntry.Value);
                }

                return objectRead;
            }

            throw new ArgumentException("There is no mapping defined for this object. Use the RedisMapper.RegisterType() method to do so.");
        }
    }
}
