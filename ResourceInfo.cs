using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compression
{
   public class ResourceInfo
    {
        string resourceName;
        string resourceType;
        int resourceSize;
        /// <summary>
        /// 资源文件名
        /// </summary>
        public string ResourceName
        {
            get
            {
                return resourceName;
            }

            set
            {
                resourceName = value;
            }
        }
        /// <summary>
        /// 资源文件类型
        /// </summary>
        public string ResourceType
        {
            get
            {
                return resourceType;
            }

            set
            {
                if (value.Split('.').Count()>1)
                {
                    resourceType = (value.Split('.')[1]);
                }
               
            }
        }
        /// <summary>
        /// 资源文件大小
        /// </summary>
        public int ResourceSize
        {
            get
            {
                return resourceSize;
            }

            set
            {
                resourceSize = (value/1000);
            }
        }
    }
}
