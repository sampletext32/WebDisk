using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.TreeNodes;

namespace Server
{
    public class SharedFolder
    {
        private string _path;
        private string _hash;

        public SharedFolder(string path)
        {
            _path = path;
        }

        public TreeNode AsTreeNode()
        {
            TreeAnalyzer analyzer = new TreeAnalyzer(_path);
            analyzer.Retrieve();
            var treeNode = analyzer.GetTree();
            return treeNode;
        }
    }
}