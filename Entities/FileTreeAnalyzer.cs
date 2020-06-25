using System;
using System.Collections.Generic;
using System.IO;

namespace Entities
{
    public class FileTreeAnalyzer
    {
        private string m_rootPath;
        private int m_maxNestingLevel;

        private FileTreeNode m_tree;

        private SortedDictionary<string, int> m_fileExtensionsStats;

        public bool IsTreeAvailable()
        {
            return m_tree != null;
        }

        public FileTreeAnalyzer(string rootPath, int maxNestingLevel = int.MaxValue)
        {
            SetNestingLevel(maxNestingLevel);
            if (rootPath == null)
            {
                throw new ArgumentNullException("rootPath was null");
            }

            m_rootPath = rootPath;
            m_tree = null;
            m_fileExtensionsStats = new SortedDictionary<string, int>();
        }

        public void Retrieve()
        {
            LogBuilder.Get()
                .AppendInfo(
                    $"Started FileTree Retrieving At \"{m_rootPath}\" With Nesting Level {(m_maxNestingLevel == int.MaxValue ? "Unlimited" : m_maxNestingLevel.ToString())}");
            m_tree = RetrieveFolder(m_rootPath, 1);
        }

        public FileTreeNode GetTree()
        {
            if (m_tree == null)
            {
                LogBuilder.Get().AppendError("Attempted To Get File Tree When Not Retrieved");
                throw new InvalidOperationException("Tree Is Not Retrieved");
            }

            return m_tree;
        }

        private FileTreeNode RetrieveFolder(string absolutePath, int nestingLevel)
        {
            if (!PathIsDirectory(absolutePath))
            {
                LogBuilder.Get().AppendError($"Attempted to retrieve folder with invalid path \"{absolutePath}\"");
                throw new ArgumentException("Folder With Invalid Path Found");//Not Possible In Real Scenario
            }

            string currentFolderName = absolutePath.Length == 3
                ? absolutePath
                : absolutePath.Substring(absolutePath.LastIndexOf(Path.DirectorySeparatorChar));

            FileTreeFolderNode currentFolderNode = new FileTreeFolderNode(currentFolderName);

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
                    FileTreeNode folderNode = RetrieveFolder(
                        innerDirectoryPath,
                        nestingLevel + 1);
                    currentFolderNode.AddChild(folderNode);

                    ProcessStatsForFolder(innerDirectoryPath);
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
                    FileTreeNode fileNode = new FileTreeFileNode(Path.GetFileName(innerFilePath),
                        new FileInfo(innerFilePath).Length);
                    currentFolderNode.AddChild(fileNode);
                    ProcessStatsForFile(innerFilePath);
                }
            }
            catch
            {
                LogBuilder.Get().AppendError($"Unable to get files in {currentFolderPath}");
            }

            return currentFolderNode;
        }
        private void CreateOrIncrementExtensionKey(string key)
        {
            if (m_fileExtensionsStats.ContainsKey(key))
            {
                //Not quite sure about dictionary handling inplace operations
                m_fileExtensionsStats[key] = m_fileExtensionsStats[key] + 1;
            }
            else
            {
                m_fileExtensionsStats[key] = 1;
            }
        }

        private void ProcessStatsForFolder(string folderPath)
        {
            CreateOrIncrementExtensionKey("folders");
        }


        private void ProcessStatsForFile(string filePath)
        {
            string extension = Path.GetExtension(filePath);
            CreateOrIncrementExtensionKey(extension);
            CreateOrIncrementExtensionKey("files");
        }

        public SortedDictionary<string, int> GetExtensionsStats()
        {
            if (m_tree == null)
            {
                LogBuilder.Get().AppendError("Attempted To Get ExtensionStats When Not Retrieved");
                throw new InvalidOperationException("Tree Is Not Retrieved");
            }

            return m_fileExtensionsStats;
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