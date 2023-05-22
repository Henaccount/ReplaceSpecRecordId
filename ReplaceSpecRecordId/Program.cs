using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Autodesk.ProcessPower.PnP3dObjects;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows.Forms;
using AcadApp = Autodesk.AutoCAD.ApplicationServices.Application;
using PlantApp = Autodesk.ProcessPower.PlantInstance.PlantApplication;

[assembly: CommandClass(typeof(ReplaceSpecRecordId.Program))]

namespace ReplaceSpecRecordId
{
    public class Program
    {
        static Form cbform;

        [CommandMethod("ReplaceSpecRecordId", CommandFlags.UsePickSet)]
        public static void launchform()
        {
            Helper.Initialize();
            cbform = new Form();
            Autodesk.AutoCAD.ApplicationServices.Application.ShowModalDialog(cbform);

        }

        public static void closeform()
        {
            cbform.Dispose();
            Helper.Terminate();
        }

        public static string datacontrol(string comboname, ObjectId objectId, string spec, string find, string replace)
        {
            string outcome = "";
            ObjectId objId = new ObjectId();

            if (find.Equals(""))
            {
                ((ComboBox)cbform.Controls[comboname]).Items.Clear();

                PromptEntityResult selResult = Helper.oEditor.GetEntity("Select object");
                objId = selResult.ObjectId;
            }
            else
            {
                objId = objectId;
            }

            if (!objId.ObjectClass.Name.Equals("AcPpDb3dConnector"))
            {

                StringCollection propnames = new StringCollection();
                propnames.Add("Spec");
                propnames.Add("SpecRecordId");
                propnames.Add("PartSizeLongDesc");
                StringCollection propvalues = Helper.ActiveDataLinksManager.GetProperties(objId, propnames, true);

                if (find.Equals(""))
                {
                    string partInfo = propvalues[0] + "|" + propvalues[1] + "|" + propvalues[2];
                    ((ComboBox)cbform.Controls[comboname]).Items.Add(partInfo);
                    ((ComboBox)cbform.Controls[comboname]).SelectedIndex = 0;
                    outcome = "populatedcombo";
                }
                else
                {
                    if (spec.Equals(propvalues[0]) && find.Equals(propvalues[1]))
                    {
                        Helper.ActiveDataLinksManager.SetProperties(objId, new StringCollection() { "SpecRecordId" }, new StringCollection() { replace });
                        outcome = "replacedid";
                    }

                }

            }
            else
            {
                using (Transaction tr = AcadApp.DocumentManager.MdiActiveDocument.Database.TransactionManager.StartTransaction())
                {

                    StringCollection jtyp = new StringCollection();
                    List<KeyValuePair<string, string>> subPartsProps = new List<KeyValuePair<string, string>>();
                    jtyp.Add("JointType");
                    StringCollection tmpcoll = PlantApp.CurrentProject.ProjectParts["Piping"].DataLinksManager.GetProperties(objId, jtyp, true);
                    string jointtype = tmpcoll[0];


                    Autodesk.ProcessPower.PnP3dObjects.Connector Conn = tr.GetObject(objId, OpenMode.ForRead) as Autodesk.ProcessPower.PnP3dObjects.Connector;

                    if (Conn != null)
                    {

                        foreach (Autodesk.ProcessPower.PnP3dObjects.SubPart sp in Conn.AllSubParts)
                        {

                            if (!sp.GetType().ToString().Equals("Autodesk.ProcessPower.PnP3dObjects.WeldSubPart") && !sp.GetType().ToString().Equals("Autodesk.ProcessPower.PnP3dObjects.JointMarkerSubPart"))
                            {

                                PartSizeProperties spprops = sp.PartSizeProperties;

                                string mySpec = "";
                                string mySpecRecordId = "";
                                string myPartSizeLongDesc = "";

                                for (int i = 0; i < spprops.PropCount; i++)
                                {
                                    try
                                    {
                                        //Helper.ActiveDataLinksManager.SetProperties(sp.PartSizeProperties.PartId, new StringCollection() { "PartSizeLongDesc" }, new StringCollection() { "test" });

                                        if (spprops.PropNames[i].Equals("Spec")) mySpec = spprops.PropValue(spprops.PropNames[i]).ToString();
                                        if (spprops.PropNames[i].Equals("SpecRecordId")) mySpecRecordId = spprops.PropValue(spprops.PropNames[i]).ToString();
                                        if (spprops.PropNames[i].Equals("PartSizeLongDesc")) myPartSizeLongDesc = spprops.PropValue(spprops.PropNames[i]).ToString();

                                        //Helper.oEditor.WriteMessage(spprops.PropValue("ShortDescription") + "_" + spprops.PropNames[i] + " ### " + spprops.PropValue(spprops.PropNames[i]).ToString() + "\r\n");
                                    }
                                    catch (System.Exception) { }
                                }

                                if (find.Equals(""))
                                {
                                    string partInfo = mySpec + "|" + mySpecRecordId + "|" + myPartSizeLongDesc;

                                    ((ComboBox)cbform.Controls[comboname]).Items.Add(partInfo);
                                    ((ComboBox)cbform.Controls[comboname]).SelectedIndex = 0;
                                    outcome = "populatedcombo";
                                }
                                else
                                {
                                    if (spec.Equals(mySpec) && find.Equals(mySpecRecordId))
                                    {
                                        Helper.ActiveDataLinksManager.SetProperties(sp.PartSizeProperties.PartId, new StringCollection() { "SpecRecordId" }, new StringCollection() { replace });
                                        outcome = "replacedid";
                                    }
                                }
                            }
                        }

                    }

                    tr.Commit();
                }
            }
            return outcome;
        }

        public static string actualReplace(ObjectId objId, bool coldrun)
        {
            string currentsetup = "";

            for (int i = 1; i < 5; i++)
            {
                ComboBox findbox = (ComboBox)cbform.Controls["FindID" + i];
                ComboBox replacebox = (ComboBox)cbform.Controls["ReplwithID" + i];
                if (findbox.SelectedIndex >= 0 && replacebox.SelectedIndex >= 0)
                {
                    string findstr = findbox.Items[findbox.SelectedIndex].ToString();
                    string replacestr = replacebox.Items[replacebox.SelectedIndex].ToString();
                    if (findstr.Contains("|") && replacestr.Contains("|"))
                    {
                        string[] findsplit = findstr.Split(new char[] { '|' });
                        string[] replacesplit = replacestr.Split(new char[] { '|' });


                        if (findsplit[0].Equals(replacesplit[0]))
                        {
                            if (!coldrun)
                            {
                                string outcome = datacontrol("", objId, findsplit[0], findsplit[1], replacesplit[1]);
                                if (outcome.Equals("replacedid")) break;
                            }
                            currentsetup += findstr + " ---> " + replacestr + "\r\n\r\n";
                        }
                        else
                            Helper.oEditor.WriteMessage("\r\nfind/replace " + i + ": Spec: " + findsplit[0] + "not Equals: " + replacesplit[0]);

                    }
                }
            }
            return currentsetup;
        }

    }
}
