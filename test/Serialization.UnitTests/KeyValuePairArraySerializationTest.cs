﻿using Newtonsoft.Json;
using YellowFlavor.Serialization.Implementation;

namespace Serialization.UnitTests
{
    public class KeyValuePairArraySerializationTest
    {
        [Fact]
        public void SerializeKeyValuePairArrayVisualBasic()
        {
            var kvpArray = new KeyValuePair<int, string>[]
            {
                new(1, "First"),
                new(2, "Second")
            };

            var serializer = new VisualBasicSerializer();

            var result = serializer.Serialize(kvpArray, JsonConvert.SerializeObject(new
            {
                IgnoreDefaultValues = true,
                IgnoreNullValues = true,
                MaxDepth = 5,
                UseFullTypeName = false,
                DateTimeInstantiation = "New",
                DateKind = "ConvertToUtc"
            }));

            Assert.Equal(
@"Dim arrayOfKeyValuePair = New KeyValuePair(Of Integer, String)(){
    New KeyValuePair(Of Integer, String)(1, ""First""),
    New KeyValuePair(Of Integer, String)(2, ""Second"")
}
", result);
        }

        [Fact]
        public void SerializeKeyValuePairArrayCSharp()
        {
            var kvpArray = new KeyValuePair<int, string>[]
            {
                new(1, "First"),
                new(2, "Second")
            };

            var serializer = new CSharpSerializer();

            var result = serializer.Serialize(kvpArray, JsonConvert.SerializeObject(new
            {
                IgnoreDefaultValues = true,
                IgnoreNullValues = true,
                MaxDepth = 5,
                UseFullTypeName = false,
                DateTimeInstantiation = "New",
                DateKind = "ConvertToUtc"
            }));

            Assert.Equal(
@"var arrayOfKeyValuePair = new KeyValuePair<int, string>[]
{
    new KeyValuePair<int, string>(1, ""First""),
    new KeyValuePair<int, string>(2, ""Second"")
};
", result);
        }

    }
}
