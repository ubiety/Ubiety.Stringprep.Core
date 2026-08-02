# Table reference

Every table in [RFC 3454 appendix A–D](https://datatracker.ietf.org/doc/html/rfc3454#appendix-A) is
included. Each is exposed twice — once under its RFC number and once under a descriptive name —
and the two are the same object, so pick whichever reads better:

```csharp
Prohibited.C21 == Prohibited.ASCIIControlCharacters;   // same list
```

The tables are generated from `Resources/tables.txt` by the T4 template in `Generated/Tables.tt`.
Do not edit `Generated/Tables.cs` by hand.

## A — Unassigned code points

Class `Unassigned`. Range table.

| RFC | Name | Contents |
| --- | --- | --- |
| A.1 | `A1`, `UnassignedCodePoints` | Code points unassigned in Unicode 3.2 |

RFC 3454 §7 asks profiles to reject these in stored strings while tolerating them in queries. It is
not prohibited by default. Because the table is frozen at Unicode 3.2, it now rejects a good many
code points that have since been assigned — include it only when the profile calls for it.

## B — Mapping tables

Class `Mapping`. Dictionary tables, from code point to zero or more replacements.

| RFC | Name | Contents |
| --- | --- | --- |
| B.1 | `B1`, `MappedToNothing` | Code points that are deleted — soft hyphen, zero width joiner, the bidi marks |
| B.2 | `B2`, `CaseFolding` | Case folding used with NFKC |
| B.3 | `B3`, `CaseFoldingWithoutNormalization` | Case folding for profiles that skip normalization |

B.2 and B.3 are alternatives, not a pair — a profile uses one or the other. B.2 is the usual
choice, and is what Nameprep uses.

A code point in B.1 maps to an empty array, which is how "mapped to nothing" is represented:

```csharp
var table = MappingTable.Create(Mapping.B1);
table.GetReplacement(0x00AD).Length;   // 0
```

## C — Prohibited output

Class `Prohibited`. Range tables.

| RFC | Name | Contents |
| --- | --- | --- |
| C.1.1 | `C11`, `ASCIISpaceCharacters` | `U+0020` |
| C.1.2 | `C12`, `NonASCIISpaceCharacters` | No-break space, en/em spaces, ideographic space |
| C.2.1 | `C21`, `ASCIIControlCharacters` | `U+0000`–`U+001F`, `U+007F` |
| C.2.2 | `C22`, `NonASCIIControlCharacters` | `U+0080`–`U+009F`, line/paragraph separators, interlinear annotation |
| C.3 | `C3`, `PrivateUseCharacters` | The three private use areas |
| C.4 | `C4`, `NonCharacterCodePoints` | `U+FDD0`–`U+FDEF` and the `FFFE`/`FFFF` of every plane |
| C.5 | `C5`, `SurrogateCodePoints` | `U+D800`–`U+DFFF` |
| C.6 | `C6`, `InappropriateForPlainText` | Interlinear annotation, object replacement |
| C.7 | `C7`, `InappropriateForCanonicalRepresentation` | Ideographic description characters |
| C.8 | `C8`, `ChangeDisplayPropertiesOrDeprecated` | Bidi overrides, deprecated formatting |
| C.9 | `C9`, `TaggingCharacters` | The `U+E0000` tag block |

C.1.1 is the ASCII space on its own. Most profiles do *not* prohibit it — SASLprep maps other
spaces onto it. It is there for profiles that forbid spaces entirely.

C.1.2 usually appears twice in a profile: once in the mapping step, turning exotic spaces into
`U+0020`, and once in the prohibited set, so that any that survive are rejected.

C.5 covers surrogate *code points*. A well formed surrogate pair encodes a supplementary character
and is not prohibited — the steps in this library walk their input by code point, so a pair is
matched as the single character it encodes. An unpaired surrogate is caught.

## D — Bidirectional

Class `BidirectionalTables`, in the `Ubiety.Stringprep.Core.Bidirectional` namespace. Range tables.

| RFC | Name | Contents |
| --- | --- | --- |
| D.1 | `D1`, `RorAL` | Characters with bidirectional property `R` or `AL` |
| D.2 | `D2`, `L` | Characters with bidirectional property `L` |

`WithBidirectionalStep()` wires both up for you along with C.8. You only need these names to build
the step explicitly.

## Code points, not code units

Every table is defined over Unicode code points, and many reach above `U+FFFF`: 524 of B.2's 2742
keys are supplementary, as are 76 of B.3's, and 37 of D.2's 360 ranges sit wholly above the BMP,
along with 16 of C.4's 18. Anything that walks a string one `char` at a time can never match them,
so the steps here iterate by code point instead.

If you write your own table, use code point values:

```csharp
var table = ValueRangeTable.Build([0x1F600, 0x1F64F]).Compile();
table.Contains(0x1F600);   // true
```

## Table shapes

The two shapes are not interchangeable.

A **range table** is a `List<int>` of start/end pairs, sorted and non-overlapping once compiled.
Everything in `Prohibited`, `Unassigned` and `BidirectionalTables` is one of these, and they go to
`ValueRangeTable.Build` or `ValueRangeTable.Create`.

A **mapping table** is an `IDictionary<int, int[]>` from code point to replacements. Everything in
`Mapping` is one of these, and they go to `MappingTable.Build` or `MappingTable.Create`.

A range table can be used as the *source* side of a mapping when the whole range collapses to one
replacement, via `WithValueRangeTable`:

```csharp
MappingTable.Build(Mapping.B1)
    .WithValueRangeTable(Prohibited.C12, ' ')   // range table -> single replacement
    .Compile();
```
