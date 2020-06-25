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
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
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