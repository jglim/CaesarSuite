using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caesar;

namespace Diogenes
{
    public class FlashItem
    {
        public string Name { get { return Container.CaesarFlashHeader.FlashName; } }

        FlashArea Area { get { return Container.CaesarFlashHeader.DescriptionHeaders[0]; } }
        FlashTable Table { get { return Area.FlashTables[0]; } }

        public string Key { get { return Table.FlashKey; } }
        public int Priority { get { return Table.Priority; } }
        public string Job { get { return Table.FlashService; } }
        public string Qualifier { get { return Table.Qualifier; } }
        public string AllowedECU { get { return Table.AllowedECUs[0]; } }

        public int Blocks { get { return Container.CaesarFlashHeader.DataBlocks.Count; } }
        public int Segments { get { return Container.CaesarFlashHeader.DataBlocks.Sum(x=> x.FlashSegments.Count); } }
        public int Securities { get { return Container.CaesarFlashHeader.DataBlocks.Sum(x => x.FlashSecurities.Count); } }

        public CaesarFlashContainer Container = null;
        public FlashItem(CaesarFlashContainer container)
        {
            Container = container;

            // haven't seen multiple areas around, so this only supports one. throw an exception if != 1
            if (container.CaesarFlashHeader.DescriptionHeaders.Count != 1)
            {
                throw new Exception($"Could not add {container.CaesarFlashHeader.FlashName} as there were {container.CaesarFlashHeader.DescriptionHeaders.Count} descriptors, expecting 1.");
            }

            if (Area.FlashTables.Length != 1) 
            {
                throw new Exception($"Could not add {container.CaesarFlashHeader.FlashName} as there were {Area.FlashTables.Length } flashtables, expecting 1.");
            }
        }
    }

}
