using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs_client.Utils
{
    public class Environment
    {
        public virtual string GetEntropy()
        {
            string entropy = System.Environment.GetEnvironmentVariable(Constants.EnvEntropy);
            return entropy == null || entropy.Length < 16 ? Constants.DefaultEntropy : entropy;
        }
    }
}
