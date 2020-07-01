using System;
using Entities.TreeNodes;

namespace Entities.SocketCommands
{
    [Serializable]
    public class DeleteNonExistentCommand : SocketCommand
    {
        public TreeNode GetData()
        {
            return (TreeNode) Data;
        }

        public DeleteNonExistentCommand(TreeNode treeNode) : base(treeNode)
        {
        }
    }
}