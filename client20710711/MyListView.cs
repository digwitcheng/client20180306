using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace client20710711
{
    class MyListView:ListView
    {
        public MyListView()
        { 
             // 开启双缓冲           
             this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true); 
             this.SetStyle(ControlStyles.EnableNotifyMessage, true);     
        }      
        protected override void OnNotifyMessage(Message m)      
        {            //Filter out the WM_ERASEBKGND message          
            if (m.Msg != 0x14)          
            {              
                base.OnNotifyMessage(m);    
            }      
        }
    }
}
