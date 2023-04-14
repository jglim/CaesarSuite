using Caesar;
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

namespace Diogenes.Forms
{
    public partial class DiagServicesView : UserControl
    {
        BindingList<DiagService> VisibleDiagServices = new BindingList<DiagService>();
        BindingList<DiagPreparation> VisiblePreparations = new BindingList<DiagPreparation>();
        DiagService SelectedDiagService = null;
        BitArray SelectedDiagBitArray = new(new byte[] { });
        PresentationEditor PresEditor;

        static readonly string RequestBuilderTitle = "Request Builder";
        

        public DiagServicesView()
        {
            InitializeComponent();

            // add presentation editor component
            PresEditor = new PresentationEditor();
            PresEditor.Dock = DockStyle.Fill;
            PresEditor.ValueChanged += PresEditor_ValueChanged;
            splitContainer2.Panel2.Controls.Add(PresEditor);

            // initialize diag picker
            dgvDiagPicker.DataSource = VisibleDiagServices;
            const double viewRatio = 0.7;
            dgvDiagPicker.Columns[0].Width = (int)(dgvDiagPicker.Width * viewRatio);
            dgvDiagPicker.Columns[1].Width = (int)(dgvDiagPicker.Width * (1 - viewRatio));

            // refresh ui for request builder
            dgvDiagPicker_SelectionChanged(null, null);

            // initialize req builder (preparations)
            dgvRequestBuilder.DataSource = VisiblePreparations;
            dgvRequestBuilder.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

        }

        public void NotifyCbfOrVariantChange() 
        {
            InternalReRenderGrid();
        }

        private void InternalReRenderGrid() 
        {
            // pause updates
            dgvDiagPicker.Suspend();
            VisibleDiagServices.RaiseListChangedEvents = false;

            VisibleDiagServices.Clear();

            if (DiogenesSharedContext.Singleton.PrimaryEcu != null)
            {
                string filter = txtDiagFilter.Text.ToLower();

                // if picking from active variant
                if (chkVariantFilter.Checked && (DiogenesSharedContext.Singleton.PrimaryVariant != null))
                {
                    // likely an issue if the bindinglist is replaced
                    foreach (var row in DiogenesSharedContext.Singleton.PrimaryVariant.DiagServices)
                    {
                        if (row.Qualifier.ToLower().Contains(filter))
                        {
                            VisibleDiagServices.Add(row); // no addrange for bindinglist..?
                        }
                    }
                }
                else
                {
                    // pick from root diagservices
                    foreach (var row in DiogenesSharedContext.Singleton.PrimaryEcu.GlobalDiagServices)
                    {
                        if (row.Qualifier.ToLower().Contains(filter))
                        {
                            VisibleDiagServices.Add(row);
                        }
                    }
                }
            }

            // restore updates
            VisibleDiagServices.RaiseListChangedEvents = true;
            VisibleDiagServices.ResetBindings();
            dgvDiagPicker.Resume();
        }

        private void chkVariantFilter_CheckedChanged(object sender, EventArgs e)
        {
            NotifyCbfOrVariantChange();
        }

        private void txtDiagFilter_TextChanged(object sender, EventArgs e)
        {
            NotifyCbfOrVariantChange();
        }

        private void dgvDiagPicker_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvDiagPicker.SelectedRows.Count == 1)
            {
                // diagnostic service selection changed
                SelectedDiagService = dgvDiagPicker.SelectedRows[0].DataBoundItem as DiagService;
                gbRequestBuilder.Text = $"{RequestBuilderTitle} : {SelectedDiagService.Qualifier}";
                btnExecuteRequest.Enabled = true;
            }
            else 
            {
                // list cleared
                SelectedDiagService = null;
                gbRequestBuilder.Text = RequestBuilderTitle;
                btnExecuteRequest.Enabled = false;
            }

            PresentRequestParams();
        }

        // when a diagservice is picked, display its preparations in the datagridview
        // the preparations are intended to be user-changeable (deep-copied)
        // the final request bytes are reassembled from these preparations
        private void PresentRequestParams()
        {
            // pause updates
            dgvRequestBuilder.Suspend();
            VisiblePreparations.RaiseListChangedEvents = false;
            VisiblePreparations.Clear();

            if (SelectedDiagService != null) 
            {
                foreach (var prep in SelectedDiagService.InputPreparations)
                {
                    VisiblePreparations.Add(prep.DeepCopy());
                }
            }

            // restore updates
            VisiblePreparations.RaiseListChangedEvents = true;
            VisiblePreparations.ResetBindings();
            dgvRequestBuilder.Resume();

            UpdateRequestPreview();
        }

        // regenerates bitarray from existing preparations, then updates the textbox preview
        private void UpdateRequestPreview() 
        {
            GenerateRequestBitArray();
            txtRequestPreview.Text = BitUtility.BytesToHex(BitArrayExtension.ToBytes(SelectedDiagBitArray), true);
            HighlightSelectedPreparationInTextboxPreview();
        }

        // when presentation editor receives a user-committed change, this is called
        // pull out the new value from the editor and write it into the respective preparation
        private void PresEditor_ValueChanged() 
        {
            var selectedPrep = dgvRequestBuilder.SelectedRows[0].DataBoundItem as DiagPreparation;
            selectedPrep.Content = PresEditor.SourceBits;
            UpdateRequestPreview();
        }

        // reassembles preparations into a single bitarray
        private void GenerateRequestBitArray() 
        {
            // clear bitarray, check diagservice actually exists
            SelectedDiagBitArray = new(new byte[] { });
            if (SelectedDiagService is null) 
            {
                return;
            }
            // initialize the array with sane defaults
            SelectedDiagBitArray = new BitArray(SelectedDiagService.RequestBytes.Length * 8);

            // fill from preparations
            foreach (var prep in VisiblePreparations) 
            {
                // fixme: ic204:DT_Memory_nur_Entwicklung_Length_number_of_bytes_of_the_Memory_Address_parameter, absurdly large value

                //  if the parameter won't fit, complain and skip
                if ((prep.BitPosition + prep.Content.Length) > SelectedDiagBitArray.Length) 
                {
                    Console.WriteLine($"{prep.Qualifier} was not written (out of bounds)");
                    continue;
                }
                for (int i = 0; i < prep.Content.Length; i++)
                {
                    SelectedDiagBitArray[i + prep.BitPosition] = prep.Content[i];
                }
            }
        }

        private void HighlightSelectedPreparationInTextboxPreview() 
        {
            if (dgvRequestBuilder.SelectedRows.Count != 1) 
            {
                return;
            }
            // highlight selected bits on textbox
            var selectedPrep = dgvRequestBuilder.SelectedRows[0].DataBoundItem as DiagPreparation;
            int lowerByteSelection = selectedPrep.BitPosition / 8;
            int selectionSize = (int)Math.Ceiling((selectedPrep.SizeInBits) / 8.0m);

            // hex values in 2 ascii bytes, one space char = 3 characters
            lowerByteSelection *= 3;
            selectionSize *= 3;
            // no trailing space for last entry
            selectionSize--;

            txtRequestPreview.Select(lowerByteSelection, selectionSize);
        }

        private void dgvRequestBuilder_SelectionChanged(object sender, EventArgs e)
        {
            bool presentationWasSet = false;

            if (dgvRequestBuilder.SelectedRows.Count == 1)
            {
                // param changed

                // refresh bit preview
                UpdateRequestPreview();

                // show selected bits on textbox
                var selectedPrep = dgvRequestBuilder.SelectedRows[0].DataBoundItem as DiagPreparation; 
                HighlightSelectedPreparationInTextboxPreview();

                // if it's a presentation, enable the editor
                if (selectedPrep.FieldType == DiagPreparation.InferredDataType.PrepPresentationType)
                {
                    var prepPres = selectedPrep.ParentECU.GlobalPrepPresentations[selectedPrep.PrepPresPoolIndex];

                    // variable-length unicode strings will break things here, disable it
                    bool sliceLengthExceedsCurrentBitarrayLength = (selectedPrep.BitPosition + selectedPrep.SizeInBits) > SelectedDiagBitArray.Length;
                    if (!sliceLengthExceedsCurrentBitarrayLength) 
                    {
                        // string log = $"{selectedPrep.Qualifier} : slicing {SelectedDiagBitArray.Length} at {selectedPrep.BitPosition} with size {selectedPrep.SizeInBits}, ending at {selectedPrep.BitPosition + selectedPrep.SizeInBits}";

                        BitArray prepValue = BitArrayExtension.Slice(SelectedDiagBitArray, selectedPrep.BitPosition, selectedPrep.SizeInBits);
                        PresEditor.SetPresentation(prepPres, prepValue);
                        presentationWasSet = true;
                    }
                }
            }

            if (!presentationWasSet) 
            {
                PresEditor.SetPresentation(null);
            }
        }

        private void btnExecuteRequest_Click(object sender, EventArgs e)
        {
            
            // executes a prepared request
            if (DiogenesSharedContext.Singleton.Channel is null) 
            {
                Console.WriteLine($"An active ECU connection is required for this request");
                return;
            }
            byte[] requestData = BitArrayExtension.ToBytes(SelectedDiagBitArray);
            Console.WriteLine($"REQ: {BitUtility.BytesToHex(requestData, true)}");
            byte[] response = DiogenesSharedContext.Singleton.Channel.Send(requestData, true);
            Console.WriteLine($"ECU: {BitUtility.BytesToHex(response, true)}");


            bool shouldInterpret = (response.Length > 1) && (response[0] != 0x7F);
            if (!shouldInterpret) 
            {
                return;
            }

            // transform the response to a human-readable string
            foreach (var prepSets in SelectedDiagService.OutputPreparations) 
            {
                foreach (var prep in prepSets) 
                {
                    var prepPres = prep.ParentECU.GlobalPresentations[prep.PresPoolIndex];
                    Console.WriteLine($"{prep.Qualifier} [{prep.SizeInBits}] @ {prep.BitPosition}");
                    BitArray responseAsBits = new BitArray(response);
                    BitArray sliced = BitArrayExtension.Slice(responseAsBits, prep.BitPosition, prep.SizeInBits);
                    Console.WriteLine(prepPres.Interpret(sliced));
                }
            }

        }
    }
}
