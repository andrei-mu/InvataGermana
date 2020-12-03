using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media;

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
            Das,
            None
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

                if (IsVerb)
                    return VerbCaption;

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

                if (Plural == "-")
                {
                    return $"{gen} {German}; <n/a> = [{Translation}]";
                }
                return $"{gen} {German}; Die {Plural} = [{Translation}]";
            }
        }

        public string VerbCaption
        {
            get
            {
                if (string.IsNullOrEmpty(Translation))
                {
                    if (Plural == "-")
                        return $"{German}; <n/a>";

                    return $"{German}; {Plural}";
                }

                if (Plural == "-")
                {
                    return $"{German}; <n/a> = [{Translation}]";
                }
                return $"{German}; {Plural} = [{Translation}]";
            }
        }


        public string NormalizedTranslation
        {
            get
            {
                return NormalizeString(Translation);
            }
        }

        public Brush ItemColor
        {
            get
            {
                if (IsNoun)
                {
                    return new SolidColorBrush(Colors.ForestGreen);
                }
                if (IsVerb)
                {
                    return new SolidColorBrush(Colors.Navy);
                }

                return new SolidColorBrush(Colors.Maroon);
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

        public bool IsVerb
        {
            get
            {
                return SpeechType == SpeechPart.Verb;
            }
        }

        private static string NormalizeString(string input)
        {
            StringBuilder sb = new StringBuilder();

            foreach (char c in input)
            {
                char dia;
                if (diacritics.TryGetValue(c, out dia))
                    sb.Append(dia);
                else
                    sb.Append(c);
            }

            return sb.ToString();
        }

        private static Dictionary<char, char> diacritics = new Dictionary<char, char> {
            { 'ă', 'a'},
            { 'Ă', 'A'},
            { 'ș', 's'},
            { 'ț', 't'},
            { 'â', 'a'},
            { 'î', 'i'},
        };
    }
}
