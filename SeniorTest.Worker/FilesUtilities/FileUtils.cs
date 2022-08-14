using System.IO.Compression;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using Aspose.Zip.SevenZip;

namespace SeniorTest.Worker.FilesUtilities;

public static class FileUtils
{
    public static Task DeleteFile(string path, string filename)
    {
        var fullPath = $"{path}{filename}";
        if (File.Exists(fullPath))
            File.Delete(fullPath);
        return Task.CompletedTask;
    }

    //This is not working properly
    public static async Task<string> CompressFileAsync(string path, string fileName, CancellationToken token)
    {
        var destinationFilename = ExtractFilenameWithoutExtension(fileName) + ".zip";
        var destinationFile = $"{path}" + ExtractFilenameWithoutExtension(fileName) + ".zip";
        var sourceFile = $"{path}{fileName}";

        if (File.Exists(destinationFile))
            return Path.GetFileName(destinationFile);

        //--------------------------------------------------------------

        if (string.IsNullOrWhiteSpace(sourceFile))
            throw new ArgumentNullException(nameof(sourceFile));

        if (string.IsNullOrWhiteSpace(destinationFile))
            throw new ArgumentNullException(nameof(destinationFile));

        FileStream streamSource = null;
        FileStream streamDestination = null;
        Stream streamCompressed = null;

        var bufferSize = 4096;
        await using (streamSource = new FileStream(sourceFile,
                         FileMode.OpenOrCreate, FileAccess.Read, FileShare.None,
                         bufferSize, useAsync: true))
        {
            await using (streamDestination = new FileStream(destinationFile,
                             FileMode.OpenOrCreate, FileAccess.Write, FileShare.None,
                             bufferSize, useAsync: true))
            {
                // read 1MB chunks and compress them
                var fileLength = streamSource.Length;

                // write out the fileLength size
                var size = BitConverter.GetBytes(fileLength);
                await streamDestination.WriteAsync(size, 0, size.Length, token);

                long chunkSize = 1048576; // 1MB
                while (fileLength > 0 || token.IsCancellationRequested)
                {
                    // read the chunk
                    var data = new byte[chunkSize];
                    await streamSource.ReadAsync(data, token);

                    // compress the chunk
                    var compressedDataStream = new MemoryStream();

                    /*CompressionMode.
                    if (compressionType == CompressionType.Deflate)
                        streamCompressed =
                            new DeflateStream(compressedDataStream,
                                CompressionMode.Compress);
                    else*/
                    streamCompressed =
                        new ZLibStream(compressedDataStream,
                            CompressionMode.Compress);

                    await using (streamCompressed)
                    {
                        // write the chunk in the compressed stream
                        await streamCompressed.WriteAsync(data, 0, data.Length, token);
                        await streamCompressed.FlushAsync(token);
                    }

                    // get the bytes for the compressed chunk
                    byte[] compressedData =
                        compressedDataStream.GetBuffer();

                    // write out the chunk size
                    size = BitConverter.GetBytes(chunkSize);
                    await streamDestination.WriteAsync(size, 0, size.Length, token);

                    // write out the compressed size
                    size = BitConverter.GetBytes(compressedData.Length);
                    await streamDestination.WriteAsync(size, 0, size.Length, token);

                    // write out the compressed chunk
                    await streamDestination.WriteAsync(compressedData, 0,
                        compressedData.Length, token);

                    await streamDestination.FlushAsync(token);

                    // subtract the chunk size from the file size
                    fileLength -= chunkSize;

                    // if chunk is less than remaining file use
                    // remaining file
                    if (fileLength < chunkSize)
                        chunkSize = fileLength;
                }
            }
        }

        return destinationFilename;
    }

    public static async Task<string> CompressTo7ZipFileAsync(string path, string fileName, CancellationToken token)
    {
        var destinationFilename = ExtractFilenameWithoutExtension(fileName) + ".7z";
        var destinationFile = $"{path}" + ExtractFilenameWithoutExtension(fileName) + ".7z";
        var sourceFile = $"{path}{fileName}";

        if (File.Exists(destinationFile))
        {
            File.Delete(destinationFile);
        }

        await using var sevenZipFile = File.Open($"{destinationFile}", FileMode.Create);
        using var archive = new SevenZipArchive();
        archive.CreateEntry($"{fileName}", $"{sourceFile}");
        archive.Save(sevenZipFile);

        return destinationFilename;
    }

    private static string ExtractFilenameWithoutExtension(string filename)
    {
        var filenameStrings = filename.Split(".");
        var result = "";

        for (var i = 0; i < filenameStrings.Length - 1; i++)
        {
            result += (i > 0) ? "." + filenameStrings[i] : filenameStrings[i];
        }

        return result;
    }


    
}