// Cyotek MD5 Utility
// https://github.com/cyotek/Md5

// Copyright (c) 2021-2023 Cyotek Ltd.

// This work is licensed under the MIT License.
// See LICENSE.TXT for the full text

// Found this code useful?
// https://www.cyotek.com/contribute

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace Cyotek.Tools.SimpleMD5
{
  internal sealed class Options
  {
    #region Private Fields

    private readonly List<string> _errors;

    private readonly bool _errorsOnly;

    private readonly List<string> _exclusions;

    private readonly List<string> _files;

    private readonly bool _fullPaths;

    private readonly string _hashPath;

    private readonly bool _newFilesOnly;

    private readonly bool _recursive;

    private readonly bool _verifyHash;

    private readonly bool _writeHash;

    #endregion Private Fields

    #region Public Constructors

    public Options(string[] args)
    {
      bool pathSwitchActive;
      bool valueSwitchActive;

      _files = new List<string>();
      _errors = new List<string>();
      _exclusions = new List<string>();

      pathSwitchActive = false;
      valueSwitchActive = false;

      for (int i = 0; i < args.Length; i++)
      {
        string arg;

        arg = args[i];

        if (!string.IsNullOrEmpty(arg))
        {
          if (pathSwitchActive)
          {
            pathSwitchActive = false;

            _hashPath = Path.Combine(Environment.CurrentDirectory, arg);
          }
          else if (valueSwitchActive)
          {
            valueSwitchActive = false;

            _exclusions.Add(arg);
          }
          else if (Options.IsSwitch(arg, out string name))
          {
            if (string.Equals(name, "r", StringComparison.OrdinalIgnoreCase))
            {
              _recursive = true;
            }
            else if (string.Equals(name, "v", StringComparison.OrdinalIgnoreCase))
            {
              pathSwitchActive = true;
              _verifyHash = true;
            }
            else if (string.Equals(name, "f", StringComparison.OrdinalIgnoreCase))
            {
              _fullPaths = true;
            }
            else if (string.Equals(name, "e", StringComparison.OrdinalIgnoreCase))
            {
              _errorsOnly = true;
            }
            else if (string.Equals(name, "w", StringComparison.OrdinalIgnoreCase))
            {
              pathSwitchActive = true;
              _writeHash = true;
            }
            else if (string.Equals(name, "n", StringComparison.OrdinalIgnoreCase))
            {
              _newFilesOnly = true;
            }
            else if (string.Equals(name, "x", StringComparison.OrdinalIgnoreCase))
            {
              valueSwitchActive = true;
            }
            else
            {
              _errors.Add(string.Format("Switch '{0}' not recognised.", name));
            }
          }
          else
          {
            _files.Add(arg);
          }
        }
      }
    }

    #endregion Public Constructors

    #region Public Properties

    public ReadOnlyCollection<string> Errors => _errors.AsReadOnly();

    public bool ErrorsOnly => _errorsOnly;

    public ReadOnlyCollection<string> Exclusions => _exclusions.AsReadOnly();

    public ReadOnlyCollection<string> Files => _files.AsReadOnly();

    public bool FullPaths => _fullPaths;

    public bool Generate => _writeHash;

    public string HashPath => _hashPath;

    public bool NewFilesOnly => _newFilesOnly;

    public bool Recursive => _recursive;

    public bool Verify => _verifyHash;

    #endregion Public Properties

    #region Private Methods

    private static bool IsSwitch(string arg, out string name)
    {
      bool isSwitch;

      isSwitch = arg[0] == '/' || arg[0] == '-';
      name = isSwitch
        ? arg.Substring(1)
        : null;

      return isSwitch;
    }

    #endregion Private Methods
  }
}