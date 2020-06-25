namespace Entities
{
    public class FileTreeFileNode : FileTreeNode
    {
        public FileTreeFileNode(string name, long size) : base(name)
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