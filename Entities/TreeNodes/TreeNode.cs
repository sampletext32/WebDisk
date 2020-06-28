using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Entities.TreeNodes
{
    [Serializable]
    public class TreeNode
    {
        protected string m_name;

        public TreeNode(string name)
        {
            this.m_name = name;
        }

        public string GetName()
        {
            return m_name;
        }

        public virtual string WrapHtml(string relativePath)
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

        public string GetHash(string absoluteRootLocation)
        {
            var s = WrapHtml(absoluteRootLocation);
            string hash = CreateMD5(s);
            return hash;
        }

        public virtual void BuildHierarchy(string absoluteLocation, bool ignoreRoot = true)
        {
        }
    }
}