using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using UnityEngine;

public static class Zip
{

    public static string GetCompressionFullPathName(string fileName) => $"{fileName}.gz";
    public static string GetCompressionFileName(string fileName) => $"{Path.GetFileName(fileName)}.gz";

    public static void Compress(string file, Action callback)
    {
        var task = InternalCompress(file);
        Core.Mono.WaitUntil(1, () => task.IsCompleted, callback);
    }

    private static async Task InternalCompress(string file)
    {
        FileInfo fileToCompress = new FileInfo(file);
        using (FileStream originalFileStream = fileToCompress.OpenRead())
        {
            if ((File.GetAttributes(fileToCompress.FullName) &
               FileAttributes.Hidden) != FileAttributes.Hidden & fileToCompress.Extension != ".gz")
            {
                using (FileStream compressedFileStream = File.Create(fileToCompress.FullName + ".gz"))
                {
                    using (GZipStream compressionStream = new GZipStream(compressedFileStream, CompressionMode.Compress))
                    {
                        await originalFileStream.CopyToAsync(compressionStream);
                    }
                }
            }
        }
    }


    public static void  Decompress(string file, Action callback)
    {
        var task = InternalDecompress(file);
        Core.Mono.WaitUntil(1, () => task.IsCompleted, callback);
    }

    private static async Task InternalDecompress(string file)
    {
        FileInfo fileToDecompress = new FileInfo(file);
        using (FileStream originalFileStream = fileToDecompress.OpenRead())
        {
            string currentFileName = fileToDecompress.FullName;
            string newFileName = currentFileName.Remove(currentFileName.Length - fileToDecompress.Extension.Length);

            using (FileStream decompressedFileStream = File.Create(newFileName))
            {
                using (GZipStream decompressionStream = new GZipStream(originalFileStream, CompressionMode.Decompress))
                {
                    await decompressionStream.CopyToAsync(decompressedFileStream);
                }
            }
        }
    }

    public static void WriteZipFile(Byte[] bytes, string filename, Action callback)
    {
        var task = InternalWriteZipFile(bytes, filename);
        Core.Mono.WaitUntil(1, () => task.IsCompleted, callback);
    }

    private static async Task InternalWriteZipFile(Byte[] bytes, string filename )
    {
        using (var stream = new FileStream( filename, FileMode.Create, FileAccess.Write, FileShare.Write, 4096, useAsync: true))
        {
            await stream.WriteAsync(bytes, 0, bytes.Length);
        }
    }

}
