using System;
using System.IO;

namespace Entities.TreeNodes
{
    public class TreeAnalyzer
    {
        private TreeNode _tree;

        public TreeAnalyzer()
        {
        }

        public bool IsTreeAvailable()
        {
            return _tree != null;
        }

        public static TreeNode BuildTree(string absolutePath)
        {
            TreeAnalyzer analyzer = new TreeAnalyzer();
            analyzer.Retrieve(absolutePath);
            return analyzer.GetTree();
        }

        public void Retrieve(string absoluteFolderPath)
        {
            LogBuilder.Get()
                .AppendInfo(
                    $"Started FileTree Retrieving At \"{absoluteFolderPath}");
            _tree = RetrieveFolder(absoluteFolderPath, "");
        }

        public TreeNode GetTree()
        {
            if (!IsTreeAvailable())
            {
                LogBuilder.Get().AppendError("Attempted To Get File Tree When Not Retrieved");
                throw new InvalidOperationException("Tree Is Not Retrieved");
            }

            return _tree;
        }

        private TreeNode RetrieveFolder(string absoluteRootLocation, string relativePath)
        {
            string absoluteLocation = Path.Combine(absoluteRootLocation, relativePath);
            if (!PathIsDirectory(absoluteLocation))
            {
                LogBuilder.Get().AppendError($"Attempted to retrieve folder with invalid path \"{absoluteLocation}\"");
                throw new ArgumentException("Folder With Invalid Path Found"); //Not Possible In Real Scenario
            }

            string currentFolderName;
            if (absoluteLocation.Length == 3)
            {
                // C:\
                currentFolderName = absoluteLocation;
            }
            else
            {
                // Any folder 
                currentFolderName = absoluteLocation.Substring(absoluteLocation.LastIndexOf(Path.DirectorySeparatorChar) + 1);
            }

            TreeFolderNode currentFolderNode = new TreeFolderNode(currentFolderName);
            currentFolderNode.RelativeLocation = relativePath;

            try
            {
                var innerDirectoriesPaths = Directory.EnumerateDirectories(absoluteLocation);

                foreach (var innerDirectoryPath in innerDirectoriesPaths)
                {
                    var folderName =
                        innerDirectoryPath.Substring(innerDirectoryPath.LastIndexOf(Path.DirectorySeparatorChar) + 1);

                    LogBuilder.Get().AppendInfo($"Found Folder \"{innerDirectoryPath}\"");

                    TreeNode folderNode = RetrieveFolder(absoluteRootLocation, Path.Combine(relativePath, folderName));
                    currentFolderNode.AddChild(folderNode);
                }
            }
            catch
            {
                LogBuilder.Get().AppendError($"Unable to get folders in {absoluteLocation}");
            }

            try
            {
                var innerFilesPaths = Directory.EnumerateFiles(absoluteLocation);

                foreach (var innerFilePath in innerFilesPaths)
                {
                    var fileName = innerFilePath.Substring(innerFilePath.LastIndexOf(Path.DirectorySeparatorChar) + 1);
                    LogBuilder.Get().AppendInfo($"Found File \"{fileName}\"");
                    TreeNode node = new TreeFileNode(fileName);
                    node.CalculateHash(absoluteLocation);
                    node.RelativeLocation = Path.Combine(relativePath, fileName);
                    currentFolderNode.AddChild(node);
                }
            }
            catch
            {
                LogBuilder.Get().AppendError($"Unable to get files in {absoluteLocation}");
            }

            return currentFolderNode;
        }

        private static bool PathIsDirectory(string path)
        {
            try
            {
                FileAttributes fa = File.GetAttributes(path);
                return fa.HasFlag(FileAttributes.Directory);
            }
            catch
            {
                return false;
            }
        }
    }
}