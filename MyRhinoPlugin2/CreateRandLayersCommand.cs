using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyRhinoPlugin2
{
     


    public class CreateRandLayers_Error : Command
    {
        public CreateRandLayers_Error()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static CreateRandLayers_Error Instance { get; private set; }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName => "CreateRandLayers_Error";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            // TODO: start here modifying the behaviour of your command.
            // ---
            // Disable redrawing of the document views
            //doc.Views.RedrawEnabled = false;

            // Start a background task
            Task.Run(() =>
            {

                doc.Views.RedrawEnabled = false;

                for ( int i = 0; i < 10; i++ )
                {
                    var layername = $"Building::{i}_{System.Guid.NewGuid()}";
                    int layerIndex = stuff.addLayerByFullPath(doc, layername.ToString(), System.Drawing.Color.DarkBlue);
                }


                doc.Views.RedrawEnabled = true;
                doc.Views.Redraw();



               

            });


            return Result.Success;
        }
    }
}
