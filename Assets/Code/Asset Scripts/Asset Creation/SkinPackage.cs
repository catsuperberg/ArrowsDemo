using System;

namespace AssetScripts.AssetCreation
{
    public class SkinPackage
    {
        public readonly string Name;
        public readonly string GLBModelPath;
        public readonly string IconPath;         
        public readonly string MetadataPath;

        public SkinPackage(string name, string gLBModelPath, string iconPath, string metadataPath)
        {
            if (gLBModelPath == null)
                throw new ArgumentNullException("Can't create " + this.GetType().Name + " without specifying path to glb model");
            if (iconPath == null)
                throw new ArgumentNullException("Can't create " + this.GetType().Name + " without specifying path to icon");
            if (metadataPath == null)
                throw new ArgumentNullException("Can't create " + this.GetType().Name + " without specifying path to metadata");

            Name = name;
            GLBModelPath = gLBModelPath;
            IconPath = iconPath;
            MetadataPath = metadataPath;
        }
    }
}