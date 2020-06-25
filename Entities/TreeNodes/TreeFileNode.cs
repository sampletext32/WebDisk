namespace Entities.TreeNodes
{
    public class TreeFileNode : TreeNode
    {
        public TreeFileNode(string name, long size) : base(name)
        {
            m_size = size;
        }

        public override string WrapHtml()
        {
            string selfHtml = $"<li>{m_name} - {GetSize()} bytes</li>";

            return selfHtml;
        }
    }
}