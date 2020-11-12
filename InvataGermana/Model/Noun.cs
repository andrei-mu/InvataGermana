using System.ComponentModel.DataAnnotations.Schema;
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

        public int ID { get; set; }
        public Gender Gen { get; set; }
        public string Singular { get; set; }
        public string Plural { get; set; }

        public int LessonID { get; set; }
        public Lesson Lesson { get; set; }

        [NotMapped]
        public string ListCaption
        {
            get
            {
                var singular = Singular.ToUpper()[0] + Singular.Substring(1);
                var plural = Plural.ToUpper()[0] + Plural.Substring(1);
                var gen = Gen.ToString();

                return $"{gen} {singular}; Die {plural}";
            }
        }
    }
}
