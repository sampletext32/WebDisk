using System;
using System.IO;

namespace Entities.TreeNodes
{
    // анализатор структуры папок
    public class TreeAnalyzer
    {
        // распакованное дерево
        private TreeNode _tree;

        public TreeAnalyzer()
        {
        }

        // проверка доступности дерева
        public bool IsTreeAvailable()
        {
            return _tree != null;
        }

        // статический метод для постройки дерева
        public static TreeNode BuildTree(string absoluteFolderLocation, string folderName)
        {
            if (Constants.Debug) Console.WriteLine($"BuildTree Folder: {{ {absoluteFolderLocation}\\{folderName} }};");

            // создаём анализатор
            TreeAnalyzer analyzer = new TreeAnalyzer();

            // получаем папку
            analyzer.Retrieve(absoluteFolderLocation, folderName);

            // возвращаем дерево
            return analyzer.GetTree();
        }

        // получение структуры папки folderName, расположенной в absoluteFolderLocation
        public void Retrieve(string absoluteFolderLocation, string folderName)
        {
            _tree = RetrieveFolder(absoluteFolderLocation, "", folderName);
        }

        // метод для получения дерева
        public TreeNode GetTree()
        {
            if (!IsTreeAvailable())
            {
                throw new InvalidOperationException("Tree Is Not Retrieved");
            }

            return _tree;
        }

        // рекурсивный метод получения папки name, c корнем в absoluteRootLocation и относительным путём relativeLocation
        private TreeNode RetrieveFolder(string absoluteRootLocation, string relativeLocation, string name)
        {
            if (Constants.Debug)
                Console.WriteLine($"RetrieveFolder Folder: {{ {absoluteRootLocation}\\{relativeLocation}\\{name} }};");

            // если переданный путь - не папка, папку получить мы не можем
            if (!Utils.PathIsDirectory(Path.Combine(absoluteRootLocation, relativeLocation, name)))
            {
                throw new ArgumentException("Folder With Invalid Path Found"); //Not Possible In Real Scenario
            }

            // создаём узел для текущей папки
            TreeFolderNode currentFolderNode = new TreeFolderNode(name);

            // устанавливаем ей относительный путь
            currentFolderNode.RelativeLocation = relativeLocation;

            try
            {
                // перечисляем все папки внутри текущей
                var innerDirectoriesPaths =
                    Directory.EnumerateDirectories(Path.Combine(absoluteRootLocation, relativeLocation, name));

                foreach (var innerDirectoryPath in innerDirectoriesPaths)
                {
                    // получаем название папки
                    var innerFolderName =
                        innerDirectoryPath.Substring(innerDirectoryPath.LastIndexOf(Path.DirectorySeparatorChar) + 1);

                    // рекурсивно строим дерево для всех вложенных папок
                    TreeNode folderNode =
                        RetrieveFolder(absoluteRootLocation, Path.Combine(relativeLocation, name), innerFolderName);

                    // добавляем узел к текущей папке
                    currentFolderNode.AddChild(folderNode);
                }
            }
            catch
            {
            }

            try
            {
                // перечисляем все файлы в текущей папке
                var innerFilesPaths =
                    Directory.EnumerateFiles(Path.Combine(absoluteRootLocation, relativeLocation, name));

                foreach (var innerFilePath in innerFilesPaths)
                {
                    // получаем название файла
                    var fileName = Path.GetFileName(innerFilePath);

                    // создаём узел
                    TreeNode node = new TreeFileNode(fileName);

                    // сохраняем относительный путь и считаем хеш файла
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