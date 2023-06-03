using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Ba2Tools;

namespace TextureScan
{
    public partial class frmMain : Form
    {
        // DDS file format info
        // https://learn.microsoft.com/en-us/windows/win32/direct3ddds/dds-header

        // DDS cubemap flag bit position
        private const int DDSCAPS2_CUBEMAP_BITPOS = 10;
        
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
            public DDSPixelFormat pixelformat;
            public DDSCaps ddscaps;
            public uint texturestage;
        }

        // DDS pixelformat struct
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct DDSPixelFormat
        {
            public uint size;           // must be 32 (size of DDSPixelFormat struct)
            public uint flags;
            public uint fourcc;
            public uint rgbbitcount;
            public uint rbitmask;
            public uint gbitmask;
            public uint bbitmask;
            public uint alphabitmask;
        }

        // DDS capabilities struct
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct DDSCaps
        {
            public uint caps1;
            public uint caps2;
            public uint caps3;
            public uint caps4;
        }

        // path separator for archived files
        private const string ArchPathSep = @"#\";

        // abort flag
        bool bAbortScan = false;

        // running flag
        bool bScanRunning = false;

        public frmMain()
        {
            InitializeComponent();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private DDSHeader ReadHeader(string filePath)
        {
            DDSHeader header;

            // texture file stream
            using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 64, FileOptions.SequentialScan))
            {
                header = ReadHeader(stream);
                stream.Close();
            }

            return header;
        }

        // read DDS header from file
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private DDSHeader ReadHeader(Stream stream)
        {
            // header struct
            DDSHeader header;

            // reset stream to start position
            stream.Seek(0,0);

            // filestream binary reader
            using (BinaryReader reader = new BinaryReader(stream))
            {                    
                // read and verify DDS magic
                byte[] signature = reader.ReadBytes(4);
                if (!(signature[0] == 'D' && signature[1] == 'D' && signature[2] == 'S' && signature[3] == ' '))
                    throw new FormatException("Not a DDS file: Magic Not Found.");

                // read and verify DDS header size
                header.size = reader.ReadUInt32();
                if (header.size != 124)
                    throw new DataMisalignedException("Corrupted DDS file: Unexpected Header Struct Size.");

                // read main header fields
                header.flags = reader.ReadUInt32();
                header.height = reader.ReadUInt32();
                header.width = reader.ReadUInt32();
                header.sizeorpitch = reader.ReadUInt32();
                header.depth = reader.ReadUInt32();
                header.mipmapcount = reader.ReadUInt32();
                header.alphabitdepth = reader.ReadUInt32();

                // read reserved bytes
                header.reserved = new uint[10];
                for (int i = 0; i < 10; i++)
                {
                    header.reserved[i] = reader.ReadUInt32();
                }

                // read and verify pixel format size field
                header.pixelformat.size = reader.ReadUInt32();
                if (header.pixelformat.size != 32)
                    throw new DataMisalignedException("Corrupted DDS file: Unexpected PixelFormat Struct Size.");
                    
                // read rest of pixel format fields
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

            // return the header
            return header;
        }

        // main function - this must go really fast
        private void ScanFolder()
        {
            try
            {
                bScanRunning = true;
                bAbortScan = false;

                // clear list
                lsvResults.Items.Clear();

                // UI chatter
                tslStatus.Text = "Enumerating files...";
                Application.DoEvents();

                // recursively enumerate texture files in path
                List<String> filePaths = new List<string>();
                if (chkProcessBA2.Checked)
                {
                    // include BA2 archives
                    filePaths = Directory.EnumerateFiles(txtScanPath.Text, "*.*", SearchOption.AllDirectories).Where(p => p.ToLower().EndsWith(".dds") || p.ToLower().EndsWith(".ba2")).ToList();
                }
                else
                {
                    // dds files only
                    filePaths = Directory.EnumerateFiles(txtScanPath.Text, "*.dds", SearchOption.AllDirectories).ToList();
                }


                // record scan start time
                DateTime startTime = DateTime.Now;

                // some stats
                int fileCountTotal = filePaths.Where(p => p.ToLower().EndsWith("dds")).Count();
                int fileCountScanned = 0;

                // local caches
                bool bListZeroMipmaps = chkListZeroMipmaps.Checked;
                bool bScanBA2 = chkProcessBA2.Checked;
                bool bAlwaysListGeneralBA2 = chkAlwaysListGeneralBA2.Checked;

                if (bScanBA2)
                {
                    foreach (string filePath in filePaths)
                    {
                        // exit foreach if abort flag is set
                        if (bAbortScan) { break; }

                        // file is archive?
                        if (filePath.ToLower().EndsWith(".ba2"))
                        {
                            try
                            {
                                // file is an archive, process it
                                using (BA2Archive archive = BA2Loader.Load(filePath))
                                {
                                    int archiveFileIndex = 0;
                                    foreach (string archiveFile in archive.FileList)
                                    {
                                        // exit foreach if abort flag is set
                                        if (bAbortScan) { break; }

                                        // is DDS texture file?
                                        if (archive.FileList[archiveFileIndex].ToLower().EndsWith(".dds"))
                                        {
                                            // add dds file to total count
                                            fileCountTotal += 1;
                                            // increase scanned filecount
                                            fileCountScanned += 1;

                                            string aFilePath = archive.FileList[archiveFileIndex].Replace("/", @"\");

                                            // UI chatter
                                            if (fileCountScanned % 21 == 0)
                                            {
                                                tslStatus.Text = "Scanning texture (" + fileCountScanned.ToString() + "/" + fileCountTotal.ToString() + "): " + filePath + ArchPathSep + aFilePath + "...";
                                                Application.DoEvents();
                                            }

                                            try
                                            {
                                                IBA2FileEntry ba2FileEntry = archive.GetFileEntry(archiveFileIndex);

                                                if (ba2FileEntry is BA2TextureFileEntry)
                                                {
                                                    // texture archive file entry, texture info available from BA2 directory entry
                                                    BA2TextureFileEntry ba2TextureFileEntry = (BA2TextureFileEntry)ba2FileEntry;
                                                    if (!IsValidSize(ba2TextureFileEntry) || (bListZeroMipmaps && ba2TextureFileEntry.NumberOfMipmaps == 0))
                                                    {
                                                        var newItem = lsvResults.Items.Add(filePath + ArchPathSep + archive.FileList[archiveFileIndex]);
                                                        newItem.SubItems.Add(ba2TextureFileEntry.TextureWidth.ToString() + " x " + ba2TextureFileEntry.TextureHeight.ToString());
                                                        newItem.SubItems.Add(ba2TextureFileEntry.NumberOfMipmaps.ToString());
                                                        newItem.SubItems.Add((ba2TextureFileEntry.IsCubemap > 0) ? "YES" : "NO");
                                                        newItem.SubItems.Add("BA2 Texture");
                                                    }
                                                }
                                                else if (ba2FileEntry is BA2GeneralFileEntry)
                                                {
                                                    // general archive file entry, requires stream extraction to look at DDS header info
                                                    BA2GeneralFileEntry ba2GeneralFileEntry = (BA2GeneralFileEntry)ba2FileEntry;

                                                    // memory stream for extraction
                                                    using (var stream = new MemoryStream())
                                                    {
                                                        try
                                                        {
                                                            // extract DDS file to memory stream
                                                            archive.ExtractToStream(ba2GeneralFileEntry.Index, stream);

                                                            tslStatus.Text = "Scanning texture (" + fileCountScanned.ToString() + "/" + fileCountTotal.ToString() + "): " + filePath + ArchPathSep + aFilePath + "...";
                                                            Application.DoEvents();

                                                            try
                                                            {
                                                                // read DDS header
                                                                DDSHeader ddsHeader = ReadHeader(stream);

                                                                // validate dimensions
                                                                if (!IsValidSize(ddsHeader) || (bListZeroMipmaps && ddsHeader.mipmapcount == 0) || bAlwaysListGeneralBA2)
                                                                {
                                                                    var newItem = lsvResults.Items.Add(filePath + ArchPathSep + aFilePath);
                                                                    newItem.SubItems.Add(ddsHeader.width.ToString() + " x " + ddsHeader.height.ToString());
                                                                    newItem.SubItems.Add(ddsHeader.mipmapcount.ToString());
                                                                    newItem.SubItems.Add(IsBitSet(ddsHeader.ddscaps.caps2, DDSCAPS2_CUBEMAP_BITPOS) ? "YES" : "NO");
                                                                    newItem.SubItems.Add("BA2 General");
                                                                }

                                                            }
                                                            catch (FormatException fmtEx)
                                                            {
                                                                // file is not a valid dds
                                                                var newItem = lsvResults.Items.Add(filePath + ArchPathSep + aFilePath);
                                                                newItem.ToolTipText = fmtEx.Message;
                                                                newItem.SubItems.Add("INVALID");
                                                                newItem.SubItems.Add("BA2 General");
                                                            }
                                                            catch (DataMisalignedException dmaEx)
                                                            {
                                                                // file has malformed dds header
                                                                var newItem = lsvResults.Items.Add(filePath + ArchPathSep + aFilePath);
                                                                newItem.ToolTipText = dmaEx.Message;
                                                                newItem.SubItems.Add("CORRUPTED");
                                                                newItem.SubItems.Add("BA2 General");
                                                            }
                                                            catch (IndexOutOfRangeException iorEx)
                                                            {
                                                                // file has malformed dds header
                                                                var newItem = lsvResults.Items.Add(filePath + ArchPathSep + aFilePath);
                                                                newItem.ToolTipText = iorEx.Message;
                                                                newItem.SubItems.Add("CORRUPTED");
                                                                newItem.SubItems.Add("BA2 General");
                                                            }
                                                            catch (Exception ex)
                                                            {
                                                                // some other error happened trying to parse the header
                                                                var newItem = lsvResults.Items.Add(filePath + ArchPathSep + aFilePath);
                                                                newItem.ToolTipText = ex.Message;
                                                                newItem.SubItems.Add("ERROR");
                                                                newItem.SubItems.Add("BA2 General");
                                                            }
                                                        }
                                                        catch (NotSupportedException nsEx)
                                                        {
                                                            // Note: should not happen here
                                                            // archived file uses unsupported DDS format, can't extract
                                                            var newItem = lsvResults.Items.Add(filePath + ArchPathSep + aFilePath);
                                                            newItem.ToolTipText = nsEx.Message;
                                                            newItem.SubItems.Add("UNSUPPORTED");
                                                            newItem.SubItems.Add("BA2 General");
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            // some other error while trying to extract from archive
                                                            var newItem = lsvResults.Items.Add(filePath + ArchPathSep + aFilePath);
                                                            newItem.ToolTipText = ex.Message;
                                                            newItem.SubItems.Add("ERROR");
                                                            newItem.SubItems.Add("BA2 General");
                                                        }
                                                        finally
                                                        {
                                                            stream.Close();
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    // unknown BA2 archive type
                                                    var newItem = lsvResults.Items.Add(filePath + ArchPathSep + aFilePath);
                                                    newItem.ToolTipText = "Unknown archive type; Cannot process.";
                                                    newItem.SubItems.Add("UNSUPPORTED");
                                                    newItem.SubItems.Add("BA2 Unknown");
                                                }

                                            }
                                            catch (NotSupportedException nsEx)
                                            {
                                                // archived file uses unsupported DDS format
                                                var newItem = lsvResults.Items.Add(filePath + ArchPathSep + aFilePath);
                                                newItem.ToolTipText = nsEx.Message;
                                                newItem.SubItems.Add("UNSUPPORTED");
                                                newItem.SubItems.Add("BA2");
                                            }
                                            catch (Exception Ex)
                                            {
                                                // some other error while processing BA2
                                                var newItem = lsvResults.Items.Add(filePath + ArchPathSep + aFilePath);
                                                newItem.ToolTipText = Ex.Message;
                                                newItem.SubItems.Add("ERROR");
                                                newItem.SubItems.Add("BA2");
                                            }
                                        }

                                        // next archive file index
                                        archiveFileIndex += 1;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                // some other error while processing BA2
                                var newItem = lsvResults.Items.Add(filePath);
                                newItem.ToolTipText = ex.Message;
                                newItem.SubItems.Add("ERROR");
                                newItem.SubItems.Add("BA2");
                            }
                        }
                        else
                        {
                            // not an archive, but a dds texture file

                            // increase filecount
                            fileCountScanned += 1;

                            // UI chatter stuff every 111th file                    
                            if (fileCountScanned % 111 == 0)
                            {
                                tslStatus.Text = "Scanning texture (" + fileCountScanned.ToString() + "/" + fileCountTotal.ToString() + "): " + filePath + "...";
                                Application.DoEvents();
                            }

                            try
                            {
                                // read DDS header
                                DDSHeader ddsHeader = ReadHeader(filePath);

                                // validate dimensions
                                if (!IsValidSize(ddsHeader) || (bListZeroMipmaps && ddsHeader.mipmapcount == 0))
                                {
                                    var newItem = lsvResults.Items.Add(filePath);
                                    newItem.SubItems.Add(ddsHeader.width.ToString() + " x " + ddsHeader.height.ToString());
                                    newItem.SubItems.Add(ddsHeader.mipmapcount.ToString());
                                    newItem.SubItems.Add(IsBitSet(ddsHeader.ddscaps.caps2, DDSCAPS2_CUBEMAP_BITPOS) ? "YES" : "NO");
                                }

                            }
                            catch (FormatException fmtEx)
                            {
                                // file is not a valid dds
                                var newItem = lsvResults.Items.Add(filePath);
                                newItem.ToolTipText = fmtEx.Message;
                                newItem.SubItems.Add("INVALID");
                            }
                            catch (DataMisalignedException dmaEx)
                            {
                                // file has malformed dds header
                                var newItem = lsvResults.Items.Add(filePath);
                                newItem.ToolTipText = dmaEx.Message;
                                newItem.SubItems.Add("CORRUPTED");
                            }
                            catch (IndexOutOfRangeException iorEx)
                            {
                                // file has malformed dds header
                                var newItem = lsvResults.Items.Add(filePath);
                                newItem.ToolTipText = iorEx.Message;
                                newItem.SubItems.Add("CORRUPTED");
                            }
                            catch (Exception ex)
                            {
                                // some other error happened trying to parse the header
                                var newItem = lsvResults.Items.Add(filePath);
                                newItem.ToolTipText = ex.Message;
                                newItem.SubItems.Add("ERROR");
                            }
                        }
                    }
                }
                else
                {
                    foreach (string filePath in filePaths)
                    {
                        // exit foreach if abort flag is set
                        if (bAbortScan) { break; }

                        // not an archive, but a dds texture file

                        // increase filecount
                        fileCountScanned += 1;

                        // UI chatter stuff every 111th file                    
                        if (fileCountScanned % 111 == 0)
                        {
                            tslStatus.Text = "Scanning texture (" + fileCountScanned.ToString() + "/" + fileCountTotal.ToString() + "): " + filePath + "...";
                            Application.DoEvents();
                        }

                        try
                        {
                            // read DDS header
                            DDSHeader ddsHeader = ReadHeader(filePath);

                            // validate dimensions
                            if (!IsValidSize(ddsHeader) || (bListZeroMipmaps && ddsHeader.mipmapcount == 0))
                            {
                                var newItem = lsvResults.Items.Add(filePath);
                                newItem.SubItems.Add(ddsHeader.width.ToString() + " x " + ddsHeader.height.ToString());
                                newItem.SubItems.Add(ddsHeader.mipmapcount.ToString());
                                newItem.SubItems.Add(IsBitSet(ddsHeader.ddscaps.caps2, DDSCAPS2_CUBEMAP_BITPOS) ? "YES" : "NO");
                            }

                        }
                        catch (FormatException fmtEx)
                        {
                            // file is not a valid dds
                            var newItem = lsvResults.Items.Add(filePath);
                            newItem.ToolTipText = fmtEx.Message;
                            newItem.SubItems.Add("INVALID");
                        }
                        catch (DataMisalignedException dmaEx)
                        {
                            // file has malformed dds header
                            var newItem = lsvResults.Items.Add(filePath);
                            newItem.ToolTipText = dmaEx.Message;
                            newItem.SubItems.Add("CORRUPTED");
                        }
                        catch (IndexOutOfRangeException iorEx)
                        {
                            // file has malformed dds header
                            var newItem = lsvResults.Items.Add(filePath);
                            newItem.ToolTipText = iorEx.Message;
                            newItem.SubItems.Add("CORRUPTED");
                        }
                        catch (Exception ex)
                        {
                            // some other error happened trying to parse the header
                            var newItem = lsvResults.Items.Add(filePath);
                            newItem.ToolTipText = ex.Message;
                            newItem.SubItems.Add("ERROR");
                        }
                    }
                }

                // UI chatter
                if (bAbortScan)
                {
                    tslStatus.Text = "Scan aborted after (" + fileCountScanned + "/" + fileCountTotal.ToString() + ") textures in (" + DateTime.Now.Subtract(startTime).TotalSeconds + ") seconds. Found (" + lsvResults.Items.Count + ") possible offenders.";
                }
                else
                {
                    tslStatus.Text = "Scanned (" + fileCountScanned + ") textures in (" + DateTime.Now.Subtract(startTime).TotalSeconds + ") seconds. Found (" + lsvResults.Items.Count + ") possible offenders.";
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
            finally { bScanRunning = false; }
        }

        // validate texture dimensions from BA2TextureFileEntry
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsValidSize(BA2TextureFileEntry ba2FileEntry)
        {
            if (ba2FileEntry.IsCubemap > 0)
            {
                // is a cubemap, dimensions must be multiple of 4
                return IsDivisible(ba2FileEntry.TextureWidth, 4) && IsDivisible(ba2FileEntry.TextureHeight, 4);
            }
            else
            {
                // not a cubemap, dimensions must be power of 2
                return IsPow2(ba2FileEntry.TextureWidth) && IsPow2(ba2FileEntry.TextureHeight);
            }
        }

        // validate texture dimensions from DDSHeader
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsValidSize(DDSHeader header)
        {
            // cubemap flag set ?
            if (IsBitSet(header.ddscaps.caps2, DDSCAPS2_CUBEMAP_BITPOS))

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
        private bool IsDivisible(uint x, uint n)
        {
            return (x % n) == 0;
        }

        // open file with default program
        private static void OpenWithDefaultProgram(string path)
        {
            using (Process fileopener = new Process())
            {
                fileopener.StartInfo.FileName = "explorer";
                fileopener.StartInfo.Arguments = "\"" + path + "\"";
                fileopener.Start();
            }
        }

        // select file in explorer
        private static void SelectInExplorer(string path)
        {
            using (Process fileopener = new Process())
            {
                fileopener.StartInfo.FileName = "explorer";
                fileopener.StartInfo.Arguments = "/select, \"" + path + "\"";
                fileopener.Start();
            }
        }

        private void AppendHeader(StringBuilder stringBuilder, ListView listView)
        {
            int count = 0;
            foreach (ColumnHeader header in listView.Columns)
            {
                count++;
                if (count != 1) { stringBuilder.Append(";"); }
                stringBuilder.Append(header.Text);
            }
            stringBuilder.Append(Environment.NewLine);
        }

        private void AppendSubItems(StringBuilder stringBuilder, ListViewItem lsvItem)
        {
            int count = 0;
            foreach (ListViewItem.ListViewSubItem subitem in lsvItem.SubItems)
            {
                count++;
                if (count == 1)
                {
                    stringBuilder.Append(subitem.Text);
                }
                else
                {
                    stringBuilder.Append(";" + subitem.Text);
                }
            }
            stringBuilder.Append(Environment.NewLine);
        }

        // UI event handlers etc
        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            // abort any running scan
            bAbortScan = true;

            // save settings
            Properties.Settings.Default.ScanPath = txtScanPath.Text;
            Properties.Settings.Default.ScanBA2 = chkProcessBA2.Checked;
            Properties.Settings.Default.ListZeroMipmaps = chkListZeroMipmaps.Checked;
            Properties.Settings.Default.AlwaysListGeneralBA2 = chkAlwaysListGeneralBA2.Checked;
            Properties.Settings.Default.Save();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            // load and apply settings
            txtScanPath.Text = Properties.Settings.Default.ScanPath;
            chkProcessBA2.Checked = Properties.Settings.Default.ScanBA2;
            chkListZeroMipmaps.Checked = Properties.Settings.Default.ListZeroMipmaps;
            chkAlwaysListGeneralBA2.Checked = Properties.Settings.Default.AlwaysListGeneralBA2;

            // update UI
            if (chkProcessBA2.Checked) { chkAlwaysListGeneralBA2.Enabled = true; } else { chkAlwaysListGeneralBA2.Enabled = false; }
        }

        private void lsvResults_Click(object sender, EventArgs e)
        {
            try
            {
                if (lsvResults.Items.Count > 0)
                {
                    String filePath = lsvResults.SelectedItems[0].Text;
                    if (!filePath.Contains(ArchPathSep))
                    {
                        OpenWithDefaultProgram(filePath);
                    }    
                    else
                    {
                        MessageBox.Show(this, "Editor linking not supported for archived files.", "DDS Texture Scanner", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
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
            if (!bScanRunning)
            {
                // ui shit
                chkProcessBA2.Enabled = false;
                chkListZeroMipmaps.Enabled = false;
                chkAlwaysListGeneralBA2.Enabled = false;
                btnBrowse.Enabled = false;
                txtScanPath.ReadOnly = true;
                btnGo.Text = "STOP";
                lsvResults.Visible = false;

                // do actual work
                ScanFolder();

                // more ui BS
                lsvResults.Visible = true;
                btnGo.Text = "GO";
                txtScanPath.ReadOnly = false;
                btnBrowse.Enabled = true;
                if (chkProcessBA2.Checked) { chkAlwaysListGeneralBA2.Enabled = true; }
                chkListZeroMipmaps.Enabled = true;
                chkProcessBA2.Enabled = true;
            }
            else
            {
                // abort
                bAbortScan = true;
            }
        }

        private void HandleException(Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void chkProcessBA2_CheckedChanged(object sender, EventArgs e)
        {
            if (chkProcessBA2.Checked) { chkAlwaysListGeneralBA2.Enabled = true; } else { chkAlwaysListGeneralBA2.Enabled = false; }
        }

        private void lnkAbout_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenWithDefaultProgram("https://www.nexusmods.com/fallout4/mods/71588");
        }

        private void contextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (lsvResults.Items.Count == 0)
            {
                e.Cancel = true;
            }
            else
            {

            }
        }

        private void cmuCopyListToClipboard_Click(object sender, EventArgs e)
        {
            StringBuilder clpText = new StringBuilder();
            AppendHeader(clpText, lsvResults);
            foreach (ListViewItem item in lsvResults.Items)
            {
                AppendSubItems(clpText, item);
            }
            Clipboard.SetText(clpText.ToString());
            clpText.Clear();
        }

        private void cmuCopyListEntryToClipboard_Click(object sender, EventArgs e)
        {
            if (lsvResults.SelectedItems.Count > 0)
            {
                StringBuilder clpText = new StringBuilder();
                AppendHeader(clpText, lsvResults);
                foreach (ListViewItem item in lsvResults.SelectedItems)
                {
                    AppendSubItems(clpText, item);
                }
                Clipboard.SetText(clpText.ToString());
                clpText.Clear();
            }
        }

        private void cmuOpenPathInExplorer_Click(object sender, EventArgs e)
        {
            try
            {
                if (lsvResults.SelectedItems.Count > 0)
                {
                    string filePath = lsvResults.SelectedItems[0].Text;
                    // strip archive subpaths
                    if (filePath.Contains(ArchPathSep))
                    {
                        filePath = filePath.Substring(0, filePath.IndexOf(ArchPathSep));
                    }                    
                    SelectInExplorer(filePath);
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }
    }
}
