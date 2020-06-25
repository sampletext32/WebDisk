using System.Collections.Generic;
using System.Text;

namespace Entities
{
    public class FileTreeFolderNode : FileTreeNode
    {
        private List<FileTreeNode> m_children;

        public FileTreeFolderNode(string name) : base(name)
        {
            m_children = new List<FileTreeNode>();
        }

        public void AddChild(FileTreeNode child)
        {
            m_children.Add(child);
            m_size += child.GetSize();
        }

        public override string WrapHtml()
        {
            StringBuilder childrenHtmlBuilder = new StringBuilder();
            foreach (var child in m_children)
            {
                childrenHtmlBuilder.Append(child.WrapHtml());
            }

            string selfHtml = $"<li>{m_name} - {GetSize()} bytes<ul>{childrenHtmlBuilder}</ul></li>";

            return selfHtml;
        }
    }
}