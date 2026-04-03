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
        public static string GenerateVariantCodingBackupDocument(string ecuName, string variantName, ECUConnection connection, List<CaesarContainer> containers, Action<int, int, string> progressCallback = null)
        {
            if (connection is null)
            {
                throw new InvalidOperationException("Please initiate contact with a target first.");
            }

            string reportDate = $"{DateTime.Now.ToShortDateString()} {DateTime.Now.ToLongTimeString()}";

            CaesarContainer container = containers.Find(x => x.GetECUVariantByName(variantName) != null);
            if (container is null)
            {
                throw new InvalidOperationException($"Could not find a loaded container for variant '{variantName}'.");
            }

            ECU ecu = container.GetECUByName(ecuName);
            if (ecu is null)
            {
                throw new InvalidOperationException($"Could not find ECU '{ecuName}' in the loaded container.");
            }

            ECUVariant variant = container.GetECUVariantByName(variantName);
            if (variant is null)
            {
                throw new InvalidOperationException($"Could not find ECU variant '{variantName}' in the loaded container.");
            }

            string containerChecksum = container.FileChecksum.ToString("X8");
            string dVersion = MainForm.GetVersion();
            string cVersion = CaesarContainer.GetCaesarVersionString();
            string connectionData = connection.FriendlyProfileName;
            string ecuCbfVersion = ecu.EcuXmlVersion;

            StringBuilder tableBuilder = new StringBuilder();
            int totalSteps = variant.VCDomains.Count + 1;

            progressCallback?.Invoke(1, totalSteps, "Reading ECU metadata");
            string metadataTable = connection.ConnectionProtocol.QueryECUMetadata(connection).GetHtmlTable(connection);

            // back up every domain since some have overlaps
            for (int i = 0; i < variant.VCDomains.Count; i++)
            {
                VCDomain domain = variant.VCDomains[i];
                progressCallback?.Invoke(i + 2, totalSteps, $"Reading {domain.Qualifier}");

                // find the read service, then execute it as-is
                DiagService readService = variant.GetDiagServiceByName(domain.ReadServiceName);
                byte[] response = connection.SendDiagRequest(readService);

                // isolate the traditional vc string
                DiagPreparation largestPrep = VCForm.GetLargestPreparation(readService.OutputPreparations);
                byte[] vcValue = response.Skip(largestPrep.BitPosition / 8).Take(largestPrep.SizeInBits / 8).ToArray();

                StringBuilder tableRowBuilder = new StringBuilder();

                // explain the vc string's settings
                for (int fragmentIndex = 0; fragmentIndex < domain.VCFragments.Count; fragmentIndex++)
                {
                    VCFragment currentFragment = domain.VCFragments[fragmentIndex];
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

    {metadataTable}
    {tableBuilder}

    <hr>

    <span id=""eof"">End of report</span>
</body>
</html>";
            return document;
        }
    }
}
