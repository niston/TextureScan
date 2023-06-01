using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextureScan
{
    public class TextureFile
    {
        public string FilePath { set; get; }
        public string ArchivePath { set; get; }
        public int ArchiveFileIndex { set; get; }

        public int Width { set; get; }
        public int Height { set; get; }
        public int MipMapCount { set; get; }
        public bool IsCubemap { set; get; }

    }
}
