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

        private TreeNode RetrieveFolder(string analyzingRoot, string relativePath)
        {
            string localRoot = Path.Combine(analyzingRoot, relativePath);
            if (!PathIsDirectory(localRoot))
            {
                LogBuilder.Get().AppendError($"Attempted to retrieve folder with invalid path \"{localRoot}\"");
                throw new ArgumentException("Folder With Invalid Path Found"); //Not Possible In Real Scenario
            }

            string currentFolderName;
            if (localRoot.Length == 3)
            {
                // C:\
                currentFolderName = localRoot;
            }
            else
            {
                // Any folder 
                currentFolderName = localRoot.Substring(localRoot.LastIndexOf(Path.DirectorySeparatorChar) + 1);
            }

            TreeFolderNode currentFolderNode = new TreeFolderNode(currentFolderName);

            try
            {
                var innerDirectoriesPaths = Directory.EnumerateDirectories(localRoot);

                foreach (var innerDirectoryPath in innerDirectoriesPaths)
                {
                    var folderName =
                        innerDirectoryPath.Substring(innerDirectoryPath.LastIndexOf(Path.DirectorySeparatorChar) + 1);

                    LogBuilder.Get().AppendInfo($"Found Folder \"{innerDirectoryPath}\"");

                    TreeNode folderNode = RetrieveFolder(analyzingRoot, Path.Combine(localRoot, folderName));
                    currentFolderNode.AddChild(folderNode);
                }
            }
            catch
            {
                LogBuilder.Get().AppendError($"Unable to get folders in {localRoot}");
            }

            try
            {
                var innerFilesPaths = Directory.EnumerateFiles(localRoot);

                foreach (var innerFilePath in innerFilesPaths)
                {
                    var fileName = innerFilePath.Substring(innerFilePath.LastIndexOf(Path.DirectorySeparatorChar) + 1);
                    LogBuilder.Get().AppendInfo($"Found File \"{fileName}\"");
                    TreeNode node = new TreeFileNode(fileName);
                    currentFolderNode.AddChild(node);
                }
            }
            catch
            {
                LogBuilder.Get().AppendError($"Unable to get files in {localRoot}");
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