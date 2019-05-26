using System;

namespace Utilities
{
    class Program
    {
        static void Main(string[] args)
        {
            var result = XlsxReader.ReadXlsx("..\\..\\..\\..\\Resources\\weatherAUS.xlsx");
        }
    }
}
