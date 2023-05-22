using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.ProcessPower.PnP3dObjects;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AcadApp = Autodesk.AutoCAD.ApplicationServices.Application;
using PlantApp = Autodesk.ProcessPower.PlantInstance.PlantApplication;

namespace ReplaceSpecRecordId
{
    public partial class Form : System.Windows.Forms.Form
    {
        public Form()
        {
            InitializeComponent();
        }

        public void FindID1pick_Click(object sender, EventArgs e)
        {
            Program.datacontrol("FindID1", new ObjectId(), "", "", "");
        }

        public void ReplwithID1pick_Click(object sender, EventArgs e)
        {
            Program.datacontrol("ReplwithID1", new ObjectId(), "", "", "");
        }

        public void FindID2pick_Click(object sender, EventArgs e)
        {
            Program.datacontrol("FindID2", new ObjectId(), "", "", "");
        }

        public void ReplwithID2pick_Click(object sender, EventArgs e)
        {
            Program.datacontrol("ReplwithID2", new ObjectId(), "", "", "");
        }

        public void FindID3pick_Click(object sender, EventArgs e)
        {
            Program.datacontrol("FindID3", new ObjectId(), "", "", "");
        }

        public void ReplwithID3pick_Click(object sender, EventArgs e)
        {
            Program.datacontrol("ReplwithID3", new ObjectId(), "", "", "");
        }

        public void FindID4pick_Click(object sender, EventArgs e)
        {
            Program.datacontrol("FindID4", new ObjectId(), "", "", "");
        }

        public void ReplwithID4pick_Click(object sender, EventArgs e)
        {
            Program.datacontrol("ReplwithID4", new ObjectId(), "", "", "");
        }
        public void ReplaceSpecRecordId_Click(object sender, EventArgs e)
        {
            PromptSelectionOptions pso = new PromptSelectionOptions();
            pso.MessageForAdding = "\nSelect objects";
            PromptSelectionResult selResult = Helper.oEditor.GetSelection(pso);
            if (selResult.Status == PromptStatus.OK)
            {
                try
                {
                    //PromptSelectionResult selResult = Helper.oEditor.SelectImplied();
                    string setup = "";
                    ObjectId[] objIdArray = selResult.Value.GetObjectIds();
                    foreach (ObjectId objId in objIdArray)
                    {
                        if (Helper.ActiveDataLinksManager.HasLinks(objId))
                        {
                            setup = Program.actualReplace(objId, false);
                        }
                    }

                    string message = "";
                    message += "\r\n replaced SpecRecordIds per setup: \r\n\r\n" + setup;
                    message += "\r\n in order to make the actual part replacement, YOU NEED TO EXECUTE THE SPEC UPDATE NOW:  _.PLANTSPECUPDATECHECK";
                    message += "\r\n if the SPECUPDATECHECK doesn't trigger any changes, do activly change something in this spec and try again..";
                    message += "\r\n this code example is experimental, not tested sufficiently";
                    message += "\r\n if not all properties will be updated (per project setup), you will run into data inconsistencies";
                    message += "\r\n if you are unsure, close the drawing now without saving";
                    Helper.InfoMessageBox(message);
                }
                catch (System.Exception ex)
                {
                    Helper.oEditor.WriteMessage(ex.Message);

                }
                finally { }

            }
        }


    }
}
