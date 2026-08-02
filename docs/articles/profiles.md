# Building a profile

A profile is a choice of tables for each of the four stringprep steps. This page covers the steps
one at a time, then puts them together.

Order matters, and `PreparationProcessBuilder` does not reorder for you — steps run in the order
you add them. RFC 3454 applies mapping, then normalization, then prohibition, then the
bidirectional check. Add them in that order unless you have a reason not to.

## Mapping

The mapping step rewrites code points. `MappingTable.Build` takes any number of dictionary tables
and returns a builder:

```csharp
var table = MappingTable.Build(Mapping.B1)
    .WithValueRangeTable(Prohibited.C12, ' ')
    .Compile();

new MappingStep(table).Run("I­X");   // "IX"
```

`WithMappingTable` adds another dictionary. `WithValueRangeTable` maps an entire range to one
replacement — useful when a whole block collapses to a single character, as the non-ASCII spaces
in C.1.2 do:

```csharp
var table = MappingTable.Build(Mapping.B1)
    .WithMappingTable(Mapping.B2)             // case folding
    .WithValueRangeTable(Prohibited.C12, ' ') // every exotic space becomes U+0020
    .Compile();
```

`Include` merges in another dictionary and, unlike `WithMappingTable`, does not overwrite entries
that already exist. `Remove` drops a single code point, and applies to both the dictionary tables
and the value ranges:

```csharp
var table = MappingTable.Build(Mapping.B1)
    .WithValueRangeTable(Prohibited.C12, ' ')
    .Remove(0x00AD)   // keep the soft hyphen that B.1 would delete
    .Remove(0x2000)   // and leave one exotic space alone
    .Compile();

table.HasReplacement(0x00AD);   // false
table.HasReplacement(0x2000);   // false
table.HasReplacement(0x2001);   // true - still mapped to U+0020
```

A replacement can be several code points, or none at all. Table B.1 maps its entries to an empty
array, which is how "mapped to nothing" is expressed:

```csharp
table.GetReplacement(0x00AD).Length;   // 0  - deleted
table.GetReplacement(0x2000)[0];       // 0x0020
```

## Normalization

There is only one decision here, and for almost every profile it is NFKC:

```csharp
.WithNormalizationStep()                            // NFKC
.WithNormalizationStep(NormalizationForm.FormKC)    // the same thing, explicit
.WithNormalizationStep(NormalizationForm.FormC)     // if a profile demands NFC
```

This delegates to `string.Normalize`, so it follows whatever Unicode version the running .NET
release ships. That is worth knowing if you persist prepared strings: a future runtime could
normalize a rare code point differently.

## Prohibition

`ValueRangeTable.Build` takes any number of range tables and merges them. Pass every table your
profile prohibits in one call — merging happens once, at compile time:

```csharp
var prohibited = ValueRangeTable.Build(
        Prohibited.C12, Prohibited.C21, Prohibited.C22, Prohibited.C3, Prohibited.C4,
        Prohibited.C5, Prohibited.C6, Prohibited.C7, Prohibited.C8, Prohibited.C9)
    .Compile();
```

You can adjust the set before compiling. `Include` and `IncludeRange` add code points; `Remove`
and `RemoveRange` take them away:

```csharp
var prohibited = ValueRangeTable.Build(Prohibited.C21)
    .IncludeRange('0', '9')   // also reject digits
    .Compile();

prohibited.Contains('5');   // true
prohibited.Contains('a');   // false
```

The compiled table is a sorted array of ranges searched by bisection, so `Contains` stays cheap no
matter how many tables went into it.

### Unassigned code points

RFC 3454 §7 says a profile should reject unassigned code points in stored strings, while queries
may tolerate them. Table A.1 is supplied for this and is not prohibited by default:

```csharp
var stored = ValueRangeTable.Build(Unassigned.A1, Prohibited.C21 /* ... */).Compile();
```

Note that A.1 reflects the Unicode version RFC 3454 was written against (3.2). Code points
unassigned then have since been assigned, so this table rejects characters that are now perfectly
valid. Include it only if your profile calls for it.

## Bidirectional

The bidirectional check enforces the three rules in RFC 3454 §6: a string containing a RandALCat
character may not also contain an LCat character, and if it contains any RandALCat character it
must both begin and end with one.

The no-argument overload wires up the standard tables:

```csharp
.WithBidirectionalStep()
```

which is equivalent to:

```csharp
.WithBidirectionalStep(
    ValueRangeTable.Create(Prohibited.ChangeDisplayPropertiesOrDeprecated),
    ValueRangeTable.Create(BidirectionalTables.RorAL),
    ValueRangeTable.Create(BidirectionalTables.L))
```

The first table is checked before the direction rules and throws `ProhibitedValueException`; the
default is C.8, the characters that change display properties. The second and third are D.1
(RandALCat) and D.2 (LCat).

## Worked profiles

### SASLprep — RFC 4013

For usernames and passwords. Maps non-ASCII spaces to `U+0020`, does not case fold.

```csharp
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

### Nameprep — RFC 3491

For internationalized domain labels. Case folds via B.2, and permits ASCII control characters
because the surrounding IDNA machinery deals with them.

```csharp
var nameprep = PreparationProcess.Build()
    .WithMappingStep(MappingTable.Create(Mapping.B1, Mapping.B2))
    .WithNormalizationStep(NormalizationForm.FormKC)
    .WithProhibitedValueStep(ValueRangeTable.Build(
            Prohibited.C12, Prohibited.C22, Prohibited.C3, Prohibited.C4, Prohibited.C5,
            Prohibited.C6, Prohibited.C7, Prohibited.C8, Prohibited.C9)
        .Compile())
    .WithBidirectionalStep()
    .Compile();

nameprep.Run("ExAmPlE");   // "example"
nameprep.Run("Bücher");    // "bücher"
```

The differences from SASLprep are the whole of the profile: B.2 instead of the space mapping, and
C.2.1 absent from the prohibited set.

## Using a step on its own

The steps are public and implement `IPreparationProcess`, so you can use one without a process
around it — handy for validating input you do not want rewritten:

```csharp
var check = new ProhibitedValueStep(ValueRangeTable.Create(Prohibited.C21));
check.Run("clean");   // returns the input unchanged, or throws
```

Every step takes a string and returns a string, which is all `IPreparationProcess` requires.
Implement it yourself to add a step of your own — a length limit, say — and pass the result
through it alongside the built-in steps.

## A note on code points

Every table in RFC 3454 is defined over Unicode *code points*, not UTF-16 code units, and many
tables contain values above `U+FFFF`. The steps in this library walk their input by code point, so
a character outside the Basic Multilingual Plane is matched as the single code point it is, and a
well formed surrogate pair is never mistaken for two prohibited surrogates.

If you build tables of your own, give them code point values for the same reason:

```csharp
var table = ValueRangeTable.Build([0x1F600, 0x1F64F]).Compile();   // emoticons block
table.Contains(0x1F600);   // true
```
