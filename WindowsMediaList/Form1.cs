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

        private TreeNode CreateDirectoryNode(DirectoryInfo di)
        {
            var directoryNode = new TreeNode(di.Name);

            foreach (var directory in di.GetDirectories())
            {
                directoryNode.Nodes.Add(CreateDirectoryNode(directory));
            }

            string stripedFile;
            var files = GetFiles(di);
            foreach (var file in files)
            {
                stripedFile = StripFile(file, di.FullName);
                directoryNode.Nodes.Add(new TreeNode(stripedFile));
            }

            //List<XElement> seq = new List<XElement>();
            //bool isFiles = false;

            //Regex SpecialChars = new Regex(@"([^a-zA-Z0-9. ])+");
            //Regex isVideoFile = new Regex(@"^[\w\-. ]+(\.mp4|\.wmv|\.mov|\.avi|\.mp3)$");
            //string newFileName;

            //foreach (var file in (di.GetFiles()).Select(f => f.Name))
            //{
            //    if (isVideoFile.IsMatch(file))
            //    {
            //        isFiles = true;

            //        newFileName = file;
            //        if (SpecialChars.IsMatch(file))
            //        {
            //            newFileName = SpecialChars.Replace(file, "_");
            //            File.Move(di.FullName + @"\" + file, di.FullName + @"\" + newFileName);
            //        }
            //        seq.Add(new XElement("media", new XAttribute("src", newFileName)));

            //        directoryNode.Nodes.Add(new TreeNode(file));
            //    }
            //}

            //if (isFiles)
            //{
            //    xDoc.Add(new XElement("smil", new XElement("head", new XElement("title", di.Name)), new XElement("body", new XElement("seq", seq))));
            //    xDoc.Save(di.FullName + @"\index.wpl");
            //}
            return directoryNode;
        }

        private bool FilterFiles(string file)
        {
            Regex FilterFile = new Regex(@"^[\w\-. ]+(\.mp4|\.wmv|\.mov|\.avi|\.mp3)$");
            return FilterFile.IsMatch(file);
        }

        private string StripFile(string oldFile, string path)
        {
            Regex SpecialChars = new Regex(@"[^a-zA-Z0-9._]+");
            if (SpecialChars.IsMatch(oldFile))
            {
                var newFile = SpecialChars.Replace(oldFile, "_") + "_ml";
                var newFilePath = MakeFilePath(newFile, path);
                if (!File.Exists(newFilePath))
                {
                    var oldFilePath = MakeFilePath(oldFile, path);

                    //File.Copy(oldFile, newFile);
                }
                oldFile = newFile;
            }

            return oldFile;
        }

        private string MakeFilePath(string item, string path)
        {
            var fullPath = new StringBuilder();
            fullPath.Append(path).Append("\\").Append(item);

            var r = new Regex(@"\\");
            item = r.Replace(fullPath.ToString(), @"\");

            return item;
        }

        private void FileCopy(string oldFile, string newFile)
        {
        }

        private List<string> GetFiles(DirectoryInfo di)
        {
            List<string> files = new List<string>();

            foreach (var file in di.GetFiles())
            {
                if (FilterFiles(file.Name))
                {
                    files.Add(file.Name);
                }
            }

            return files;
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
