﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Entities.TreeNodes
{
    [Serializable]
    public class TreeFolderNode : TreeNode
    {
        private List<TreeNode> _children;

        public TreeFolderNode(string name) : base(name)
        {
            _children = new List<TreeNode>();
        }

        public void AddChild(TreeNode child)
        {
            _children.Add(child);
        }

        public override string WrapHtml(string absoluteLocation)
        {
            StringBuilder childrenHtmlBuilder = new StringBuilder();
            foreach (var child in _children)
            {
                childrenHtmlBuilder.AppendLine(child.WrapHtml(Path.Combine(absoluteLocation, GetName())));
            }

            string selfHtml = $"<li>{GetName()}<ul>{childrenHtmlBuilder}</ul></li>";

            return selfHtml;
        }

        public override string WrapHashedXML(string absoluteLocation, bool ignoreRoot = true)
        {
            StringBuilder childrenHtmlBuilder = new StringBuilder();
            foreach (var child in _children)
            {
                childrenHtmlBuilder.Append(child.WrapHashedXML(Path.Combine(absoluteLocation, GetName()), false));
            }

            string selfHtml;
            if (ignoreRoot)
            {
                selfHtml = $"<items>{childrenHtmlBuilder}</items>";
            }
            else
            {
                selfHtml = $"<folder><name>{GetName()}</name><items>{childrenHtmlBuilder}</items></folder>";
            }

            return selfHtml;
        }

        public override void BuildHierarchy(string absoluteLocation, bool ignoreRoot = true)
        {
            if (!ignoreRoot)
            {
                Directory.CreateDirectory(Path.Combine(absoluteLocation, GetName()));

                foreach (var b in _children.Where(t => t is TreeFolderNode))
                {
                    b.BuildHierarchy(Path.Combine(absoluteLocation, GetName()), false);
                }

                foreach (var b in _children.Where(t => t is TreeFileNode))
                {
                    b.BuildHierarchy(Path.Combine(absoluteLocation, GetName()));
                }
            }
            else
            {
                foreach (var b in _children.Where(t => t is TreeFolderNode))
                {
                    b.BuildHierarchy(Path.Combine(absoluteLocation), false);
                }

                foreach (var b in _children.Where(t => t is TreeFileNode))
                {
                    b.BuildHierarchy(Path.Combine(absoluteLocation));
                }
            }
        }

        public override void Synchronize(TreeNode remoteNode, string absoluteLocation, bool ignoreRoot = true)
        {
        }
    }
}