﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Web
{
    /// <summary>
    /// Represents a partial update of a model, intended for use in
    /// a web api http patch operation with a json payload.
    /// </summary>
    public class JsonDelta<TModel> : IDelta<TModel>
    {
        private readonly string _jsonData;

        public JsonDelta(string jsonData)
        {
            _jsonData = jsonData;
        }

        /// <summary>
        /// Updates an existing model with the data from the delta.
        /// </summary>
        /// <param name="model">Model to update.</param>
        public void Patch(TModel model)
        {
            JsonConvert.PopulateObject(_jsonData, model);
        }
    }
}