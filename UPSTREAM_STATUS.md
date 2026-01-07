# FakeXrmEasy.Community - Upstream Status

This document consolidates the status of issues and PRs from the archived [jordimontana82/fake-xrm-easy](https://github.com/jordimontana82/fake-xrm-easy) repository (archived June 2024).

**Last Updated:** 2026-01-07
**Version:** 1.0.2+

---

## Quick Summary

| Category | Total | Fixed | TODO | Won't Fix |
|----------|-------|-------|------|-----------|
| Plugin/Pipeline | 12 | 4 | 3 | 0 |
| Query Engine | 18 | 10 | 5 | 0 |
| Date/Time | 6 | 6 | 0 | 0 |
| Message Executors | 8 | 3 | 4 | 0 |
| Metadata | 9 | 3 | 4 | 2 |
| CRUD/Core | 13 | 3 | 6 | 0 |
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

---

## TODO Items (Remaining Work)

### High Priority

| # | Title | Category | Notes |
|---|-------|----------|-------|
| 610 | ExecuteTransactionRequest | Executors | Transactional batch operations |
| 538 | RetrieveMetadataChangesRequest | Executors | PR exists from MarkMpn |
| 608 | LIKE condition NullReferenceException | Query | Add defensive null checks |
| 607 | Generic NRE in queries | Query | Related to #608 |
| 547 | Null operator in nested filter | Query | Investigate LinkEntity filters |
| 545 | Aggregate with nested outer joins | Query | Returns zero incorrectly |

### Medium Priority

| # | Title | Category | Notes |
|---|-------|----------|-------|
| 514 | FetchXml valueof column comparison | Query | Column-to-column comparison |
| 510 | WinQuoteRequest | Executors | PR exists |
| 455 | UtcTimeFromLocalTimeRequest | Executors | PR from RachaelBooth |
| 562 | Min date validation 01/01/1753 | Core | CRM minimum date |
| 508 | Alternate keys in AssociateRequest | Core | Currently not supported |
| 572 | IEntityDataSourceRetrieverService | Metadata | PR exists |
| 557 | Expose Metadata generation | Metadata | PR exists |
| 447 | PicklistAttributeMetadata options | Metadata | PR from Nianwei |

### Lower Priority

| # | Title | Category | Notes |
|---|-------|----------|-------|
| 553 | RowVersion property | Core | Low user demand |
| 476 | Fiscal period operators | Date | Beyond fiscal year |
| 509 | LIKE wildcards [X-Y] | Query | Advanced patterns |
| 461 | next-x-timeperiod operators | Date | PR from RachaelBooth |
| 460 | last-x-weeks operator | Date | PR from RachaelBooth |

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

These items need verification against the current codebase:

| # | Title | Category |
|---|-------|----------|
| 615 | UpsertRequest issues | May be fixed with our Upsert work |
| 612 | StateCode cast error | Query engine |
| 606 | Complex nested filters | Query engine |
| 569 | ObjectTypeCode casting | Query engine |
| 566 | Upsert alt key copy | May be fixed |
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

### Integrate Soon
| PR | Title | Author | Notes |
|----|-------|--------|-------|
| 538 | RetrieveMetadataChangesRequest | MarkMpn | Metadata tracking |
| 510 | WinQuoteRequest | BenjaminP-GitHub | Quote to order |
| 455 | UtcTimeFromLocalTimeRequest | RachaelBooth | Time conversion |
| 572 | IEntityDataSourceRetrieverService | jimbonovak | Managed identities |
| 557 | Expose Metadata generation | janssen-io | CrmSvcUtilMetadataGenerator |

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

### 2026-01-07
- Verified: Between dates, Left outer join, Multiple filters, Date operators, EntityReference.Name all FIXED
- Consolidated from multiple tracking documents

### 2025-11-11
- Added bulk operations (CreateMultiple, UpdateMultiple, DeleteMultiple, UpsertMultiple)
- Added auto-populate entity images
- Added filtering attributes validation
- Added auto-register relationships from metadata
