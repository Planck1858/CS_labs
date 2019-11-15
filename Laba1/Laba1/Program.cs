using System;

namespace Laba
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine((int)'А');
            Console.WriteLine((int)'Я');
            Console.WriteLine((int)'а');
            Console.WriteLine((int)'я');

            Laba1 p = new Laba1();
            p.Parse(@"D:/My/Универ-МИРЭА/7 семестр/Защита ОП/laba1/textFile.txt");
        }

        //static UInt64 hash(UInt32 x)
        //{
        //    UInt64 result = 0;
        //    for (int i = 0; i < 32; ++i)
        //    {
        //        result |= ((x >> i) & 0x1) << (i * 2) % 10;
        //    }
        //    return result;
        //}
    }
}
