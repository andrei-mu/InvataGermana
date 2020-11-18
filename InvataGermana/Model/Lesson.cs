using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvataGermana.Model
{
    class Lesson
    {
        public int ID { get; set; }
        public string Title { get; set; }

        public virtual ICollection<Word> Words{ get; set; }

        [NotMapped]
        public string ListCaption
        {
            get
            {
                return Title;
            }
        }

        public override string ToString()
        {
            return Title;
        }
    }
}
