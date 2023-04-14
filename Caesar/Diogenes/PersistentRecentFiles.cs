using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Diogenes
{
    internal class PersistentRecentFiles
    {
        const int PersistentFileMaxCount = 12;

        private ToolStripMenuItem m_SupervisedMenu;
        private Action<string> m_ClickCallback;

        public PersistentRecentFiles(ToolStripMenuItem supervisedMenu, Action<string> clickCallback) 
        {
            m_SupervisedMenu = supervisedMenu;
            m_ClickCallback = clickCallback;

            if (Properties.Settings.Default.RecentlyOpenedCBF is null) 
            {
                ClearRecentFiles();
            }
            PopulateMenu();
        }

        public void PopulateMenu() 
        {
            m_SupervisedMenu.DropDownItems.Clear();
            foreach (string row in Properties.Settings.Default.RecentlyOpenedCBF)
            {
                ToolStripMenuItem menu = new ToolStripMenuItem(row);
                menu.Click += (sender, e) => { m_ClickCallback((sender as ToolStripMenuItem).Text); };
                m_SupervisedMenu.DropDownItems.Add(menu);
            }

            if (m_SupervisedMenu.DropDownItems.Count == 0) 
            {
                ToolStripMenuItem menu = new ToolStripMenuItem("(Recently opened files will be shown here)");
                menu.Enabled = false;
                m_SupervisedMenu.DropDownItems.Add(menu);
            }

            m_SupervisedMenu.DropDownItems.Add(new ToolStripSeparator());
            ToolStripMenuItem clearMenu = new ToolStripMenuItem("Clear Recent Files");
            clearMenu.Click += (sender, e) => { ClearRecentFiles(); };
            m_SupervisedMenu.DropDownItems.Add(clearMenu);
        }

        public void ClearRecentFiles() 
        {
            Properties.Settings.Default.RecentlyOpenedCBF = new StringCollection();
            Properties.Settings.Default.Save();
            PopulateMenu();
        }

        public void AddRecentFile(string path) 
        {
            StringCollection newCollection = new StringCollection();
            newCollection.Add(path);
            int index = 1;
            foreach (string row in Properties.Settings.Default.RecentlyOpenedCBF)
            {
                if (row == path) 
                {
                    continue;
                }
                newCollection.Add(row);

                index++;
                if (index >= PersistentFileMaxCount) 
                {
                    break;
                }
            }
            Properties.Settings.Default.RecentlyOpenedCBF = newCollection;
            Properties.Settings.Default.Save();
            PopulateMenu();
        }


        public void RemoveEntry(string path)
        {
            Properties.Settings.Default.RecentlyOpenedCBF.Remove(path);
            Properties.Settings.Default.Save();
            PopulateMenu();
        }
    }
}
