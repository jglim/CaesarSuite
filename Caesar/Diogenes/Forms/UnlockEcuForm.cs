using Caesar;
using Diogenes.SecurityAccess.NativeUnlock;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Diogenes
{
    public class UnlockEcuForm : Form
    {
        private readonly ECUConnection connection;
        private readonly DataGridView dgvDefinitions = new DataGridView();
        private readonly TextBox txtFilter = new TextBox();
        private readonly TextBox txtSeed = new TextBox();
        private readonly TextBox txtPayload = new TextBox();
        private readonly TextBox txtRequest = new TextBox();
        private readonly TextBox txtResponse = new TextBox();
        private readonly TextBox txtDetails = new TextBox();
        private readonly Label lblStatus = new Label();
        private readonly Button btnRequestSeed = new Button();
        private readonly Button btnCompute = new Button();
        private readonly Button btnSendUnlock = new Button();
        private readonly Button btnCopyPayload = new Button();
        private readonly Button btnCopyRequest = new Button();
        private readonly Button btnPasteSeed = new Button();
        private readonly Button btnClose = new Button();

        private List<UnlockDefinition> visibleDefinitions = new List<UnlockDefinition>();

        public UnlockEcuForm(ECUConnection connection)
        {
            this.connection = connection;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Font monoFont = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point);

            Text = "Unlock ECU";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            MinimumSize = new Size(980, 640);
            Size = new Size(1180, 760);

            Label lblFilter = new Label();
            lblFilter.AutoSize = true;
            lblFilter.Location = new Point(12, 16);
            lblFilter.Text = "Filter";

            txtFilter.Location = new Point(60, 12);
            txtFilter.Size = new Size(450, 23);
            txtFilter.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtFilter.TextChanged += txtFilter_TextChanged;

            dgvDefinitions.Location = new Point(12, 44);
            dgvDefinitions.Size = new Size(1138, 280);
            dgvDefinitions.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            dgvDefinitions.ReadOnly = true;
            dgvDefinitions.AllowUserToAddRows = false;
            dgvDefinitions.AllowUserToDeleteRows = false;
            dgvDefinitions.AllowUserToResizeRows = false;
            dgvDefinitions.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvDefinitions.MultiSelect = false;
            dgvDefinitions.RowHeadersVisible = false;
            dgvDefinitions.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dgvDefinitions.SelectionChanged += dgvDefinitions_SelectionChanged;
            EnableDoubleBuffer(dgvDefinitions, true);

            Label lblSeed = new Label();
            lblSeed.AutoSize = true;
            lblSeed.Location = new Point(12, 338);
            lblSeed.Text = "Seed";

            txtSeed.Location = new Point(80, 334);
            txtSeed.Size = new Size(540, 23);
            txtSeed.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtSeed.Font = monoFont;
            txtSeed.TextChanged += txtSeed_TextChanged;

            btnPasteSeed.Location = new Point(630, 332);
            btnPasteSeed.Size = new Size(90, 27);
            btnPasteSeed.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnPasteSeed.Text = "Paste";
            btnPasteSeed.Click += btnPasteSeed_Click;

            btnRequestSeed.Location = new Point(726, 332);
            btnRequestSeed.Size = new Size(100, 27);
            btnRequestSeed.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnRequestSeed.Text = "Request Seed";
            btnRequestSeed.Click += btnRequestSeed_Click;

            btnCompute.Location = new Point(832, 332);
            btnCompute.Size = new Size(90, 27);
            btnCompute.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnCompute.Text = "Compute";
            btnCompute.Click += btnCompute_Click;

            btnSendUnlock.Location = new Point(928, 332);
            btnSendUnlock.Size = new Size(110, 27);
            btnSendUnlock.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSendUnlock.Text = "Send Unlock";
            btnSendUnlock.Click += btnSendUnlock_Click;

            Label lblPayload = new Label();
            lblPayload.AutoSize = true;
            lblPayload.Location = new Point(12, 372);
            lblPayload.Text = "Payload";

            txtPayload.Location = new Point(80, 368);
            txtPayload.Size = new Size(540, 23);
            txtPayload.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtPayload.Font = monoFont;
            txtPayload.ReadOnly = true;

            btnCopyPayload.Location = new Point(630, 366);
            btnCopyPayload.Size = new Size(90, 27);
            btnCopyPayload.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnCopyPayload.Text = "Copy";
            btnCopyPayload.Click += btnCopyPayload_Click;

            Label lblRequest = new Label();
            lblRequest.AutoSize = true;
            lblRequest.Location = new Point(12, 406);
            lblRequest.Text = "Request";

            txtRequest.Location = new Point(80, 402);
            txtRequest.Size = new Size(540, 23);
            txtRequest.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtRequest.Font = monoFont;
            txtRequest.ReadOnly = true;

            btnCopyRequest.Location = new Point(630, 400);
            btnCopyRequest.Size = new Size(90, 27);
            btnCopyRequest.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnCopyRequest.Text = "Copy";
            btnCopyRequest.Click += btnCopyRequest_Click;

            Label lblResponse = new Label();
            lblResponse.AutoSize = true;
            lblResponse.Location = new Point(12, 440);
            lblResponse.Text = "Response";

            txtResponse.Location = new Point(80, 436);
            txtResponse.Size = new Size(540, 23);
            txtResponse.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtResponse.Font = monoFont;
            txtResponse.ReadOnly = true;

            Label lblDetails = new Label();
            lblDetails.AutoSize = true;
            lblDetails.Location = new Point(12, 474);
            lblDetails.Text = "Definition";

            txtDetails.Location = new Point(15, 496);
            txtDetails.Size = new Size(1135, 190);
            txtDetails.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtDetails.Font = monoFont;
            txtDetails.Multiline = true;
            txtDetails.ReadOnly = true;
            txtDetails.ScrollBars = ScrollBars.Vertical;

            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(12, 694);
            lblStatus.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblStatus.ForeColor = SystemColors.GrayText;
            lblStatus.Text = "Pick a definition, enter or request a seed, then compute or send the unlock request.";

            btnClose.Location = new Point(1060, 688);
            btnClose.Size = new Size(90, 27);
            btnClose.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnClose.Text = "Close";
            btnClose.Click += btnClose_Click;

            Controls.Add(lblFilter);
            Controls.Add(txtFilter);
            Controls.Add(dgvDefinitions);
            Controls.Add(lblSeed);
            Controls.Add(txtSeed);
            Controls.Add(btnPasteSeed);
            Controls.Add(btnRequestSeed);
            Controls.Add(btnCompute);
            Controls.Add(btnSendUnlock);
            Controls.Add(lblPayload);
            Controls.Add(txtPayload);
            Controls.Add(btnCopyPayload);
            Controls.Add(lblRequest);
            Controls.Add(txtRequest);
            Controls.Add(btnCopyRequest);
            Controls.Add(lblResponse);
            Controls.Add(txtResponse);
            Controls.Add(lblDetails);
            Controls.Add(txtDetails);
            Controls.Add(lblStatus);
            Controls.Add(btnClose);

            Load += UnlockEcuForm_Load;
        }

        private void UnlockEcuForm_Load(object sender, EventArgs e)
        {
            UpdateGrid();

            string connectedEcu = connection?.EcuContext?.Qualifier;
            if (!string.IsNullOrWhiteSpace(connectedEcu))
            {
                txtFilter.Text = connectedEcu;
                lblStatus.Text = $"Connected ECU detected: {connectedEcu}. Matching definitions have been pre-filtered.";
            }

            UpdateConnectionButtonState();
        }

        private static void EnableDoubleBuffer(DataGridView dgv, bool setting)
        {
            PropertyInfo property = dgv.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            property?.SetValue(dgv, setting, null);
        }

        private void UpdateGrid()
        {
            visibleDefinitions = NativeUnlockService.GetAvailableDefinitions(txtFilter.Text).ToList();

            DataTable dt = new DataTable();
            dt.Columns.Add("Index", typeof(string));
            dt.Columns.Add("ECU", typeof(string));
            dt.Columns.Add("Origin", typeof(string));
            dt.Columns.Add("Level", typeof(string));
            dt.Columns.Add("Seed", typeof(string));
            dt.Columns.Add("Payload", typeof(string));
            dt.Columns.Add("Provider", typeof(string));

            for (int i = 0; i < visibleDefinitions.Count; i++)
            {
                UnlockDefinition definition = visibleDefinitions[i];
                dt.Rows.Add(
                    i.ToString(),
                    definition.EcuName,
                    definition.Origin,
                    definition.AccessLevel.ToString(),
                    definition.SeedLength.ToString(),
                    definition.KeyLength.ToString(),
                    definition.Provider);
            }

            dgvDefinitions.DataSource = dt;
            if (dgvDefinitions.Columns.Count > 0)
            {
                dgvDefinitions.Columns[0].Visible = false;
                dgvDefinitions.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dgvDefinitions.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgvDefinitions.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                dgvDefinitions.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                dgvDefinitions.Columns[5].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                dgvDefinitions.Columns[6].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }

            if (visibleDefinitions.Count > 0)
            {
                dgvDefinitions.Rows[0].Selected = true;
                dgvDefinitions.CurrentCell = dgvDefinitions.Rows[0].Cells[1];
            }
            else
            {
                txtDetails.Clear();
                txtPayload.Clear();
                txtRequest.Clear();
                txtResponse.Clear();
            }
        }

        private UnlockDefinition GetSelectedDefinition()
        {
            if (dgvDefinitions.SelectedRows.Count != 1)
            {
                return null;
            }

            int selectedIndex = int.Parse(dgvDefinitions.SelectedRows[0].Cells[0].Value.ToString());
            if (selectedIndex < 0 || selectedIndex >= visibleDefinitions.Count)
            {
                return null;
            }

            return visibleDefinitions[selectedIndex];
        }

        private void dgvDefinitions_SelectionChanged(object sender, EventArgs e)
        {
            UnlockDefinition definition = GetSelectedDefinition();
            txtDetails.Text = NativeUnlockService.DescribeDefinition(definition);
            txtResponse.Clear();
            TryRefreshPayload(false);
            UpdateConnectionButtonState();
        }

        private void txtFilter_TextChanged(object sender, EventArgs e)
        {
            UpdateGrid();
        }

        private void txtSeed_TextChanged(object sender, EventArgs e)
        {
            TryRefreshPayload(false);
        }

        private void btnPasteSeed_Click(object sender, EventArgs e)
        {
            if (Clipboard.ContainsText())
            {
                txtSeed.Text = Clipboard.GetText();
            }
        }

        private void btnCompute_Click(object sender, EventArgs e)
        {
            TryRefreshPayload(true);
        }

        private void btnCopyPayload_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtPayload.Text))
            {
                Clipboard.SetText(txtPayload.Text);
            }
        }

        private void btnCopyRequest_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtRequest.Text))
            {
                Clipboard.SetText(txtRequest.Text);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnRequestSeed_Click(object sender, EventArgs e)
        {
            UnlockDefinition definition = GetSelectedDefinition();
            if (definition is null)
            {
                SetStatus("Select an unlock definition first.", true);
                return;
            }

            if (!CanUseConnection())
            {
                SetStatus("A live connection with unlock support is required to request a seed.", true);
                return;
            }

            byte[] request = NativeUnlockService.BuildSeedRequest(definition);
            byte[] response = connection.SendMessage(request);
            txtResponse.Text = BitUtility.BytesToHex(response, true);

            if ((response.Length >= 3) && (response[0] == 0x67) && (response[1] == definition.AccessLevel))
            {
                byte[] seed = response.Skip(2).ToArray();
                txtSeed.Text = BitUtility.BytesToHex(seed, true);
                SetStatus($"Received {seed.Length}-byte seed for {definition.EcuName} level {definition.AccessLevel}.", false);
            }
            else
            {
                SetStatus($"Seed request was not accepted. Response: {BitUtility.BytesToHex(response, true)}", true);
            }
        }

        private void btnSendUnlock_Click(object sender, EventArgs e)
        {
            UnlockDefinition definition = GetSelectedDefinition();
            if (definition is null)
            {
                SetStatus("Select an unlock definition first.", true);
                return;
            }

            if (!CanUseConnection())
            {
                SetStatus("A live connection with unlock support is required to send an unlock request.", true);
                return;
            }

            if (!TryRefreshPayload(true))
            {
                return;
            }

            byte[] payload = BitUtility.BytesFromHex(txtPayload.Text);
            byte[] request = NativeUnlockService.BuildUnlockRequest(definition, payload);
            byte[] response = connection.SendMessage(request);
            txtResponse.Text = BitUtility.BytesToHex(response, true);

            if ((response.Length >= 2) && (response[0] == 0x67) && (response[1] == definition.AccessLevel + 1))
            {
                SetStatus($"Unlock accepted for {definition.EcuName} level {definition.AccessLevel}.", false);
            }
            else
            {
                SetStatus($"Unlock response: {BitUtility.BytesToHex(response, true)}", true);
            }
        }

        private bool TryRefreshPayload(bool showErrors)
        {
            UnlockDefinition definition = GetSelectedDefinition();
            if (definition is null)
            {
                txtPayload.Clear();
                txtRequest.Clear();
                return false;
            }

            if (!TryParseHexBytes(txtSeed.Text, out byte[] seed, out string error))
            {
                txtPayload.Clear();
                txtRequest.Clear();
                txtSeed.BackColor = string.IsNullOrWhiteSpace(txtSeed.Text) ? SystemColors.Window : Color.LavenderBlush;
                if (showErrors)
                {
                    SetStatus(error, true);
                }
                return false;
            }

            txtSeed.BackColor = SystemColors.Window;

            if (!NativeUnlockService.TryGeneratePayload(definition, seed, out byte[] payload, out string providerError))
            {
                txtPayload.Clear();
                txtRequest.Clear();
                if (showErrors)
                {
                    SetStatus(providerError, true);
                }
                return false;
            }

            txtPayload.Text = BitUtility.BytesToHex(payload, true);
            txtRequest.Text = BitUtility.BytesToHex(NativeUnlockService.BuildUnlockRequest(definition, payload), true);
            if (showErrors)
            {
                SetStatus($"Generated payload with {definition.Provider}.", false);
            }

            return true;
        }

        private void UpdateConnectionButtonState()
        {
            bool connectionAvailable = CanUseConnection();
            btnRequestSeed.Enabled = connectionAvailable;
            btnSendUnlock.Enabled = connectionAvailable;
        }

        private bool CanUseConnection()
        {
            return (connection?.ConnectionProtocol?.SupportsUnlocking() ?? false) &&
                   (connection.State == ECUConnection.ConnectionState.EcuContacted);
        }

        private void SetStatus(string text, bool isError)
        {
            lblStatus.ForeColor = isError ? Color.Firebrick : SystemColors.GrayText;
            lblStatus.Text = text;
        }

        private static bool TryParseHexBytes(string input, out byte[] bytes, out string error)
        {
            string cleaned = Regex.Replace(input ?? string.Empty, "[^0-9A-Fa-f]", string.Empty);
            if (cleaned.Length == 0)
            {
                bytes = Array.Empty<byte>();
                error = "Enter a seed value.";
                return false;
            }

            if ((cleaned.Length % 2) != 0)
            {
                bytes = Array.Empty<byte>();
                error = "Seed hex must contain an even number of characters.";
                return false;
            }

            if (!Regex.IsMatch(cleaned, "\\A[0-9A-Fa-f]+\\z"))
            {
                bytes = Array.Empty<byte>();
                error = "Seed contains invalid hex characters.";
                return false;
            }

            bytes = BitUtility.BytesFromHex(cleaned);
            error = string.Empty;
            return true;
        }
    }
}
