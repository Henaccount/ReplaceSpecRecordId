# ReplaceSpecRecordId
Replace the SpecRecordIds of Plant 3D objects, then execute the SPECUPDATECHECK in order to replace parts.

This is experimental code, not tested sufficiently, use at own risk!

This workflow heavity depends on the  SPECUPDATECHECK to execute as expected. It is assumed that all properties are selected to update, if this is not the case, then data inconsistency can arise from the workflow. 

The workflow is based on the fact that the SpecRecordId of a spec part is its reference to the spec, so if you change that SpecRecordId to a different part of the same spec, then the SPECUPDATECHECK will update the part accordinly. This is used here to replace parts from the same spec. With the UI you can select parts to be found and to be replaced. When clicking "Replace Spec Record Ids" you will be prompted to do a selection of parts, e.g. a full pipeline or the whole drawing. Then after pushing "enter" the SpecRecordIds will be replaced accordingly. It is very important that directly after this action there will be the "SPECUPDATECHECK" invoked and executed successfully. If this does not happen to your expectation, you should immediately close the drawing without saving.

Again, use at own risk..
