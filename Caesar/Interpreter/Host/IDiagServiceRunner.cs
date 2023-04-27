using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaesarInterpreter.Host
{
    public interface IDiagServiceRunner
    {
        public bool IsNegativeResponse();
        public void DoDiagService();
        public int GetPresParamType(int index);
        public int GetPrepParamDumpLength(int index);
        public int GetPresParamDumpLength(int index);
        public byte[] GetDumpPresParam(int index);
        public void SetDumpPreparationParam(int index, byte[] value);
        public void SetUWordPreparationParam(int index, ushort value);
        public void SetSLongPreparationParam(int index, int value);

        public string GetStringPresentationParam(int index);
        public byte[] GetPresentationDumpValue(int index);
    }
}
