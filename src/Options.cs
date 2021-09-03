// Cyotek MD5 Utility
// https://github.com/cyotek/Md5

// Copyright (c) 2021 Cyotek Ltd.

// This work is licensed under the MIT License.
// See LICENSE.TXT for the full text

// Found this code useful?
// https://www.cyotek.com/contribute

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Cyotek.Tools.SimpleMD5
{
  internal sealed class Options
  {
    #region Private Fields

    private readonly List<string> _files;
    private readonly List<string> _errors;

    private readonly string _hashPath;

    private readonly bool _recursive;

    private readonly bool _verifyHash;

    private readonly bool _writeHash;

    #endregion Private Fields

    #region Public Constructors

    public Options(string[] args)
    {
      bool pathSwitchActive;

      _files = new List<string>();
      _errors = new List<string>();
      pathSwitchActive = false;

      for (int i = 0; i < args.Length; i++)
      {
        string arg;

        arg = args[i];

        if (!string.IsNullOrEmpty(arg))
        {
          if (this.IsSwitch(arg, out string name))
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
            else if (string.Equals(name, "w", StringComparison.OrdinalIgnoreCase))
            {
              pathSwitchActive = true;
              _writeHash = true;
            }
            else
            {
              _errors.Add(string.Format("Switch '{0}' not recognised.", name));
            }
          }
          else if (pathSwitchActive)
          {
            pathSwitchActive = false;

            _hashPath = arg;
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

    public ReadOnlyCollection<string> Files => _files.AsReadOnly();
    public ReadOnlyCollection<string> Errors => _errors.AsReadOnly();

    public bool Generate => _writeHash;

    public string HashPath => _hashPath;

    public bool Recursive => _recursive;

    public bool Verify => _verifyHash;

    #endregion Public Properties

    #region Private Methods

    private bool IsSwitch(string arg, out string name)
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