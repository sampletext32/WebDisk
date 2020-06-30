using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Entities.TreeNodes
{
    [Serializable]
    public class TreeNode
    {
        protected string _name;

        protected string _relativeLocation;

        protected string _hash;

        public TreeNode(string name)
        {
            this._name = name;
        }

        public string GetHash()
        {
            return _hash;
        }

        public void SetHash(string hash)
        {
            _hash = hash;
        }

        public string GetName()
        {
            return _name;
        }

        public void SetRelativePath(string relativePath)
        {
            _relativeLocation = relativePath;
        }

        public virtual string WrapHtml(string absoluteLocation)
        {
            return "";
        }

        public virtual string WrapHashedXML(string absoluteLocation, bool ignoreRoot = true)
        {
            return "";
        }

        protected static string CreateMD5(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder(32);
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }

                return sb.ToString();
            }
        }

        protected static string CreateMD5(Stream input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] hashBytes = md5.ComputeHash(input);

                StringBuilder sb = new StringBuilder(32);
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }

                return sb.ToString();
            }
        }

        public virtual string CalculateHash(string absoluteLocation)
        {
            return "Unsupported call on base";
        }

        public virtual void BuildHierarchy(string absoluteLocation, bool ignoreRoot = true)
        {
        }

        public virtual void Synchronize(TreeNode remoteNode, string absoluteLocation, bool ignoreRoot = true)
        {
        }

        public virtual void Download(TreeNode remoteNode, string absoluteLocation, bool ignoreRoot = true)
        {
        }
    }
}