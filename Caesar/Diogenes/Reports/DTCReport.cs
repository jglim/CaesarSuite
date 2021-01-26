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
    public class DTCReport
    {

        public static void CreateDTCReport(List<DTCContext> dtcContexts, ECUConnection connection, ECUVariant variant)
        {
            Cursor.Current = Cursors.WaitCursor;

            string reportDate = $"{DateTime.Now.ToShortDateString()} {DateTime.Now.ToLongTimeString()}";

            string containerChecksum = variant.ParentECU.ParentContainer.FileChecksum.ToString("X8");
            string dVersion = MainForm.GetVersion();
            string cVersion = CaesarContainer.GetCaesarVersionString();
            string connectionData = connection is null ? "(Unavailable)" : connection.FriendlyProfileName;
            string ecuCbfVersion = variant.ParentECU.EcuXmlVersion;

            StringBuilder tableBuilder = new StringBuilder();

            // back up every domain since some have overlaps
            foreach (DTCContext dtcCtx in dtcContexts)
            {
                StringBuilder tableRowBuilder = new StringBuilder();

                // env, description
                for (int i = 0; i < dtcCtx.EnvironmentContext.Count; i++)
                {
                    string tableRowBlock = $@"
        <tr>
            <td>{dtcCtx.EnvironmentContext[i][0]}</td>
            <td>{dtcCtx.EnvironmentContext[i][1]}</td>
        </tr>
";
                    tableRowBuilder.Append(tableRowBlock);
                }

                string tableBlock = $@"
    <hr>

    <h2>{dtcCtx.DTC.Qualifier}</h2>

    <p>{dtcCtx.DTC.Description}</p>

    <table>
        <tr>
            <th>Environment</th>
            <th>Description</th>
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
    <title>{variant.ParentECU.Qualifier} : DTC Report</title>
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
    <h1>{variant.ParentECU.Qualifier}</h1>
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
            <td>{variant.Qualifier}</td>
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
            sfd.Title = "Specify a location to save your new DTC report";
            sfd.Filter = "HTML file (*.html)|*.html|All files (*.*)|*.*";
            sfd.FileName = $"DTC_{variant.Qualifier}_{DateTime.Now.ToString("yyyyMMdd_HHmm")}.html";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(sfd.FileName, document.ToString());
                MessageBox.Show($"Report successfully saved to {sfd.FileName}", "Export complete");
            }

        }

    }
}
