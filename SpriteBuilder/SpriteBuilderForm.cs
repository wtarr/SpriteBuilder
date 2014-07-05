using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SpriteBuilder
{
    public partial class SpriteBuilderForm : Form
    {
        private List<ImageEntity> _imageList;


        public SpriteBuilderForm()
        {
            InitializeComponent();
            InitialiseOpenFileDialog();
            _imageList = new List<ImageEntity>();
        }

        private void InitialiseOpenFileDialog()
        {
            // Set the file dialog to filter for graphics files
            this.openFileDialog.Filter =
                "Images (*.BMP;*.JPG;*.GIF;*.PNG)|*.BMP;*.JPG;*.GIF;*.PNG";

            // Allow the user to select multiple images
            this.openFileDialog.Multiselect = true;
            this.openFileDialog.Title = "Image browser";

        }

        private void mnuExit_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void mnuOpen_Click(object sender, EventArgs e)
        {
            // Open folder dialog and select files
            DialogResult dr = this.openFileDialog.ShowDialog();
            if (dr == System.Windows.Forms.DialogResult.OK)
            {
                // Read the files and populate the load file list box
                foreach (var fileName in this.openFileDialog.FileNames)
                {
                    var img = new ImageEntity(Path.GetFileName(fileName), fileName);
                    _imageList.Add(img);
                    lbxLoaded.Items.Add(img.Filename);
                }
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!lbxStaging.Items.Contains(lbxLoaded.SelectedItem))
                lbxStaging.Items.Add(lbxLoaded.SelectedItem);
        }

        private void btnAddAll_Click(object sender, EventArgs e)
        {
            foreach (var item in lbxLoaded.Items.Cast<object>().Where(item => !lbxStaging.Items.Contains(item)))
            {
                lbxStaging.Items.Add(item);
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            var itemindex = lbxStaging.SelectedIndex;
            lbxStaging.Items.RemoveAt(itemindex);
        }

        void MoveUp(ListBox myListBox)
        {
            int selectedIndex = myListBox.SelectedIndex;
            if (selectedIndex > 0 & selectedIndex != -1)
            {
                myListBox.Items.Insert(selectedIndex - 1, myListBox.Items[selectedIndex]);
                myListBox.Items.RemoveAt(selectedIndex + 1);
                myListBox.SelectedIndex = selectedIndex - 1;
            }
        }

        void MoveDown(ListBox myListBox)
        {
            int selectedIndex = myListBox.SelectedIndex;
            if (selectedIndex < myListBox.Items.Count - 1 & selectedIndex != -1)
            {
                myListBox.Items.Insert(selectedIndex + 2, myListBox.Items[selectedIndex]);
                myListBox.Items.RemoveAt(selectedIndex);
                myListBox.SelectedIndex = selectedIndex + 1;

            }
        }
    }
}
