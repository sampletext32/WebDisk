using System;
using Entities.TreeNodes;

namespace Entities.SocketCommands
{
    [Serializable]
    public class ResponseGetTreeCommand : SocketCommand
    {
        public TreeNode GetData()
        {
            return (TreeNode) Data;
        }

        public ResponseGetTreeCommand(TreeNode data) : base(data)
        {
        }
    }
}