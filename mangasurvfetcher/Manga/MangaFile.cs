using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace mangasurvlib.Manga
{
    public class MangaFile : IComparable
    {
        public string FileName
        {
            get;
            internal set;
        }

        public int FileNumber
        {
            get;
            internal set;
        }

        internal MangaFile(string sFileName)
        {
            this.FileName = sFileName;
            this.SetChapter();
        }

        internal MangaFile()
        { }

        private void SetChapter()
        {
            if(!String.IsNullOrEmpty(this.FileName))
            {
                string sFileName = Path.GetFileNameWithoutExtension(this.FileName);
                if (sFileName.EndsWith("_"))
                {
                    sFileName = sFileName.Substring(0, sFileName.Length - 1);
                }
                int iStart = 0;
                for(int i = sFileName.Length - 1; i > 0; i--)
                {
                    if(sFileName[i] == '_')
                    {
                        iStart = i + 1;
                        break;
                    }
                }
                this.FileNumber = Convert.ToInt32(sFileName.Substring(iStart));
            }
        }

        public override string ToString()
        {
            string sFileName = Path.GetFileNameWithoutExtension(this.FileName);
            if (String.IsNullOrEmpty(sFileName))
            {
                return base.ToString();
            }

            return "" + this.FileNumber;
        }

        public int CompareTo(object obj)
        {
            MangaFile temp = obj as MangaFile;


            if (this.FileNumber < temp.FileNumber)
                return -1;
            else
                return 1;
        }
    }
}
