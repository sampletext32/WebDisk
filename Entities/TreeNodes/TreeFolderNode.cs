using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Entities.TreeNodes
{
    public class TreeFolderNode : TreeNode
    {
        private List<TreeNode> m_children;

        public TreeFolderNode(string name) : base(name)
        {
            m_children = new List<TreeNode>();
        }

        public void AddChild(TreeNode child)
        {
            m_children.Add(child);
            m_size += child.GetSize();
        }

        public override string WrapHtml(string relativePath)
        {
            StringBuilder childrenHtmlBuilder = new StringBuilder();
            foreach (var child in m_children)
            {
                childrenHtmlBuilder.Append(child.WrapHtml(Path.Combine(relativePath, m_name)));
            }

            string selfHtml = $"<li>{m_name}<ul>{childrenHtmlBuilder}</ul></li>";

            return selfHtml;
        }
    }
}