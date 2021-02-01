using Caesar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Diogenes
{
    public class VCReport
    {

        public static void treeViewSelectVariantCodingBackup(TreeNode node, ECUConnection connection, List<CaesarContainer> containers)
        {

            if (connection is null)
            {
                return;
            }

            Cursor.Current = Cursors.WaitCursor;

            string variantName = node.Parent.Text;
            string ecuName = node.Parent.Parent.Text;
            string reportDate = $"{DateTime.Now.ToShortDateString()} {DateTime.Now.ToLongTimeString()}";

            StringBuilder report = new StringBuilder();

            CaesarContainer container = containers.Find(x => x.GetECUVariantByName(variantName) != null);
            ECU ecu = container.GetECUByName(ecuName);
            ECUVariant variant = container.GetECUVariantByName(variantName);

            string containerChecksum = container.FileChecksum.ToString("X8");
            string dVersion = MainForm.GetVersion();
            string cVersion = CaesarContainer.GetCaesarVersionString();
            string connectionData = connection is null ? "(Unavailable)" : connection.FriendlyProfileName;
            string ecuCbfVersion = ecu.EcuXmlVersion;

            report.Append($"ECU Variant: {variant.Qualifier}\r\n");

            StringBuilder tableBuilder = new StringBuilder();

            // back up every domain since some have overlaps
            foreach (VCDomain domain in variant.VCDomains)
            {
                report.Append($"\r\nCoding Service: {domain.Qualifier}\r\n");
                // find the read service, then execute it as-is
                DiagService readService = variant.GetDiagServiceByName(domain.ReadServiceName);
                byte[] response = connection.SendDiagRequest(readService);

                // isolate the traditional vc string
                DiagPreparation largestPrep = VCForm.GetLargestPreparation(readService.OutputPreparations);
                byte[] vcValue = response.Skip(largestPrep.BitPosition / 8).Take(largestPrep.SizeInBits / 8).ToArray();

                StringBuilder tableRowBuilder = new StringBuilder();

                // explain the vc string's settings
                for (int i = 0; i < domain.VCFragments.Count; i++)
                {
                    VCFragment currentFragment = domain.VCFragments[i];
                    VCSubfragment subfragment = currentFragment.GetSubfragmentConfiguration(vcValue);

                    string fragmentValue = subfragment is null ? "(?)" : subfragment.NameResolved;
                    string fragmentSupplementKey = subfragment is null ? "(?)" : subfragment.SupplementKey;

                    string tableRowBlock = $@"
        <tr>
            <td>{currentFragment.Qualifier}</td>
            <td>{fragmentValue}</td>
            <td>{fragmentSupplementKey}</td>
        </tr>
";
                    tableRowBuilder.Append(tableRowBlock);
                }

                string tableBlock = $@"
    <hr>

    <h2>{domain.Qualifier}</h2>

    <table class=""coding-data"">
        <tr>
            <td class=""fifth"">Coding String (Hex)</td>
            <td class=""monospace"">{BitUtility.BytesToHex(vcValue, true)}</td>
        </tr>
        <tr>
            <td class=""fifth"">Raw Coding String (Hex)</td>
            <td class=""monospace"">{BitUtility.BytesToHex(response, true)}</td>
        </tr>
    </table>

    <table>
        <tr>
            <th>Fragment</th>
            <th>Value</th>
            <th>Supplement Key</th>
        </tr>
        {tableRowBuilder}
    </table>
";
                tableBuilder.Append(tableBlock);
            }


            string document = $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <title>{ecuName} : Backup</title>
    <style>
        body
        {{
            padding: 10px 20% 15px 15%;
            font-family: sans-serif;
        }}
        .pull-right
        {{
            float: right;
        }}
        hr
        {{
            border-bottom: 0;
            opacity: 0.2;
        }}
        table
        {{
            width: 100%;
            margin: 20px 0;
        }}
        #eof
        {{
            text-transform: uppercase;
            font-weight: bold;
            opacity: 0.15;
            letter-spacing: 0.4em;
        }}
        .coding-data
        {{
            opacity: 0.8;
        }}
        .monospace
        {{
            font-family: monospace;
        }}
        .fifth
        {{
            width: 20%;
        }}
        th
        {{
            text-align: left;
        }}
    </style>
</head>
<body>
    <h1 class=""pull-right"">Diogenes</h1>
    <h1>{ecuName}</h1>

    <hr>
    <table>
        <tr>
            <td>CBF Checksum</td>
            <td>{containerChecksum}</td>
        </tr>
        <tr>
            <td>Date</td>
            <td>{reportDate}</td>
        </tr>
        <tr>
            <td>Client Version</td>
            <td>Diogenes: {dVersion}, Caesar: {cVersion}</td>
        </tr>
        <tr>
            <td>ECU CBF Version</td>
            <td>{ecuCbfVersion}</td>
        </tr>
        <tr>
            <td>ECU Variant</td>
            <td>{variantName}</td>
        </tr>
        <tr>
            <td>Connection Info</td>
            <td>{connectionData}</td>
        </tr>
    </table>

    {connection.ConnectionProtocol.QueryECUMetadata(connection).GetHtmlTable(connection)}
    {tableBuilder}

    <hr>

    <span id=""eof"">End of report</span>
</body>
</html>";


            Cursor.Current = Cursors.Default;

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "Specify a location to save your new VC backup";
            sfd.Filter = "HTML file (*.html)|*.html|All files (*.*)|*.*";
            sfd.FileName = $"VC_{variantName}_{DateTime.Now.ToString("yyyyMMdd_HHmm")}.html";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(sfd.FileName, document.ToString());
                MessageBox.Show($"Backup successfully saved to {sfd.FileName}", "Export complete");
            }

        }

    }
}
