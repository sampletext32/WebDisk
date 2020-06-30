using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Entities.TreeNodes
{
    [Serializable]
    public class TreeFolderNode : TreeNode
    {
        private List<TreeNode> _children;

        public TreeFolderNode(string name) : base(name)
        {
            _children = new List<TreeNode>();
        }

        public void AddChild(TreeNode child)
        {
            _children.Add(child);
        }

        public override string WrapHtml(string absoluteLocation)
        {
            StringBuilder childrenHtmlBuilder = new StringBuilder();
            foreach (var child in _children)
            {
                childrenHtmlBuilder.AppendLine(child.WrapHtml(Path.Combine(absoluteLocation, GetName())));
            }

            string selfHtml = $"<li>{GetName()}<ul>{childrenHtmlBuilder}</ul></li>";

            return selfHtml;
        }

        public override string WrapHashedXML(string absoluteLocation, bool ignoreRoot = true)
        {
            StringBuilder childrenHtmlBuilder = new StringBuilder();
            foreach (var child in _children)
            {
                childrenHtmlBuilder.Append(child.WrapHashedXML(Path.Combine(absoluteLocation, GetName()), false));
            }

            string selfHtml;
            if (ignoreRoot)
            {
                selfHtml = $"<items>{childrenHtmlBuilder}</items>";
            }
            else
            {
                selfHtml = $"<folder><name>{GetName()}</name><items>{childrenHtmlBuilder}</items></folder>";
            }

            return selfHtml;
        }

        public override void BuildHierarchy(string absoluteLocation, bool ignoreRoot = true)
        {
            if (!ignoreRoot)
            {
                Directory.CreateDirectory(Path.Combine(absoluteLocation, GetName()));

                foreach (var b in _children.Where(t => t is TreeFolderNode))
                {
                    b.BuildHierarchy(Path.Combine(absoluteLocation, GetName()), false);
                }

                foreach (var b in _children.Where(t => t is TreeFileNode))
                {
                    b.BuildHierarchy(Path.Combine(absoluteLocation, GetName()));
                }
            }
            else
            {
                foreach (var b in _children.Where(t => t is TreeFolderNode))
                {
                    b.BuildHierarchy(Path.Combine(absoluteLocation), false);
                }

                foreach (var b in _children.Where(t => t is TreeFileNode))
                {
                    b.BuildHierarchy(Path.Combine(absoluteLocation));
                }
            }
        }

        public override void Download(TreeNode remoteNode, string absoluteLocation, bool ignoreRoot = true)
        {
            Directory.Delete(Path.Combine(absoluteLocation, GetName()), true);
            Directory.CreateDirectory(Path.Combine(absoluteLocation, GetName()));
            if (!ignoreRoot)
            {
                var treeFolderNode = remoteNode as TreeFolderNode;

                foreach (var b in treeFolderNode._children.Where(t => t is TreeFolderNode))
                {
                    TreeNode folderNode = new TreeFolderNode(b.GetName());
                    folderNode.Download(b, Path.Combine(absoluteLocation, GetName()), false);
                }

                foreach (var b in treeFolderNode._children.Where(t => t is TreeFileNode))
                {
                    TreeNode fileNode = new TreeFileNode(b.GetName());
                    fileNode.Download(b, Path.Combine(absoluteLocation, GetName()));
                }
            }
            else
            {
                var treeFolderNode = remoteNode as TreeFolderNode;

                foreach (var b in treeFolderNode._children.Where(t => t is TreeFolderNode))
                {
                    TreeNode folderNode = new TreeFolderNode(b.GetName());
                    folderNode.Download(b, Path.Combine(absoluteLocation), false);
                }

                foreach (var b in treeFolderNode._children.Where(t => t is TreeFileNode))
                {
                    TreeNode fileNode = new TreeFileNode(b.GetName());
                    fileNode.Download(b, Path.Combine(absoluteLocation));
                }
            }
        }

        public override void Synchronize(TreeNode remoteNode, string absoluteLocation, bool ignoreRoot = true)
        {
            var treeFolderNode = (TreeFolderNode) remoteNode;
            var remoteFolderFiles = treeFolderNode._children.Where(t => t is TreeFileNode);
            var localFolderFiles = _children.Where(t => t is TreeFileNode);

            foreach (var localFolderFile in localFolderFiles)
            {
                // Проверяем наличие такого файла по имени
                var remotePairByName = remoteFolderFiles.FirstOrDefault(t => t.GetName() == localFolderFile.GetName());

                if (remotePairByName == null)
                {
                    // Удалённого файла не существует
                    // TODO: Проверить по хешам и переименовать файл
                    // TODO: Проверить одинаковые ли хеши от файлов с разными названиями но одинаковым контентом
                }
                else
                {
                    // Найден файл с таким же именем

                    if (remotePairByName.GetHash() == localFolderFile.GetHash())
                    {
                        // Если хеш файла одинаковый - файл с таким именем не изменился, игнорируем его инхронизацию
                    }
                    else
                    {
                        // файл с этим именем изменился, синхронизируем его
                        // TODO: что делать? скачивать удалённый файл, тогда сервер - мастер, или отправлять локальный, тогда клиент - мастер
                    }

                    // var remotePairByHash =
                    //     remoteFolderFiles.FirstOrDefault(t => t.GetHash() == localFolderFile.GetHash());
                    // if (remotePairByHash == null)
                    // {
                    //     // Удалённо файл не существует
                    //     // TODO: Upload current file
                    // }
                    // else
                    // {
                    // }
                }
            }
        }
    }
}