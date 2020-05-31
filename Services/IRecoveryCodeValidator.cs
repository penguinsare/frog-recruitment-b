using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace crm.Services
{
    public interface IRecoveryCodeValidator
    {
        bool RecoveryCodeValidAndUnused(string code);
    }
}
