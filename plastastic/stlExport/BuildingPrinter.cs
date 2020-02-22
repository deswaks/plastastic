using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;


namespace BuildingPrinter
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]

    public class ExportSTL : IExternalCommand
    {

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            Document uidoc = commandData.Application.ActiveUIDocument.Document;
            Document doc = uiDoc.Document;


            //FilteredElementCollector finder alle sheets i dokumentet
            // List<Element> alleSheets = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Sheets).ToElements().ToList();

            List<Element> allWalls = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Walls).ToElements().ToList();
            List<ElementId> elementIds = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Walls).ToElementIds().ToList();



            // view isolateWalls = new FilteredElementCollector(doc).OfClass(typeof(View)).Cast<View>().

            string elementstrings = "";
            foreach (ElementId id in elementIds)
            {
                elementstrings = elementstrings + "," + id.ToString();
            }

            Autodesk.Revit.DB.View currentview = doc.ActiveView;

            // MessageBox.Show(elementstrings, "Wall element id's");

            DialogResult dialogResult = MessageBox.Show("Do you wanna export walls to stl?", "stl export", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                
            }
            else if (dialogResult == DialogResult.No)
            {
                //do something else
            }


            // Starter transaction
            Transaction t = new Transaction(doc);
            t.Start("Isolate walls");

            currentview.IsolateElementsTemporary(elementIds);

            // Transaction close:
            t.Commit();
            




            return Result.Succeeded;
        }
    }


    // The Availability Class must be added before the project can be loaded into Revit:
    public class Availability : IExternalCommandAvailability
    {
        public bool IsCommandAvailable(UIApplication a, CategorySet b) { return true; }
    }
}
