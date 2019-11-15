using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

namespace Laba
{
    class Laba1
    {
        public string input;
        public static string filePathOutFinal = @"D:/My/Универ-МИРЭА/7 семестр/Защита ОП/laba1/textFileOut.txt";
        public List<KeyValuePair<char, int>> charTextSorted = new List<KeyValuePair<char, int>>();
        public string encryptedString;
        public List<KeyValuePair<char, int>> charEncryptedSorted = new List<KeyValuePair<char, int>>();
        public string decryptedString;

        //Парсер для шифрования и дешифрования текста
        public void Parse(string filePath)
        {
            input = System.IO.File.ReadAllText(filePath);

            charTextSorted = CharFreqSorter(input);
            encryptedString = Encrypter(input,3);
            charEncryptedSorted = CharFreqSorter(encryptedString);
            decryptedString = Decrypter(encryptedString, charTextSorted, charEncryptedSorted);

            StreamWriter sr1 = new StreamWriter(filePathOutFinal, false);
            sr1.Write(decryptedString);
            sr1.Close();

            //Console.WriteLine("============= Input text ============= \n");
            //Console.WriteLine(input + "\n");
            //Console.WriteLine("============= Encrypted text ============= \n");
            //Console.WriteLine(encryptedString + "\n");
            //Console.WriteLine("============= Final text ============= \n");
            //Console.WriteLine(decryptedString + "\n");

            Console.WriteLine(BiEncrypter(input));
        }

        public List<KeyValuePair<char,int>> CharFreqSorter(string text)
        {
            Dictionary<char, int> charFrequency = new Dictionary<char, int>();
            List<KeyValuePair<char, int>> charFrequencySorted = new List<KeyValuePair<char, int>>();

            foreach (var ch in text)
            {
                if ((int)ch < 1040 || (int)ch > 1103)
                    continue;
                //var lowerCh = Char.ToLower(ch);
                if (charFrequency.ContainsKey(ch))
                {
                    charFrequency[ch]++;
                }
                else
                {
                    charFrequency.Add(ch, 1);
                }
            }

            foreach (var pair in charFrequency)
            {
                charFrequencySorted.Add(pair);
            }

            charFrequencySorted.Sort((pair1, pair2) => pair2.Value.CompareTo(pair1.Value));

            return charFrequencySorted;
        }

        public string Encrypter(string input, int shift)
        {
            var encryptedString = "";

            for (int i = 0; i < input.Length; i++)
            {
                //Проверка на кириллицу
                if (((int)(input[i]) < 1040) || ((int)(input[i]) > 1103))
                    encryptedString += input[i];
                //Строчные буквы
                if ((Convert.ToInt16(input[i]) >= 1072) && (Convert.ToInt16(input[i]) <= 1103))
                {
                    //Если буква, после сдвига выходит за пределы алфавита
                    if (Convert.ToInt16(input[i]) + shift > 1103)
                        encryptedString += Convert.ToChar(Convert.ToInt16(input[i]) + shift - 32);
                    //Если буква может быть сдвинута в пределах алфавита
                    else
                        encryptedString += Convert.ToChar(Convert.ToInt16(input[i]) + shift);
                }
                //Прописные буквы
                if ((Convert.ToInt16(input[i]) >= 1040) && (Convert.ToInt16(input[i]) <= 1071))
                {
                    //Если буква, после сдвига выходит за пределы алфавита
                    if (Convert.ToInt16(input[i]) + shift > 1071)
                        encryptedString += Convert.ToChar(Convert.ToInt16(input[i]) + shift - 32);
                    //Если буква может быть сдвинута в пределах алфавита
                    else
                        encryptedString += Convert.ToChar(Convert.ToInt16(input[i]) + shift);
                }
            }
            return encryptedString;
        }

        public string Decrypter(string input, List<KeyValuePair<char, int>> originalCharFreq, List<KeyValuePair<char, int>> encryptedCharFreq)
        {
            string decryptedString = "";
            List<KeyValuePair<int, char>> _originalCharFreq = new List<KeyValuePair<int, char>>();
            List<KeyValuePair<int, char>> _encryptedCharFreq = new List<KeyValuePair<int, char>>();

            var i = 0;
            foreach (var el in originalCharFreq)
            {
                var newEl = el.Key;
                KeyValuePair<int, char> newElDict = new KeyValuePair<int, char>(i++, newEl);
                _originalCharFreq.Add(newElDict);
            }

            i = 0;
            foreach (var el in encryptedCharFreq)
            {
                var newEl = el.Key;
                KeyValuePair<int, char> newElDict = new KeyValuePair<int, char>(i++, newEl);
                _encryptedCharFreq.Add(newElDict);
            }

            for (int s = 0; s < input.Length; s++)
            {
                //Проверка на кириллицу
                if (((int)(input[s]) < 1040) || ((int)(input[s]) > 1103))
                    decryptedString += input[s];
                //Подстановка в шифровынный текст букв на основе количества повторений в оригинале
                if ((Convert.ToInt16(input[s]) >= 1040) && (Convert.ToInt16(input[s]) <= 1103))
                {
                    var sChar = Convert.ToChar(input[s]);
                    var eKey = _encryptedCharFreq.SingleOrDefault(x => x.Value == sChar).Key;
                    var oChar = _originalCharFreq.SingleOrDefault(x => x.Key == eKey).Value;
                    decryptedString += oChar;
                }
            }
            return decryptedString;
        }

        public string BiEncrypter(string input)
        {
            var encryptedString = "";
            Dictionary<int,string> dict = new Dictionary<int, string>();
            input = input.ToLower();

            for (int i = 0; i < input.Length; i++)
            {
                //Проверка на кириллицу
                if (((int)(input[i]) < 1040) || ((int)(input[i]) > 1103))
                    encryptedString += input[i];
                //Строчные буквы
                if ((Convert.ToInt16(input[i]) >= 1072) && (Convert.ToInt16(input[i]) <= 1103))
                {
                    var a = input[i];
                    if ((i+1) >= input.Length)
                    {
                        
                    }
                    var b = input[i+1];
                    if ()
                    {

                    }
                }
            }

            return encryptedString;
        }
    }
}
