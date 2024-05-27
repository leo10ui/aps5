using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.ViewModel;

public class FileHandling
{
    public static string ConvertFileToBase64(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("File not found", filePath);
        }

        byte[] fileBytes = File.ReadAllBytes(filePath);
        string base64String = Convert.ToBase64String(fileBytes);

        string extension = Path.GetExtension(filePath).ToLower();
        string fileType = GetFileType(extension);

        if (fileType == null)
        {
            throw new InvalidOperationException("Unsupported file type");
        }

        return base64String;
    }

    public static string GetFileType(string extension)
    {
        switch (extension)
        {
            case ".jpg":
            case ".jpeg":
            case ".png":
            case ".gif":
            case ".bmp":
            case ".tiff":
                return "imagemessage";
            case ".pdf":
            case ".doc":
            case ".docx":
            case ".xls":
            case ".xlsx":
            case ".ppt":
            case ".pptx":
            case ".txt":
            case ".rtf":
                return "documentmessage";
            default:
                return "message";
        }
    }

    // Function to decode base64 string back to the original file
    public static void DecodeBase64ToFile(string base64String, string outputFilePath, string extension)
    {
        if (string.IsNullOrEmpty(base64String))
        {
            throw new ArgumentException("Base64 string is null or empty", nameof(base64String));
        }

        if (string.IsNullOrEmpty(outputFilePath))
        {
            throw new ArgumentException("Output file path is null or empty", nameof(outputFilePath));
        }

        try
        {
            byte[] fileBytes = Convert.FromBase64String(base64String);
            File.WriteAllBytes(outputFilePath + "." + extension, fileBytes);
            Console.WriteLine($"File successfully saved to {outputFilePath}");
        }
        catch (FormatException ex)
        {
            Console.WriteLine($"Invalid Base64 string: {ex.Message}");
        }
        catch (IOException ex)
        {
            Console.WriteLine($"Error writing file: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error: {ex.Message}");
        }
    }
}


