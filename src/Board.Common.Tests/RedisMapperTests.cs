using System;
using System.Collections.Generic;
using Board.Common.Utils;
using StackExchange.Redis;
using Xunit;

namespace Board.Common.Tests
{
    public class TestType1
    {
        public int IntegerProperty { get; set; }
        public long LongProperty { get; set; }
        public float FloatProperty { get; set; }
        public double DoubleProperty { get; set; }
        public bool BoolProperty { get; set; }
        public string StringProperty { get; set; }
        public byte[] ByteArrayProperty { get; set; }
        public DateTime DateTimeProperty { get; set; }
        public Guid GuidProperty { get; set; }
    }

    public class UnsupportedPropertyTestType1
    {
        public int IntegerProperty { get; set; }
        public object UnsupportedProperty { get; set; }
    }

    public class UnsupportedPropertyTestType2
    {
        public int IntegerProperty { get; set; }
        public ICollection<int> UnsupportedProperty { get; set; }
    }

    public class CollectionOfStringsTestType1
    {
        public ICollection<string> StringCollection { get; set; }
    }

    public class CollectionOfStringsTestType2
    {
        public List<string> StringCollection { get; set; }
    }

    public class RedisMapperTests : IDisposable
    {
        public void Dispose()
        {
            RedisMapper.UnregisterAll();
        }

        [Fact]
        public void RegisterType_should_register_public_get_set_properties()
        {
            RedisMapper.RegisterType<TestType1>();
            Assert.Equal(9, RedisMapper.RegisteredTypes[typeof(TestType1)].Count);
        }

        [Fact]
        public void RegisterType_should_register_ICollection_of_string_properties()
        {
            RedisMapper.RegisterType<CollectionOfStringsTestType1>();
            Assert.Contains(RedisMapper.RegisteredTypes[typeof(CollectionOfStringsTestType1)], props => props.Key == "StringCollection" && props.Value == typeof(ICollection<string>));

            RedisMapper.RegisterType<CollectionOfStringsTestType2>();
            Assert.Contains(RedisMapper.RegisteredTypes[typeof(CollectionOfStringsTestType2)], props => props.Key == "StringCollection" && props.Value == typeof(List<string>));
        }

        [Fact]
        public void RegisterType_should_throw_by_default_when_an_unsupported_type_is_detected()
        {
            Assert.Throws<ArgumentException>(() => RedisMapper.RegisterType<UnsupportedPropertyTestType1>());
            Assert.Throws<ArgumentException>(() => RedisMapper.RegisterType<UnsupportedPropertyTestType2>());
        }
        
        [Fact]
        public void ToRedis_should_be_able_to_handle_an_empty_collection_of_strings()
        {
            RedisMapper.RegisterType<CollectionOfStringsTestType1>();
            Assert.Equal(1, RedisMapper.RegisteredTypes[typeof(CollectionOfStringsTestType1)].Count);

            RedisMapper.RegisterType<CollectionOfStringsTestType2>();
            Assert.Equal(1, RedisMapper.RegisteredTypes[typeof(CollectionOfStringsTestType2)].Count);

            var subject = new CollectionOfStringsTestType2
            {
                StringCollection = new List<string>()
            };

            var result = subject.ToRedis();
            Assert.Equal($"{typeof(List<string>).AssemblyQualifiedName}", result[0].Value);
        }

        [Fact]
        public void ToRedis_should_be_able_to_handle_collection_of_strings()
        {
            RedisMapper.RegisterType<CollectionOfStringsTestType1>();
            Assert.Equal(1, RedisMapper.RegisteredTypes[typeof(CollectionOfStringsTestType1)].Count);

            RedisMapper.RegisterType<CollectionOfStringsTestType2>();
            Assert.Equal(1, RedisMapper.RegisteredTypes[typeof(CollectionOfStringsTestType2)].Count);

            var subject = new CollectionOfStringsTestType2
            {
                StringCollection = new List<string>
                {
                    "item1",
                    "item2",
                    "item3",
                    @"|item4"
                }
            };

            var result = subject.ToRedis();
            Assert.Equal($@"{typeof(List<string>).AssemblyQualifiedName}||item1||item2||item3||\|item4", result[0].Value);
        }
        
        [Fact]
        public void FromRedis_should_return_an_object_of_a_supported_type_without_a_collection()
        {
            RedisMapper.RegisterType<TestType1>();
            var subject = new[]
            {
                new HashEntry("IntegerProperty", 1),
                new HashEntry("FloatProperty", 2.0f),
                new HashEntry("DoubleProperty", 3.0d),
                new HashEntry("BoolProperty", true),
                new HashEntry("StringProperty", "teszt"),
                new HashEntry("ByteArrayProperty", new byte[]{0, 1, 2}),
                new HashEntry("DateTimeProperty", new DateTime(2000, 1, 1, 12, 0, 0).ToString("O")),
                new HashEntry("GuidProperty", Guid.Empty.ToString())
            };

            var result = Assert.IsType<TestType1>(subject.FromRedis<TestType1>());
            Assert.Equal(1, result.IntegerProperty);
            Assert.Equal(2.0f, result.FloatProperty);
            Assert.Equal(3.0d, result.DoubleProperty);
            Assert.Equal(true, result.BoolProperty);
            Assert.Equal("teszt", result.StringProperty);
            Assert.Equal(3, result.ByteArrayProperty.Length);
            Assert.Equal(0, result.ByteArrayProperty[0]);
            Assert.Equal(1, result.ByteArrayProperty[1]);
            Assert.Equal(2, result.ByteArrayProperty[2]);
            Assert.Equal(new DateTime(2000, 1, 1, 12, 0, 0), result.DateTimeProperty);
            Assert.Equal(Guid.Empty, result.GuidProperty);
        }

        [Fact]
        public void FromRedis_should_return_an_object_of_a_supported_type_with_an_ICollection_of_strings()
        {
            RedisMapper.RegisterType<CollectionOfStringsTestType1>();
            var subject = new[]
            {
                new HashEntry(
                    "StringCollection",
                    $@"{typeof(List<string>).AssemblyQualifiedName}||item1||item2||item3||\|item4")
            };

            var result = Assert.IsType<CollectionOfStringsTestType1>(subject.FromRedis<CollectionOfStringsTestType1>());

            Assert.Equal(4, result.StringCollection.Count);
            Assert.Contains("item1", result.StringCollection);
            Assert.Contains("item2", result.StringCollection);
            Assert.Contains("item3", result.StringCollection);
            Assert.Contains("|item4", result.StringCollection);
        }
    }
}