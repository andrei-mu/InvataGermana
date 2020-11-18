using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvataGermana.Model
{
    class Word
    {
        public enum SpeechPart
        {
            Noun,
            Verb,
            Other
        };

        public enum Gender
        {
            Der,
            Die,
            Das
        };

        public int ID { get; set; }
        public SpeechPart SpeechType { get; set; }
        public Gender Gen { get; set; }
        public string German { get; set; }
        public string Plural { get; set; }
        public string Translation { get; set; }

        public int LessonId { get; set; }
        public virtual Lesson Lesson { get; set; }

        public string ListCaption
        {
            get
            {
                if (IsNoun)
                    return NounCaption;

                return GenericCaption;
            }
        }

        public string NounCaption
        {
            get
            {
                var gen = Gen.ToString();

                if (string.IsNullOrEmpty(Translation))
                {
                    return $"{gen} {German}; Die {Plural}";
                }

                return $"{gen} {German}; Die {Plural} = [{Translation}]";
            }
        }

        public string GenericCaption
        {
            get
            {
                return $"{German} = [{Translation}]";
            }
        }

        public string SingularCaption
        {
            get
            {
                return $"{Gen.ToString()} {German}";
            }
        }

        public bool IsNoun
        {
            get
            {
                return SpeechType == SpeechPart.Noun;
            }
        }

    }
}
