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
            var folderData = new FolderData(RelativeLocation, Name);
            var createFolderCommand = new CreateFolderCommand(folderData);
            var createFolderCommandBytes = createFolderCommand.Serialize();
            var responseCreateFolderBytes = requestPerformer.PerformRequest(createFolderCommandBytes);
            // ignore response, it's empty
            
            foreach (var b in _children.Where(t => t is TreeFolderNode))
            {
                b.Upload(rootLocation, requestPerformer, false);
            }

            foreach (var b in _children.Where(t => t is TreeFileNode))
            {
                b.Upload(rootLocation, requestPerformer);
            }
        }
    }
}