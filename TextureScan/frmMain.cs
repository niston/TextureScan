using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TextureScan
{
    public partial class frmMain : Form
    {
        // DDS file format info
        // https://learn.microsoft.com/en-us/windows/win32/direct3ddds/dds-header

        // DDS cubemap flag
        private const int DDSCAPS2_CUBEMAP = 0x200;
        
        // DDS header struct        
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct DDSHeader
        {
            public uint size;		        // must be 124 (size of DDSHeader struct)
            public uint flags;
            public uint height;
            public uint width;
            public uint sizeorpitch;
            public uint depth;
            public uint mipmapcount;
            public uint alphabitdepth;
            public uint[] reserved;

            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
            public struct DDSPixelFormat
            {
                public uint size;
                public uint flags;
                public uint fourcc;
                public uint rgbbitcount;
                public uint rbitmask;
                public uint gbitmask;
                public uint bbitmask;
                public uint alphabitmask;
            }
            public DDSPixelFormat pixelformat;

            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
            public struct DDSCaps
            {
                public uint caps1;
                public uint caps2;
                public uint caps3;
                public uint caps4;
            }
            public DDSCaps ddscaps;
            public uint texturestage;
        }

        // abort flag
        bool bAbortScan = false;

        public frmMain()
        {
            InitializeComponent();
        }

        private DDSHeader ReadHeader(string filePath)
        {
            // header struct
            DDSHeader header;
            
            // texture file stream
            using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                // filestream binary reader
                using (BinaryReader reader = new BinaryReader(stream))
                {                    
                    // read and verify DDS magic
                    byte[] signature = reader.ReadBytes(4);
                    if (!(signature[0] == 'D' && signature[1] == 'D' && signature[2] == 'S' && signature[3] == ' '))
                        throw new FormatException("Not a DDS file (" + filePath + "): Magic Not Found.");

                    // read and verify DDS header size
                    header.size = reader.ReadUInt32();
                    if (header.size != 124)
                        throw new FormatException("Not a DDS file (" + filePath + "): Unexpected Header Struct Size.");

                    // read main header fields
                    header.flags = reader.ReadUInt32();
                    header.height = reader.ReadUInt32();
                    header.width = reader.ReadUInt32();
                    header.sizeorpitch = reader.ReadUInt32();
                    header.depth = reader.ReadUInt32();
                    header.mipmapcount = reader.ReadUInt32();
                    header.alphabitdepth = reader.ReadUInt32();

                    // read 10 reserved bytes
                    header.reserved = new uint[10];
                    for (int i = 0; i < 10; i++)
                    {
                        header.reserved[i] = reader.ReadUInt32();
                    }

                    // read pixel format field
                    header.pixelformat.size = reader.ReadUInt32();
                    header.pixelformat.flags = reader.ReadUInt32();
                    header.pixelformat.fourcc = reader.ReadUInt32();
                    header.pixelformat.rgbbitcount = reader.ReadUInt32();
                    header.pixelformat.rbitmask = reader.ReadUInt32();
                    header.pixelformat.gbitmask = reader.ReadUInt32();
                    header.pixelformat.bbitmask = reader.ReadUInt32();
                    header.pixelformat.alphabitmask = reader.ReadUInt32();

                    // read DDS capabilities fields
                    header.ddscaps.caps1 = reader.ReadUInt32();
                    header.ddscaps.caps2 = reader.ReadUInt32();
                    header.ddscaps.caps3 = reader.ReadUInt32();
                    header.ddscaps.caps4 = reader.ReadUInt32();

                    // read texture stage field
                    header.texturestage = reader.ReadUInt32();
                    
                    // done
                    reader.Close();
                }
                stream.Close();
            }

            return header;
        }

        // main function
        private void ScanFolder()
        {
            try
            {
                bAbortScan = false;

                // clear list
                lsvResults.Items.Clear();                

                // recursively iterate dds files in path
                tslStatus.Text = "Enumerating texture files in path...";
                Application.DoEvents();

                // enumerate texture files
                string[] filePaths = Directory.GetFiles(txtScanPath.Text, "*.dds", SearchOption.AllDirectories);

                // record scan start time
                DateTime startTime = DateTime.Now;

                // scan enumerated files
                int fileCount = 0;

                foreach (string filePath in filePaths)
                {
                    // exit foreach if abort flag is set
                    if (bAbortScan) { break; }

                    // increase filecount
                    fileCount += 1;

                    // UI chatter stuff every 11th file                    
                    if (fileCount % 11 == 0)
                    {
                        tslStatus.Text = "Scanning texture (" + fileCount.ToString() + "/" + filePaths.Length.ToString() + "): " + Path.GetFileName(filePath) + "...";
                        Application.DoEvents();                     
                    }

                    try
                    {
                        // read DDS header
                        DDSHeader ddsHeader = ReadHeader(filePath);
                        
                        // validate dimensions
                        if (!IsValidSize(ddsHeader))
                        {
                            var newItem = lsvResults.Items.Add(filePath);
                            newItem.SubItems.Add(ddsHeader.width.ToString() + " x " + ddsHeader.height.ToString());
                            newItem.SubItems.Add(ddsHeader.mipmapcount.ToString());
                            newItem.SubItems.Add(IsBitSet(ddsHeader.ddscaps.caps2, DDSCAPS2_CUBEMAP) ? "YES" : "NO");
                        }

                    }
                    catch (FormatException fEx)
                    {
                        // file is not a valid dds
                        var newItem = lsvResults.Items.Add(filePath);
                        newItem.SubItems.Add("INVALID");
                    }
                    catch (IndexOutOfRangeException iorEx)
                    {
                        // file has malformed dds header
                        var newItem = lsvResults.Items.Add(filePath);
                        newItem.SubItems.Add("CORRUPTED");
                    }
                }

                tslStatus.Text = "Scanned (" + fileCount + ") textures in (" + DateTime.Now.Subtract(startTime).TotalSeconds + ") seconds.";
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        // open file with default program
        public static void OpenWithDefaultProgram(string path)
        {
            using (Process fileopener = new Process())
            {
                fileopener.StartInfo.FileName = "explorer";
                fileopener.StartInfo.Arguments = "\"" + path + "\"";
                fileopener.Start();
            }
        }

        // validate texture dimensions from DDSHeader
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsValidSize(DDSHeader header)
        {
            // cubemap flag set ?
            if (IsBitSet(header.ddscaps.caps2, DDSCAPS2_CUBEMAP))

            {
                // is a cubemap, dimensions must be multiple of 4
                return IsDivisible(header.width, 4) && IsDivisible(header.height, 4);
            }
            else
            {
                // not a cubemap, dimensions must be power of 2
                return IsPow2(header.width) && IsPow2(header.height);
            }
        }

        // validation helpers
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsBitSet<T>(T t, int pos) where T : struct, IConvertible
        {
            var value = t.ToInt64(CultureInfo.CurrentCulture);
            return (value & (1 << pos)) != 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsPow2(uint n)
        {
            return (n & (n - 1)) == 0 && n > 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsDivisible(uint x, uint n)
        {
            return (x % n) == 0;
        }

        // UI functions
        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            bAbortScan = true;
            Properties.Settings.Default.ScanPath = txtScanPath.Text;
            Properties.Settings.Default.Save();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            txtScanPath.Text = Properties.Settings.Default.ScanPath;
        }

        private void lsvResults_Click(object sender, EventArgs e)
        {
            try
            {
                if (lsvResults.Items.Count > 0)
                {
                    String filePath = lsvResults.SelectedItems[0].Text;
                    OpenWithDefaultProgram(filePath);
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            try
            {
                FolderBrowserDialog fDlg = new FolderBrowserDialog();
                fDlg.SelectedPath = txtScanPath.Text;
                if (fDlg.ShowDialog() == DialogResult.OK)
                {
                    txtScanPath.Text = fDlg.SelectedPath;
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            ScanFolder();
        }

        private void HandleException(Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
