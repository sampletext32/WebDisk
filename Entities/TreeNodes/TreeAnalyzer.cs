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
            var folderName =
                absoluteFolderPath.Substring(absoluteFolderPath.LastIndexOf(Path.DirectorySeparatorChar) + 1);
            _tree = RetrieveFolder(absoluteFolderPath, "", folderName);
        }

        public TreeNode GetTree()
        {
            if (!IsTreeAvailable())
            {
                throw new InvalidOperationException("Tree Is Not Retrieved");
            }

            return _tree;
        }

        private TreeNode RetrieveFolder(string absoluteRootLocation, string relativeLocation, string name)
        {
            string absoluteLocation = Path.Combine(absoluteRootLocation, relativeLocation);
            if (!PathIsDirectory(absoluteLocation))
            {
                throw new ArgumentException("Folder With Invalid Path Found"); //Not Possible In Real Scenario
            }

            TreeFolderNode currentFolderNode = new TreeFolderNode(name);
            currentFolderNode.RelativeLocation = relativeLocation;

            try
            {
                var innerDirectoriesPaths = Directory.EnumerateDirectories(absoluteLocation);

                foreach (var innerDirectoryPath in innerDirectoriesPaths)
                {
                    var folderName =
                        innerDirectoryPath.Substring(innerDirectoryPath.LastIndexOf(Path.DirectorySeparatorChar) + 1);

                    LogBuilder.Get().AppendInfo($"Found Folder \"{innerDirectoryPath}\"");

                    TreeNode folderNode =
                        RetrieveFolder(absoluteRootLocation, Path.Combine(relativeLocation), folderName);
                    currentFolderNode.AddChild(folderNode);
                }
            }
            catch
            {
            }

            try
            {
                var innerFilesPaths = Directory.EnumerateFiles(absoluteLocation);

                foreach (var innerFilePath in innerFilesPaths)
                {
                    var fileName = innerFilePath.Substring(innerFilePath.LastIndexOf(Path.DirectorySeparatorChar) + 1);
                    TreeNode node = new TreeFileNode(fileName);
                    node.CalculateHash(absoluteLocation);
                    node.RelativeLocation = Path.Combine(relativeLocation);
                    currentFolderNode.AddChild(node);
                }
            }
            catch
            {
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