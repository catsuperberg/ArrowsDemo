using System.Text.RegularExpressions;

namespace Utils
{
    public static class PathUtils
    {            
        public static string ReplaceBackSlashes(this string path)
            => path.Replace("\\", "/");
            
        public static string GetResourcesOnlyPath(this string path)
            => Regex.Match(path.ReplaceBackSlashes(), "(?<=Resources/).*").ToString();
            
        public static string GetPathWithoutExtension(this string path)
            => Regex.Match(path, @".*(?=\..*)").ToString();
            
             
        public static string GetAtResourcesWithNoExtension(this string path)
            => path.GetResourcesOnlyPath().GetPathWithoutExtension();
    }
}