using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvataGermana.Model
{
    class Lesson
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public List<Noun> Nouns { get; }

        public override string ToString()
        {
            return Title;
        }
    }
}
