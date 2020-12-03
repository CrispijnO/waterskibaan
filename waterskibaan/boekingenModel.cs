using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace waterskibaan
{
    class boekingenModel
    {
        private List<ApiModel> boekingen;

        public List<ApiModel> Boekingen { get => boekingen; set => boekingen = value; }
    }
}
