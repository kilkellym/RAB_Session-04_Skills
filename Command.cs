#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;

#endregion

namespace RAB_Session_04_Skills
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            IList<Element> pickList = uidoc.Selection.PickElementsByRectangle("Select elements");

            List<CurveElement> lineList = new List<CurveElement>();

            foreach(Element element in pickList)
            {
                if(element is CurveElement)
                {
                    CurveElement curve = element as CurveElement;

                    if(curve.CurveElementType == CurveElementType.ModelCurve)
                        lineList.Add(curve);
                }
            }

            Transaction t = new Transaction(doc);
            t.Start("Create wall");

            Level newLevel = Level.Create(doc, 15);
            WallType currentWT = GetWallTypeByName(doc, "Storefront");
            MEPSystemType pipeSystemType = GetMEPSystemTypeByName(doc, "Domestic Hot Water");
            PipeType pipeType = GetPipeTypeByName(doc, "Default");

            MEPSystemType ductSystemType = GetMEPSystemTypeByName(doc, "Supply Air");
            DuctType ductType = GetDuctTypeByName(doc, "Default");

            foreach (CurveElement currentCurve in lineList)
            {
                GraphicsStyle currentGS = currentCurve.LineStyle as GraphicsStyle;
                Debug.Print(currentGS.Name);

                Curve curve = currentCurve.GeometryCurve;
                XYZ startPoint = curve.GetEndPoint(0);
                XYZ endPoint = curve.GetEndPoint(1);

                //Wall newWall = Wall.Create(doc, curve, newLevel.Id, false);
                //Wall newWall = Wall.Create(doc, curve, currentWT.Id, newLevel.Id, 20, 0, false, false);
                //Pipe newPipe = Pipe.Create(doc, pipeSystemType.Id, pipeType.Id, newLevel.Id, startPoint, endPoint);
                
                switch (currentGS.Name)
                {
                    case "<Lines>":
                        TaskDialog.Show("test", "The line is <Lines>");
                        Duct newDuct = Duct.Create(doc, ductSystemType.Id, ductType.Id, newLevel.Id, startPoint, endPoint);
                        break;

                    case "yellow":
                        TaskDialog.Show("test", "The color is yellow");
                        break;

                    case "blue":
                        TaskDialog.Show("test", "the color is blue");
                        break;

                    default:
                        TaskDialog.Show("test", "There is no color :(");
                        break;
                }
            }

            t.Commit();
            t.Dispose();
            

            TaskDialog.Show("Test", "I have " + lineList.Count + " lines.");

            return Result.Succeeded;
        }

        private WallType GetWallTypeByName(Document doc, string wallType)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfClass(typeof(WallType));
            //collector.OfCategory(BuiltInCategory.OST_Walls).WhereElementIsElementType();

            foreach(WallType currentWT in collector)
            {
                if (currentWT.Name == wallType)
                    return currentWT;
            }

            return null;

        }
        private MEPSystemType GetMEPSystemTypeByName(Document doc, string wallType)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfClass(typeof(MEPSystemType));
            //collector.OfCategory(BuiltInCategory.OST_Walls).WhereElementIsElementType();

            foreach (MEPSystemType currentST in collector)
            {
                if (currentST.Name == wallType)
                    return currentST;
            }

            return null;

        }
        private PipeType GetPipeTypeByName(Document doc, string pipeType)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfClass(typeof(PipeType));
            //collector.OfCategory(BuiltInCategory.OST_Walls).WhereElementIsElementType();

            foreach (PipeType currentPT in collector)
            {
                if (currentPT.Name == pipeType)
                    return currentPT;
            }

            return null;

        }
        private DuctType GetDuctTypeByName(Document doc, string ductType)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfClass(typeof(DuctType));
            //collector.OfCategory(BuiltInCategory.OST_Walls).WhereElementIsElementType();

            foreach (DuctType currentPT in collector)
            {
                if (currentPT.Name == ductType)
                    return currentPT;
            }

            return null;

        }
    }
}
