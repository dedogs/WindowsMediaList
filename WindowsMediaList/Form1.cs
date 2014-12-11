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
using System.Threading.Tasks;

namespace WindowsMediaList
{
    public partial class WMLCreate : Form
    {
        public WMLCreate()
        {
            InitializeComponent();
        }

        private async void ListDirectory(TreeView treeView, string path)
        {
            treeView.Nodes.Clear();
            var rootDirectoryInfo = new DirectoryInfo(path);
            treeView.Nodes.Add(await CreateDirectoryNode(rootDirectoryInfo));
        }

        private async Task<TreeNode> CreateDirectoryNode(DirectoryInfo di)
        {
            var directoryNode = new TreeNode(di.Name);

            foreach (var directory in di.GetDirectories())
            {
                directoryNode.Nodes.Add(await CreateDirectoryNode(directory));
            }

            var files = GetFiles(di);
            foreach (var file in files)
            {
                directoryNode.Nodes.Add(new TreeNode(file));
            }

            var mediaListFile = MakeFilePath("index.wpl", di.FullName);
            XDocument xDoc = new XDocument(new XElement("smil", new XElement("head", new XElement("title", di.Name)), new XElement("body", new XElement("seq", files.Select(f => new XElement("media", new XAttribute("src", f)))))));
            xDoc.Save(mediaListFile);

            return directoryNode;
        }

        private string MakeFilePath(string item, string path)
        {
            var fullPath = new StringBuilder();
            fullPath.Append(path).Append("\\").Append(item);

            var r = new Regex(@"\\");
            item = r.Replace(fullPath.ToString(), @"\");

            return item;
        }

        private bool FilterFiles(string file)
        {
            Regex FilterFile = new Regex(@"^[\w\-. ]+(\.mp4|\.wmv|\.mov|\.avi|\.mp3)$");
            return FilterFile.IsMatch(file);
        }

        private string StripFile(string oldFile)
        {
            string newFile = null;

            Regex SpecialChars = new Regex(@"[^a-zA-Z0-9._\- ]+", RegexOptions.Multiline);

            if (SpecialChars.IsMatch(oldFile))
            {
                newFile = SpecialChars.Replace(oldFile, "");
            }

            newFile = newFile ?? oldFile;

            return (new Regex(@"[\-]+", RegexOptions.Multiline)).Replace(newFile, "_");
        }

        private List<string> GetFiles(DirectoryInfo di)
        {
            List<string> files = new List<string>();
            string stripedFile;
            var path = di.FullName;

            foreach (var file in di.GetFiles())
            {
                stripedFile = StripFile(file.Name);

                if (FilterFiles(stripedFile))
                {
                    var newFilePath = MakeFilePath(stripedFile, path);

                    if (!File.Exists(newFilePath))
                    {
                        var oldFilePath = MakeFilePath(file.Name, path);

                        File.Move(oldFilePath, newFilePath);
                    }

                    files.Add(stripedFile);
                }
            }

            return files;
        }

        private void Create_Click(object sender, EventArgs e)
        {
            ListDirectory(DirTreeView, FilePath.Text);
            //WMLCreate.ActiveForm.Text = "Done";
        }

        private void Browse_Click(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            folderBrowserDialog1.ShowDialog();
            FilePath.Text = folderBrowserDialog1.SelectedPath;
        }

        private void WMLCreate_Load(object sender, EventArgs e)
        {
        }

    }
}
