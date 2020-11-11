using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvataGermana.Model
{
    class Noun
    {
        public enum Gender
        {
            Der,
            Die,
            Das
        };

        public int Id { get; set; }
        public Gender Gen { get; set; }
        public string Singular { get; set; }
        public string Plural { get; set; }

        public Lesson ParentLesson { get; set; }
    }
}
