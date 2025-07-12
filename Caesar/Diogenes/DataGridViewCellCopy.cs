using System;
using System.Windows.Forms;

namespace Diogenes
{
    public class DataGridViewCellCopy
    {
        int copyRowIndex = -1;
        int copyColumnIndex = -1;
        DataGridView view;

        private DataGridViewCellCopy(DataGridView newView)
        {
            view = newView;
        }

        // Adds a context menu that copies the current cells value to provided grid
        public static void AddCopyCellMenu(DataGridView newView)
        {
            var instance = new DataGridViewCellCopy(newView);
            instance.AddCopyContextMenuAndListeners();
        }

        private void AddCopyContextMenuAndListeners()
        {
            ContextMenuStrip contextMenuStrip = new ContextMenuStrip();
            ToolStripItem copyItem = contextMenuStrip.Items.Add("Copy");
            copyItem.Click += (sender, e) => CopySelectedCell();
            view.ContextMenuStrip = contextMenuStrip;
            view.CellMouseDown += (sender, e) => HandleCellMouseDown(e);
        }

        private void CopySelectedCell()
        {
            if (copyRowIndex >= 0 && copyColumnIndex >= 0)
            {
                Clipboard.SetText(view.Rows[copyRowIndex].Cells[copyColumnIndex].Value.ToString());
            }
        }

        private void HandleCellMouseDown(DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                copyRowIndex = e.RowIndex;
                copyColumnIndex = e.ColumnIndex;
            }
        }
    }


}