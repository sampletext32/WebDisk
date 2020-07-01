using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Entities.DataObjects;
using Entities.Commands;

namespace Entities.TreeNodes
{
    // Узел папки в дереве
    [Serializable]
    public class TreeFolderNode : TreeNode
    {
        // список дочерних узлов
        private List<TreeNode> _children;

        public TreeFolderNode(string name) : base(name)
        {
            _children = new List<TreeNode>();
        }

        // добавление узла
        public void AddChild(TreeNode child)
        {
            _children.Add(child);
        }

        // метод для рассчёта хеша папки
        public override string CalculateHash(string rootLocation)
        {
            if (Constants.Debug) Console.WriteLine($"CalculateHash Folder: {{ {this} }};");

            // заворачиваем папку в хешированный XML
            string xml = WrapHashedXML(rootLocation, false);

            // возвращаем хеш от XML
            return Utils.CreateMD5(xml);
        }

        // метод построения хешированного HTML
        public override string WrapHtml(string rootLocation)
        {
            if (Constants.Debug) Console.WriteLine($"WrapHtml Folder: {{ {this} }};");

            // создаём построитель строк - т.к. добавлений может быть много и не нужно перевыделять память
            StringBuilder childrenHtmlBuilder = new StringBuilder();
            foreach (var child in _children)
            {
                // для каждого дочернего узла - добавляем завёрнутый HTML к построителю
                childrenHtmlBuilder.AppendLine(child.WrapHtml(rootLocation));
            }

            // заворачиваем текущий узел и добавляем к нему всех дочерних
            string selfHtml = $"<li>{Name}<ul>{childrenHtmlBuilder}</ul></li>";

            return selfHtml;
        }

        // метод построения хешированного XML
        public override string WrapHashedXML(string rootLocation, bool ignoreRoot = true)
        {
            if (Constants.Debug) Console.WriteLine($"WrapHashedXML Folder: {{ {this} }};");


            // создаём построитель строк - т.к. добавлений может быть много и не нужно перевыделять память
            StringBuilder childrenHtmlBuilder = new StringBuilder();
            foreach (var child in _children)
            {
                // для каждого дочернего узла - добавляем завёрнутый XML к построителю
                childrenHtmlBuilder.Append(child.WrapHashedXML(rootLocation, false));
            }

            // заворачиваем текущий узел и добавляем к нему всех дочерних
            string selfXml;
            if (ignoreRoot)
            {
                selfXml = $"<items>{childrenHtmlBuilder}</items>";
            }
            else
            {
                selfXml = $"<folder><name>{Name}</name><items>{childrenHtmlBuilder}</items></folder>";
            }

            return selfXml;
        }

        // рекурсивный метод загрузки папки
        public override void Download(string rootLocation, IRequestPerformer requestPerformer, bool ignoreRoot = true)
        {
            if (Constants.Debug) Console.WriteLine($"Download Folder: {{ {this} }};");

            // если мы не игнорируем узел - создаём ему папку
            if (!ignoreRoot)
            {
                Directory.CreateDirectory(Path.Combine(rootLocation, RelativeLocation, Name));
            }

            // всем папкам из дочерних элементов вызываем загрузку без игнорирования
            foreach (var b in _children.Where(t => t is TreeFolderNode))
            {
                b.Download(rootLocation, requestPerformer, false);
            }

            // всем файлам из дочерних - вызываем загрузку
            foreach (var b in _children.Where(t => t is TreeFileNode))
            {
                b.Download(rootLocation, requestPerformer);
            }
        }

        // рекурсивный метод выгрузки дерева
        public override void Upload(string rootLocation, IRequestPerformer requestPerformer, bool ignoreRoot = true)
        {
            if (Constants.Debug) Console.WriteLine($"Upload Folder: {{ {this} }};");

            // строим данные о папке
            var folderData = new FolderData(RelativeLocation, Name);

            // создаём команду создания папки и выполняем её
            var createFolderCommand = new CommandCreateFolder(folderData);
            var createFolderCommandBytes = createFolderCommand.Serialize();
            var responseCreateFolderBytes = requestPerformer.PerformRequest(createFolderCommandBytes);
            // ignore response, it's empty
            
            // всем папкам из дочерних элементов вызываем выгрузку без игнорирования
            foreach (var b in _children.Where(t => t is TreeFolderNode))
            {
                b.Upload(rootLocation, requestPerformer, false);
            }

            // всем файлам из дочерних - вызываем выгрузку
            foreach (var b in _children.Where(t => t is TreeFileNode))
            {
                b.Upload(rootLocation, requestPerformer);
            }
        }

        // рекурсивный метод для удаления несуществующих узлов
        public override void DeleteNonExistent(string rootLocation, bool ignoreRoot = true)
        {
            if (Constants.Debug) Console.WriteLine($"DeleteNonExistent Folder: {{ {this} }};");

            // перечисляем отдельно все папки и файлы на диске внутри текущей папки
            var directories = Directory.EnumerateDirectories(Path.Combine(rootLocation, RelativeLocation, Name));
            var files = Directory.EnumerateFiles(Path.Combine(rootLocation, RelativeLocation, Name));

            foreach (var directory in directories)
            {
                // для каждой папки на диске ищем её узел в дочерних текущего узла
                var directoryName = directory.Substring(directory.LastIndexOf(Path.DirectorySeparatorChar) + 1);
                var child = _children.FirstOrDefault(t => t.Name == directoryName);

                // если узел найти не удалось - узел удалён
                if (child == null)
                {
                    // папка не найдена - удаляем саму папку рекурсивно
                    Directory.Delete(directory, true);
                    Console.WriteLine($"Server deleted folder: {{ {directoryName} }}");
                }
                else
                {
                    // папка найдена - удаляем контент рекурсивно
                    child.DeleteNonExistent(rootLocation, false);
                }
            }

            foreach (var file in files)
            {
                // для каждого файла на диске ищем его узел в дочерних текущего узла
                var fileName = Path.GetFileName(file);
                var child = _children.FirstOrDefault(t => t.Name == fileName);

                // если узел найти не удалось - узел удалён
                if (child == null)
                {
                    // серверный файл на клиенте отсутствует - удаляем
                    File.Delete(file);
                    Console.WriteLine($"Server deleted file: {{ {fileName} }}");
                }
                else
                {
                    // Удалять файл нет смысла, он есть в дереве
                }
            }
        }
    }
}