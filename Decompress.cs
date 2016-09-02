using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compression
{
   public class Decompress
    {

        /// <summary>
        /// 解压所有
        /// </summary>
        /// <param name="path"></param>
        /// <param name="zipPath"></param>
        public Decompress(string filename, string zipPath)
        {
            using (ZipFile zf = new ZipFile(zipPath, Encoding.UTF8))
            {
                zf.ExtractAll(filename);
            }          
         }

        /// <summary>
        /// 解压指定文件
        /// </summary>
        /// <param name="paths"></param>
        /// <param name="zipPath"></param>
        public Decompress(string[] Filenames, string decompressPath,string zipPath)
        {

            using (ZipFile zf = new ZipFile(zipPath, Encoding.UTF8))
            {
                if (Filenames.Count()<=0)
                {
                    return;
                    
                }
                foreach (string name in Filenames)
                {
                    zf[name].Extract(decompressPath);
                }
            }

        }

    }
}
