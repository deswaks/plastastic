using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using BIM.STLExport;

namespace BuildingPrinter
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]

    public class Print : IExternalCommand
    {
        // Til fuckface
        private BIM.STLExport.DataGenerator m_Generator = null;
        private SortedDictionary<string, Category> m_CategoryList = new SortedDictionary<string, Category>();
        private SortedDictionary<string, DisplayUnitType> m_DisplayUnits = new SortedDictionary<string, DisplayUnitType>();
        private static DisplayUnitType m_SelectedDUT = DisplayUnitType.DUT_UNDEFINED;
        private Autodesk.Revit.UI.UIApplication m_Revit = null;
        string fileName;
        bool cbExportColor;
        bool cbIncludeLinked;
        bool cbExportSharedCoordinates;
        DisplayUnitType dup;


        public void fuckFace(Autodesk.Revit.UI.UIApplication revit)
        {
            m_Revit = revit;
            // Create data generator
            m_Generator = new DataGenerator(m_Revit.Application,
                                            m_Revit.ActiveUIDocument.Document,
                                            m_Revit.ActiveUIDocument.Document.ActiveView);

            // Set file name of exported stl
            fileName = @"C:\fuckFace.stl";
            
            // Set binary save format
            SaveFormat saveFormat = SaveFormat.Binary;

            // Set export range
            ElementsExportRange exportRange;
            exportRange = ElementsExportRange.OnlyVisibleOnes;

            // Include linked
            cbIncludeLinked = true;

            // Export in color
            cbExportColor = false;

            // Export in shared coordinates
            cbExportSharedCoordinates = false;

            // Set dup to 2 for millimeters
            dup = DisplayUnitType.DUT_MILLIMETERS;

            // scan for categories and add each of them to selectedCategories
            m_CategoryList = m_Generator.ScanCategories(true);
            List<Category> selectedCategories = m_CategoryList.Values.ToList();

            // create settings object to save setting information
            BIM.STLExport.Settings aSetting = new BIM.STLExport.Settings(saveFormat,
                                                                        exportRange,
                                                                        cbIncludeLinked,
                                                                        cbExportColor,
                                                                        cbExportSharedCoordinates,
                                                                        selectedCategories,
                                                                        dup);
            // save Revit document's triangular data in a temporary file
            m_Generator = new DataGenerator(m_Revit.Application,
                                            m_Revit.ActiveUIDocument.Document,
                                            m_Revit.ActiveUIDocument.Document.ActiveView);
            // Save STL file
            DataGenerator.GeneratorStatus succeed = m_Generator.SaveSTLFile(fileName, aSetting);
        }

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
                fuckFace(uiApp);
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
