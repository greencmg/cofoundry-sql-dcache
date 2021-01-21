﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Use this to decorate an integer array of ImageAssetIds and indicate that it should be a collection 
    /// of image assets. The editor allows for sorting of linked image assets and you can set filters for restricting image sizes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ImageCollectionAttribute : Attribute, IMetadataAttribute
    {
        #region constructor

        public ImageCollectionAttribute()
            : base()
        {
        }

        #endregion

        #region interface implementation

        public void Process(DisplayMetadataProviderContext context)
        {
            MetaDataAttributePlacementValidator.ValidateCollectionPropertyType(this, context, typeof(int));

            var modelMetaData = context.DisplayMetadata;

            modelMetaData
                .AddAdditionalValueIfNotEmpty("Width", Width)
                .AddAdditionalValueIfNotEmpty("Height", Height)
                .AddAdditionalValueIfNotEmpty("MinWidth", MinWidth)
                .AddAdditionalValueIfNotEmpty("MinHeight", MinHeight);

            modelMetaData.TemplateHint = "ImageAssetCollection";
        }

        public IEnumerable<EntityDependency> GetRelations(object model, PropertyInfo propertyInfo)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            if (propertyInfo == null) throw new ArgumentNullException(nameof(propertyInfo));

            var ids = propertyInfo.GetValue(model) as ICollection<int>;

            foreach (var id in EnumerableHelper.Enumerate(ids))
            {
                yield return new EntityDependency(ImageAssetEntityDefinition.DefinitionCode, id, false);
            }
        }

        #endregion

        #region properties

        /// <summary>
        /// The width of the image for which to browse.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// The height of the image for which to browse.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// The minimum width of the image for which to browse.
        /// </summary>
        public int MinWidth { get; set; }

        /// <summary>
        /// The minimum height of the image for which to browse.
        /// </summary>
        public int MinHeight { get; set; }

        #endregion
    }
}
