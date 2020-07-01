using System;
using Entities.TreeNodes;

namespace Entities.Commands
{
    [Serializable]
    public class CommandGetTreeResponse : Command
    {
        public TreeNode GetData()
        {
            return (TreeNode) Data;
        }

        public CommandGetTreeResponse(TreeNode data) : base(data)
        {
        }
    }
}