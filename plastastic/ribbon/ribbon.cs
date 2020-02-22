using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace Ribbon
{

    // Knap class. For hvert script/funktion oprettes en Knap med data om funktionen/scriptet.
    public class Button
    {
        public string interntKnapnavn;
        public string synligtKnapNavn;
        public string namespaceNavn;
        public string className;
        public string kortTooltip;
        public string langtTooltip;
        public string panelNavn;
        public string ikonFilnavn;

        /// <summary>
        /// Knap Constructor. Opretter Knap ved angivelse af tilhørende data
        /// </summary>
        /// <param name="interntKnapnavn"> Fremgår ikke i Revit </param>
        /// <param name="synligtKnapNavn"     >Navn, som ses under selve knappen i Revit</param>
        /// <param name="namespaceNavn">Navn på namespace i tilhørende dll-fil</param>
        /// <param name="className"     >Navn på den class, som skal køres</param>
        /// <param name="kortTooltip"  >Kort tooltip, der vises med det samme i Revit ved mouseover</param>
        /// <param name="langtTooltip" >Langt tooltip, der folder sig ned efter nogle sekunders mouseover</param>
        /// <param name="panelNavn"    >Navn på det panel, knappen hører under</param>
        /// <param name="ikonFilnavn"  >// Knap-ikon (png eller ico). 32x32px for store ikoner, 16x16px for små ikoner</param>
        public Button(string interntKnapnavn, string synligtKnapNavn, string namespaceNavn, string className, string kortTooltip, string langtTooltip, string panelNavn, string ikonFilnavn)
        {

            this.interntKnapnavn = interntKnapnavn; // Fremgår ikke i Revit
            this.synligtKnapNavn = synligtKnapNavn; // Navn, som ses under selve knappen i Revit
            this.namespaceNavn = namespaceNavn;   // Navn på namespace i tilhørende dll-fil
            this.className = className;       // Navn på den class, som skal køres
            this.kortTooltip = kortTooltip;     // Kort tooltip, der vises med det samme i Revit ved mouseover
            this.langtTooltip = langtTooltip;    // Langt tooltip, der folder sig ned efter nogle sekunders mouseover
            this.panelNavn = panelNavn;       // Navn på det panel, knappen hører under
            this.ikonFilnavn = ikonFilnavn;     // Knap-ikon (png eller ico). 32x32px for store ikoner, 16x16px for små ikoner
        }
    }

    class Ribbons : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication application)
        {
            // Laver RibbonTab 
            string ribbonName = "Plastastic";
            application.CreateRibbonTab(ribbonName);
            // Læser Revit version (fx '2019')
            string versionNumber = application.ControlledApplication.VersionNumber;

            // Sti til denne dll (sti til dll med dét script, en knap skal køre):
            string thisAssemblyPath = System.Reflection.Assembly.GetExecutingAssembly().Location;

            // Laver fx ribbon panel kaldt "STL Export" under fanen "plastastic"
            Dictionary<string, RibbonPanel> buttonPanel = new Dictionary<string, RibbonPanel>();

            // Navne på panels
            List<string> panelNames = new List<string>(new string[]{
                "Building Printer"
            });

            // Opretter RibbonPanel for hvert navn:
            foreach (string panelName in panelNames)
            {
                buttonPanel.Add(panelName, application.CreateRibbonPanel(ribbonName, panelName));
            }

            // Liste med knapper for alle scripts. Hver knap angiver et script
            List<Button> buttons = new List<Button>
            {
                new Button("stlExport", "Export STL", "BuildingPrinter", "Print", "Exports an STL","", "Building Printer", "exportstl.PNG"),
            };

            // Opretter knapper på Revit ribbon
            foreach (Button but in buttons)
            {
                Uri bt_ikon = new Uri(@"C:\ProgramData\Autodesk\Revit\Addins\" + versionNumber + @"\deswaks\" + but.ikonFilnavn);

                PushButtonData bt = new PushButtonData(but.interntKnapnavn, but.synligtKnapNavn, thisAssemblyPath, but.namespaceNavn + "." + but.className)
                {
                    AvailabilityClassName = but.namespaceNavn + ".Availability",
                    ToolTip = but.kortTooltip,
                    LongDescription = but.langtTooltip,
                    LargeImage = new BitmapImage(bt_ikon)
                };
                buttonPanel[but.panelNavn].AddItem(bt);
            }
            return Result.Succeeded;
        }

        // Lukker ribbon når Revit lukkes
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

    }
}
