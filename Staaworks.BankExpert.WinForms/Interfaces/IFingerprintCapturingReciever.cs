using DPUruNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Staaworks.BankExpert.WinForms.Interfaces
{
    public interface IFingerprintCapturingReciever
    {
        Reader CurrentReader { get; set; }

        bool CaptureFinger(ref Fid fid);
    }
}
