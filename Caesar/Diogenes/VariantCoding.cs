using Caesar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Diogenes
{
    public class VariantCoding
    {

        private static void ExecVCWrite(byte[] request, DiagService service, ECUConnection connection, bool writesEnabled)
        {
            bool allowVcWrite = writesEnabled; // allowWriteVariantCodingToolStripMenuItem.Checked;
            if (allowVcWrite)
            {
                connection.ExecUserDiagJob(request, service);
                Console.WriteLine("VC Write completed");
            }
            else
            {
                MessageBox.Show("This VC write action has to be manually enabled under \r\nFile >  Allow Write Variant Coding\r\nPlease make sure that you understand the risks before doing so.",
                    "Accidental Brick Protection");
            }
        }

        /*
            test: med40

            jg: dumping pres
            jg: q: SID_RQ pos byte: 0 size bytes: 1 modecfg:323 fieldtype: IntegerType dump: 2E 00 00 00
            jg: q: RecordDataIdentifier pos byte: 1 size bytes: 2 modecfg:324 fieldtype: IntegerType dump: 01 10 00 00
            jg: q: #0 pos byte: 33 size bytes: 16 modecfg:6430 fieldtype: BitDumpType dump: 
            jg: q: #1 pos byte: 49 size bytes: 1 modecfg:6423 fieldtype: IntegerType dump: 
            jg: q: #2 pos byte: 50 size bytes: 1 modecfg:6423 fieldtype: IntegerType dump: 
            jg: q: #3 pos byte: 51 size bytes: 1 modecfg:6423 fieldtype: IntegerType dump: 
            jg: q: #4 pos byte: 52 size bytes: 1 modecfg:6423 fieldtype: IntegerType dump: 
            jg: q: #5 pos byte: 3 size bytes: 50 modecfg:6410 fieldtype: ExtendedBitDumpType dump: 
            jg: done dumping pres

            */


        public static void DoVariantCoding(ECUConnection connection, VCForm vcForm, bool writesEnabled) 
        {
            Console.WriteLine($"Operator requesting for VC: {BitUtility.BytesToHex(vcForm.VCValue, true)}");

            RunDiagForm runDiagForm = new RunDiagForm(vcForm.WriteService);

            // construct a write command from presentations: fill up the VC value first, fill up all available dumps, then inherit the last values (fingerprints, scn) from the read command
            byte[] vcParameter = vcForm.VCValue;
            byte[] writeCommand = vcForm.WriteService.RequestBytes;
            byte[] priorReadCommand = vcForm.UnfilteredReadValue;


            // start with a list of all values that we will have to fill
            List<DiagPreparation> preparationsToProcess = new List<DiagPreparation>(vcForm.WriteService.InputPreparations);

            // fill up vc, which pretty much always uses the ExtendedBitDumpType type
            DiagPreparation vcPrep = preparationsToProcess.Find(x => x.FieldType == DiagPreparation.InferredDataType.ExtendedBitDumpType);
            if (vcPrep is null)
            {
                MessageBox.Show("VC: Could not find the VC ExtendedBitDump prep, stopping early to save your ECU.", "VC Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            int vcPrepSizeInBytes = vcPrep.SizeInBits / 8;
            int vcPrepBytePosition = vcPrep.BitPosition / 8;
            if (vcPrepSizeInBytes < vcParameter.Length)
            {
                MessageBox.Show("VC: VC string is longer than the parameter can fit, stopping early to save your ECU.", "VC Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            // zero out the destination buffer for the param that we intend to write since there's a possibility that our param is shorter than the actual prep's size
            for (int i = vcPrepBytePosition; i < (vcPrepBytePosition + vcPrepSizeInBytes); i++)
            {
                writeCommand[i] = 0;
            }
            // copy the parameter in
            Array.ConstrainedCopy(vcParameter, 0, writeCommand, vcPrepBytePosition, vcParameter.Length);
            preparationsToProcess.Remove(vcPrep);
            // done with vc prep


            // merge prefilled constants such as the (SID_RQ, id..)
            List<DiagPreparation> prefilledValues = new List<DiagPreparation>();
            foreach (DiagPreparation prep in preparationsToProcess)
            {
                if (prep.Dump.Length > 0)
                {
                    prefilledValues.Add(prep);
                    if (prep.FieldType == DiagPreparation.InferredDataType.IntegerType)
                    {
                        byte[] fixedDump = prep.Dump.Take(prep.SizeInBits / 8).Reverse().ToArray();
                        Array.ConstrainedCopy(vcParameter, 0, writeCommand, vcPrepBytePosition, vcParameter.Length);
                    }
                    else
                    {
                        Console.WriteLine($"Skipping prefill for {prep.Qualifier} as the data type {prep.FieldType} is unsupported.");
                    }
                }
            }
            // "mark" the constants as done
            foreach (DiagPreparation prep in prefilledValues)
            {
                preparationsToProcess.Remove(prep);
            }

            // isolate the SCN if it exists
            foreach (DiagPreparation prep in preparationsToProcess)
            {
                int bytePosition = prep.BitPosition / 8;
                int byteLength = prep.SizeInBits / 8;
                // SCN is always 16-bytes
                if (byteLength != 16)
                {
                    continue;
                }
                byte[] originalValue = new byte[byteLength];
                Array.ConstrainedCopy(priorReadCommand, bytePosition, originalValue, 0, byteLength);

                // SCN values are ASCII numerals (between 0x30 - 0x39)
                bool isValidSCN = true;
                foreach (byte b in originalValue) 
                {
                    if ((b > 0x39) || (b < 0x30)) 
                    {
                        isValidSCN = false;
                        break;
                    }
                }
                if (!isValidSCN) 
                {
                    continue;
                }

                Console.WriteLine($"Found SCN value: {Encoding.ASCII.GetString(originalValue)}");

                // check if operator is using vediamo-style variant-coding, where the SCN is set to 0000000000000000 when writing VC
                // otherwise, we can reuse the last value as read from the ECU
                bool useVediamoBehaviorZeroSCN = Preferences.GetValue(Preferences.PreferenceKey.EnableSCNZero) == "true";
                if (useVediamoBehaviorZeroSCN) 
                {
                    for (int i = 0; i < originalValue.Length; i++) 
                    {
                        originalValue[i] = 0x30;
                    }
                }

                Console.WriteLine($"Using {Encoding.ASCII.GetString(originalValue)} as new SCN value");

                // write-back the recognized (and optionally, modified) SCN
                Array.ConstrainedCopy(originalValue, 0, writeCommand, bytePosition, byteLength);
                preparationsToProcess.Remove(prep);
                break;
            }

            // isolate the fingerprint if it exists
            // normally fingerprint is appended at the tail of the VC command
            List<int> tailIndices = new List<int>() { writeCommand.Length - 1, writeCommand.Length - 2, writeCommand.Length - 3, writeCommand.Length - 4 };
            List<DiagPreparation> possibleFingerprintFields = new List<DiagPreparation>();
            foreach (DiagPreparation prep in preparationsToProcess)
            {
                int bytePosition = prep.BitPosition / 8;
                int byteLength = prep.SizeInBits / 8;

                if ((prep.FieldType == DiagPreparation.InferredDataType.IntegerType) && (byteLength == 1) && (tailIndices.Contains(bytePosition)))
                {
                    tailIndices.Remove(bytePosition);
                    possibleFingerprintFields.Add(prep);
                }
            }
            if (possibleFingerprintFields.Count == 4)
            {
                // copy in the fingerprint, and "mark" the constants as done
                byte[] fingerprint = priorReadCommand.Skip(priorReadCommand.Length - 4).ToArray();
                Console.WriteLine($"Found original fingerprint as {BitUtility.BytesToHex(fingerprint)}");

                // default behavior is to clone last fingerprint, else use stored fingerprint
                if (Preferences.GetValue(Preferences.PreferenceKey.EnableFingerprintClone) == "false") 
                {
                    uint altFingerprintValue = uint.Parse(Preferences.GetValue(Preferences.PreferenceKey.FingerprintValue));
                    fingerprint[3] = (byte)(altFingerprintValue & 0xFF);
                    altFingerprintValue >>= 8;
                    fingerprint[2] = (byte)(altFingerprintValue & 0xFF);
                    altFingerprintValue >>= 8;
                    fingerprint[1] = (byte)(altFingerprintValue & 0xFF);
                    altFingerprintValue >>= 8;
                    fingerprint[0] = (byte)(altFingerprintValue & 0xFF);
                }

                Console.WriteLine($"Using {BitUtility.BytesToHex(fingerprint)} as new fingerprint value.");

                Array.ConstrainedCopy(fingerprint, 0, writeCommand, writeCommand.Length - 4, 4);

                foreach (DiagPreparation prep in possibleFingerprintFields)
                {
                    preparationsToProcess.Remove(prep);
                }
            }


            // at this point, whatever that's left in preparationsToProcess are stuff that are variable, but should be copied verbatim from the original read request (e.g. fingerprints, scn)
            // log the assumptions, show it the operator just in case
            StringBuilder assumptionsMade = new StringBuilder();
            if (preparationsToProcess.Count > 0)
            {
                if (writeCommand.Length != priorReadCommand.Length)
                {
                    MessageBox.Show("There are some preparations that do not have a default value. \r\n" +
                        "The input and output values do not have matching lengths, which means that the automatic assumption may be wrong. \r\n" +
                        "Please be very careful when proceeding.", "Warning");
                }

                foreach (DiagPreparation prep in preparationsToProcess)
                {
                    int bytePosition = prep.BitPosition / 8;
                    int byteLength = prep.SizeInBits / 8;
                    Array.ConstrainedCopy(priorReadCommand, bytePosition, writeCommand, bytePosition, byteLength);
                    assumptionsMade.Append($"{prep.Qualifier} : {BitUtility.BytesToHex(priorReadCommand.Skip(bytePosition).Take(byteLength).ToArray(), true)}\r\n");
                }
            }

            /*
            // lazy me dumping the values
            for (int i = 0; i < vcForm.WriteService.InputPreparations.Count; i++)
            {
                DiagPreparation prep = vcForm.WriteService.InputPreparations[i];
                Console.WriteLine($"debug: q: {prep.Qualifier} pos byte: {(prep.BitPosition / 8)} size bytes: {(prep.SizeInBits / 8)} modecfg:{prep.ModeConfig:X} fieldtype: {prep.FieldType} dump: {BitUtility.BytesToHex(prep.Dump, true)}");
            }
            */

            // we are done preparing the command, if we are confident we can send the command straight to the ECU, else, let the user review
            if (assumptionsMade.Length > 0)
            {
                if (MessageBox.Show("Some assumptions were made when preparing the write parameters. \r\n\r\n" +
                    "You may wish to review them by selecting Cancel, or select OK to execute the write command immediately.\r\n\r\n" + assumptionsMade.ToString(),
                    "Review assumptions", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
                {
                    ExecVCWrite(writeCommand, vcForm.WriteService, connection, writesEnabled);
                }
                else
                {
                    runDiagForm.Result = writeCommand;
                    if (runDiagForm.ShowDialog() == DialogResult.OK)
                    {
                        ExecVCWrite(runDiagForm.Result, vcForm.WriteService, connection, writesEnabled);
                    }
                }
            }
            else
            {
                // everything accounted for, immediately write
                ExecVCWrite(writeCommand, vcForm.WriteService, connection, writesEnabled);
            }
        }
    }
}
