using System;
using Entities.TreeNodes;

namespace Entities.Commands
{
    [Serializable]
    public class CommandDeleteNonExistent : Command
    {
        public TreeNode GetData()
        {
            return (TreeNode) Data;
        }

        public CommandDeleteNonExistent(TreeNode treeNode) : base(treeNode)
        {
        }
    }
}