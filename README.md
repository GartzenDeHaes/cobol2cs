# cobol2cs
COBOL / SCOBOL to C#/WPF Translator

## Summary

- Pre-BETA software.
- HP Non-Stop COBOL to C# translator with an 85% to 99% accuracy rate.  The translator always outputs something, even if it's just he original COBOL, allowing for manual translation of problematic COBOL input.
- It can also parse SQL/MP and ouput Java servlets with dynamic SQL queries.

## Projects

### cob2cs

The commandline COBOL to C# driver tool.

### cobinfo

Command line tool to output information about COBOL programs, copy books, defines, etc.

### CobolCodeBase

This housed the generated code.

### CobolParser

The COBOL parser and C# code generator.

### CobView

A mainframe file viewer.

### DOR.Core

General purpose library.

### DOR.WorkingStorage

WORKING-STORAGE emulation.  In an analysis of a 5 million line COBOL code base, about 40% of all records cannot be converted to standalone variables.  A common example would be dates that are referenced both by their full DATE record and by sub-fields such as DAY and YEAR.

### WinFormsUI

A WPF docking container UI used in the Windows runtime.
