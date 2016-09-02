using Ionic.Zip;
using System;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Compression
{
    /// <summary>
    /// 
    /// </summary>
    public  class CompressFiles
    {
   
        /// <summary>
        /// 压缩文件
        /// </summary>
        /// <param name="paths"></param>
        /// <param name="zipPath"></param>
        /// <param name="i">如果有此参数表明是向现有的压缩文件添加</param>
        public CompressFiles(string[] paths, string zipPath)
        {
            using (ZipFile zf = new ZipFile(zipPath, Encoding.UTF8))
            {
              
                if (paths.Count()<=0)
                {
                    return;
                }
                zf.AddFiles(paths,"");
                zf.Save();
            }
        }
    
        /// <summary>
        /// 循环添加文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="zipPath"></param>
        public CompressFiles(string path, string zipPath)
        {
            try
            {
                using (ZipFile zf = new ZipFile(zipPath, Encoding.UTF8))
                {


                    if (path.Count() <= 0)
                    {
                        return;
                    }
                    if (zipPath.Contains(".zip"))
                    {
                        zf.AddFile(path, "");
                        zf.Save();
                    }
                    else if (zipPath.Contains(".exe"))
                    {
                        zf.AddFile(path, "");
                        zf.SaveSelfExtractor(zipPath, SelfExtractorFlavor.WinFormsApplication);
                    }
                    else
                    {
                        MessageBox.Show("文件名必须以文件后缀名.exe或者.zip结束");
                    }

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            


        }

    }
}
