using System;
using System.IO;

namespace Entities.TreeNodes
{
    public class TreeAnalyzer
    {
        private string m_rootPath;
        private int m_maxNestingLevel;

        private TreeNode m_tree;

        public bool IsTreeAvailable()
        {
            return m_tree != null;
        }

        public TreeAnalyzer(string rootPath, int maxNestingLevel = int.MaxValue)
        {
            SetNestingLevel(maxNestingLevel);
            if (rootPath == string.Empty)
            {
                throw new ArgumentNullException("rootPath was empty");
            }

            m_rootPath = rootPath;
            m_tree = null;
        }

        public void Retrieve()
        {
            LogBuilder.Get()
                .AppendInfo(
                    $"Started FileTree Retrieving At \"{m_rootPath}\" With Nesting Level {(m_maxNestingLevel == int.MaxValue ? "Unlimited" : m_maxNestingLevel.ToString())}");
            m_tree = RetrieveFolder(m_rootPath, 1);
        }

        public TreeNode GetTree()
        {
            if (!IsTreeAvailable())
            {
                LogBuilder.Get().AppendError("Attempted To Get File Tree When Not Retrieved");
                throw new InvalidOperationException("Tree Is Not Retrieved");
            }

            return m_tree;
        }

        private TreeNode RetrieveFolder(string absolutePath, int nestingLevel)
        {
            if (!PathIsDirectory(absolutePath))
            {
                LogBuilder.Get().AppendError($"Attempted to retrieve folder with invalid path \"{absolutePath}\"");
                throw new ArgumentException("Folder With Invalid Path Found");//Not Possible In Real Scenario
            }

            string currentFolderName = absolutePath.Length == 3
                ? absolutePath
                : absolutePath.Substring(absolutePath.LastIndexOf(Path.DirectorySeparatorChar));

            TreeFolderNode currentFolderNode = new TreeFolderNode(currentFolderName);

            string currentFolderPath = Path.Combine(m_rootPath, absolutePath);

            if (nestingLevel > m_maxNestingLevel)
            {
                LogBuilder.Get().AppendInfo($"Reached Nesting Limit At {currentFolderPath}");
                return currentFolderNode;
            }

            try
            {
                var innerDirectoriesPaths = Directory.EnumerateDirectories(currentFolderPath);

                foreach (var innerDirectoryPath in innerDirectoriesPaths)
                {
                    LogBuilder.Get().AppendInfo($"Found Folder \"{innerDirectoryPath}\"");
                    TreeNode folderNode = RetrieveFolder(
                        innerDirectoryPath,
                        nestingLevel + 1);
                    currentFolderNode.AddChild(folderNode);
                }
            }
            catch
            {
                LogBuilder.Get().AppendError($"Unable to get folders in {currentFolderPath}");
            }

            try
            {
                var innerFilesPaths = Directory.EnumerateFiles(currentFolderPath);

                foreach (var innerFilePath in innerFilesPaths)
                {
                    LogBuilder.Get().AppendInfo($"Found File \"{innerFilePath}\"");
                    TreeNode node = new TreeFileNode(Path.GetFileName(innerFilePath),
                        new FileInfo(innerFilePath).Length);
                    currentFolderNode.AddChild(node);
                }
            }
            catch
            {
                LogBuilder.Get().AppendError($"Unable to get files in {currentFolderPath}");
            }

            return currentFolderNode;
        }

        private static bool PathIsDirectory(string path)
        {
            FileAttributes fa = File.GetAttributes(path);
            return fa.HasFlag(FileAttributes.Directory);
        }

        public void SetNestingLevel(int nestingLevel)
        {
            if (nestingLevel < 0)
            {
                throw new ArgumentOutOfRangeException("nestingLevel can't be negative");
            }

            m_maxNestingLevel = nestingLevel;
        }
    }
}