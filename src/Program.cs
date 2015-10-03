using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Cyotek.Tools.SimpleMD5
{
  internal class Program
  {
    #region Static Methods

    private static byte[] GetHash(HashAlgorithm algorithm, string fileName)
    {
      byte[] result;

      using (Stream stream = File.OpenRead(fileName))
      {
        result = algorithm.ComputeHash(stream);
      }

      return result;
    }

    private static string GetHashString(byte[] data)
    {
      StringBuilder sb;

      sb = new StringBuilder(data.Length * 2);

      foreach (byte value in data)
      {
        sb.Append(value.ToString("x2"));
      }

      return sb.ToString();
    }

    private static byte[] GetMd5Hash(string fileName)
    {
      using (HashAlgorithm hash = MD5.Create())
      {
        return GetHash(hash, fileName);
      }
    }

    private static int Main(string[] args)
    {
      int exitCode;

      exitCode = 1; // Assume fail by default

      if (args.Length == 0)
      {
        Console.WriteLine("No file specified.");
      }
      else
      {
        string fileName;

        fileName = Path.Combine(Environment.CurrentDirectory, args[0]);

        if (!File.Exists(fileName))
        {
          Console.WriteLine("File not found.");
        }
        else
        {
          try
          {
            byte[] hash;

            hash = GetMd5Hash(fileName);

            WriteHash(fileName, hash);

            exitCode = 0;
          }
          catch (Exception ex)
          {
            Console.WriteLine(ex.GetBaseException().
                                 Message);
          }
        }
      }

      if (Debugger.IsAttached)
      {
        Console.WriteLine("(Press any key to exit)");
        Console.ReadKey(true);
      }

      return exitCode;
    }

    private static void WriteHash(string fileName, byte[] hash)
    {
      Console.WriteLine($"{Path.GetFileName(fileName)}: {GetHashString(hash)}");
    }

    #endregion
  }
}
