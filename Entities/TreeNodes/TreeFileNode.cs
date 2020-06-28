using System;
using System.IO;

namespace Entities.TreeNodes
{
    [Serializable]
    public class TreeFileNode : TreeNode
    {
        public TreeFileNode(string name) : base(name)
        {
        }

        public override string WrapHtml(string absoluteLocation)
        {
            string hash;
            using (FileStream stream = new FileStream(Path.Combine(absoluteLocation, GetName()), FileMode.Open))
            {
                hash = CreateMD5(stream);
            }

            string selfHtml = $"<li>{hash}</li>";

            return selfHtml;
        }

        public override void BuildHierarchy(string absoluteLocation, bool ignoreRoot = true)
        {
            File.Create(Path.Combine(absoluteLocation, GetName())).Close();
        }
    }
}