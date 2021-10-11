# Simple MD5

Windows command line utility for viewing, generating and
verifying MD5 hashes of files.

Usage:

`md5.exe [file1[+file2][+file3]...] [/r] [[/w [path]] | [/v
[path]]] [/f]`

File name arguments may point to either a file or a directory.
When pointing to a directory, all files in the directory will be
processed.

Wildcards can be used for file name arguments, which will be
evaluated against the contents of the current directory.

If no file name arguments are provided, all files in the current
directory will be scanned.

> Note: File names ending with .md5 are always ignored for write
> or verify operations

## Options

| Switch      | Description                                                                                                                                                         |
| ----------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `/r`        | Scan directories recursively                                                                                                                                        |
| `/w <path>` | Write the hash into `<filename>.md5`. If `path` is specified, file will be written to specified location instead of alongside source file. Cannot be used with `/v` |
| `/v <path>` | Verify hashes. If `path` is specified, hash will be read from specified location. Cannot be used with `/w`                                                          |
| `/f`        | Show full paths                                                                                                                                                     |
| `/e`        | Show errors only (only applicable with `/v`)                                                                                                                        |
| `/n`        | Write hash information for new files only (only applicable with `/w`)                                                                                               |

## Exit Codes

| Code | Description   |
| ---- | ------------- |
| `0`  | Success       |
| `1`  | Failed        |
| `2`  | Verify Failed |

## Examples

### Displaying hashes for all files in the current directory

> $ md5

```text
6b24faec50a945f7d0fc36a8c041b781: cyowcopy--x64.zip
845532e9859d841dc4c2f121886d62c4: setup-cyowcopy-1.9.0.0-x64-alpha.exe
```

### Displaying hashes for all exe files in the current directory

> $ md5 *.exe

```text
845532e9859d841dc4c2f121886d62c4: setup-cyowcopy-1.9.0.0-x64-alpha.exe
```

### Displaying hashes for all exe files in the current directory recursively

> $ md5 *.exe /r

```text
845532e9859d841dc4c2f121886d62c4: setup-cyowcopy-1.9.0.0-x64-alpha.exe
5170fc8e5a71d5e6db2bba132255c75e: x64\ctkaupld.exe
8b0a35dcbf9dd4928b042a27fd3bfcef: x64\cyowcopy.exe
a2903cb6bdcd2a6e1da9e116b1c24f30: x64\remfdbck.exe
64b729790db0f44174a4d1aea095de6e: x64\setmgr.exe
95f219aa871719dc4a8a78a9e85ea5d2: x64\tdiagvwr.exe
c1acb440ee6507b581ea3a0ff8119e26: x64\updchk.exe
a3b255d9e09962ae39829b3da1169ef1: x64\wcopy.exe
```

### Displaying full paths of file names

> $ md5 /f

```text
6b24faec50a945f7d0fc36a8c041b781: D:\Checkout\trunk\cyotek\source\Applications\WebCopy\deployment\cyowcopy--x64.zip
845532e9859d841dc4c2f121886d62c4: D:\Checkout\trunk\cyotek\source\Applications\WebCopy\deployment\setup-cyowcopy-1.9.0.0-x64-alpha.exe
```

### Saving hashes alongside source files

> $ md5 /w

```text
6b24faec50a945f7d0fc36a8c041b781: cyowcopy--x64.zip
845532e9859d841dc4c2f121886d62c4: setup-cyowcopy-1.9.0.0-x64-alpha.exe
```

> $ dir /b

```text
cyowcopy--x64.zip
cyowcopy--x64.zip.md5
setup-cyowcopy-1.9.0.0-x64-alpha.exe
setup-cyowcopy-1.9.0.0-x64-alpha.exe.md5
```

### Saving hashes to a secondary folder tree

> $ md5 /w ..\hashset

```text
6b24faec50a945f7d0fc36a8c041b781: cyowcopy--x64.zip
845532e9859d841dc4c2f121886d62c4: setup-cyowcopy-1.9.0.0-x64-alpha.exe
```

> $ dir /b

```text
cyowcopy--x64.zip
setup-cyowcopy-1.9.0.0-x64-alpha.exe
```

> $ dir ..\hashset /b

```text
cyowcopy--x64.zip.md5
setup-cyowcopy-1.9.0.0-x64-alpha.exe.md5
```

### Verifying saved hashes

> $ md5 /v

```text
6b24faec50a945f7d0fc36a8c041b781: cyowcopy--x64.zip
VERIFIED
845532e9859d841dc4c2f121886d62c4: setup-cyowcopy-1.9.0.0-x64-alpha.exe
VERIFIED
```

### Verifying saved hashes in a secondary folder tree

> $ md5 /v ..\hashset

```text
6b24faec50a945f7d0fc36a8c041b781: cyowcopy--x64.zip
VERIFIED
845532e9859d841dc4c2f121886d62c4: setup-cyowcopy-1.9.0.0-x64-alpha.exe
VERIFIED
```
