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

        public override string CalculateHash(string rootLocation)
        {
            string xml = WrapHashedXML(rootLocation, false);
            return CreateMD5(xml);
        }

        public override string WrapHtml(string rootLocation)
        {
            StringBuilder childrenHtmlBuilder = new StringBuilder();
            foreach (var child in _children)
            {
                childrenHtmlBuilder.AppendLine(child.WrapHtml(rootLocation));
            }

            string selfHtml = $"<li>{Name}<ul>{childrenHtmlBuilder}</ul></li>";

            return selfHtml;
        }

        public override string WrapHashedXML(string rootLocation, bool ignoreRoot = true)
        {
            StringBuilder childrenHtmlBuilder = new StringBuilder();
            foreach (var child in _children)
            {
                childrenHtmlBuilder.Append(child.WrapHashedXML(rootLocation, false));
            }

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

        public override void BuildHierarchy(string rootLocation, bool ignoreRoot = true)
        {
            if (!ignoreRoot)
            {
                Directory.CreateDirectory(Path.Combine(rootLocation, Name));

                foreach (var b in _children.Where(t => t is TreeFolderNode))
                {
                    b.BuildHierarchy(Path.Combine(rootLocation, Name), false);
                }

                foreach (var b in _children.Where(t => t is TreeFileNode))
                {
                    b.BuildHierarchy(Path.Combine(rootLocation, Name));
                }
            }
            else
            {
                foreach (var b in _children.Where(t => t is TreeFolderNode))
                {
                    b.BuildHierarchy(Path.Combine(rootLocation), false);
                }

                foreach (var b in _children.Where(t => t is TreeFileNode))
                {
                    b.BuildHierarchy(Path.Combine(rootLocation));
                }
            }
        }

        public override void Download(string rootLocation, IRequestPerformer requestPerformer, bool ignoreRoot = true)
        {
            if (!ignoreRoot)
            {
                Directory.CreateDirectory(Path.Combine(rootLocation, RelativeLocation, Name));
            }

            foreach (var b in _children.Where(t => t is TreeFolderNode))
            {
                b.Download(rootLocation, requestPerformer, false);
            }

            foreach (var b in _children.Where(t => t is TreeFileNode))
            {
                b.Download(rootLocation, requestPerformer);
            }
        }

        public override void Upload(string rootLocation, IRequestPerformer requestPerformer, bool ignoreRoot = true)
        {
        }

        public void Synchronize(TreeNode remoteNode, string absoluteLocation, bool ignoreRoot = true)
        {
            var treeFolderNode = (TreeFolderNode) remoteNode;
            var remoteFolderFiles = treeFolderNode._children.Where(t => t is TreeFileNode);
            var localFolderFiles = _children.Where(t => t is TreeFileNode);

            foreach (var localFolderFile in localFolderFiles)
            {
                // Проверяем наличие такого файла по имени
                var remotePairByName = remoteFolderFiles.FirstOrDefault(t => t.Name == localFolderFile.Name);

                if (remotePairByName == null)
                {
                    // Удалённого файла не существует
                    // TODO: Проверить по хешам и переименовать файл
                    // TODO: Проверить одинаковые ли хеши от файлов с разными названиями но одинаковым контентом
                }
                else
                {
                    // Найден файл с таким же именем

                    if (remotePairByName.Hash == localFolderFile.Hash)
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