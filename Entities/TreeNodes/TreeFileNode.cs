using System;
using System.IO;

namespace Entities.TreeNodes
{
    [Serializable]
    public class TreeFileNode : TreeNode
    {
        public TreeFileNode(string name, long size) : base(name)
        {
            m_size = size;
        }

        public override string WrapHtml(string relativePath)
        {
            string hash;
            using (FileStream stream = new FileStream(Path.Combine(relativePath, m_name), FileMode.Open))
            {
                hash = CreateMD5(stream);
            }
            string selfHtml = $"<li>{hash}</li>";

            return selfHtml;
        }
    }
}