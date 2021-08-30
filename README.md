Simple MD5
===========

Ultra simple MD5 command line utility.

Usage:

`md5.exe [file1[+file2][+file3]...] [/r] [/w] [/v]`

File name arguments may point to either a file or a directory.
When pointing to a directory, all files in the directory will be
processed.

Wildcards can be used for file name arguments, which will be
evaluated against the contents of the current directory.

If no file name arguments are provided, all files in the current
directory will be scanned.

> Note: File names ending with .md5 are always ignored for write
> or verify operations

Options
-------

* `/r` - specifies that directories should be scanned
  recursively
* `/w` - specifies that a `.md5` file containing the hash should
  be written for each scanned file
* `/v` - specifies that the contents of each file will be
  verified against a previous saved file in a `.md5` file

Exit Codes
----------

| Code | Description   |
| ---- | ------------- |
| `0`  | Success       |
| `1`  | Failed        |
| `2`  | Verify Failed |

Examples
--------

![Displaying the hash of all .exe files in the current
directory][screenshot1]

Displaying the hash of all `.exe` files in the current directory

![Displaying the hash of all .exe files in the current
directory and child folders][screenshot2]

Displaying the hash of all `.exe` files in the current directory
and child folders

![Verifying hashes against previous saved values][screenshot3]

Verifying hashes against previous saved values

[screenshot1]: res/screenshot1.png
[screenshot2]: res/screenshot2.png
[screenshot3]: res/screenshot3.png
