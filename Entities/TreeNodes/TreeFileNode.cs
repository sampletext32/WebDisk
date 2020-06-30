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

        public override string CalculateHash(string rootLocation)
        {
            string hash;
            using (FileStream stream = new FileStream(Path.Combine(rootLocation, RelativeLocation, Name), FileMode.Open))
            {
                hash = CreateMD5(stream);
            }

            Hash = hash;
            return hash;
        }

        public override string WrapHtml(string rootLocation)
        {
            string hash = CalculateHash(rootLocation);
            string selfHtml = $"<li>{hash}</li>";

            return selfHtml;
        }

        public override string WrapHashedXML(string rootLocation, bool ignoreRoot = true)
        {
            string hash = CalculateHash(rootLocation);

            string selfHtml = $"<file>{hash}</file>";
            return selfHtml;
        }

        public override void BuildHierarchy(string absoluteLocation, bool ignoreRoot = true)
        {
            File.Create(Path.Combine(absoluteLocation, Name)).Close();
        }

        public override void Download(TreeNode remoteNode, string absoluteLocation, bool ignoreRoot = true)
        {

        }
    }
}