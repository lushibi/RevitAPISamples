using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using Autodesk.Revit;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using System.Linq;
using Autodesk.Revit.ApplicationServices;
using Result = Autodesk.Revit.UI.Result;

namespace TestScript
{
    [Transaction(TransactionMode.Manual)]
    public class Family_CombineSweeps : IExternalCommand
    {
        public static Document RevitDoc;
        public static Autodesk.Revit.ApplicationServices.Application RevitApp;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            RevitApp = commandData.Application.Application;
            if (commandData.Application.ActiveUIDocument == null)
            {
                message = "Please open a document";
                return Result.Failed;
            }
            RevitDoc = commandData.Application.ActiveUIDocument.Document;
            var uiSel = commandData.Application.ActiveUIDocument.Selection;

            var sweepFilter = new ElementClassFilter(typeof(Sweep));
            FilteredElementCollector sweeps = new FilteredElementCollector(RevitDoc);
            sweeps = sweeps.WherePasses(sweepFilter);
            CombinableElementArray cea = new CombinableElementArray();
            foreach (Sweep item in sweeps)
            {
                cea.Append(item);
            }
            using (Transaction transaction = new Transaction(RevitDoc))
            {
                transaction.Start("Combine sweeps");
                RevitDoc.CombineElements(cea);
                transaction.Commit();
            }
            return Result.Succeeded;
        }
    }
}
