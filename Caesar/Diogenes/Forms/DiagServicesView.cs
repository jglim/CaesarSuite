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
        DiagService SelectedDiagService = null;
        DiagServiceRunner DiagServiceToExecute = null;

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
            dgvRequestBuilder.DataSource = new BindingList<DiagPreparation>();
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
                        if (row.Qualifier.ToLower().Contains(filter)) // && (row.DiagServiceCode.Count > 0)
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
                string groupboxLabel = $"{RequestBuilderTitle} : {SelectedDiagService.Qualifier} ";

                foreach (var dsc in SelectedDiagService.DiagServiceCode) 
                {
                    groupboxLabel += $"[Script: {dsc.Qualifier}] ";
                }
                DiagServiceToExecute = new DiagServiceRunner(SelectedDiagService);
                dgvRequestBuilder.DataSource = DiagServiceToExecute.VisiblePreparations;
                gbRequestBuilder.Text = groupboxLabel;
                btnExecuteRequest.Enabled = true;
            }
            else 
            {
                // list cleared
                SelectedDiagService = null;
                DiagServiceToExecute = null;
                dgvRequestBuilder.DataSource = new BindingList<DiagPreparation>();
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
            dgvRequestBuilder.Suspend();
            DiagServiceToExecute?.ResetPreparations();
            dgvRequestBuilder.Resume();
            UpdateRequestPreview();
        }

        // regenerates bitarray from existing preparations, then updates the textbox preview
        private void UpdateRequestPreview() 
        {
            txtRequestPreview.Text = "";
            if (DiagServiceToExecute is null) 
            {
                return;
            }
            DiagServiceToExecute?.GenerateRequestBitArray();
            txtRequestPreview.Text = BitUtility.BytesToHex(BitArrayExtension.ToBytes(DiagServiceToExecute.DiagBits), true);
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
                    bool sliceLengthExceedsCurrentBitarrayLength = (selectedPrep.BitPosition + selectedPrep.SizeInBits) > DiagServiceToExecute.DiagBits.Length;
                    if (!sliceLengthExceedsCurrentBitarrayLength) 
                    {
                        // string log = $"{selectedPrep.Qualifier} : slicing {SelectedDiagBitArray.Length} at {selectedPrep.BitPosition} with size {selectedPrep.SizeInBits}, ending at {selectedPrep.BitPosition + selectedPrep.SizeInBits}";

                        BitArray prepValue = BitArrayExtension.Slice(DiagServiceToExecute.DiagBits, selectedPrep.BitPosition, selectedPrep.SizeInBits);
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

            DiagServiceToExecute.DoDiagService();
            Console.WriteLine(DiagServiceToExecute.GetOutputPreparationResultString());


        }
    }
}
