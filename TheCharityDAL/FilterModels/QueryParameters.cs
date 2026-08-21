using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheCharityDAL.FilterModels
{
    public class QueryParameters
    {
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 20;
    }
}
