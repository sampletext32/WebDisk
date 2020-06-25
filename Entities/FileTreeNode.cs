namespace Entities
{
    public class FileTreeNode
    {
        protected string m_name;
        protected long m_size;//using backing field for not recollecting size

        public FileTreeNode(string name)
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
    }
}