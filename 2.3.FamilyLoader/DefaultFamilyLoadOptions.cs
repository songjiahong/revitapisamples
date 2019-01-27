using Autodesk.Revit.DB;

namespace FamilyLoader
{
    public class DefaultFamilyLoadOptions : IFamilyLoadOptions
    {
        public bool OnFamilyFound(bool familyInUse, out bool overwriteParameterValues)
        {
            overwriteParameterValues = !familyInUse;

            return overwriteParameterValues;
        }

        public bool OnSharedFamilyFound(Family sharedFamily, bool familyInUse, out FamilySource source, out bool overwriteParameterValues)
        {
            if (familyInUse)
            {
                source = FamilySource.Project;
                overwriteParameterValues = false;
            }
            else
            {
                source = FamilySource.Family;
                overwriteParameterValues = true;
            }

            return overwriteParameterValues;
        }
    }
}
