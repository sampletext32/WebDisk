namespace Entities.TreeNodes
{
    public class TreeNode
    {
        protected string m_name;
        protected long m_size;//using backing field for not recollecting size

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
    }
}