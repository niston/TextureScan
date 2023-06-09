﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using Ba2Tools.Internal;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace Ba2Tools
{
    /// <summary>
    /// Represents texture archive type.
    /// </summary>
    public sealed class BA2TextureArchive : BA2Archive
    {
        /// <summary>
        /// File entries in archive. Length of array equals to header.TotalFiles.
        /// </summary>
        private BA2TextureFileEntry[] m_entries = null;

        private object m_lock = new object();

        #region BA2Archive Overrides

        /// <summary>
        /// Extract all files from archive to specified directory.
        /// </summary>
        /// <param name="destination">Destination directory where extracted files will be placed.</param>
        /// <param name="overwriteFiles">Overwrite files on disk with extracted ones?</param>
        public override void ExtractAll(string destination, bool overwriteFiles)
        {
            lock (m_lock)
            {
                CheckDisposed();
                this.ExtractFilesInternal(m_entries, destination, CancellationToken.None, null, overwriteFiles);
            }
        }

        /// <summary>
        /// Extract all files from archive to specified directory with
        /// cancellation token.
        /// </summary>
        /// <param name="destination">Directory where extracted files will be placed.</param>
        /// <param name="overwriteFiles">Overwrite existing files in extraction directory?</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public override void ExtractAll(string destination, bool overwriteFiles, CancellationToken cancellationToken)
        {
            lock (m_lock)
            {
                CheckDisposed();
                this.ExtractFilesInternal(m_entries, destination, cancellationToken, null, overwriteFiles);
            }
        }

        /// <summary>
        /// Extract all files from archive to specified directory with
        /// cancellation token and progress reporter.
        /// </summary>
        /// <param name="destination">Absolute or relative directory path directory where extracted files will be placed.</param>
        /// <param name="overwriteFiles">Overwrite files on disk with extracted ones?</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <param name="progress">Progress reporter ranged from 0 to archive's total files count.</param>
        public override void ExtractAll(string destination, bool overwriteFiles, CancellationToken cancellationToken,
            IProgress<int> progress)
        {
            lock (m_lock)
            {
                CheckDisposed();
                this.ExtractFilesInternal(m_entries, destination, cancellationToken, progress, overwriteFiles);
            }
        }

        /// <summary>
        /// Extract all files from archive.
        /// </summary>
        /// <param name="fileNames">Files to extract.</param>
        /// <param name="destination">Directory where extracted files will be placed.</param>
        /// <param name="overwriteFiles">Overwrite existing files in extraction directory?</param>
        public override void ExtractFiles(IEnumerable<string> fileNames, string destination, bool overwriteFiles)
        {
            lock (m_lock)
            {
                CheckDisposed();
                this.ExtractFilesInternal(ConstructEntriesFromIndexes(GetIndexesFromFilenames(fileNames)), destination,
                    CancellationToken.None, null, overwriteFiles);
            }
        }

        /// <summary>
        /// Extract all files from archive to specified directory
        /// with cancellation token and progress reporter.
        /// </summary>
        /// <param name="fileNames">Files to extract.</param>
        /// <param name="destination">Absolute or relative directory path where extracted files will be placed.</param>
        /// <param name="overwriteFiles">Overwrite existing files in extraction directory?</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <param name="progress">Progress reporter ranged from 0 to <c>fileNames.Count()</c>.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.ArgumentException"></exception>
        /// <exception cref="BA2ExtractionException"></exception>
        public override void ExtractFiles(IEnumerable<string> fileNames, string destination, bool overwriteFiles,
            CancellationToken cancellationToken, IProgress<int> progress)
        {
            lock (m_lock)
            {
                CheckDisposed();
                this.ExtractFilesInternal(ConstructEntriesFromIndexes(GetIndexesFromFilenames(fileNames)), destination, cancellationToken,
                    progress, overwriteFiles);
            }
        }

        /// <summary>
        /// Extract's specified files, accessed by index to the
        /// specified directory.
        /// </summary>
        /// <param name="indexes">The indexes.</param>
        /// <param name="destination">
        /// Destination folder where extracted files will be placed.
        /// </param>
        /// <param name="overwriteFiles">Overwrite files in destination folder?</param>
        public override void ExtractFiles(IEnumerable<int> indexes, string destination, bool overwriteFiles)
        {
            lock (m_lock)
            {
                CheckDisposed();
                this.ExtractFilesInternal(ConstructEntriesFromIndexes(indexes), destination, CancellationToken.None, null, overwriteFiles);
            }
        }

        /// <summary>
        /// Extracts specified files, accessed by index to the specified
        /// directory with cancellation token and progress reporter.
        /// </summary>
        /// <param name="indexes">The indexes.</param>
        /// <param name="destination">
        /// Destination folder where extracted files will be placed.
        /// </param>
        /// <param name="overwriteFiles">Overwrite files in destination folder?</param>
        /// <param name="cancellationToken">
        /// The cancellation token. Set it to <c>CancellationToken.None</c>
        /// if you don't wanna cancel operation.
        /// </param>
        /// <param name="progress">
        /// Progress reporter ranged from 0 to <c>indexes.Count()</c>.
        /// Set it to <c>null</c> if you don't want to handle progress
        /// of operation.
        /// </param>
        public override void ExtractFiles(IEnumerable<int> indexes, string destination, bool overwriteFiles,
            CancellationToken cancellationToken, IProgress<int> progress)
        {
            lock (m_lock)
            {
                CheckDisposed();
                this.ExtractFilesInternal(ConstructEntriesFromIndexes(indexes), destination, cancellationToken, progress, overwriteFiles);
            }
        }

        /// <summary>
        /// Extract file contents to stream.
        /// </summary>
        /// <param name="fileName">File name or file path from archive.</param>
        /// <param name="stream">The stream.</param>
        /// <returns>
        /// Success is true, failure is false.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// <c>stream</c> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <c>fileName</c> is null or whitespace.
        /// </exception>
        public override bool ExtractToStream(string fileName, Stream stream)
        {
            lock (m_lock)
            {
                CheckDisposed();
                if (stream == null)
                    throw new ArgumentNullException(nameof(stream));
                if (string.IsNullOrWhiteSpace(fileName))
                    throw new ArgumentException(nameof(fileName));

                BA2TextureFileEntry entry = null;
                if (!TryGetEntryFromName(fileName, out entry))
                    return false;

                ExtractToStreamInternal(entry, stream);
                return true;
            }
        }

        /// <summary>
        /// Extract file, accessed by index, to the stream.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="stream">The stream.</param>
        /// <returns>
        /// Always returns true, catch exception.
        /// </returns>
        /// <exception cref="IndexOutOfRangeException">
        /// <c>index</c> is less than 0 or more than total files in archive.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <c>stream</c> is null.
        /// </exception>
        public override bool ExtractToStream(int index, Stream stream)
        {
            lock (m_lock)
            {
                CheckDisposed();
                if (index < 0 || index > this.TotalFiles)
                    throw new IndexOutOfRangeException(nameof(index));
                if (stream == null)
                    throw new ArgumentException(nameof(stream));

                ExtractToStreamInternal(m_entries[index], stream);
                return true;
            }
        }

        /// <summary>
        /// Extract single file to specified directory by index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="destination">Absolute or relative directory path where extracted file will be placed.</param>
        /// <param name="overwriteFile">Overwrite existing file in directory with extracted one?</param>
        /// <exception cref="IndexOutOfRangeException">
        /// <c>index</c> is less than 0 or more than total files in archive.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <c>destination</c> is null or whitespace.
        /// </exception>
        /// <exception cref="BA2ExtractionException"></exception>
        public override void Extract(int index, string destination, bool overwriteFile)
        {
                CheckDisposed();
                if (index < 0 || index > this.TotalFiles)
                    throw new IndexOutOfRangeException(nameof(index));
                if (string.IsNullOrWhiteSpace(destination))
                    throw new ArgumentException(nameof(destination));

                BA2TextureFileEntry entry = m_entries[index];
                string extractPath = CreateDirectoryAndGetPath(entry, destination, overwriteFile);

                using (var stream = File.Create(extractPath, 4096, FileOptions.SequentialScan))
                    ExtractToStreamInternal(entry, stream);
        }

        /// <summary>
        /// Extract single file from archive.
        /// </summary>
        /// <param name="fileName">File path, directories separated with backslash (\)</param>
        /// <param name="destination">Destination directory where file will be extracted to.</param>
        /// <param name="overwriteFile">Overwrite existing file with extracted one?</param>
        /// <exception cref="System.ArgumentNullException">
        /// </exception>
        /// <exception cref="BA2ExtractionException">
        /// Overwrite is not permitted.
        /// </exception>
        public override void Extract(string fileName, string destination, bool overwriteFile)
        {
            lock (m_lock)
            {
                CheckDisposed();
                if (fileName == null)
                    throw new ArgumentNullException(nameof(fileName));
                if (destination == null)
                    throw new ArgumentNullException(nameof(destination));

                BA2TextureFileEntry entry = null;
                if (!TryGetEntryFromName(fileName, out entry))
                    throw new BA2ExtractionException($"Cannot find file name \"{fileName}\" in archive");

                ExtractFileInternal(entry, destination, overwriteFile);
            }
        }

        public override List<IBA2FileEntry> GetFileEntries()
        {
            return m_entries.ToList<IBA2FileEntry>();
        }

        public override IBA2FileEntry GetFileEntry(int index)
        {
            return m_entries[index];
        }

        /// <summary>
        /// Preloads the data.
        /// </summary>
        /// <remarks>
        /// Do not call base.PreloadData().
        /// </remarks>
        /// <param name="reader">The reader.</param>
        internal override void PreloadData(BinaryReader reader)
        {
            lock (m_lock)
            {
                CheckDisposed();
                BuildFileList();

                m_archiveStream.Seek(BA2Loader.HeaderSize, SeekOrigin.Begin);
                m_entries = new BA2TextureFileEntry[TotalFiles];

                for (int i = 0; i < TotalFiles; i++)
                {
                    BA2TextureFileEntry entry = new BA2TextureFileEntry()
                    {
                        Unknown0 = reader.ReadUInt32(),
                        Extension = Encoding.ASCII.GetChars(reader.ReadBytes(4)),
                        Unknown1 = reader.ReadUInt32(),
                        Unknown2 = reader.ReadByte(),
                        NumberOfChunks = reader.ReadByte(),
                        ChunkHeaderSize = reader.ReadUInt16(),
                        TextureHeight = reader.ReadUInt16(),
                        TextureWidth = reader.ReadUInt16(),
                        NumberOfMipmaps = reader.ReadByte(),
                        Format = reader.ReadByte(),
                        IsCubemap = reader.ReadByte(),
                        TileMode = reader.ReadByte(),
                        Index = i
                    };

                    ReadChunksForEntry(reader, entry);

                    m_entries[i] = entry;
                }
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Returns size of file at specified file index. Returns 0 if fileIndex is out of bounds.
        /// </summary>
        public override UInt32 GetFileSize(int fileIndex)
        {
            CheckDisposed();
            if (fileIndex < 0 || fileIndex >= m_entries.Length)
                return 0;
            var entry = m_entries[fileIndex];
            UInt32 result = Dds.DDS_HEADER_SIZE + 4;
            foreach (var chunk in entry.Chunks)
            {
                result += chunk.UnpackedLength;
            }
            return result;
        }

        private void ExtractFilesInternal(BA2TextureFileEntry[] entries, string destination, CancellationToken cancellationToken,
            IProgress<int> progress, bool overwriteFiles)
        {
            if (string.IsNullOrWhiteSpace(destination))
                throw new ArgumentException(nameof(destination));

            int totalEntries = entries.Count();

            bool shouldUpdate = cancellationToken != null || progress != null;

            int counter = 0;
            int updateFrequency = Math.Max(1, totalEntries / 100);
            int nextUpdate = updateFrequency;

            BlockingCollection<string> readyFilenames = new BlockingCollection<string>(totalEntries);

            Action createDirs = () =>
                CreateDirectoriesForFiles(entries, readyFilenames, cancellationToken, destination, overwriteFiles);

            if (IsMultithreaded)
                Task.Run(createDirs, cancellationToken);
            else
                createDirs();

            for (int i = 0; i < totalEntries; i++)
            {
                BA2TextureFileEntry entry = entries[i];
                using (var stream = File.Create(readyFilenames.Take(), 4096, FileOptions.SequentialScan))
                {
                    ExtractToStreamInternal(entry, stream);
                }

                counter++;
                if (shouldUpdate && counter >= nextUpdate)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    progress?.Report(counter);
                    nextUpdate += updateFrequency;
                }
            }
        }

        private void ExtractFileInternal(BA2TextureFileEntry entry, string destinationFolder, bool overwriteFile)
        {
            string filePath = m_fileList[entry.Index];

            string extension = new string(entry.Extension).Trim('\0');
            string finalPath = Path.Combine(destinationFolder, filePath);

            string finalDest = Path.GetDirectoryName(finalPath);
            Directory.CreateDirectory(finalDest);

            if (overwriteFile == false && File.Exists(finalPath))
                throw new BA2ExtractionException("Overwrite is not permitted.");

            using (var fileStream = new FileStream(finalPath, FileMode.Create, FileAccess.Write, FileShare.Read, 4096, FileOptions.None))
            {
                ExtractToStreamInternal(entry, fileStream);
            }
        }

        /// <summary>
        /// Reads the chunks for entry.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="entry">The entry.</param>
        private void ReadChunksForEntry(BinaryReader reader, BA2TextureFileEntry entry)
        {
            var chunks = new TextureChunk[entry.NumberOfChunks];

            for (int i = 0; i < entry.NumberOfChunks; i++)
            {
                TextureChunk chunk = new TextureChunk()
                {
                    Offset = reader.ReadUInt64(),
                    PackedLength = reader.ReadUInt32(),
                    UnpackedLength = reader.ReadUInt32(),
                    StartMipmap = reader.ReadUInt16(),
                    EndMipmap = reader.ReadUInt16(),
                    Unknown = reader.ReadUInt32()
                };

                chunks[i] = chunk;
            }

            entry.Chunks = chunks;
        }

        /// <summary>
        /// Retrieves BA2TextureFileEntry from archive file name.
        /// </summary>
        /// <param name="fileName">File name in archive.</param>
        /// <param name="entry">The entry.</param>
        /// <returns>True if entry is found and populated, false otherwise.</returns>
        private bool TryGetEntryFromName(string fileName, out BA2TextureFileEntry entry)
        {
            int index = GetFileIndex(fileName);
            if (index == -1)
            {
                entry = null;
                return false;
            }

            entry = m_entries[index];
            return true;
        }

        /// <summary>
        /// Extracts and decompresses texture data, then combines it in valid DDS texture, then writes it to destination stream.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <param name="destStream">Destination stream where ready texture will be placed.</param>
        /// <remarks>
        /// No validation of arguments performed.
        /// </remarks>
        private void ExtractToStreamInternal(BA2TextureFileEntry entry, Stream destStream)
        {
            using (BinaryWriter writer = new BinaryWriter(destStream, Encoding.ASCII, leaveOpen: true))
            {
                DdsHeader header = CreateDdsHeaderForEntry(entry);

                writer.Write(Dds.DDS_MAGIC);
                writer.Write(header.dwSize);
                writer.Write(header.dwHeaderFlags);
                writer.Write(header.dwHeight);
                writer.Write(header.dwWidth);
                writer.Write(header.dwPitchOrLinearSize);
                writer.Write(header.dwDepth);
                writer.Write(header.dwMipMapCount);
                for (int i = 0; i < 11; i++)
                {
                    writer.Write(0U);
                }
                writer.Write(header.ddspf.dwSize);
                writer.Write(header.ddspf.dwFlags);
                writer.Write(header.ddspf.dwFourCC);
                writer.Write(header.ddspf.dwRGBBitCount);
                writer.Write(header.ddspf.dwRBitMask);
                writer.Write(header.ddspf.dwGBitMask);
                writer.Write(header.ddspf.dwBBitMask);
                writer.Write(header.ddspf.dwABitMask);
                writer.Write(header.dwSurfaceFlags);
                writer.Write(header.dwCubemapFlags);
                for (int i = 0; i < 3; i++)
                {
                    writer.Write(0U);
                }

                const long zlibHeaderSize = 2;
                for (uint i = 0; i < entry.NumberOfChunks; i++)
                {
                    var chunk = entry.Chunks[i];

                    m_archiveStream.Seek((long)chunk.Offset + zlibHeaderSize, SeekOrigin.Begin);

                    byte[] destBuffer = new byte[chunk.UnpackedLength];
                    using (var uncompressStream = new DeflateStream(m_archiveStream, CompressionMode.Decompress, leaveOpen: true))
                    {
                        var bytesReaden = uncompressStream.Read(destBuffer, 0, (int)chunk.UnpackedLength);
                        // Debug.Assert(bytesReaden == chunk.UnpackedLength);
                    }

                    writer.Write(destBuffer, 0, (int)chunk.UnpackedLength);
                }
            }
        }


        private const uint TILE_MODE_DEFAULT = 0x8;

        /// <summary>
        /// Creates valid DDS Header for entry's texture.
        /// </summary>
        /// <param name="entry">Valid BA2TextureFileEntry instance.</param>
        /// <returns>
        /// Valid DDS Header.
        /// </returns>
        /// <exception cref="System.NotSupportedException">Entry DDS format is not supported.</exception>
        private DdsHeader CreateDdsHeaderForEntry(BA2TextureFileEntry entry)
        {
            var header = new DdsHeader();
            DxgiFormat format = (DxgiFormat)entry.Format;

            header.dwSize = Dds.DDS_HEADER_SIZE;
            header.dwHeaderFlags = Dds.DDS_HEADER_FLAGS_TEXTURE |
                Dds.DDS_HEADER_FLAGS_LINEARSIZE | Dds.DDS_HEADER_FLAGS_MIPMAP;
            header.dwHeight = (uint)entry.TextureHeight;
            header.dwWidth = (uint)entry.TextureWidth;
            header.dwMipMapCount = (uint)entry.NumberOfMipmaps;
            header.ddspf.dwSize = 32; // sizeof(DDS_PIXELFORMAT);
            header.dwSurfaceFlags = Dds.DDS_SURFACE_FLAGS_TEXTURE | Dds.DDS_SURFACE_FLAGS_MIPMAP;
            header.dwCubemapFlags = entry.IsCubemap == 1 ? (uint)(Dds.DDSCAPS2.CUBEMAP
                | Dds.DDSCAPS2.CUBEMAP_NEGATIVEX | Dds.DDSCAPS2.CUBEMAP_POSITIVEX
                | Dds.DDSCAPS2.CUBEMAP_NEGATIVEY | Dds.DDSCAPS2.CUBEMAP_POSITIVEY
                | Dds.DDSCAPS2.CUBEMAP_NEGATIVEZ | Dds.DDSCAPS2.CUBEMAP_POSITIVEZ
                | Dds.DDSCAPS2.CUBEMAP_ALLFACES) : 0u;

            switch (format)
            {
                case DxgiFormat.BC1_UNORM:
                    header.ddspf.dwFlags = Dds.DDS_FOURCC;
                    header.ddspf.dwFourCC = (uint)Dds.MakeFourCC('D', 'X', 'T', '1');
                    header.dwPitchOrLinearSize = (uint)entry.TextureWidth * (uint)entry.TextureHeight / 2u; // 4bpp
                    break;
                case DxgiFormat.BC2_UNORM:
                    header.ddspf.dwFlags = Dds.DDS_FOURCC;
                    header.ddspf.dwFourCC = (uint)Dds.MakeFourCC('D', 'X', 'T', '3');
                    header.dwPitchOrLinearSize = (uint)entry.TextureWidth * (uint)entry.TextureHeight;      // 8bpp
                    break;
                case DxgiFormat.BC3_UNORM:
                    header.ddspf.dwFlags = Dds.DDS_FOURCC;
                    header.ddspf.dwFourCC = (uint)Dds.MakeFourCC('D', 'X', 'T', '5');
                    header.dwPitchOrLinearSize = (uint)entry.TextureWidth * (uint)entry.TextureHeight;      // 8bpp
                    break;
                case DxgiFormat.BC5_UNORM:
                    header.ddspf.dwFlags = Dds.DDS_FOURCC;
                    // ATI2
                    header.ddspf.dwFourCC = (uint)Dds.MakeFourCC('D', 'X', 'T', '5');
                    header.dwPitchOrLinearSize = (uint)entry.TextureWidth * (uint)entry.TextureHeight;      // 8bpp
                    break;
                case DxgiFormat.BC1_UNORM_SRGB:
                    header.ddspf.dwFlags = Dds.DDS_FOURCC;
                    header.ddspf.dwFourCC = (uint)Dds.MakeFourCC('D', 'X', '1', '0');
                    header.dwPitchOrLinearSize = (uint)(entry.TextureWidth * entry.TextureHeight / 2);      // 4bpp
                    break;
                case DxgiFormat.BC3_UNORM_SRGB:
                case DxgiFormat.BC4_UNORM:
                case DxgiFormat.BC5_SNORM:
                case DxgiFormat.BC6H_UF16:
                case DxgiFormat.BC7_UNORM:
                case DxgiFormat.BC7_UNORM_SRGB:
                    header.ddspf.dwFlags = Dds.DDS_FOURCC;
                    header.ddspf.dwFourCC = (uint)Dds.MakeFourCC('B', 'C', '7', '\0');
                    //header.ddspf.dwFourCC = (uint)Dds.MakeFourCC('D', 'X', '1', '0'); // ???
                    header.dwPitchOrLinearSize = (uint)entry.TextureWidth * (uint)entry.TextureHeight;      // 8bpp
                    break;
                case DxgiFormat.R8G8B8A8_UNORM:
                case DxgiFormat.R8G8B8A8_UNORM_SRGB:
                    header.ddspf.dwFlags = Dds.DDS_RGBA;
                    header.ddspf.dwRGBBitCount = 32;
                    header.ddspf.dwRBitMask = 0x000000FF;
                    header.ddspf.dwGBitMask = 0x0000FF00;
                    header.ddspf.dwBBitMask = 0x00FF0000;
                    header.ddspf.dwABitMask = 0xFF000000;
                    header.dwPitchOrLinearSize = (uint)(entry.TextureWidth * entry.TextureHeight * 4u);      // 32bpp
                    break;
                case DxgiFormat.B5G6R5_UNORM:
                    header.ddspf.dwFlags = Dds.DDS_RGB;
                    header.ddspf.dwRGBBitCount = 16;
                    header.ddspf.dwRBitMask = 0x0000F800;
                    header.ddspf.dwGBitMask = 0x000007E0;
                    header.ddspf.dwBBitMask = 0x0000001F;
                    header.dwPitchOrLinearSize = (uint)(entry.TextureWidth * entry.TextureHeight * 2u);     // 16bpp
                    break;
                case DxgiFormat.B8G8R8A8_UNORM:
                case DxgiFormat.B8G8R8X8_UNORM:
                    header.ddspf.dwFlags = Dds.DDS_RGBA;
                    header.ddspf.dwRGBBitCount = 32;
                    header.ddspf.dwRBitMask = 0x00FF0000;
                    header.ddspf.dwGBitMask = 0x0000FF00;
                    header.ddspf.dwBBitMask = 0x000000FF;
                    header.ddspf.dwABitMask = 0xFF000000;
                    header.dwPitchOrLinearSize = (uint)entry.TextureWidth * (uint)entry.TextureHeight * 4u; // 32bpp
                    break;                
                case DxgiFormat.R8_UNORM:
                    header.ddspf.dwFlags = Dds.DDS_RGB;
                    header.ddspf.dwRGBBitCount = 8;
                    header.ddspf.dwRBitMask = 0xFF;
                    header.dwPitchOrLinearSize = (uint)entry.TextureWidth * (uint)entry.TextureHeight;      // 8bpp
                    break;
                default:
                    throw new NotSupportedException($"DDS format \"{format.ToString()}\" is not supported.");
            }

            // xbox crap
            if (entry.TileMode != TILE_MODE_DEFAULT)
            {
                switch ((DxgiFormat)entry.Format)
                {
                    case DxgiFormat.BC1_UNORM:
                    case DxgiFormat.BC1_UNORM_SRGB:
                    case DxgiFormat.BC2_UNORM:
                    case DxgiFormat.BC3_UNORM:
                    case DxgiFormat.BC3_UNORM_SRGB:
                    case DxgiFormat.BC4_UNORM:
                    case DxgiFormat.BC5_SNORM:
                    case DxgiFormat.BC5_UNORM:
                    case DxgiFormat.BC7_UNORM:
                    case DxgiFormat.BC7_UNORM_SRGB:
                        header.ddspf.dwFourCC = (uint)Dds.MakeFourCC('X', 'B', 'O', 'X');
                        break;
                }
            }

            return header;
        }

        private BA2TextureFileEntry[] ConstructEntriesFromIndexes(IEnumerable<int> indexes)
        {
            BA2TextureFileEntry[] entries = new BA2TextureFileEntry[indexes.Count()];
            int i = 0;
            foreach (int index in indexes)
            {
                entries[i] = m_entries[index];
                i++;
            }

            return entries;
        }

        #endregion

        #region Disposal

        public sealed override void Dispose()
        {
            lock (m_lock)
            {
                if (m_disposed) return;

                m_entries = null;

                base.Dispose();
            }
        }

        #endregion
    }
}
