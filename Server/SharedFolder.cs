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

        public SharedFolder(string path)
        {
            _path = path;
        }

        public string GetPath()
        {
            return _path;
        }

        public TreeNode AsTreeNode()
        {
            TreeAnalyzer analyzer = new TreeAnalyzer();
            analyzer.Retrieve(_path);
            var treeNode = analyzer.GetTree();
            return treeNode;
        }
    }
}