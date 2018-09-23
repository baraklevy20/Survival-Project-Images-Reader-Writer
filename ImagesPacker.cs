using System;
using System.IO;
using System.Linq;
using System.Text;

namespace SurvivalProjectImagePacker
{
    internal class ImagesPacker
    {
        // The version name length must be <= 26
        const string VersionName = "Barak lol";
        private static int CurrentFile { get; set; }
        private static int TotalFiles { get; set; }

        internal static void Pack(string filePath)
        {
            if (!Directory.Exists(filePath))
            {
                throw new FileNotFoundException("The given path must be a directory");
            }

            // Create the index and pack files
            using (var indexStream = new BinaryWriter(File.Create(Program.INDEX_FILE)))
            using (var packStream = new BinaryWriter(File.Create(Program.PACK_FILE)))
            {
                // Add 38 0's to the pack file
                packStream.Write(new byte[38]);

                // Write the name of the version
                indexStream.Write(Encoding.ASCII.GetBytes(VersionName));
                indexStream.Write(new byte[26 - VersionName.Length]); // The name must be 26 characters long

                // Write the size of the pack file
                indexStream.Write(0);

                // Write the size of the index file
                indexStream.Write(0);

                // Write the number of files
                TotalFiles = Directory.EnumerateFiles(filePath, "*", SearchOption.AllDirectories).Count();
                indexStream.Write(TotalFiles);

                // Go through the images folder and recursively write every image
                PackFilesRecursively(filePath, indexStream, packStream);

                // Edit the pack and index files sizes
                indexStream.Seek(26, SeekOrigin.Begin);
                indexStream.Write((int)new FileInfo(Program.PACK_FILE).Length);
                indexStream.Write((int)new FileInfo(Program.INDEX_FILE).Length);
            }
        }

        private static void PackFilesRecursively(string filePath, BinaryWriter indexStream, BinaryWriter packStream)
        {
            // If it's a file, pack it
            if (File.Exists(filePath))
            {
                var relativeFilePath = StripAbsolutePath(filePath);
                PackFile(filePath, relativeFilePath, indexStream, packStream);
                Console.WriteLine($"Packed file {++CurrentFile}/{TotalFiles} - {relativeFilePath}");
                return;
            }

            // Otherwise, it's a directory
            foreach (var fileName in Directory.EnumerateFileSystemEntries(filePath)) {
                PackFilesRecursively(fileName, indexStream, packStream);
            }
        }

        private static string StripAbsolutePath(string filePath)
        {
            return filePath.Substring(filePath.LastIndexOf("images"));
        }

        private static void PackFile(string fileName, string relativeFilePath, BinaryWriter indexStream, BinaryWriter packStream)
        {
            // Edit the index file
            var relativeFileNameBytes = Encoding.ASCII.GetBytes(relativeFilePath);
            indexStream.Write(ComputeChecksum(relativeFilePath)); // 4 bytes, might be crc
            indexStream.Write(relativeFileNameBytes); // Relative file path
            indexStream.Write(new byte[255 - relativeFileNameBytes.Length]); // Pad the file name to 255 bytes
            indexStream.Write((int)(new FileInfo(fileName).Length)); // File size
            indexStream.Write((int)packStream.BaseStream.Position); // File offset

            // Edit the pack file
            packStream.Write(File.ReadAllBytes(fileName));
        }

        private static uint ComputeChecksum(string fileName)
        {
            uint v2 = 0;
            uint v3 = 0;
            for (int i = 0; i < fileName.Length; i++)
            {
                var v5 = fileName[i];
                v3 = (v5 | (v3 << 8)) % 0xFFFFFD;
                v2 ^= v5;
            }

            return v3 | (v2 << 24);
        }
    }
}