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
            Name = name;
            GLBModelPath = gLBModelPath ?? throw new ArgumentNullException("Can't create " + this.GetType().Name + " without specifying path to glb model");
            IconPath = iconPath ?? throw new ArgumentNullException("Can't create " + this.GetType().Name + " without specifying path to icon");
            MetadataPath = metadataPath ?? throw new ArgumentNullException("Can't create " + this.GetType().Name + " without specifying path to metadata");
        }
    }
}