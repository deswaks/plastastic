using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;


namespace ExportSTL
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
            List<Element> alleSheets = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Sheets).ToElements().ToList();
            

            MessageBox.Show("!", "Dette er en meddelelse");

            // Starter transaction
            Transaction t = new Transaction(doc);
            t.Start("Export STL");



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
