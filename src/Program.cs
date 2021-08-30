// Cyotek MD5 Utility
// https://github.com/cyotek/Md5

// Copyright (c) 2015-2021 Cyotek Ltd.

// This work is licensed under the MIT License.
// See LICENSE.TXT for the full text

// Found this code useful?
// https://www.cyotek.com/contribute

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Cyotek.Tools.SimpleMD5
{
  internal sealed class Program
  {
    #region Private Fields

    private bool _recursive;

    private bool _verifyHash;

    private bool _writeHash;

    #endregion Private Fields

    #region Private Methods

    private static int Main(string[] args)
    {
      return new Program().Run(args);
    }

    private IEnumerable<string> ExpandFileNames(string path, string mask)
    {
      if (mask.IndexOf('?') != -1 || mask.IndexOf('*') != -1)
      {
        Queue<string> pending;
        WildcardPatternMatcher matcher;

        pending = new Queue<string>();
        pending.Enqueue(path);

        matcher = new WildcardPatternMatcher();

        do
        {
          string folder;
          folder = pending.Dequeue();

          foreach (string file in Directory.GetFiles(folder, "*"))
          {
            if (matcher.IsMatch(file, mask, true))
            {
              yield return file;
            }
          }

          if (_recursive)
          {
            foreach (string child in Directory.GetDirectories(folder))
            {
              pending.Enqueue(child);
            }
          }
        } while (pending.Count > 0);
      }
      else
      {
        yield return Path.Combine(path, mask);
      }
    }

    private byte[] GetHash(HashAlgorithm algorithm, string fileName)
    {
      byte[] result;

      using (Stream stream = File.OpenRead(fileName))
      {
        result = algorithm.ComputeHash(stream);
      }

      return result;
    }

    private string GetHashString(byte[] data)
    {
      StringBuilder sb;

      sb = new StringBuilder(data.Length * 2);

      foreach (byte value in data)
      {
        sb.Append(value.ToString("x2"));
      }

      return sb.ToString();
    }

    private string GetMd5FileName(string fileName)
    {
      return fileName + ".md5";
    }

    private byte[] GetMd5Hash(string fileName)
    {
      using (HashAlgorithm hash = MD5.Create())
      {
        return this.GetHash(hash, fileName);
      }
    }

    private bool HasFileNameArguments(string[] args)
    {
      bool result;

      result = false;

      for (int i = 0; i < args.Length; i++)
      {
        if (args[i][0] != '/')
        {
          result = true;
          break;
        }
      }

      return result;
    }

    private bool IsMd5File(string fileName)
    {
      return string.Equals(".md5", Path.GetExtension(fileName), StringComparison.OrdinalIgnoreCase);
    }

    private void LoadOptions(string[] args)
    {
      for (int i = 0; i < args.Length; i++)
      {
        string arg;

        arg = args[i];

        if (arg[0] == '/')
        {
          if (string.Equals(arg, "/r", StringComparison.OrdinalIgnoreCase))
          {
            _recursive = true;
          }
          else if (string.Equals(arg, "/w", StringComparison.OrdinalIgnoreCase))
          {
            _writeHash = true;
          }
          else if (string.Equals(arg, "/v", StringComparison.OrdinalIgnoreCase))
          {
            _verifyHash = true;
          }
        }
      }
    }

    private ExitCode ProcessDirectory(string directoryName)
    {
      ExitCode exitCode;
      ExitCode checkCode;
      string[] fileNames;

      exitCode = ExitCode.Success;
      fileNames = Directory.GetFiles(directoryName);

      for (int i = 0; i < fileNames.Length; i++)
      {
        checkCode = this.ProcessFile(fileNames[i]);

        if (checkCode > exitCode)
        {
          exitCode = checkCode;
        }
      }

      if (_recursive)
      {
        string[] directoryNames;

        directoryNames = Directory.GetDirectories(directoryName);

        for (int i = 0; i < directoryNames.Length; i++)
        {
          checkCode = this.ProcessDirectory(directoryNames[i]);

          if (checkCode > exitCode)
          {
            exitCode = checkCode;
          }
        }
      }

      return exitCode;
    }

    private ExitCode ProcessFile(string fileName)
    {
      ExitCode result;

      try
      {
        result = this.ProcessFile(fileName, this.GetMd5Hash(fileName));
      }
      catch (Exception ex)
      {
        ColorEcho.EchoLine("{0c}ERROR:{#} " + ex.Message);
        result = ExitCode.Error;
      }

      return result;
    }

    private ExitCode ProcessFile(string fileName, byte[] hash)
    {
      ExitCode exitCode;
      string hashString;

      exitCode = ExitCode.Success;
      hashString = this.GetHashString(hash);

      ColorEcho.Echo("{0b}" + hashString + "{#}: " + Path.GetFileName(fileName) + "\n");

      if (!this.IsMd5File(fileName))
      {
        this.UpdateHash(fileName, hashString);
        exitCode = this.VerifyHash(fileName, hashString);
      }

      return exitCode;
    }

    private ExitCode ProcessFileNameArguments(string[] args, string path)
    {
      ExitCode result;

      result = ExitCode.Success;

      for (int i = 0; i < args.Length; i++)
      {
        string arg;

        arg = args[i];

        if (arg[0] != '/')
        {
          foreach (string fileName in this.ExpandFileNames(path, arg))
          {
            ExitCode checkCode;
            bool isFolder;
            bool isFile;

            isFile = File.Exists(fileName);
            isFolder = Directory.Exists(fileName);

            if (!isFolder && !isFile)
            {
              ColorEcho.EchoLine("{0c}File not found:{#} " + fileName);
              checkCode = ExitCode.Error;
            }
            else
            {
              try
              {
                if (isFile)
                {
                  checkCode = this.ProcessFile(fileName);
                }
                else
                {
                  checkCode = this.ProcessDirectory(fileName);
                }
              }
              catch (Exception ex)
              {
                ColorEcho.EchoLine("{0c}ERROR:{#} " + ex.Message);
                checkCode = ExitCode.Error;
              }
            }

            if (checkCode > result)
            {
              result = checkCode;
            }
          }
        }
      }

      return result;
    }

    private int Run(string[] args)
    {
      ExitCode exitCode;
      string path;

      this.LoadOptions(args);

      path = Environment.CurrentDirectory;

      exitCode = this.HasFileNameArguments(args)
        ? this.ProcessFileNameArguments(args, path)
        : this.ProcessDirectory(path);

      if (Debugger.IsAttached)
      {
        Console.WriteLine("Exit code: " + exitCode);
        Console.WriteLine("(Press any key to exit)");
        Console.ReadKey(true);
      }

      return (int)exitCode;
    }

    private void UpdateHash(string fileName, string hash)
    {
      string md5FileName;

      md5FileName = this.GetMd5FileName(fileName);

      if (_writeHash && (!File.Exists(md5FileName) || !string.Equals(hash, File.ReadAllText(md5FileName))))
      {
        File.WriteAllText(md5FileName, hash, Encoding.ASCII);
      }
    }

    private ExitCode VerifyHash(string fileName, string hash)
    {
      ExitCode result;
      string md5FileName;

      result = ExitCode.Success;
      md5FileName = this.GetMd5FileName(fileName);

      if (_verifyHash)
      {
        if (File.Exists(md5FileName))
        {
          if (!string.Equals(hash, File.ReadAllText(md5FileName)))
          {
            ColorEcho.EchoLine("{0c}VERIFY FAILED:{#} Hash mismatch");
            result = ExitCode.VerifyFailed;
          }
          else
          {
            ColorEcho.EchoLine("{0a}VERIFIED");
          }
        }
        else
        {
          ColorEcho.EchoLine("{0c}VERIFY FAILED:{#} Missing hash");
          result = ExitCode.VerifyFailed;
        }
      }

      return result;
    }

    #endregion Private Methods
  }
}