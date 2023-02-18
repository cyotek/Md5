// Cyotek MD5 Utility
// https://github.com/cyotek/Md5

// Copyright (c) 2015-2023 Cyotek Ltd.

// This work is licensed under the MIT License.
// See LICENSE.TXT for the full text

// Found this code useful?
// https://www.cyotek.com/contribute

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Cyotek.Tools.SimpleMD5
{
  internal sealed class Program
  {
    #region Private Fields

    private string _basePath;

    private Options _options;

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

        matcher = WildcardPatternMatcher.Default;

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

          if (_options.Recursive)
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

    private string GetBasePath()
    {
      string path;

      path = _options.Files.Count != 1 || !this.IsDirectory(_options.Files[0])
        ? Environment.CurrentDirectory
        : _options.Files[0];

      if (path[path.Length - 1] != Path.DirectorySeparatorChar || path[path.Length - 1] != Path.AltDirectorySeparatorChar)
      {
        path += Path.DirectorySeparatorChar;
      }

      return path;
    }

    private string GetFileNameLabel(string basePath, string fileName)
    {
      return _options.FullPaths
        ? fileName
        : this.TruncatePath(basePath, fileName);
    }

    private string GetMd5FileName(string fileName)
    {
      return string.IsNullOrEmpty(_options.HashPath)
        ? fileName + ".md5"
        : Path.Combine(_options.HashPath, fileName.Substring(_basePath.Length)) + ".md5";
    }

    private bool IsDirectory(string path)
    {
      return path.IndexOfAny(Path.GetInvalidPathChars()) == -1 && Directory.Exists(path);
    }

    private bool IsMd5File(string fileName)
    {
      return string.Equals(".md5", Path.GetExtension(fileName), StringComparison.OrdinalIgnoreCase);
    }

    private ExitCode ProcessDirectory(string basePath, string directoryName)
    {
      ExitCode exitCode;

      exitCode = ExitCode.Success;

      if (this.ShouldInclude(directoryName, true))
      {
        string[] fileNames;
        ExitCode checkCode;

        fileNames = Directory.GetFiles(directoryName);

        for (int i = 0; i < fileNames.Length; i++)
        {
          checkCode = this.ProcessFile(basePath, fileNames[i]);

          if (checkCode > exitCode)
          {
            exitCode = checkCode;
          }
        }

        if (_options.Recursive)
        {
          string[] directoryNames;

          directoryNames = Directory.GetDirectories(directoryName);

          for (int i = 0; i < directoryNames.Length; i++)
          {
            checkCode = this.ProcessDirectory(basePath, directoryNames[i]);

            if (checkCode > exitCode)
            {
              exitCode = checkCode;
            }
          }
        }
      }
      else if (!_options.ErrorsOnly)
      {
        ColorEcho.EchoLine("{0e}SKIPPED:{#} " + this.GetFileNameLabel(basePath, directoryName));
      }

      return exitCode;
    }

    private ExitCode ProcessFile(string basePath, string fileName)
    {
      ExitCode result;

      try
      {
        result = ExitCode.Success;

        if (this.ShouldInclude(fileName, false))
        {
          if (!_options.NewFilesOnly || (!this.IsMd5File(fileName) && !File.Exists(this.GetMd5FileName(fileName))))
          {
            result = this.ProcessFile(basePath, fileName, HashUtilities.GetMd5Hash(fileName));
          }
        }
        else if (!_options.ErrorsOnly)
        {
          ColorEcho.EchoLine("{0e}SKIPPED:{#} " + this.GetFileNameLabel(basePath, fileName));
        }
      }
      catch (Exception ex)
      {
        ColorEcho.EchoLine("{0c}ERROR:{#} " + ex.Message);
        result = ExitCode.Error;
      }

      return result;
    }

    private ExitCode ProcessFile(string basePath, string fileName, byte[] hash)
    {
      ExitCode exitCode;
      string hashString;

      exitCode = ExitCode.Success;
      hashString = HashUtilities.GetHashString(hash);

      if (!_options.ErrorsOnly)
      {
        ColorEcho.EchoLine("{0b}" + hashString + "{#}: " + this.GetFileNameLabel(basePath, fileName));
      }

      if (!this.IsMd5File(fileName))
      {
        this.UpdateHash(fileName, hashString);
        exitCode = this.VerifyHash(basePath, fileName, hashString);
      }

      return exitCode;
    }

    private ExitCode ProcessFileNameArguments(string basePath)
    {
      ExitCode result;

      result = ExitCode.Success;

      foreach (string arg in _options.Files)
      {
        foreach (string path in this.ExpandFileNames(basePath, arg))
        {
          ExitCode checkCode;
          bool isFolder;
          bool isFile;

          isFile = File.Exists(path);
          isFolder = Directory.Exists(path);

          if (!isFolder && !isFile)
          {
            ColorEcho.EchoLine("{0c}File not found:{#} " + path);
            checkCode = ExitCode.Error;
          }
          else
          {
            try
            {
              checkCode = isFile
                ? this.ProcessFile(basePath, path)
                : this.ProcessDirectory(basePath, path);
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

      return result;
    }

    private int Run(string[] args)
    {
      ExitCode exitCode;

      _options = new Options(args);

      _basePath = this.GetBasePath();

      if (this.ValidateOptions())
      {
        exitCode = _options.Files.Count > 0
          ? this.ProcessFileNameArguments(_basePath)
          : this.ProcessDirectory(_basePath, _basePath);
      }
      else
      {
        exitCode = ExitCode.Error;
      }

      if (Debugger.IsAttached)
      {
        Console.WriteLine("Exit code: " + exitCode);
        Console.WriteLine("(Press any key to exit)");
        Console.ReadKey(true);
      }

      return (int)exitCode;
    }

    private bool ShouldInclude(string path, bool treatAsPath)
    {
      WildcardPatternMatcher matcher;
      bool result;

      path = path.Replace(Path.DirectorySeparatorChar, '/');

      if (treatAsPath && path[path.Length - 1] != '/')
      {
        path += '/';
      }

      matcher = WildcardPatternMatcher.Default;

      result = true;

      foreach (string raw in _options.Exclusions)
      {
        string pattern;

        if (raw[0] != '*')
        {
          pattern = raw[0] == '/'
            ? "*" + raw
            : "*/" + raw;
        }
        else
        {
          pattern = raw;
        }

        if (matcher.IsMatch(path, pattern, true))
        {
          result = false;
          break;
        }
      }

      return result;
    }

    private bool ShouldWriteFileHash(string fileName, string hash)
    {
      return _options.Generate && (!File.Exists(fileName) || (!_options.NewFilesOnly && !string.Equals(hash, File.ReadAllText(fileName))));
    }

    private string TruncatePath(string basePath, string fileName)
    {
      return fileName.StartsWith(basePath, StringComparison.InvariantCultureIgnoreCase)
        ? fileName.Substring(basePath.Length)
        : Path.GetFileName(fileName);
    }

    private void UpdateHash(string fileName, string hash)
    {
      string md5FileName;

      md5FileName = this.GetMd5FileName(fileName);

      if (this.ShouldWriteFileHash(md5FileName, hash))
      {
        this.WriteFile(md5FileName, hash);
      }
    }

    private bool ValidateOptions()
    {
      bool result;

      result = false;

      if (_options.Errors.Count > 0)
      {
        foreach (string error in _options.Errors)
        {
          ColorEcho.EchoLine("{0e}ERROR{#}: " + error);
        }
      }
      else if (_options.Generate && _options.Verify)
      {
        ColorEcho.EchoLine("{0e}ERROR{#}: Cannot specify both /w and /v.");
      }
      else if (!string.IsNullOrEmpty(_options.HashPath) && !Directory.Exists(_options.HashPath))
      {
        ColorEcho.EchoLine("{0e}ERROR{#}: Specified hash location not found.");
      }
      else
      {
        result = true;
      }

      return result;
    }

    private ExitCode VerifyHash(string basePath, string fileName, string hash)
    {
      ExitCode result;
      string md5FileName;

      result = ExitCode.Success;
      md5FileName = this.GetMd5FileName(fileName);

      if (_options.Verify)
      {
        if (File.Exists(md5FileName))
        {
          if (!string.Equals(hash, File.ReadAllText(md5FileName)))
          {
            ColorEcho.EchoLine("{0c}VERIFY FAILED:{#} " + this.GetFileNameLabel(basePath, fileName) + " (Hash mismatch)");
            result = ExitCode.VerifyFailed;
          }
          else if (!_options.ErrorsOnly)
          {
            ColorEcho.EchoLine("{0a}VERIFIED");
          }
        }
        else
        {
          ColorEcho.EchoLine("{0c}VERIFY FAILED:{#} " + this.GetFileNameLabel(basePath, fileName) + " (Missing hash)");
          result = ExitCode.VerifyFailed;
        }
      }

      return result;
    }

    private void WriteFile(string fileName, string content)
    {
#if DEBUG
      Debug.WriteLine(string.Format("Writing '{0}'...", fileName));
#endif

      Directory.CreateDirectory(Path.GetDirectoryName(fileName));

      File.WriteAllText(fileName, content, Encoding.ASCII);
    }

    #endregion Private Methods
  }
}