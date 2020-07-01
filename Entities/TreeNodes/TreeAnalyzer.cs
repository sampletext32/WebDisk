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

        public static TreeNode BuildTree(string absoluteFolderLocation, string folderName)
        {
            if (Constants.Debug) Console.WriteLine($"BuildTree Folder: {{ {absoluteFolderLocation}\\{folderName} }};");

            TreeAnalyzer analyzer = new TreeAnalyzer();
            analyzer.Retrieve(absoluteFolderLocation, folderName);
            return analyzer.GetTree();
        }

        public void Retrieve(string absoluteFolderLocation, string folderName)
        {
            _tree = RetrieveFolder(absoluteFolderLocation, "", folderName);
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
            if (Constants.Debug) Console.WriteLine($"RetrieveFolder Folder: {{ {absoluteRootLocation}\\{relativeLocation}\\{name} }};");

            if (!Utils.PathIsDirectory(Path.Combine(absoluteRootLocation, relativeLocation, name)))
            {
                throw new ArgumentException("Folder With Invalid Path Found"); //Not Possible In Real Scenario
            }

            TreeFolderNode currentFolderNode = new TreeFolderNode(name);
            currentFolderNode.RelativeLocation = relativeLocation;

            try
            {
                var innerDirectoriesPaths =
                    Directory.EnumerateDirectories(Path.Combine(absoluteRootLocation, relativeLocation, name));

                foreach (var innerDirectoryPath in innerDirectoriesPaths)
                {
                    var innerFolderName =
                        innerDirectoryPath.Substring(innerDirectoryPath.LastIndexOf(Path.DirectorySeparatorChar) + 1);

                    TreeNode folderNode =
                        RetrieveFolder(absoluteRootLocation, Path.Combine(relativeLocation, name), innerFolderName);

                    currentFolderNode.AddChild(folderNode);
                }
            }
            catch
            {
            }

            try
            {
                var innerFilesPaths =
                    Directory.EnumerateFiles(Path.Combine(absoluteRootLocation, relativeLocation, name));

                foreach (var innerFilePath in innerFilesPaths)
                {
                    var fileName = Path.GetFileName(innerFilePath);

                    TreeNode node = new TreeFileNode(fileName);
                    node.RelativeLocation = Path.Combine(relativeLocation, name);
                    node.CalculateHash(absoluteRootLocation);
                    currentFolderNode.AddChild(node);
                }
            }
            catch
            {
            }

            return currentFolderNode;
        }
    }
}