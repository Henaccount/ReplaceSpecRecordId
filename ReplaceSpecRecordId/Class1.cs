using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace ReplaceSpecRecordId
{
    public class Class1
    {

        [CommandMethod("ReplaceSpecRecordIdtest")]
        public void ReplaceSpecRecordId()
        {
            // Get the current database and start a transaction
            Database acCurDb;
            acCurDb = Application.DocumentManager.MdiActiveDocument.Database;

            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {

                    
                    acTrans.Commit();
                    Application.ShowAlertDialog("gfdsgfdsgfdsf");
                


            }
        }
    }
}