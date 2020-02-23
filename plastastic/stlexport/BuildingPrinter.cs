using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using BIM.STLExport;
using System.Windows.Forms;

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
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();

            // Set file name of exported stl
            string fileName;
            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                fileName = folderDialog.SelectedPath + "\\buildingPrint.stl";
            }
            else
            {
                return;
            }


            // Set binary save format
            SaveFormat saveFormat = SaveFormat.Binary;

            // Set export range
            ElementsExportRange exportRange = ElementsExportRange.OnlyVisibleOnes;

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


            //////////
            // FILTER
            //////////
            List<ElementId> elementIds = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Walls).ToElementIds().ToList();
            Transaction t = new Transaction(doc);
            t.Start("Isolate walls");

            doc.ActiveView.IsolateElementsTemporary(elementIds);

            // Transaction close:
            t.Commit();


            //////////
            // EXPORT ALL VISIBLE ELEMENTS
            //////////
            DialogResult dialogResult = MessageBox.Show("Do you want to export to stl?", "stl export", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                fuckFace(uiApp);
            }


            return Result.Succeeded;
        }

    }


    // The Availability Class must be added before the project can be loaded into Revit:
    public class Availability : IExternalCommandAvailability
    {
        public bool IsCommandAvailable(UIApplication a, CategorySet b) { return true; }
    }
}
