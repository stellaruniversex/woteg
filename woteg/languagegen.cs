using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
// generator to generate languages
// syllable selections currently limited to CV, VC, CVC and VCC
// generates meaningless, somewhat pronounceable words
namespace politicsgame
{
    class languagegen
    {
        public static void langgen()
        {
            Random random = new Random();
            char[] consonants = { 'b', 'c', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'm', 'n', 'p', 'q', 'r', 's', 't', 'v', 'w', 'x', 'y', 'z' };
            char[] vowels = { 'a', 'e', 'i', 'o', 'u' };
            string[] syllstruc = { "cv", "vc", "cvc"};
            //string[] syllstruc = { "cv", "vc", "cvc", "vcc" };
            int numsyllables = random.Next(2, 5);
            string word = "";
            string word2 = "";
            for (int i = 0; i < numsyllables; i++)
            {
                word += syllstruc[random.Next(0, 3)];
            }
            char[] newword = new char[word.Length];
            for (int i = 0; i < word.Length; i++)
            {
                newword[i] = word[i];
            }
            for (int i = 0; i < word.Length; i++)
            {
                if (newword[i] == 'c') newword[i] = consonants[random.Next(0, 21)];
                if (newword[i] == 'v') newword[i] = vowels[random.Next(0, 5)];
            }
            newword[0] = char.ToUpper(newword[0]);
            for (int i = 0; i < word.Length; i++)
            {
                word2 += newword[i];
            }
            Console.WriteLine(word2);
        }
    }
}
