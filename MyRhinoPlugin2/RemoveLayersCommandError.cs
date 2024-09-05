using Rhino;
using Rhino.Commands;
using System.Threading.Tasks;
using stuffFsharp;


namespace MyRhinoPlugin2
{
     


    public class RemoveLayers_Error : Command
    {
        public RemoveLayers_Error()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static RemoveLayers_Error Instance { get; private set; }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName => "RemoveLayers_Error";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            // TODO: start here modifying the behaviour of your command.
            // ---
            // Disable redrawing of the document views
            //doc.Views.RedrawEnabled = false;

            // Start a background task
            Task.Run(() =>
            {

                RhinoDocManagement.purgeEmptyLayers(doc, false);


            });


            return Result.Success;
        }
    }
}
