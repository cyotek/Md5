# Simple MD5

## Change Log

### 1.4

#### Added

* Added `/x` option to allow files or directories to be skipped

### 1.3.1

#### Added

* Added `/n` switch. When used in conjunction with `/w`, will
  only generate hashes for new files, ignoring updated or
  removed content

### 1.3.0

#### Added

* Added `/e` switch. When used in conjunction with `/v`, will
  only report errors. No output will be produced for successful
  compares.

### 1.2.3

#### Fixed

* Now uses explicit path combines

### 1.2.3

#### Fixed

* Missing directories are no longer created using a hash is to
  be written
* Now displays relative paths when performing recursive scanning
  instead of only showing the filename

#### 1.2.1

#### Added

* Added `/f` switch to enable full path display

#### Changed

* Command line arguments are now validated and the program will
  not continue if it finds unknown arguments

#### 1.2

#### Added

* The `/v` or `/w` switches now support a path value, allowing
  hashes to be read/written to a different directory tree

#### 1.1

#### Added

* Added wildcard support

#### Changed

* If ran without any arguments, now scans all files in the
  current directory

#### 1.0

* Initial commit
