# Survival-Project-Images-Reader-Writer
**A console interface that allows the user to unpack the PAK file of the game Survival Project and pack it back**


Survival project's images are packed using two files - image.idx and image.pak.

image.pak contains the data itself while image.idx contains pointers to the data in the pak file.

# Usage
-e PATH to extract the PAK file. PATH must contain image.idx and image.pak
-p PATH to pack into a PAK file. PATH needs to be the extracted images folder

# image.idx file format

### Header
26 bytes - The packer version

4 bytes - The PAK file size in bytes

4 bytes - The IDX file size in bytes

4 bytes - Total number of files

Then a list of files follows.

### Each file has the following fields
4 bytes - Checksum of the file name (below).

255 bytes - File name

4 bytes - File size

4 bytes - File offset in the PAK file

# image.pak file format

38 bytes - not used

Then a list of files follows. Each file is simply the binary data of the file (the size and offset of each file appears in the image.idx file).

# Checksum calculation
In order to repack the image.idx file, one needs to calculate the correct checksum of each file. The checkname is calculated using the file name as follows:
```
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
```
