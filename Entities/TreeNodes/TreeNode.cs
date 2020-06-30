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

        public static string CreateMD5(Stream input)
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

        public virtual string CalculateHash(string rootLocation)
        {
            Debug.WriteLine("CalculateHash on base");
            return "Unsupported call on base";
        }

        public virtual void Download(string rootLocation, IRequestPerformer requestPerformer, bool ignoreRoot = true)
        {
        }

        public virtual void Upload(string rootLocation, IRequestPerformer requestPerformer, bool ignoreRoot = true)
        {
        }

        public override string ToString()
        {
            return Path.Combine(RelativeLocation, Name);
        }
    }
}