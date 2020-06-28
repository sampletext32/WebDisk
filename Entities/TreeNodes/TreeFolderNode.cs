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

        public override void BuildDirectories(string absolutePath)
        {
            Directory.CreateDirectory(Path.Combine(absolutePath, m_name));
            foreach (var b in m_children.Where(t=>t is TreeFolderNode))
            {
                b.BuildDirectories(Path.Combine(absolutePath, m_name));
            }
        }
    }
}