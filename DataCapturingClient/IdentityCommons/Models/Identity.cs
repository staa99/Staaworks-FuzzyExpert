using DPUruNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityCommons.Models
{
    public class Identity
    {
        public string IdentityID { get; set; }
        public IPerson Person { get; set; }
        public DateTime DateIssued { get; set; }
        
        public void savePicture()
        {
            
        }
        public void enrollFinger(int fingerId,List<Fid> fingerimagedata)
        {
           
        }
    }
}
