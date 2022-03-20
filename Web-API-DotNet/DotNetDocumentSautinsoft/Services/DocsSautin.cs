using SautinSoft.Document;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetDocumentSautinsoft.Services
{
    public class DocsSautin
    {
        public static string Convert(String UploadPath, string DownloadPath, string FileUniqueName, string SourceFileExt, string DestinationFileExt)
        {
          ///  string serial = "123456789XXX"; //Product Key
            try
            {
                UploadPath = UploadPath + "\\" + FileUniqueName + SourceFileExt;
                DownloadPath = DownloadPath + "\\" + FileUniqueName + DestinationFileExt;

                DocumentCore dc = DocumentCore.Load(UploadPath);
             ///   DocumentCore.Serial = serial; 
                dc.Save(DownloadPath);
                return "Success";
            }
            catch (Exception ex)
            {
                return "Conversion Error" + ex;
            }
        }
        public static void directoryCleaner(String Path)
        {
            string[] files = Directory.GetFiles(Path);

            foreach (string file in files)
            {
                FileInfo fi = new FileInfo(file);
                if (fi.LastAccessTime > DateTime.Now.AddMinutes(-2))
                    fi.Delete();

            }
        }
        public static void directoryDownloadCleaner(String Path)
        {
            string[] files = Directory.GetFiles(Path);

            foreach (string file in files)
            {
                FileInfo fi = new FileInfo(file);
                if (fi.LastAccessTime < DateTime.Now.AddMinutes(-2))
                    fi.Delete();

            }
        }
    }
}
