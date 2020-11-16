﻿using System.ComponentModel.DataAnnotations.Schema;
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
        public string Translation { get; set; }

        public int LessonId { get; set; }
        public virtual Lesson Lesson { get; set; }

        [NotMapped]
        public string ListCaption
        {
            get
            {
                var gen = Gen.ToString();

                if (string.IsNullOrEmpty(Translation))
                {
                    return $"{gen} {Singular}; Die {Plural}";
                }

                return $"{gen} {Singular}; Die {Plural} = [{Translation}]";
            }
        }

        [NotMapped]
        public string SingularCaption
        {
            get
            {
                return $"{Gen.ToString()} {Singular}";
            }
        }

    }
}
