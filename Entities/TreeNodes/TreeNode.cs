using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Entities.TreeNodes
{
    [Serializable]
    public class TreeNode
    {
        public string Name { get; set; }

        public string RelativeLocation { get; set; }

        public string Hash { get; set; }

        public TreeNode(string name)
        {
            this.Name = name;
        }

        public TreeNode(string name, string relativeLocation, string hash)
        {
            this.Name = name;
            RelativeLocation = relativeLocation;
            Hash = hash;
        }

        public virtual string WrapHtml(string rootLocation)
        {
            return "";
        }

        public virtual string WrapHashedXML(string rootLocation, bool ignoreRoot = true)
        {
            return "";
        }

        public virtual string CalculateHash(string rootLocation)
        {
            return "Unsupported call on base";
        }

        public virtual void Download(string rootLocation, IRequestPerformer requestPerformer, bool ignoreRoot = true)
        {
        }

        public virtual void Upload(string rootLocation, IRequestPerformer requestPerformer, bool ignoreRoot = true)
        {
        }

        public virtual void DeleteNonExistent(string rootLocation, bool ignoreRoot = true)
        {
        }

        public override string ToString()
        {
            return Path.Combine(RelativeLocation, Name);
        }
    }
}