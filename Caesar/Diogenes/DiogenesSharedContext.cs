using Caesar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Diogenes
{
    // singletons go here
    public class DiogenesSharedContext
    {
        public static DiogenesSharedContext Singleton = new DiogenesSharedContext();

        public CaesarContainer PrimaryContainer = null;
        public BindingList<RawComParam> PreConnectParameters = new BindingList<RawComParam>();
        public CaesarConnection.Connection Connection = null;
        public CaesarConnection.Protocol.BaseProtocol Channel = null;
        public ECUVariant PrimaryVariant = null;

        private int UserInitiatedRequestActive = 0;

        public class RawComParam : INotifyPropertyChanged
        {
            string m_Name;
            int m_Value;
            public string Name { get { return m_Name; } set { m_Name = value; NotifyPropertyChanged("Name"); } } 
            public int Value { get { return m_Value; } set { m_Value = value; NotifyPropertyChanged("Value"); } }

            public event PropertyChangedEventHandler PropertyChanged;

            private void NotifyPropertyChanged(string p)
            {
                if (PropertyChanged != null) 
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(p));
                }
            }
        }

        public ECU PrimaryEcu
        {
            get
            {
                // haven't seen any CBFs with more than one ecu
                if (PrimaryContainer is null) 
                {
                    return null;
                }
                return PrimaryContainer.CaesarECUs[0];
            }
        }

        public void TryLoadCBF(string fileName)
        {
            byte[] fileBytes = File.ReadAllBytes(fileName);
            if (CaesarContainer.VerifyChecksum(fileBytes, out uint checksum))
            {
                PrimaryContainer = new CaesarContainer(fileBytes);
            }
            else
            {
                Console.WriteLine($"File {Path.GetFileName(fileName)} was not loaded as the checksum is invalid");
            }
        }

        public bool StartUserInitiatedRequest() 
        {
            return Interlocked.CompareExchange(ref UserInitiatedRequestActive, 1, 0) == 0;
        }

        public void EndUserInitiatedRequest() 
        {
            Interlocked.Decrement(ref UserInitiatedRequestActive);
        }
    }
}
