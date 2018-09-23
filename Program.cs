using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurvivalProjectImagePacker
{
    class Program
    {
        public const string BASE_DIRECTORY = "images";
        public const string INDEX_FILE = "image.idx";
        public const string PACK_FILE = "image.pak";

        static void Main(string[] args)
        {
            args = new string[2];
            args[0] = "-e";
            args[1] = @"C:\Users\Barak\source\repos\SurvivalProjectImagePacker\SurvivalProjectImagePacker\bin\Debug";
            //args[1] = @"C:\Users\Barak\source\repos\SurvivalProjectImagePacker\SurvivalProjectImagePacker\bin\Debug\images";

            if (args.Length != 2 || (args[0] != "-e" && args[0] != "-p"))
            {
                Console.WriteLine("Invalid usage. Usage:");
                Console.WriteLine("-e PATH to extract the PAK file. PATH must contain image.idx and image.pak");
                Console.WriteLine("-p PATH to pack into a PAK file. PATH needs to be the extracted images folder");
                // Console.WriteLine("-d PATH to decompress a PGF. PATH needs to be the extracted images folder");
                // Console.WriteLine("-c PATH to compress an image into PGF. PATH needs to be the extracted images folder");
            }

            if (args[0] == "-e")
            {
                ImagesExtractor.Extract(args[1]);
            }
            else
            {
                ImagesPacker.Pack(args[1]);
            }
        }
    }
}
