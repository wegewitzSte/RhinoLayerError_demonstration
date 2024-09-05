using Rhino.DocObjects;
using Rhino;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyRhinoPlugin2
{
    public static class stuff
    {


        /// <summary>
        /// Merge string parts by index. Used for merging rhino paths together at ceratin indexes
        /// </summary>
        /// <param name="parts">Layer paths contain one or more valid layers names, with each name separated by ModelComponent.NamePathSeparator . For example, "Grandfather::Father::Son".</param>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static string MergeStringPartsByIndex(string[] parts, int index)
        {
            if ( index < 0 || index >= parts.Length )
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Index must be within the bounds of the parts array.");
            }

            StringBuilder mergedString = new StringBuilder();
            for ( int i = 0; i <= index; i++ )
            {
                mergedString.Append(parts[i]);
                if ( i < index ) // Add the separator if it's not the last element being processed
                {
                    mergedString.Append("::");
                }
            }

            return mergedString.ToString();
        }

        /// <summary>
        /// Adds layer by a given path. If no path can be created it does nothing. Does not check Parts of the Layer if they have a valid name. 
        /// The layerpaths needs to be already splitted into different chunks without seperator "::"
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="RhinoLayerPathParts">Layer paths contain one or more valid layers names, with each name separated by ModelComponent.NamePathSeparator . For example, "Grandfather::Father::Son".</param>
        /// <param name="color">color of the layer</param>
        /// <returns></returns>
        private static int AddLayerByPath(RhinoDoc doc, string[] RhinoLayerPathParts, System.Drawing.Color color)
        {
            int index = 0;
            int previousLayer = 0;

            for ( int i = 0; i < RhinoLayerPathParts.Length; i++ )
            {
                var currentLayerPath = MergeStringPartsByIndex(RhinoLayerPathParts, i);
                var indexCurrentLayer = doc.Layers.FindByFullPath(currentLayerPath, RhinoMath.UnsetIntIndex);

                if ( indexCurrentLayer == RhinoMath.UnsetIntIndex )
                {
                    // Add current Layer to doc
                    var newLayer = new Layer();
                    newLayer.Name = RhinoLayerPathParts[i];
                    newLayer.Color = color;

                    if ( previousLayer != 0 )
                    {
                        var Parent = doc.Layers.FindIndex(previousLayer);
                        newLayer.ParentLayerId = Parent.Id;
                    }

                    indexCurrentLayer = doc.Layers.Add(newLayer);

                }

                index = indexCurrentLayer;
                previousLayer = indexCurrentLayer;
            }
            return index;
        }

        /// <summary>
        /// Adds a layer to Rhino with given rhinoLayerpath syntax
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="RhinoLayerPath">Layer paths contain one or more valid layers names, with each name separated by ModelComponent.NamePathSeparator . For example, "Grandfather::Father::Son".</param>
        /// <param name="color"></param>
        /// <returns></returns>
        public static int addLayerByFullPath(RhinoDoc doc, string RhinoLayerPath, System.Drawing.Color color)
        {
            var foundlayer = doc.Layers.FindByFullPath(RhinoLayerPath, RhinoMath.UnsetIntIndex);

            /// if layer not found
            if ( foundlayer == RhinoMath.UnsetIntIndex )
            {
                // Split the string based on the "::" delimiter
                string[] parts = RhinoLayerPath.Split(new string[] { "::" }, StringSplitOptions.None);

                foreach ( var name in parts )
                {
                    if ( Layer.IsValidName(name) == false )
                    {
                        RhinoApp.WriteLine($"Layername {name} invalid");
                        return RhinoMath.UnsetIntIndex;
                    }
                }

                foundlayer = AddLayerByPath(doc, parts, color);
            }

            return foundlayer;
        }

    }

}
