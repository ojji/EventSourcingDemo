using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using FastMember;
using StackExchange.Redis;

namespace Board.Common.Utils
{
    public static class RedisMapper
    {
        public static Dictionary<Type, Dictionary<string, Type>> RegisteredTypes = new Dictionary<Type, Dictionary<string, Type>>();
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
        private static readonly Dictionary<Type, Func<RedisValue, object>> RedisConverters = new Dictionary<Type, Func<RedisValue, object>>
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

        public static void UnregisterAll()
        {
            RegisteredTypes.Clear();
        }

        public static void RegisterType<T>(bool throwOnUnsupportedProperty = true)
        {
            var getSetProperties = typeof(T).GetProperties(BindingFlags.DeclaredOnly |
                                                     BindingFlags.Public |
                                                     BindingFlags.Instance |
                                                     BindingFlags.GetProperty |
                                                     BindingFlags.SetProperty)
                                            .Where(p => p.GetSetMethod() != null);
            RegisterType<T>(getSetProperties.Select(p => p.Name).ToArray(), throwOnUnsupportedProperty);
        }

        public static void RegisterType<T>(string[] properties, bool throwOnUnsupportedProperty = true)
        {
            var typeProperties = new Dictionary<string, Type>();
            foreach (var property in properties)
            {
                Type propertyType = typeof(T).GetProperty(property).PropertyType;
                if (throwOnUnsupportedProperty &&
                    !ImplementsICollectionOfStrings(propertyType) &&
                    !RedisConverters.ContainsKey(propertyType))
                {
                    throw new ArgumentException("Unsupported property type detected on property: {0}", property);
                }
                typeProperties.Add(property, propertyType);
            }

            RegisteredTypes.Add(typeof(T), typeProperties);
        }

        public static HashEntry[] ToRedis<T>(this T @object)
        {
            var objectType = typeof(T);
            if (RegisteredTypes.ContainsKey(objectType))
            {
                var objectAccessor = ObjectAccessor.Create(@object);
                var hashEntries = new List<HashEntry>();
                foreach (var properties in RegisteredTypes[objectType])
                {
                    var hashKey = properties.Key;
                    RedisValue hashValue;
                    if (ImplementsICollectionOfStrings(properties.Value))
                    {
                        hashValue = CollectionToRedisValue((ICollection<string>) objectAccessor[properties.Key]);
                    }
                    else
                    {
                        hashValue = ToRedisValueConverters[properties.Value](objectAccessor[properties.Key]);
                    }
                    hashEntries.Add(new HashEntry(hashKey, hashValue));
                }
                return hashEntries.ToArray();
            }
            throw new ArgumentException("There is no mapping defined for this object. Use the RedisMapper.RegisterType() method to do so.");
        }

        private static RedisValue CollectionToRedisValue(ICollection<string> stringCollection)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(stringCollection.GetType().AssemblyQualifiedName);
            sb.Append("||");

            foreach (var str in stringCollection)
            {
                sb.Append(str.Replace("|", @"\|"));
                sb.Append("||");
            }
            return sb.ToString(0, sb.Length - 2);
        }

        private static ICollection<string> RedisValueToCollection(string redisValue)
        {
            var splitString = redisValue.Split(new[] {"||"}, StringSplitOptions.None);

            string typeName = splitString[0];

            var collection = (ICollection<string>) Activator.CreateInstance(Type.GetType(typeName));

            for (int i = 1; i < splitString.Length; i++)
            {
                collection.Add(splitString[i].Replace(@"\|", "|"));
            }
            return collection;
        }

        private static bool ImplementsICollectionOfStrings(Type t)
        {
            return t.IsConstructedGenericType && t.GetGenericTypeDefinition() == typeof(ICollection<>) && t.GenericTypeArguments[0] == typeof(string) ||
                t.GetInterfaces().Any(x =>
                x.IsConstructedGenericType &&
                    x.GetGenericTypeDefinition() == typeof(ICollection<>) &&
                    x.GenericTypeArguments[0] == typeof(string)
            );
        }

        public static T FromRedis<T>(this HashEntry[] hashEntries) where T : new()
        {
            var objectType = typeof(T);
            if (RegisteredTypes.ContainsKey(objectType))
            {
                var objectAccessor = TypeAccessor.Create(objectType);
                T objectRead = new T();

                foreach (var hashEntry in hashEntries)
                {
                    var propertyType = RegisteredTypes[typeof(T)][hashEntry.Name];
                    if (ImplementsICollectionOfStrings(propertyType))
                    {
                        objectAccessor[objectRead, hashEntry.Name] = RedisValueToCollection(hashEntry.Value);
                    }
                    else
                    {
                        objectAccessor[objectRead, hashEntry.Name] = RedisConverters[propertyType](hashEntry.Value);
                    }
                }

                return objectRead;
            }

            throw new ArgumentException("There is no mapping defined for this object. Use the RedisMapper.RegisterType() method to do so.");
        }
    }
}
