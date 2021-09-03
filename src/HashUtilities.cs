// Cyotek MD5 Utility
// https://github.com/cyotek/Md5

// Copyright (c) 2021 Cyotek Ltd.

// This work is licensed under the MIT License.
// See LICENSE.TXT for the full text

// Found this code useful?
// https://www.cyotek.com/contribute

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Cyotek.Tools.SimpleMD5
{
  internal static class HashUtilities
  {
    #region Private Fields

    [ThreadStatic]
    private static StringBuilder _stringBuilder;

    #endregion Private Fields

    #region Public Methods

    public static string GetHashString(byte[] data)
    {
      if (_stringBuilder == null)
      {
        _stringBuilder = new StringBuilder(data.Length * 2);
      }
      else
      {
        _stringBuilder.Length = 0;
      }

      foreach (byte value in data)
      {
        _stringBuilder.Append(value.ToString("x2"));
      }

      return _stringBuilder.ToString();
    }

    public static byte[] GetMd5Hash(string fileName)
    {
      using (HashAlgorithm hash = MD5.Create())
      {
        return HashUtilities.GetHash(hash, fileName);
      }
    }

    #endregion Public Methods

    #region Private Methods

    private static byte[] GetHash(HashAlgorithm algorithm, string fileName)
    {
      byte[] result;

      using (Stream stream = File.OpenRead(fileName))
      {
        result = algorithm.ComputeHash(stream);
      }

      return result;
    }

    #endregion Private Methods
  }
}