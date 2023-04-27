using Caesar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diogenes
{
    public class DiagServiceHost : CaesarInterpreter.Host.IDiagService
    {
        public CaesarInterpreter.Host.IDiagServiceRunner CreateFromName(string name) 
        {
            DiagService ds = null;
            // try to get the first diagservice within the variant with a matching name
            if (DiogenesSharedContext.Singleton.PrimaryVariant != null) 
            {
                ds = DiogenesSharedContext.Singleton.PrimaryVariant.DiagServices.FirstOrDefault(x => x.Qualifier == name);
            }

            if (ds is null) 
            {
                // if we don't have a variant, search the entire ecu catalog
                // this path is much less desirable but might be better than failing outright for ecus without variants?
                ds = DiogenesSharedContext.Singleton.PrimaryEcu.GlobalDiagServices.FirstOrDefault(x => x.Qualifier == name);
            }
            return new DiagServiceRunner(ds);
        }
    }
}
