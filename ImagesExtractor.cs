using System;
using System.IO;
using System.Text;

namespace SurvivalProjectImagePacker
{
    internal class ImagesExtractor
    {
        internal static void Extract(string filePath)
        {
            if (!File.Exists(Path.Combine(filePath, Program.INDEX_FILE)) || !File.Exists(Path.Combine(filePath, Program.PACK_FILE)))
            {
                throw new FileNotFoundException("The directory does not contain the image.idx and image.pak files");
            }

            using (var indexBuffer = new BinaryReader(new MemoryStream(File.ReadAllBytes(Path.Combine(filePath, Program.INDEX_FILE)))))
            using (var packBuffer = new BinaryReader(new MemoryStream(File.ReadAllBytes(Path.Combine(filePath, Program.PACK_FILE)))))
            {
                // Skip the first 34 bytes of the index file
                // The first 26 are the pack version name, the next 4 are the pack file size and
                // the next 4 are the index file size
                indexBuffer.ReadBytes(34);

                // Skip the first 38 bytes of the pack file
                packBuffer.ReadBytes(38);

                // Get the total number of files
                var numberOfFiles = indexBuffer.ReadInt32();
                
                // Extract each file
                for (var i = 0; i < numberOfFiles; i++)
                {
                    var fileName = ExtractFile(numberOfFiles, indexBuffer, packBuffer);
                    Console.WriteLine($"Extracted file {i}/{numberOfFiles} - {fileName}");
                }
            }
        }

        internal static string ExtractFile(int numberOfFiles, BinaryReader indexReader, BinaryReader packReader)
        {
            // The first 4 bytes of the file in the index file are the checksum of the name of the file
            indexReader.ReadInt32();

            // Read the file name
            var fileName = Encoding.ASCII.GetString(indexReader.ReadBytes(255)).TrimEnd((char)0);

            // Read the size of the file and the offset in the pak file
            var fileSize = indexReader.ReadInt32();
            var fileOffset = indexReader.ReadInt32(); // Not really used

            // Create the required directory
            Directory.CreateDirectory(fileName.Substring(0, fileName.LastIndexOf("\\")));

            // Create the file and read the content of the file from the pack file
            using (var fileStream = File.Create(fileName))
            {
                fileStream.Write(packReader.ReadBytes(fileSize), 0, fileSize);
            }

            return fileName;
        }
    }
}