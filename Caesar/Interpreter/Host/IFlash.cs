using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaesarInterpreter.Host
{
    public interface IFlash
    {
        void Download(int blockIndex);
        byte[] GetChecksum();
        byte[] GetSignature();

        void SetActiveBlockIndex(int blockIndex);
        int GetNumberOfSecurities(int blockIndex);
    }
}
