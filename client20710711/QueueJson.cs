using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGV_V1._0.Queue
{
  public  class QueueJson:IQueue<string>
    {
        private static QueueJson instance;
        public static QueueJson Instance
        {
            get
            {
                if (null == instance)
                {
                    instance = new QueueJson();
                }
                return instance;
            }
        }
    }
}
