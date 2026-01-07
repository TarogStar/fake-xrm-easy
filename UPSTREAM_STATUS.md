# FakeXrmEasy.Community - Upstream Status

This document consolidates the status of issues and PRs from the archived [jordimontana82/fake-xrm-easy](https://github.com/jordimontana82/fake-xrm-easy) repository (archived June 2024).

**Last Updated:** 2026-01-07
**Version:** 1.0.2+

---

## Platform Parity Target

- **Target Platform:** Dynamics 365 v9.x+ / Dataverse (cloud)
- **Framework:** .NET Framework 4.6.2
- **SDK:** Microsoft.CrmSdk.CoreAssemblies 9.0.2.60+
- **Approach:** Strictly parity-focused (throw/fault like Dataverse)

### Known Differences

This framework aims for high fidelity but is not a perfect emulator. Known differences:
- Virtual entities require manual `IEntityDataSourceRetrieverService` setup
- Some advanced metadata scenarios require explicit `InitializeMetadata()` calls
- Async plugin execution is synchronous in tests

---

## Quick Summary

| Category | Total | Fixed | P0-P3 TODO | Won't Fix |
|----------|-------|-------|------------|-----------|
| Plugin/Pipeline | 12 | 4 | 3 | 0 |
| Query Engine | 21 | 17 | 2 | 0 |
| Date/Time | 9 | 7 | 4 | 0 |
| Message Executors | 13 | 9 | 2 | 0 |
| Metadata | 10 | 5 | 2 | 2 |
| CRUD/Core | 13 | 3 | 7 | 0 |
| Other | 5 | 0 | 1 | 4 |

---

## Completed Enhancements

### Core Framework (v1.0.0-1.0.2)

| Feature | Version | Description |
|---------|---------|-------------|
| IPluginExecutionContext4 | v1.0.0 | Full interface hierarchy with Azure AD object IDs |
| SDK-Style Projects | v1.0.0 | Modern project format, automatic binding redirects |
| CalculateRollupFieldRequest | v1.0.1 | New executor for rollup field testing |
| Auto-Populate Entity Images | v1.0.2 | ExecutePluginWithTarget auto-retrieves pre/post images |
| Filtering Attributes Validation | v1.0.2 | Plugins only execute when filtering attrs present |
| Bulk Operations | v1.0.2 | CreateMultiple, UpdateMultiple, DeleteMultiple, UpsertMultiple |
| Auto-Register Relationships | v1.0.2 | InitializeMetadata auto-registers N:N, 1:N, N:1 relationships |

---

## Fixed Issues (Verified)

### Query Engine / FetchXML

| # | Title | Status | Implementation |
|---|-------|--------|----------------|
| 506 | Quick find queries with multiple filter nodes | **FIXED** | `XmlExtensionsForFetchXml.ToCriteria()` combines multiple filters with AND |
| 507 | FetchXml multiple filter nodes (PR) | **FIXED** | Integrated - filters collected into list and combined |
| 588 | Between dates query end of day | **FIXED** | `TranslateConditionExpressionBetween()` extends to 23:59:59.999 |
| 503 | Left outer join fix | **FIXED** | Proper GroupJoin + SelectMany + DefaultIfEmpty pattern |
| 324 | Left outer join aggregation | **FIXED** | Same as #503 |
| 255 | Outer join with conditions | **FIXED** | Same as #503 |
| 485 | Aggregate alias/name validation | **FIXED** | Proper string comparison in aggregation code |
| 482 | Aggregates with optionset groupby | **FIXED** | Uses OptionSetValue.Value for comparison |
| 484 | OptionSetValue groupby (PR) | **FIXED** | Integrated |
| 486 | Alias and name checks (PR) | **FIXED** | Integrated |

### Date/Time Operators

| # | Title | Status | Implementation |
|---|-------|--------|----------------|
| 587 | ThisMonth fails on month-end | **FIXED** | End date includes 23:59:59.999 |
| 551 | LastMonth ignores time | **FIXED** | Same end-of-day handling |
| 543 | Week operators failing | **FIXED** | Culture-aware `ToFirstDayOfDeltaWeek()` |
| 539 | UTC timezone handling | **FIXED** | `SystemTimeZone` property on context |
| 588 | Between dates (PR) | **FIXED** | Integrated |

### Plugin/Pipeline

| # | Title | Status | Implementation |
|---|-------|--------|----------------|
| 500 | PreValidation steps ignored | **FIXED** | Pipeline executes PreValidation stage |
| 496 | Plugin Images + PreValidation (PR) | **FIXED** | We went further with auto-populate |
| 451 | 1:N relationship metadata missing | **FIXED** | `AutoRegisterRelationshipsFromMetadata()` |

### CRUD/Core

| # | Title | Status | Implementation |
|---|-------|--------|----------------|
| 555 | EntityReference.Name not populated | **FIXED** | `PopulateEntityReferenceNames()` in Retrieve executors |
| 524 | Duplicate intersect records | **FIXED** | AssociateRequest checks for duplicates |
| 554 | N:N duplicate fix (PR) | **FIXED** | Integrated |

### Message Executors

| # | Title | Status | Implementation |
|---|-------|--------|----------------|
| 538 | RetrieveMetadataChangesRequest | **FIXED** | `RetrieveMetadataChangesRequestExecutor` - metadata filtering/projection |
| 455 | UtcTimeFromLocalTimeRequest | **FIXED** | `UtcTimeFromLocalTimeRequestExecutor` - time zone conversion |
| 510 | WinQuoteRequest | **FIXED** | `WinQuoteRequestExecutor` - sets quote to Won state, creates QuoteClose activity |
| 572 | IEntityDataSourceRetrieverService | **FIXED** | `EntityDataSourceRetriever` property, virtual entity data provider testing |
| 610 | ExecuteTransactionRequest | **FIXED** | `ExecuteTransactionExecutor` - batch transactional execution |
| 615 | UpsertRequest issues | **FIXED** | `UpsertRequestExecutor` with alternate key support via `GetRecordUniqueId()` |

### Query Engine - Null Handling

| # | Title | Status | Implementation |
|---|-------|--------|----------------|
| 608 | LIKE condition NullReferenceException | **FIXED** | Null-safe string handling in `TranslateConditionExpressionLike` |
| 607 | Filtered linked entity NRE | **FIXED** | Defensive null checks on condition values, treats null as empty string |

### Query Engine - Nested Queries

| # | Title | Status | Implementation |
|---|-------|--------|----------------|
| 547 | Null operator in nested filter | **FIXED** | `TranslateConditionExpressionNull` unwraps AliasedValue before null check |
| 545 | Aggregate with nested outer joins | **FIXED** | Aggregation uses immediate parent alias, not full ancestor path |
| 606 | Complex nested filters | **FIXED** | Recursive filter processing + #547 fix covers this |
| 612 | StateCode cast error | **FIXED** | StateCode/StatusCode properly mapped to OptionSetValue in query engine |

### Query Engine - Column Comparison

| # | Title | Status | Implementation |
|---|-------|--------|----------------|
| 514 | FetchXml valueof column comparison | **FIXED** | `TranslateColumnComparisonExpression` supports valueof attribute and SDK CompareColumns property |

---

## Prioritized Roadmap

Based on analysis of modern Dataverse SDK requirements and real-world usage patterns.

### P0 — Parity Bugs (Most likely to break real projects)

| # | Title | Category | Notes |
|---|-------|----------|-------|
| 293 | Output parameters lost | Pipeline | Critical for chained plugin logic |
| 479 | Statecode on create | Core | Affects downstream behaviors and queries |
| 458 | DateTime.Kind differences | Date | Top source of cross-environment test drift |
| 491 | UTC conversion issues | Date | Related to #458 |
| 573 | Pipeline NRE | Pipeline | Instability undermines confidence |
| 562 | Min date validation 01/01/1753 | Core | Prevents false-positive tests |

### P1 — Modern Data/Key Scenarios (Common in cloud)

| # | Title | Category | Notes |
|---|-------|----------|-------|
| 508 | Alternate keys in AssociateRequest | Core | Needed now that we support alt-key Upsert |
| 521 | Composite alternate keys | Core | Check with UpsertMultiple |
| 470 | Alt key with early-bound | Core | Early-bound alternate key resolution |
| 553 | RowVersion / optimistic concurrency | Core | Increasingly used in enterprise code |
| 472 | OwningBusinessUnit on assign | Core | Security + ownership logic |

### P2 — Query Completeness / Advanced Operators

| # | Title | Category | Notes |
|---|-------|----------|-------|
| 461 | next-x-timeperiod operators | Date | PR from RachaelBooth |
| 460 | last-x-weeks operator | Date | PR from RachaelBooth |
| 476 | Fiscal period operators | Date | Beyond fiscal year |
| 509 | LIKE wildcards [X-Y] | Query | Advanced patterns |
| NEW | Any/All related-record filtering | Query | AnyAllFilterLinkEntity support |

### P3 — Developer Experience / Sustainability

| # | Title | Category | Notes |
|---|-------|----------|-------|
| 557 | Expose Metadata generation | Metadata | Make MetadataGenerator public/static |
| 447 | PicklistAttributeMetadata options | Metadata | OptionSet behavior fidelity |
| NEW | ExecuteMultiple ContinueOnError | Messages | Fault behavior + partial results |
| NEW | README placeholder cleanup | Docs | Replace YOUR_ORG with actual org |

### Not Tracked - New SDK Features to Consider

| Feature | Category | Notes |
|---------|----------|-------|
| AnyAllFilterLinkEntity | Query | Any/NotAny/All/NotAll join operators |
| ExecuteMultiple semantics | Messages | ContinueOnError, per-request faults |
| Alternate key metadata fidelity | Metadata | Rich attribute metadata for alt keys |

---

## Won't Fix

| # | Title | Reason |
|---|-------|--------|
| 523 | Microsoft.CrmSdk.Extensions | Xrm.Client namespace deprecated |
| 453 | VS2019 unit test hang | We target VS2022+ |
| 414 | Assembly version error | Version-specific, not applicable |
| 513 | Release config constants (PR) | SDK-style projects handle this |
| 448 | Cleanup deprecated methods (PR) | We already removed deprecations |
| 459 | Test cleanup (PR) | Low value, test-only changes |

---

## Needs Investigation

Items requiring reproduction tests and decisions (fix vs document as known difference):

| # | Title | Category |
|---|-------|----------|
| 569 | ObjectTypeCode casting | Query engine |
| 566 | Upsert alt key copy | May be fixed with #615 |
| 521 | Composite alternate keys | Check with UpsertMultiple |
| 479 | Statecode on create | Core CRUD |
| 472 | OwningBusinessUnit on assign | Core CRUD |
| 470 | Alt key with early-bound | Core CRUD |
| 458 | DateTime.Kind differences | Date handling |
| 491 | UTC conversion | Date handling |
| 573 | Pipeline NRE | Plugin pipeline |
| 293 | Output parameters lost | Plugin pipeline |

---

## Open PRs Summary

### Consider for Implementation
| PR | Title | Author | Notes |
|----|-------|--------|-------|
| 557 | Expose Metadata generation | janssen-io | Make MetadataGenerator public/static for external use |
| 447 | PicklistAttributeMetadata options | Nianwei | Implement independently if useful |
| 461 | next-x-timeperiod operators | RachaelBooth | Implement independently if useful |
| 460 | last-x-weeks operator | RachaelBooth | Implement independently if useful |

**Note:** We implement the spirit of community PRs independently rather than directly copying code, to ensure proper ownership and avoid copyright concerns.

### Already Integrated/Fixed
| PR | Title | Status |
|----|-------|--------|
| 588 | Between dates end of day | FIXED |
| 507 | FetchXml multiple filters | FIXED |
| 503 | Left outer join fix | FIXED |
| 554 | N:N duplicate check | FIXED |
| 496 | Plugin Images/PreValidation | FIXED (we did better) |
| 484 | OptionSetValue groupby | FIXED |
| 486 | Aggregate alias/name | FIXED |
| 538 | RetrieveMetadataChangesRequest | FIXED |
| 510 | WinQuoteRequest | FIXED |
| 455 | UtcTimeFromLocalTimeRequest | FIXED |
| 572 | IEntityDataSourceRetrieverService | FIXED |

---

## Architecture Decisions

### What We Support
- Dynamics 365 v9.x and later only
- .NET Framework 4.6.2
- Modern SDK-style projects
- VS2019/VS2022

### What We Don't Support
- Legacy CRM versions (2011, 2013, 2015, 2016)
- Old project formats
- Deprecated Xrm.Client namespace

---

## Contributing

When fixing an upstream issue:
1. Update the status in this document
2. Add tests covering the fix
3. Update CHANGELOG.md if significant

When integrating a PR:
1. Review code for v9.x compatibility
2. Adapt to our coding patterns
3. Add/update tests
4. Mark as FIXED in this document

---

## Changelog

### 2026-01-07 (Part 5)
- Restructured TODO into Prioritized Roadmap (P0-P3) based on modern SDK analysis
- Added Platform Parity Target section with known differences
- Added new SDK features to consider: AnyAllFilterLinkEntity, ExecuteMultiple semantics
- P0 priorities: #293, #479, #458, #491, #573, #562 (parity bugs)
- P1 priorities: #508, #521, #470, #553, #472 (modern key scenarios)
- P2 priorities: #461, #460, #476, #509, Any/All filtering (query completeness)
- P3 priorities: #557, #447, ExecuteMultiple ContinueOnError (DX improvements)

### 2026-01-07 (Part 4)
- Fixed: #514 - FetchXML valueof attribute for column-to-column comparison
  - Supports operators: eq, ne, gt, ge, lt, le
  - Handles various value types: string, int, Money, DateTime, EntityReference
  - Also supports SDK QueryExpression's `CompareColumns` property
  - Added 11 test cases covering both FetchXML and QueryExpression approaches

### 2026-01-07 (Part 3)
- Verified: #615 - UpsertRequest issues already fixed with UpsertRequestExecutor + alternate key support
- Verified: #612 - StateCode cast error already handled with proper OptionSetValue mapping
- Verified: #606 - Complex nested filters already working with recursive processing + #547 fix
- Updated PR approach: implement spirit of PRs independently for proper ownership

### 2026-01-07 (Part 2)
- Fixed: #547 - ConditionOperator.Null in nested LinkEntity filters now works correctly
  - Root cause: AliasedValue wrapper not being unwrapped in null check
  - Fix: `TranslateConditionExpressionNull` now checks `AliasedValue.Value` for null
- Fixed: #545 - Aggregate queries with nested outer joins now return correct values
  - Root cause: Aggregation used full ancestor alias path, but query uses immediate alias only
  - Fix: Aggregation now uses only immediate parent link-entity alias
- Added 9 new test cases for nested query issues

### 2026-01-07
- Verified: Between dates, Left outer join, Multiple filters, Date operators, EntityReference.Name all FIXED
- Added: RetrieveMetadataChangesRequestExecutor (PR #538) - metadata filtering and projection
- Added: UtcTimeFromLocalTimeRequestExecutor (PR #455) - time zone conversion
- Added: WinQuoteRequestExecutor (PR #510) - quote winning with QuoteClose activity
- Verified: IEntityDataSourceRetrieverService (PR #572) - virtual entity data provider testing already implemented
- Verified: NullReferenceException fixes (#608, #607) - LIKE, Contains, EndsWith operators handle null values
- Verified: ExecuteTransactionRequest (#610) - transactional batch execution already implemented
- Consolidated from multiple tracking documents

### 2025-11-11
- Added bulk operations (CreateMultiple, UpdateMultiple, DeleteMultiple, UpsertMultiple)
- Added auto-populate entity images
- Added filtering attributes validation
- Added auto-register relationships from metadata
