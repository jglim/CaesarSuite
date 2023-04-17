using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Caesar;

namespace Diogenes.Forms
{
    public partial class PresentationEditor : UserControl
    {
        DiagPresentation ActivePresentation = null;
        public BitArray SourceBits = null;

        public Action ValueChanged = null;

        public PresentationEditor()
        {
            InitializeComponent();
            SetPresentation(null);
        }

        public void UpdateLabel() 
        {
            var presType = ActivePresentation.GetDataType();
            
            string scaleInfo = "";
            if (ActivePresentation.Scales.Count > 0) 
            {
                var activeScale = ActivePresentation.Scales[0];
                scaleInfo = $"(Scale: Preparation bounds: [{activeScale.PrepLowBound}-{activeScale.PrepUpBound}], Enum bounds: [{activeScale.EnumLowBound}-{activeScale.EnumUpBound}]) Adjustments: Mul: {activeScale.MultiplyFactor}, Add: {activeScale.AddConstOffset}";
            }

            string labelText = $"{ActivePresentation.Qualifier} ({presType}) [{BitArrayExtension.ToBitString(SourceBits)}] {scaleInfo}";
            lblDescription.Text = labelText;
        }

        public void SetPresentation(DiagPresentation pres, BitArray sourceBits = null)
        {
            ActivePresentation = pres;
            SourceBits = sourceBits;
            txtValue.Visible = false;
            cbEnum.Visible = false;

            if ((ActivePresentation is null)  || (SourceBits is null))
            {
                lblDescription.Text = "-";
                return;
            }

            var presType = ActivePresentation.GetDataType();
            UpdateLabel();

            switch (presType) 
            {
                case DiagPresentation.PresentationTypes.PresBasicEnum:
                    {
                        cbEnum.Items.Clear();
                        cbEnum.Items.AddRange(ActivePresentation.BasicEnums.Select(x => x.EnumName).ToArray());

                        // parse and set current value
                        // value can be between 1-32 bits, promote to 32 bit while working here, then mask again when fetching result
                        int sourceValue = BitArrayExtension.PromoteToInt32(SourceBits, bigEndian: true);
                        bool foundValueMatchingEnum = false;
                        for (int i = 0; i < ActivePresentation.BasicEnums.Count; i++) 
                        {
                            if (ActivePresentation.BasicEnums[i].EnumValue == sourceValue) 
                            {
                                cbEnum.SelectedIndex = i;
                                foundValueMatchingEnum = true;
                                break;
                            }
                        }
                        if (!foundValueMatchingEnum) 
                        {
                            // this can sometimes occur when starting from an empty dump
                            // help the user pick a default value
                            cbEnum.SelectedIndex = 0;
                            //throw new Exception($"Could not find a matching enum for value {sourceValue} in {ActivePresentation.Qualifier}");
                        }

                        cbEnum.Visible = true;
                        break;
                    }
                case DiagPresentation.PresentationTypes.PresBytes: 
                    {
                        txtValue.Text = BitUtility.BytesToHex(BitArrayExtension.ToBytes(SourceBits), true);
                        txtValue.Visible = true;
                        break;
                    }
                case DiagPresentation.PresentationTypes.PresIEEEFloat: 
                    {
                        txtValue.Text = BitArrayExtension.ToFloat(SourceBits, true).ToString();
                        txtValue.Visible = true;
                        break;
                    }
                case DiagPresentation.PresentationTypes.PresInt:
                    {
                        int val = BitArrayExtension.PromoteToInt32(SourceBits, bigEndian: true);
                        txtValue.Text = val.ToString();
                        txtValue.Visible = true;
                        break;
                    }
                case DiagPresentation.PresentationTypes.PresScaledDecimal:
                    {
                        if (pres.Scales.Count == 1)
                        {
                            // see DL_Oelresettabelle_ASSYST, km stand
                            var activeScale = pres.Scales[0];
                            decimal val = BitArrayExtension.PromoteToInt32(SourceBits, bigEndian: true);
                            val /= (decimal)activeScale.MultiplyFactor;
                            val -= (decimal)activeScale.AddConstOffset;
                            txtValue.Text = val.ToString();
                            txtValue.Visible = true;
                        }
                        else 
                        {
                            Console.WriteLine($"Skipped parsing of a scaled decimal ({pres.Qualifier}) as there were {pres.Scales.Count} scales (expecting 1)");
                        }
                        break;
                    }
                case DiagPresentation.PresentationTypes.PresScaledEnum:
                    {
                        break;
                    }
                case DiagPresentation.PresentationTypes.PresString:
                    {
                        break;
                    }
                case DiagPresentation.PresentationTypes.PresUInt:
                    {
                        uint val = BitArrayExtension.PromoteToUInt32(SourceBits, bigEndian: true);
                        txtValue.Text = val.ToString();
                        txtValue.Visible = true;
                        break;
                    }
                default:
                    break;
            }

        }

        private void cbEnum_SelectedIndexChanged(object sender, EventArgs e)
        {
            int newValue = ActivePresentation.BasicEnums[cbEnum.SelectedIndex].EnumValue;
            BitArray newValueBits = BitArrayExtension.ToBitArray(newValue, true);
            // length of new value will be either equal or longer than the original buffer
            // copy as much as the original buffer will fit
            BitArrayExtension.CopyBitsClamped(SourceBits, newValueBits);
            UpdateLabel();
            RaiseValueChanged();
        }

        private void RaiseValueChanged()
        {
            if (ValueChanged != null)
            {
                ValueChanged();
            }
        }

        private void txtValue_TextChanged(object sender, EventArgs e)
        {
            // assume data is invalid first
            bool dataValid = false;

            

            var presType = ActivePresentation.GetDataType();
            switch (presType)
            {
                case DiagPresentation.PresentationTypes.PresBytes:
                    {
                        if (BitUtility.TryParseHex(txtValue.Text, out byte[] hexValue))
                        {
                            BitArray newValue = new BitArray(hexValue);
                            if (newValue.Length == SourceBits.Length)
                            {
                                BitArrayExtension.CopyBitsClamped(SourceBits, newValue);
                                dataValid = true;
                            }
                        }
                        break;
                    }
                case DiagPresentation.PresentationTypes.PresIEEEFloat:
                    {
                        if (float.TryParse(txtValue.Text, out float result)) 
                        {
                            BitArrayExtension.CopyBitsClamped(SourceBits, BitArrayExtension.ToBitArray(result, bigEndian: true));
                            dataValid = true;
                        }
                        break;
                    }
                case DiagPresentation.PresentationTypes.PresScaledDecimal:
                    {
                        if (ActivePresentation.Scales.Count == 1)
                        {
                            var activeScale = ActivePresentation.Scales[0];
                            if (decimal.TryParse(txtValue.Text, out decimal val))
                            {
                                val += (decimal)activeScale.AddConstOffset;
                                val *= (decimal)activeScale.MultiplyFactor;
                                BitArrayExtension.CopyBitsClamped(SourceBits, BitArrayExtension.ToBitArray((int)val, bigEndian: true, bitArraySize: SourceBits.Length));
                                dataValid = true;
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Skipped serialization of a scaled decimal ({ActivePresentation.Qualifier}) as there were {ActivePresentation.Scales.Count} scales (expecting 1)");
                        }
                        break;
                    }
                case DiagPresentation.PresentationTypes.PresInt:
                    {
                        if (int.TryParse(txtValue.Text, out int val))
                        {
                            BitArrayExtension.CopyBitsClamped(SourceBits, BitArrayExtension.ToBitArray(val, bigEndian: true, bitArraySize: SourceBits.Length));
                            dataValid = true;
                        }
                        break;
                    }
                case DiagPresentation.PresentationTypes.PresUInt:
                    {
                        if (uint.TryParse(txtValue.Text, out uint val))
                        {
                            BitArrayExtension.CopyBitsClamped(SourceBits, BitArrayExtension.ToBitArray(val, bigEndian: true, bitArraySize: SourceBits.Length));
                            dataValid = true;
                        }
                        break;
                    }
            }

            if (dataValid)
            {
                UpdateLabel();
                RaiseValueChanged();
                txtValue.BackColor = SystemColors.Window;
            }
            else 
            {
                txtValue.BackColor = SystemColors.Info;
            }

        }
    }
}
