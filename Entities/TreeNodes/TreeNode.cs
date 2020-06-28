using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Entities.TreeNodes
{
    public class TreeNode
    {
        protected string m_name;
        protected long m_size; //using backing field for not recollecting size

        public TreeNode(string name)
        {
            this.m_name = name;
        }

        public long GetSize()
        {
            return m_size;
        }

        public virtual string WrapHtml()
        {
            return "";
        }

        private static string CreateMD5(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }

                return sb.ToString();
            }
        }

        private static string CreateMD5(Stream input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] hashBytes = md5.ComputeHash(input);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }

                return sb.ToString();
            }
        }

        public string GetHash()
        {
            var s = WrapHtml();
            string hash = CreateMD5(s);
            return hash;
        }
    }
}