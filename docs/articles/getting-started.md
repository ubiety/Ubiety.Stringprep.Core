# Getting started

## Install

```shell
dotnet package add Ubiety.Stringprep.Core
```

## What stringprep does

[RFC 3454](https://datatracker.ietf.org/doc/html/rfc3454) describes how to take a string a user
typed and reduce it to a canonical form, so that two spellings of the same name compare equal and
strings that would be dangerous or ambiguous are rejected outright.

It is deliberately incomplete. The RFC defines four steps and a pile of character tables, then
leaves it to other specifications — *profiles* — to say which tables belong in which step. This
library ships the tables and the steps; you assemble the profile.

The four steps, in the order the RFC applies them:

| Step | What it does |
| --- | --- |
| Mapping | Replaces or deletes code points — case folding, stripping soft hyphens, collapsing exotic spaces |
| Normalization | Applies a Unicode normalization form, almost always NFKC |
| Prohibition | Throws if any code point is in the prohibited set |
| Bidirectional | Throws if right-to-left and left-to-right characters are mixed illegally |

## Build a profile

`PreparationProcess.Build()` returns a builder. Add the steps you want and compile it once, at
startup — compiling walks and merges the tables, so it is not something to do per call.

This is SASLprep ([RFC 4013](https://datatracker.ietf.org/doc/html/rfc4013)), the profile used for
usernames and passwords:

```csharp
using System.Text;
using Ubiety.Stringprep.Core;

var saslprep = PreparationProcess.Build()
    .WithMappingStep(MappingTable.Build(Mapping.B1)
        .WithValueRangeTable(Prohibited.C12, ' ')
        .Compile())
    .WithNormalizationStep(NormalizationForm.FormKC)
    .WithProhibitedValueStep(ValueRangeTable.Build(
            Prohibited.C12, Prohibited.C21, Prohibited.C22, Prohibited.C3, Prohibited.C4,
            Prohibited.C5, Prohibited.C6, Prohibited.C7, Prohibited.C8, Prohibited.C9)
        .Compile())
    .WithBidirectionalStep()
    .Compile();
```

The mapping step does two things: table B.1 maps a set of code points to nothing (soft hyphens,
zero-width joiners), and every non-ASCII space in table C.1.2 becomes a plain `U+0020`.

## Run it

`Run` returns the prepared string, or throws.

```csharp
saslprep.Run("user");    // "user"
saslprep.Run("USER");    // "USER"  - SASLprep does not case fold
saslprep.Run("I­X");     // "IX"    - U+00AD soft hyphen mapped to nothing
saslprep.Run("ª");       // "a"     - U+00AA normalized by NFKC
saslprep.Run("Ⅸ");       // "IX"    - U+2168 roman numeral nine
saslprep.Run(" ");  // " "     - no-break space mapped to U+0020
```

Those are the test vectors from [RFC 4013 section 3](https://datatracker.ietf.org/doc/html/rfc4013#section-3),
and the profile above reproduces all of them.

The process is immutable once compiled and holds no per-call state, so a single instance is safe
to share across threads. Build it once and keep it in a static field or your container.

## Handle rejection

Two exceptions come out of a prepared string, both from `Ubiety.Stringprep.Core.Exceptions`:

```csharp
using Ubiety.Stringprep.Core.Exceptions;

try
{
    return saslprep.Run(input);
}
catch (ProhibitedValueException e)
{
    // e.CodePoint is the offending code point, or -1 if not raised for a specific one
    throw new ArgumentException($"Username contains U+{e.CodePoint:X4}, which is not allowed.", e);
}
catch (BidirectionalFormatException e)
{
    throw new ArgumentException("Username mixes text directions.", e);
}
```

`ProhibitedValueException.CodePoint` gives you the code point that failed, so you can report
something more useful than "invalid input":

```csharp
saslprep.Run("badvalue");
// ProhibitedValueException: The string contains the prohibited value: U+0007
```

`BidirectionalFormatException` carries a message describing which of the three RFC 3454 §6 rules
was broken:

```csharp
saslprep.Run("ا1");
// BidirectionalFormatException: A character from the RandAL set must be the
// last character in an RandAL string
```

## Where to next

- [Building a profile](profiles.md) covers each step in detail, custom tables, and Nameprep.
- [Table reference](tables.md) lists every RFC 3454 table and what to call it here.
