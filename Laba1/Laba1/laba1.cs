using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

namespace Laba
{
    class Laba1
    {
        public string inputText;
        public static string filePathOutFinalCaesar = @"D:/My/Универ-МИРЭА/7 семестр/Защита ОП/laba1/textFileOutCaesar.txt";
        public static string filePathOutFinalBigramm = @"D:/My/Универ-МИРЭА/7 семестр/Защита ОП/laba1/textFileOutBigramm.txt";
        public List<KeyValuePair<char, int>> charTextSorted = new List<KeyValuePair<char, int>>();
        public string encryptedString;
        public List<KeyValuePair<char, int>> charEncryptedSorted = new List<KeyValuePair<char, int>>();
        public string decryptedStringCaesar;
        public Dictionary<int,string> bigramFreqSortedText = new Dictionary<int, string>();
        public Dictionary<int, string> bigramFreqSortedEncrypted = new Dictionary<int, string>();
        public string decryptedStringBigram;

        //Парсер для шифрования и дешифрования текста
        public void Parse(string filePath)
        {
            inputText = System.IO.File.ReadAllText(filePath);
            inputText = inputText.ToLower(); //для упрощения будем работать только со строчными буквами

            charTextSorted = CharFreqSorter(inputText);
            encryptedString = Encrypter(inputText,3);
            charEncryptedSorted = CharFreqSorter(encryptedString);
            decryptedStringCaesar = Decrypter(encryptedString, charTextSorted, charEncryptedSorted);

            bigramFreqSortedText = BigramFreqSorter(inputText);
            bigramFreqSortedEncrypted = BigramFreqSorter(encryptedString);
            decryptedStringBigram = DecrypterBigram(encryptedString, bigramFreqSortedText, bigramFreqSortedEncrypted);

            StreamWriter sr1 = new StreamWriter(filePathOutFinalCaesar, false);
            sr1.Write(decryptedStringCaesar);
            sr1.Close();

            StreamWriter sr2 = new StreamWriter(filePathOutFinalBigramm, false);
            sr2.Write(decryptedStringBigram);
            sr2.Close();

            Console.WriteLine("============= Input text ============= \n");
            Console.WriteLine(inputText + "\n");
            Console.WriteLine("============= Encrypted text ============= \n");
            Console.WriteLine(encryptedString + "\n");
            Console.WriteLine("============= Final text ============= \n");
            Console.WriteLine(decryptedStringCaesar + "\n");
        }

        //Подсчет кол-ва каждого из символов в тексте
        public List<KeyValuePair<char,int>> CharFreqSorter(string text)
        {
            Dictionary<char, int> charFrequency = new Dictionary<char, int>();
            List<KeyValuePair<char, int>> charFrequencySorted = new List<KeyValuePair<char, int>>();

            foreach (var ch in text)
            {
                if ((int)ch < 1040 || (int)ch > 1103)
                    continue;
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

        //Шифр Цезаря
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
            }
            return encryptedString;
        }

        //Расшифровка на основании частоты повторения букв в изначальном и зашифрованом текстах
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

        //Подсчет биграмм в тексте
        public Dictionary<int, string> BigramFreqSorter(string input)
        {
            var dict = new Dictionary<int, string>();
            var index = 0;
            char a = ' ';
            char b = ' ';

            for (int i = 0; i < input.Length; i++)
            {
                //Строчные буквы
                if ((Convert.ToInt16(input[i]) >= 1072) && (Convert.ToInt16(input[i]) <= 1103))
                {
                    if (((i+1) < input.Length))
                    {
                        if ((Convert.ToInt16(input[i + 1]) >= 1072) && (Convert.ToInt16(input[i + 1]) <= 1103))
                        {
                            a = input[i];
                            b = input[i + 1];
                        }
                        var c = String.Concat(a, b);
                        if (!dict.ContainsValue(c))
                        {
                            dict[index++] = c;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return dict;
        }

        //Расшифровка для биграмм
        private string DecrypterBigram(string input, Dictionary<int, string> bigramFreqSortedText, Dictionary<int, string> bigramFreqSortedEncrypted)
        {
            string decryptedString = "";
            List<KeyValuePair<int, string>> _bigramFreqSortedText = new List<KeyValuePair<int, string>>();
            List<KeyValuePair<int, string>> _bigramFreqSortedEncrypted = new List<KeyValuePair<int, string>>();
            char a = ' ';
            char b = ' ';

            var i = 0;
            foreach (var el in bigramFreqSortedText)
            {
                var newEl = el.Value;
                KeyValuePair<int, string> newElDict = new KeyValuePair<int, string>(i++, newEl);
                _bigramFreqSortedText.Add(newElDict);
            }

            i = 0;
            foreach (var el in bigramFreqSortedEncrypted)
            {
                var newEl = el.Value;
                KeyValuePair<int, string> newElDict = new KeyValuePair<int, string>(i++, newEl);
                _bigramFreqSortedEncrypted.Add(newElDict);
            }

            for (int s = 0; s < input.Length; s++)
            {
                //Проверка на кириллицу
                if (((int)(input[s]) < 1040) || ((int)(input[s]) > 1103))
                    decryptedString += input[s];
                //Подстановка в шифровынный текст букв на основе количества повторений в оригинале
                if ((Convert.ToInt16(input[s]) >= 1040) && (Convert.ToInt16(input[s]) <= 1103))
                {
                    if (((s + 1) < input.Length))
                    {
                        if ((Convert.ToInt16(input[s + 1]) >= 1072) && (Convert.ToInt16(input[s + 1]) <= 1103))
                        {
                            a = input[s];
                            b = input[s + 1];
                            var sBi = String.Concat(a, b);

                            var eKey = _bigramFreqSortedEncrypted.SingleOrDefault(x => x.Value == sBi).Key;
                            var oBi = _bigramFreqSortedText.SingleOrDefault(x => x.Key == eKey).Value;
                            decryptedString += oBi;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return decryptedString;
        }
    }
}