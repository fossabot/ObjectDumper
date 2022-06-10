﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Newtonsoft.Json.Embedded;
using Newtonsoft.Json.Embedded.Converters;
using Newtonsoft.Json.Embedded.Serialization;
using ObjectFormatter.Implementation.Settings;

namespace ObjectFormatter.Implementation
{
    internal class XmlSerializer: ISerializer
    {
        private static JsonSerializerSettings XmlSettings => new()
        {
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore,
            ContractResolver = new SpecificContractResolver
            {
                ExcludeTypes = new[] { "Avro.Schema" }
            },
            Converters = { new StringEnumConverter() },
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            MaxDepth = 100
        };

        public string Format(object obj, string settings)
        {
            var xmlSettings = GetXmlSettings(settings);
            var useFullTypeName = xmlSettings.TypeNameHandling == TypeNameHandling.All;
            xmlSettings.TypeNameHandling = TypeNameHandling.None;
            return $"<?xml version=\"1.0\" encoding=\"utf-8\"?>{Environment.NewLine}{GetXml(obj, xmlSettings, useFullTypeName)}";
        }

        private static JsonSerializerSettings GetXmlSettings(string settings)
        {
            var newSettings = XmlSettings;
            if (settings == null) return newSettings;

            var xmlSettings = JsonConvert.DeserializeObject<XmlSettings>(settings);
            newSettings.NullValueHandling = xmlSettings.IgnoreNullValues ? NullValueHandling.Ignore : NullValueHandling.Include;
            newSettings.DefaultValueHandling = xmlSettings.IgnoreDefaultValues ? DefaultValueHandling.Ignore : DefaultValueHandling.Include;
            newSettings.MaxDepth = xmlSettings.MaxDepth;
            newSettings.TypeNameHandling = xmlSettings.UseFullTypeName ? TypeNameHandling.All : TypeNameHandling.None;

            if (!xmlSettings.SerializeEnumAsString)
            {
                newSettings.Converters = null;
            }

            if (xmlSettings.NamingStrategy != "CamelCase")
            {
                newSettings.ContractResolver = new SpecificContractResolver
                {
                    NamingStrategy = (NamingStrategy)Activator.CreateInstance(Type.GetType($"Newtonsoft.Json.Embedded.Serialization.{xmlSettings.NamingStrategy}NamingStrategy")),
                    ExcludeTypes = new[] { "Avro.Schema" }
                };
            }

            return newSettings;
        }

        private const int XmlIndentSize = 2;
        private static string GetXml(object obj, JsonSerializerSettings xmlSettings, bool useFullTypeName, int nestingLevel = 0)
        {
            string indent = new(' ', nestingLevel * XmlIndentSize);

            if (obj == null) return $"{indent}<!--NULL VALUE-->";

            var elementName = GetElementName(obj, useFullTypeName);

            if (obj is IEnumerable and not IDictionary and not string)
            {
                var xmlCollection = string.Join(Environment.NewLine,
                    ((IEnumerable)obj)
                    .Cast<object>()
                    .Select(o => GetXml(o, xmlSettings, useFullTypeName, nestingLevel + 1)));

                var itemTypeName = obj.GetType()
                    .GetInterfaces()
                    .FirstOrDefault(x => x.IsGenericType
                    && x.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                    ?.GenericTypeArguments
                    .FirstOrDefault()
                    ?.Name;

                elementName = GetElementName(itemTypeName) ?? elementName;
                elementName = XmlConvert.EncodeName($"ArrayOf{elementName}");

                return $"<{elementName}>{Environment.NewLine}{xmlCollection}{Environment.NewLine}</{elementName}>";
            }

            var json = JsonConvert.SerializeObject(obj, xmlSettings);
            var objectType = obj.GetType();

            string xml;
            if (IsSimpleType(objectType))
            {
                elementName = GetElementName(obj, useFullTypeName);
                xml = $"<{elementName}>{json.Trim('"')}</{elementName}>";
            }
            else
                xml = JsonConvert.DeserializeXNode(json, elementName)?.ToString();

            if (nestingLevel == 0) return xml;

            return string.Join(Environment.NewLine,
                               xml?.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                                   .Select(x => $"{indent}{x}") ?? Enumerable.Empty<string>());
        }

        private static bool IsSimpleType(Type type)
        {
            return type.IsPrimitive
                || type.IsValueType
                || type == typeof(string);
        }

        private static string GetElementName(object obj, bool useFullTypeName)
        {
            var type = obj.GetType();
            var typeName = useFullTypeName ? type.FullName : type.Name;
            return GetElementName(typeName);
        }

        private static string GetElementName(string typeName)
        {
            return typeName?.Contains("AnonymousType") == true
                ? "AnonymousType"
                : typeName;
        }
    }
}
