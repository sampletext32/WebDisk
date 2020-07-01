using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Entities.TreeNodes
{
    // узел дерева файлов
    [Serializable]
    public class TreeNode
    {
        // название текущего узла
        public string Name { get; set; }

        // относительный путь внутри дерева
        public string RelativeLocation { get; set; }

        // хеш текущего узла
        public string Hash { get; set; }

        public TreeNode(string name)
        {
            this.Name = name;
        }

        public TreeNode(string name, string relativeLocation, string hash)
        {
            this.Name = name;
            RelativeLocation = relativeLocation;
            Hash = hash;
        }

        // свёртка в HTML
        public virtual string WrapHtml(string rootLocation)
        {
            return "";
        }

        // свёртка в XML
        public virtual string WrapHashedXML(string rootLocation, bool ignoreRoot = true)
        {
            return "";
        }

        // построение хеша узла
        public virtual string CalculateHash(string rootLocation)
        {
            return "Unsupported call on base";
        }

        // загрузка узла
        public virtual void Download(string rootLocation, IRequestPerformer requestPerformer, bool ignoreRoot = true)
        {
        }

        // выгрузка узла
        public virtual void Upload(string rootLocation, IRequestPerformer requestPerformer, bool ignoreRoot = true)
        {
        }

        // удаление несуществующих данных
        public virtual void DeleteNonExistent(string rootLocation, bool ignoreRoot = true)
        {
        }

        // удобный ToString для дебага
        public override string ToString()
        {
            return Path.Combine(RelativeLocation, Name);
        }
    }
}