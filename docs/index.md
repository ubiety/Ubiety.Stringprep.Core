---
_layout: landing
---

# Ubiety.Stringprep.Core

An implementation of stringprep — the preparation of internationalized strings of
[RFC 3454](https://datatracker.ietf.org/doc/html/rfc3454) — for .NET. It ships the RFC's character
tables and lets you assemble them into the profile you need.

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

saslprep.Run("I­X");   // "IX"  - the soft hyphen is mapped to nothing
saslprep.Run("Ⅸ");     // "IX"  - roman numeral nine, normalized
saslprep.Run("");     // throws ProhibitedValueException
```

- [Getting started](articles/getting-started.md) — install, build a profile, run it
- [Building a profile](articles/profiles.md) — the four steps, custom tables, worked profiles
- [Table reference](articles/tables.md) — every RFC 3454 table and the name it has here
- [API reference](api/index.md) — generated from the source

Stringprep is not a protocol. RFC 3454 defines the machinery — a mapping step, a normalization
step, a prohibition step and a bidirectional check — and leaves it to other specifications to say
which tables go in which step. Those specifications are *profiles*: SASLprep
([RFC 4013](https://datatracker.ietf.org/doc/html/rfc4013)) for usernames and passwords, Nameprep
([RFC 3491](https://datatracker.ietf.org/doc/html/rfc3491)) for domain labels, and others. This
library gives you the tables and the steps; you compose the profile.
