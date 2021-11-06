# PathUtility

Welcome to PathUtility.

This is a small utility to encode / decode file names, so that any characters could be used as a file or folder name.

## Scenario

You want to leverage folder name for category of the books, and book name as file name, for your application, for example:

```shell
Books
└ Thriller
    │ Book1
    └ Book2
```

That make sense. However, not all the characters are allowed, for example, `:`, `*`, `?`. You also can't end a filename with a period `.`. Imaging books like this:

```shell
Books
└ Unknown*                                          # `*` isn't allowed
    │ Into the Abyss: An Extraordinary True Story   # `:` isn't allowed
    | Do Androids Dream of Electric Sheep?          # `?` isn't allowed
```

To address that, this library encodes string for those special characters and rules. For example

```csharp
using CodeWithSaar;
string encodedBookName = FileUtility.Encode("Into the Abyss: An Extraordinary True Story");
// encodedBookName will be: Into the Abyss%003A An Extraordinary True Story
```

And to read the folder and get the string back:

```csharp
using CodeWithSaar;
string originalString = FileUtility.Decode("Into the Abyss%003A An Extraordinary True Story");
// originalString will be: Into the Abyss: An Extraordinary True Story
```

## What does this library handles

1. Special characters: `<>:\"/\\|?*`
1. Reserved device names: `CON, AUX, NUL, COM1 ...`
1. Special file names: `.`, `..` (current folder or parent folder)
1. Period or space at the end of the files.

## Reference

[Naming Files, Paths, and Namespaces](https://docs.microsoft.com/en-us/windows/win32/fileio/naming-a-file)
