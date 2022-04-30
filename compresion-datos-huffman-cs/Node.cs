using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace compresion_datos_huffman_cs
{
    [Serializable]
    public class Node
    {
        private string id;
        private int frequency;
        private string code;
        private Node parent;
        private Node left;  
        private Node right;
        private bool isLeaf;
        
        public Node(string value)
        {
            id = value;
            frequency = 1;

            right = left = parent = null;

            code = "";
            isLeaf = true;
        }


        public Node(Node node1, Node node2)
        {
            id = "";
            code = "";
            isLeaf = false;
            parent = null;

            if (node1.frequency >= node2.frequency)
            {
                right = node1;
                left = node2;
                right.parent = left.parent = this;
                frequency = node1.frequency + node2.frequency;
            }
            else if (node1.frequency < node2.frequency)
            {
                right = node2;
                left = node1;
                left.parent = right.parent = this;
                frequency = node2.frequency + node1.frequency;
            }
        }


        public int CompareTo(Node otherNode)
        {
            return this.frequency.CompareTo(otherNode.frequency);
        }


        public void frequencyIncrease()
        {
            frequency++;
        }
        
        public string getId() { return id; }
        public int getFrequency() { return frequency; }
        public string getCode() { return code; }
        public Node getParent() { return parent; }
        public Node getLeft() { return left; }
        public Node getRight() { return right; }
        public bool getIsLeaf() { return isLeaf; }
        public Node getNode() { return this; }
        
        public void setId(string i) { id = i; }
        public void setFrequency(int f) { frequency = f; }
        public void setCode(string s) { code = s; }
        public void setParent(Node p) { parent = p; }
        public void setLeft(Node p) { left = p; }
        public void setRight(Node p) { right = p; }
        public void setIsLeaf(bool b) { isLeaf = b; }

    }
    [Serializable]
    public class HuffmanTree
    {
        private List<Node> nodeList { get; set; }
        public Node tree { get; set; }
        public string huffmanCode;
        public string text;
        public int index;
        //public List<bool> huffman;
        // byte[] huffByte;
        public HuffmanTree()
        {
            nodeList = new List<Node>();
            huffmanCode = "";
            text = "";
            //huffman = new List<bool>();
        }
        public void getListFromFile(string stream)
        {
                
            foreach(char i in stream)
            {
                string read = Convert.ToChar(i).ToString();
                if (nodeList.Exists(x => x.getId() == read))
                    nodeList[nodeList.FindIndex(y => y.getId() == read)].frequencyIncrease();
                else
                    nodeList.Add(new Node(read));
            }
            //nodeList.Sort();
            nodeList = nodeList.OrderBy(o=>o.getFrequency()).ToList();
            getTreeFromList();
            setCodeToTheTree("", tree);
            buildHuffCode(stream, tree);
            //binaryToBool();
            //binaryToByte();
        }
        public void getTreeFromList()
        {
            while (nodeList.Count > 1)
            {
                Node node1 = nodeList[0];
                nodeList.RemoveAt(0);
                Node node2 = nodeList[0];
                nodeList.RemoveAt(0);
                nodeList.Add(new Node(node1, node2));
                nodeList = nodeList.OrderBy(o=>o.getFrequency()).ToList();
            }
            tree = nodeList[0];
        }
        public void setCodeToTheTree(string code, Node node)
        {
            if (node == null)
                return;
            if (node.getLeft() == null && node.getRight() == null)
            {
                node.setCode(code);
                return;
            }
            setCodeToTheTree(code + "0", node.getLeft());
            setCodeToTheTree(code + "1", node.getRight());
        }
        public void saveBinary(string fileName)
        {
            nodeList.Clear();
            //huffmanCode = "";
            using (Stream stream = File.Open(fileName, FileMode.Create))
            {
                 var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                 bformatter.Serialize(stream, this);
            }
        }
        public void readBinary(string fileName)
        {
            using (Stream stream = File.Open(fileName, FileMode.Open))
            {
                var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                HuffmanTree huff = (HuffmanTree)bformatter.Deserialize(stream);
                nodeList = huff.nodeList;
                tree = huff.tree;
                huffmanCode = huff.huffmanCode;
                //huffByte = huff.huffByte;
                //ByteToBinary();
                stream.Close();
            }
        }
        public void buildHuffCode(string fileContent, Node node)
        {
            foreach (var v in fileContent)
            {
                filterById(v.ToString(), node);
            }
        }
        public void filterById(string c, Node node)
        {
            if (node == null)
                return;
            if (node.getId() == c)
                huffmanCode += node.getCode().ToString();
            filterById(c, node.getRight());
            filterById(c, node.getLeft());
        }
        public void buildText(Node node, string huffmanCode)
        {
            //string temp = huffmanCode;
            //for(int i = 1; i <= temp.Length; i++)
            //{
            //    if(filterByCode(text, temp.Substring(0, i), tree)==true){
            //        //filterByCode(text, temp.Substring(0, i), tree);
            //        temp.Remove(0, i);
            //        i = 1;
            //    }
            //}
            //return text;
            if (node == null)
            {
                return;
            }

            if (node.getIsLeaf())
            {
                text += node.getId();
                return;
            }

            index++;

            if (huffmanCode[index] == '0')
                buildText(node.getLeft(), huffmanCode);
            else
                buildText(node.getRight(), huffmanCode);
        }
        public void Decode()
        {
            index = -1;
            while (index < huffmanCode.Length - 2)
            {
                buildText(tree, huffmanCode);
            }
        }
        //public void filterByCode(string test, string cod, Node node)
        //{
        //    if (node == null)
        //        return false;
        //    if (node.getCode() == cod)
        //    {
        //        return true;
        //    }
        //    filterByCode(test, cod, node.getRight());
        //    filterByCode(test, cod, node.getLeft());
        //    return false;
        //    //return "error";
        //}
        //public void binaryToByte()
        //{
        //    //huffmanCode += new string('0', huffmanCode.Length % 64);
        //    //long[] binary = new long[huffmanCode.Length / 64].Select((x, y) => Convert.ToInt64(huffmanCode.Substring(y * 64, 64), 2)).ToArray();
        //    //huffByte = new byte[binary.Length * sizeof(long)];
        //    //Buffer.BlockCopy(binary, 0, huffByte, 0, huffByte.Length);
        //    huffByte= Convert.FromBase64String(huffmanCode);
        //}
        //public void ByteToBinary()
        //{
        //    huffmanCode = Convert.ToBase64String(huffByte);
        //}
        //public void binaryToBool()
        //{
        //    foreach(char c in huffmanCode)
        //    {
        //        if (c == '0')
        //            huffman.Add(false);
        //        if (c == '1')
        //            huffman.Add(true);
        //    }
        //}
    }
}
