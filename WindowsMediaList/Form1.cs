using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.Collections;

namespace WindowsMediaList
{
    public partial class WMLCreate : Form
    {
        public WMLCreate()
        {
            InitializeComponent();
        }

        private void ListDirectory(TreeView treeView, string path)
        {
            treeView.Nodes.Clear();
            var rootDirectoryInfo = new DirectoryInfo(path);
            treeView.Nodes.Add(CreateDirectoryNode(rootDirectoryInfo));
        }

        private static TreeNode CreateDirectoryNode(DirectoryInfo directoryInfo)
        {
            var directoryNode = new TreeNode(directoryInfo.Name);
            XDocument xDoc = new XDocument();

            foreach (var directory in directoryInfo.GetDirectories())
            {
                directoryNode.Nodes.Add(CreateDirectoryNode(directory));
            }

            List<XElement> seq = new List<XElement>();
            bool isFiles = false;

            Regex SpecialChars = new Regex(@"([^a-zA-Z0-9. ])+");
            Regex isVideoFile = new Regex(@"^[\w\-. ]+(\.mp4|\.wmv|\.mov|\.avi|\.mp3)$");
            string newFileName;

            foreach (var file in (directoryInfo.GetFiles()).Select(f => f.Name))
            {
                if (isVideoFile.IsMatch(file))
                {
                    isFiles = true;

                    newFileName = file;
                    if (SpecialChars.IsMatch(file))
                    {
                        newFileName = SpecialChars.Replace(file, "_");
                        File.Move(directoryInfo.FullName + @"\" + file, directoryInfo.FullName + @"\" + newFileName);
                    }
                    seq.Add(new XElement("media", new XAttribute("src", newFileName)));

                    directoryNode.Nodes.Add(new TreeNode(file));
                }
            }

            if (isFiles)
            {
                xDoc.Add(new XElement("smil", new XElement("head", new XElement("title", directoryInfo.Name)), new XElement("body", new XElement("seq", seq))));
                xDoc.Save(directoryInfo.FullName + @"\index.wpl");
            }
            return directoryNode;
        }

        private void Create_Click(object sender, EventArgs e)
        {
            ListDirectory(DirTreeView, FilePath.Text);
        }

        private void Browse_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();
            FilePath.Text = folderBrowserDialog1.SelectedPath;
        }

    }
}
