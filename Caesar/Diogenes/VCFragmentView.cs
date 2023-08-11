using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caesar;

namespace Diogenes
{
    public class VCFragmentView
    {
        private VCFragment Fragment { get; set; } = null;

        public string Name
        { 
            get 
            {
                return Fragment.ParentECU.Language.GetString(Fragment.Name_CTF);
            } 
        }

        public VCFragmentView(VCDomain domain, VCFragment fragment) 
        {
            Fragment = fragment;
        }
    }
}
