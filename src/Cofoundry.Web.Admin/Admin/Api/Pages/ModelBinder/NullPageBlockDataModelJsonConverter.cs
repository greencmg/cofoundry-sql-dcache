﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Domain;
using System.Reflection;

namespace Cofoundry.Web.Admin
{
    /// <summary>
    /// Used to ignore parsing of an IPageBlockDataModel child object, because the type cannot be
    /// inferred without mapping the parent object first.
    /// </summary>
    public class NullPageBlockDataModelJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(IPageBlockTypeDataModel).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            // need to read the object to prevent exception
            var jObject = JObject.Load(reader);
            return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}