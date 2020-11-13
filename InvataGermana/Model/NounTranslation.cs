using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvataGermana.Model
{
    class NounTranslation
    {
        public int ID { get; set; }

        public int NounID { get; set; }
        public Noun Noun { get; set; }

        public string Translation { get; set; }

    }
}
