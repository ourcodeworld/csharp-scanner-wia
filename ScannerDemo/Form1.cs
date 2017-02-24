using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WIA;

namespace ScannerDemo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ListScanners();

            // Set start output folder TMP
            textBox1.Text = Path.GetTempPath();
            // Set JPEG as default
            comboBox1.SelectedIndex = 1;

        }

        private void ListScanners()
        {
            // Clear the ListBox.
            listBox1.Items.Clear();

            // Create a DeviceManager instance
            var deviceManager = new DeviceManager();

            // Loop through the list of devices and add the name to the listbox
            for (int i = 1; i <= deviceManager.DeviceInfos.Count; i++)
            {
                // Add the device only if it's a scanner
                if (deviceManager.DeviceInfos[i].Type != WiaDeviceType.ScannerDeviceType)
                {
                    continue;
                }

                // Add the Scanner device to the listbox (the entire DeviceInfos object)
                // Important: we store an object of type scanner (which ToString method returns the name of the scanner)
                listBox1.Items.Add(
                    new Scanner(deviceManager.DeviceInfos[i])
                );
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Task.Factory.StartNew(StartScanning).ContinueWith(result => TriggerScan());
        }

        private void TriggerScan()
        {
            Console.WriteLine("Image succesfully scanned");
        }

        public void StartScanning()
        {
            Scanner device = null;

            this.Invoke(new MethodInvoker(delegate ()
            {
                device = listBox1.SelectedItem as Scanner;
            }));

            if (device == null)
            {
                MessageBox.Show("You need to select first an scanner device from the list",
                                "Warning",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }else if(String.IsNullOrEmpty(textBox2.Text))
            {
                MessageBox.Show("Provide a filename",
                                "Warning",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ImageFile image = new ImageFile();
            string imageExtension = "";

            this.Invoke(new MethodInvoker(delegate ()
            {
                switch (comboBox1.SelectedIndex)
                {
                    case 0:
                        image = device.ScanPNG();
                        imageExtension = ".png";
                        break;
                    case 1:
                        image = device.ScanJPEG();
                        imageExtension = ".jpeg";
                        break;
                    case 2:
                        image = device.ScanTIFF();
                        imageExtension = ".tiff";
                        break;
                }
            }));
            
            
            // Save the image
            var path = Path.Combine(textBox1.Text, textBox2.Text + imageExtension);

            if (File.Exists(path))
            {
                File.Delete(path);
            }

            image.SaveFile(path);

            pictureBox1.Image = new Bitmap(path);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderDlg = new FolderBrowserDialog();
            folderDlg.ShowNewFolderButton = true;
            DialogResult result = folderDlg.ShowDialog();

            if (result == DialogResult.OK)
            {
                textBox1.Text = folderDlg.SelectedPath;
            }
        }
         
    }
}
