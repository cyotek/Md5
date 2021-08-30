// Cyotek MD5 Utility
// https://github.com/cyotek/Md5

// Copyright (c) 2018-2021 Cyotek Ltd.

// This work is licensed under the MIT License.
// See LICENSE.TXT for the full text

// Found this code useful?
// https://www.cyotek.com/contribute

using System;
using System.Collections.Generic;

// IsMatch routine based on http://www.c-sharpcorner.com/uploadfile/b81385/efficient-string-matching-algorithm-with-use-of-wildcard-characters/

namespace Cyotek
{
  internal sealed class WildcardPatternMatcher
  {
    #region Public Fields

    public const char DefaultMultipleWildcard = '*';

    public const char DefaultSingleWildcard = '?';

    public static readonly WildcardPatternMatcher Default = new WildcardPatternMatcher(DefaultSingleWildcard, DefaultMultipleWildcard);

    #endregion Public Fields

    #region Private Fields

    private readonly char _multiple;

    private readonly char _single;

    private string[] _patterns;

    #endregion Private Fields

    #region Public Constructors

    public WildcardPatternMatcher()
      : this(DefaultSingleWildcard, DefaultMultipleWildcard)
    {
    }

    public WildcardPatternMatcher(char single, char multiple)
    {
      _single = single;
      _multiple = multiple;
    }

    public WildcardPatternMatcher(char single, char multiple, string pattern)
      : this(single, multiple, new[]
                               {
                                 pattern
                               })
    {
    }

    public WildcardPatternMatcher(string pattern)
      : this(new[]
             {
               pattern
             })
    {
    }

    public WildcardPatternMatcher(string[] patterns)
      : this(DefaultSingleWildcard, DefaultMultipleWildcard, patterns)
    {
    }

    public WildcardPatternMatcher(ICollection<string> patterns)
      : this(DefaultSingleWildcard, DefaultMultipleWildcard)
    {
      _patterns = new string[patterns.Count];
      patterns.CopyTo(_patterns, 0);
    }

    public WildcardPatternMatcher(char single, char multiple, string[] patterns)
      : this(single, multiple)
    {
      _patterns = patterns;
    }

    #endregion Public Constructors

    #region Public Properties

    public string[] Patterns
    {
      get => _patterns;
      set => _patterns = value;
    }

    #endregion Public Properties

    #region Public Methods

    public bool IsMatch(string input, string pattern)
    {
      return this.IsMatch(input, pattern, false, _single, _multiple);
    }

    public bool IsMatch(string input, string pattern, bool ignoreCase)
    {
      return this.IsMatch(input, pattern, ignoreCase, _single, _multiple);
    }

    public bool IsMatch(string input, string[] patterns)
    {
      return this.IsMatch(input, patterns, false);
    }

    public bool IsMatch(string input, string[] patterns, bool ignoreCase)
    {
      bool result;

      result = false;

      // ReSharper disable once ForCanBeConvertedToForeach
      for (int i = 0; i < patterns.Length; i++)
      {
        result = this.IsMatch(input, patterns[i], ignoreCase, _single, _multiple);

        if (result)
        {
          break;
        }
      }

      return result;
    }

    public bool IsMatch(string input)
    {
      return this.IsMatch(input, _patterns, false);
    }

    public bool IsMatch(string input, bool ignoreCase)
    {
      return this.IsMatch(input, _patterns, ignoreCase);
    }

    #endregion Public Methods

    #region Private Methods

    /// <summary>
    /// Tests whether specified string can be matched agains provided pattern string. Pattern may contain single- and multiple-replacing
    /// wildcard characters.
    /// </summary>
    /// <param name="input">String which is matched against the pattern.</param>
    /// <param name="pattern">Pattern against which string is matched.</param>
    /// <param name="singleWildcard">Character which can be used to replace any single character in input string.</param>
    /// <param name="multipleWildcard">Character which can be used to replace zero or more characters in input string.</param>
    /// <param name="ignoreCase"><c>true</c> if characters should be compared in a case-insensitive manner; otherwise <c>false</c>.</param>
    /// <returns>true if <paramref name="pattern"/> matches the string <paramref name="input"/>; otherwise false.</returns>
    private bool IsMatch(string input, string pattern, bool ignoreCase, char singleWildcard, char multipleWildcard)
    {
      int patternLength;
      int inputLength;
      int[] inputPosStack;
      int[] patternPosStack;
      bool[,] pointTested;
      int stackPos;
      int inputPos;
      int patternPos;
      bool matched;

      if (string.IsNullOrEmpty(input))
      {
        throw new ArgumentNullException(nameof(input));
      }

      if (string.IsNullOrEmpty(pattern))
      {
        throw new ArgumentNullException(nameof(input));
      }

      patternLength = pattern.Length;
      inputLength = input.Length;

      inputPosStack = new int[(inputLength + 1) * (patternLength + 1)]; // Stack containing input positions that should be tested for further matching
      patternPosStack = new int[inputPosStack.Length]; // Stack containing pattern positions that should be tested for further matching
      pointTested = new bool[inputLength + 1, patternLength + 1]; // Each true value indicates that input position vs. pattern position has been tested

      stackPos = -1; // Points to last occupied entry in stack; -1 indicates that stack is empty
      inputPos = 0; // Position in input matched up to the first multiple wildcard in pattern
      patternPos = 0; // Position in pattern matched up to the first multiple wildcard in pattern

      // by default, we have not made a match
      matched = false;

      // Match beginning of the string until first multiple wildcard in pattern
      while (inputPos < inputLength && patternPos < patternLength && pattern[patternPos] != multipleWildcard && (input[inputPos] == pattern[patternPos] || pattern[patternPos] == singleWildcard || (ignoreCase && char.IsLetter(input[patternPos]) && char.ToUpperInvariant(input[patternPos]) == char.ToUpperInvariant(pattern[patternPos]))))
      {
        inputPos++;
        patternPos++;
      }

      // Push this position to stack if it points to end of pattern or to a general wildcard
      if (patternPos == patternLength || pattern[patternPos] == multipleWildcard)
      {
        pointTested[inputPos, patternPos] = true;
        inputPosStack[++stackPos] = inputPos;
        patternPosStack[stackPos] = patternPos;
      }

      // Repeat matching until either string is matched against the pattern or no more parts remain on stack to test
      while (stackPos >= 0 && !matched)
      {
        inputPos = inputPosStack[stackPos]; // Pop input and pattern positions from stack
        patternPos = patternPosStack[stackPos--]; // Matching will succeed if rest of the input string matches rest of the pattern

        if (inputPos == inputLength)
        {
          if (patternPos == patternLength)
          {
            matched = true; // Reached end of both pattern and input string, hence matching is successful
          }
          else if (patternPos == patternLength - 1 && pattern[patternLength - 1] == '*')
          {
            // Reached the end of the input, and the last part of the pattern is a multiple wildcard
            matched = true;
          }
        }
        else
        {
          // First character in next pattern block is guaranteed to be multiple wildcard
          // So skip it and search for all matches in value string until next multiple wildcard character is reached in pattern
          for (int curInputStart = inputPos; curInputStart < inputLength; curInputStart++)
          {
            int curInputPos = curInputStart;
            int curPatternPos = patternPos + 1;
            if (curPatternPos == patternLength)
            {
              // Pattern ends with multiple wildcard, hence rest of the input string is matched with that character
              curInputPos = inputLength;
            }
            else
            {
              while (curInputPos < inputLength && curPatternPos < patternLength && pattern[curPatternPos] != multipleWildcard && (input[curInputPos] == pattern[curPatternPos] || pattern[curPatternPos] == singleWildcard || (ignoreCase && char.IsLetter(input[curInputPos]) && char.ToUpperInvariant(input[curInputPos]) == char.ToUpperInvariant(pattern[curPatternPos]))))
              {
                curInputPos++;
                curPatternPos++;
              }
            }

            // If we have reached next multiple wildcard character in pattern without breaking the matching sequence, then we have another candidate for full match
            // This candidate should be pushed to stack for further processing
            // At the same time, pair (input position, pattern position) will be marked as tested, so that it will not be pushed to stack later again
            if (((curPatternPos == patternLength && curInputPos == inputLength) || (curPatternPos < patternLength && pattern[curPatternPos] == multipleWildcard)) && !pointTested[curInputPos, curPatternPos])
            {
              pointTested[curInputPos, curPatternPos] = true;
              inputPosStack[++stackPos] = curInputPos;
              patternPosStack[stackPos] = curPatternPos;
            }
          }
        }
      }

      return matched;
    }

    #endregion Private Methods
  }
}