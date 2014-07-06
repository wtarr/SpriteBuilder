using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SpriteBuilder
{
    public partial class SpriteBuilderForm : Form
    {
        public static BindingList<ImageEntity> LoadedImageList;
        public static BindingList<ImageEntity> StagedImageList;
        public static volatile bool Play;
        public static volatile float Duration;


        public SpriteBuilderForm()
        {
            InitializeComponent();
            InitialiseOpenFileDialog();
            LoadedImageList = new BindingList<ImageEntity>();
            StagedImageList = new BindingList<ImageEntity>();
            //LockObject = new object();

            lbxLoaded.DataSource = LoadedImageList;
            lbxLoaded.DisplayMember = "Filename";
            lbxLoaded.ValueMember = "FilePath";

            lbxStaging.DataSource = StagedImageList;
            lbxStaging.DisplayMember = "Filename";
            lbxStaging.ValueMember = "FilePath";

            Duration = (float)numericUpDown1.Value;

        }

        private void InitialiseOpenFileDialog()
        {
            // Set the file dialog to filter for graphics files
            this.openFileDialog.Filter =
                "Images (*.PNG)|*.PNG";

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
            if (dr == DialogResult.OK)
            {
                // Read the files and populate the load file list box
                foreach (var fileName in this.openFileDialog.FileNames)
                {
                    LoadedImageList.Add(new ImageEntity(Path.GetFileName(fileName), fileName));
                }
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (lbxLoaded.SelectedIndex != -1)
            {
                lock (this)
                {
                    var ie = lbxLoaded.SelectedItem as ImageEntity;
                    StagedImageList.Add(new ImageEntity(ie.Filename, ie.FilePath));
                }
            }
        }

        private void btnAddAll_Click(object sender, EventArgs e)
        {
            lock (this)
            {
                foreach (var imageEntity in LoadedImageList)
                {
                    bool contains = false;

                    foreach (var entity in StagedImageList)
                    {
                        if (imageEntity.Filename.Equals(entity.Filename)) contains = true;
                    }

                    if (!contains) StagedImageList.Add(new ImageEntity(imageEntity.Filename, imageEntity.FilePath));
                }
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            lock (this)
            {
                var itemindex = lbxStaging.SelectedIndex;
                StagedImageList.RemoveAt(itemindex);    
            }
        }


        #region Reorder the image stack
        // http://stackoverflow.com/a/15767630

        private void MoveUp(ListBox myListBox)
        {
            int selectedIndex = myListBox.SelectedIndex;
            if (selectedIndex > 0 & selectedIndex != -1)
            {
                
                StagedImageList.Insert(selectedIndex - 1, (ImageEntity)myListBox.Items[selectedIndex]);
                StagedImageList.RemoveAt(selectedIndex + 1);
                myListBox.SelectedIndex = selectedIndex - 1;
            }
        }

        private void MoveDown(ListBox myListBox)
        {
            int selectedIndex = myListBox.SelectedIndex;
            if (selectedIndex < myListBox.Items.Count - 1 & selectedIndex != -1)
            {
                StagedImageList.Insert(selectedIndex + 2, (ImageEntity)myListBox.Items[selectedIndex]);
                StagedImageList.RemoveAt(selectedIndex);
                myListBox.SelectedIndex = selectedIndex + 1;
            }
        }

        #endregion

        private void btnMoveUp_Click(object sender, EventArgs e)
        {
            MoveUp(lbxStaging);
        }

        private void btnMoveDown_Click(object sender, EventArgs e)
        {
            MoveDown(lbxStaging);
        }

        private void btnAnimate_Click(object sender, EventArgs e)
        {
            Play = true;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            Play = false;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            Duration = (float)numericUpDown1.Value;
        }

        private void btnOutput_Click(object sender, EventArgs e)
        {
            // Output the preview stack as a full sprite sheet
            // set the columns and rows requested accordingly
            lbxLoaded.Enabled = false;
            lbxStaging.Enabled = false;

            var imgList = new List<Image>();
            

            
            
            
            //Bitmap bitmap = new Bitmap();
            // for now join images as one row
            int width = 0;
            int height = 0;
            foreach (var img in CycleImages())
            {
                width += img.Width;
                imgList.Add(img);
            }

            Bitmap bitmap = new Bitmap(width, imgList.First().Height);
           
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                int runninglenght = 0;
                foreach (var image in imgList)
                {
                    g.DrawImage(image, runninglenght, 0, image.Width, image.Height);
                    runninglenght += image.Width;
                }
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();

            saveFileDialog.FileName = "SpriteSheetOutput.png";
            saveFileDialog.Filter = "png (*.png)|*.png";
            saveFileDialog.RestoreDirectory = true;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                bitmap.Save(saveFileDialog.FileName, ImageFormat.Png);
                bitmap.Dispose();
            }

            
            
            lbxLoaded.Enabled = true;
            lbxStaging.Enabled = true;
        }

        private IEnumerable<Image> CycleImages()
        {
            var en = StagedImageList.GetEnumerator();
            while (en.MoveNext())
            {
                var img = Image.FromFile(en.Current.FilePath);
                yield return img;
            }
        }
    }
}
