# FakeXrmEasy.Community - Upstream Status

This document consolidates the status of issues and PRs from the archived [jordimontana82/fake-xrm-easy](https://github.com/jordimontana82/fake-xrm-easy) repository (archived June 2024).

**Last Updated:** 2026-01-07
**Version:** 1.1.0

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

### Known Limitations

**#293 - Plugin Output Parameters:** The convenience method `ExecutePluginWithTarget<T>(Entity target)` does not provide access to plugin output parameters. If your plugin sets output parameters that you need to verify, use the full overload instead:

```csharp
// ❌ Cannot access output parameters with this convenience method
context.ExecutePluginWithTarget<MyPlugin>(target);

// ✓ Use this approach when you need output parameters
var pluginContext = context.GetDefaultPluginContext();
pluginContext.InputParameters["Target"] = target;
context.ExecutePluginWith<MyPlugin>(pluginContext);
var result = pluginContext.OutputParameters["MyOutput"]; // ✓ Accessible
```

**#491 - DateTime Kind on Input:** ~~Resolved~~ The framework now handles DateTime.Kind correctly based on verified Dataverse behavior:
- `DateTimeKind.Local` → Converted to UTC using `ToUniversalTime()` (matches real Dataverse)
- `DateTimeKind.Utc` → Stored as-is
- `DateTimeKind.Unspecified` → Treated as UTC (stored raw, marked as UTC)

---

## Quick Summary

| Status | Count | Issues |
|--------|-------|--------|
| **Fixed** | 34 | See "Fixed Issues" section below |
| **Custom Additions** | 18 | See "Custom Additions" section below |
| **Needs Evaluation** | 34 | See "Needs Evaluation" section below |
| **Won't Fix** | 3 | #414, #453, #523 |
| **TOTAL (Upstream)** | **71** | All open issues from archived upstream repo |
| **TOTAL (All Work)** | **89** | Upstream issues (71) + Custom additions (18) |

**Fixed Issues (34 total):**
- **Plugin/Pipeline:** #451, #500, #573
- **Query Engine:** #485, #506, #509, #514, #545, #547, #569, #607, #608, #612
- **Date/Time:** #458, #491, #539, #543, #551, #587
- **Message Executors:** #610, #615
- **CRUD/Core:** #255, #470, #472, #476, #479, #482, #508, #521, #524, #553, #555, #562, #566

**P0-P3 Roadmap Status: COMPLETE** - All prioritized items resolved

**Needs Evaluation:** 34 open issues from the archived repository require triage. See the "Needs Evaluation" section below for the complete list.

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
| 458 | DateTime.Kind differences | **FIXED** | DateOnly/TimeZoneIndependent return `Unspecified`; UserLocal returns `Utc` |
| 491 | UTC conversion on input | **FIXED** | `ConvertToUtc()` converts Local→UTC; Utc/Unspecified stored as-is |

### Plugin/Pipeline

| # | Title | Status | Implementation |
|---|-------|--------|----------------|
| 500 | PreValidation steps ignored | **FIXED** | Pipeline executes PreValidation stage |
| 496 | Plugin Images + PreValidation (PR) | **FIXED** | We went further with auto-populate |
| 451 | 1:N relationship metadata missing | **FIXED** | `AutoRegisterRelationshipsFromMetadata()` |
| 573 | Pipeline NRE | **FIXED** | Null-safe EntityTypeCode in `RegisterPluginStep<TPlugin, TEntity>` |

### CRUD/Core

| # | Title | Status | Implementation |
|---|-------|--------|----------------|
| 555 | EntityReference.Name not populated | **FIXED** | `PopulateEntityReferenceNames()` in Retrieve executors |
| 524 | Duplicate intersect records | **FIXED** | AssociateRequest checks for duplicates |
| 554 | N:N duplicate fix (PR) | **FIXED** | Integrated |
| 479 | Statecode on create | **FIXED** | Throws FaultException for non-Active statecode; matches Dataverse Create+Update pattern |
| 562 | Min date validation 01/01/1753 | **FIXED** | `ValidateDateTime()` throws FaultException for dates before SQL Server minimum |

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

| # | Title | Category | Status |
|---|-------|----------|--------|
| 293 | Output parameters lost | Pipeline | DOCUMENTED - Use full overload for output params (see Known Limitations) |
| 479 | Statecode on create | Core | **FIXED** - Throws for non-Active state |
| 458 | DateTime.Kind differences | Date | **FIXED** - DateOnly/TZI return Unspecified |
| 491 | UTC conversion issues | Date | **FIXED** - Local→UTC conversion verified against real Dataverse |
| 573 | Pipeline NRE | Pipeline | **FIXED** - Null-safe EntityTypeCode |
| 562 | Min date validation 01/01/1753 | Core | **FIXED** - ValidateDateTime throws FaultException |

### P1 — Modern Data/Key Scenarios (Common in cloud)

| # | Title | Category | Status |
|---|-------|----------|--------|
| 508 | Alternate keys in AssociateRequest | Core | **FIXED** - Associate/Disassociate resolve KeyAttributes to IDs |
| 521 | Composite alternate keys | Core | **FIXED** - Uniqueness enforced on Create/Update, 22 tests |
| 470 | Alt key with early-bound | Core | **FIXED** - GetRecordUniqueId throws when no match found |
| 553 | RowVersion / optimistic concurrency | Core | **FIXED** - Auto-incrementing versionnumber + ConcurrencyBehavior validation |
| 472 | OwningBusinessUnit on assign | Core | **FIXED** - Assign updates owningbusinessunit from new owner's BU |

### P2 — Query Completeness / Advanced Operators

| # | Title | Category | Status |
|---|-------|----------|--------|
| 461 | next-x-timeperiod operators | Date | **FIXED** - Already implemented (TranslateConditionExpressionNext) |
| 460 | last-x-weeks operator | Date | **FIXED** - Already implemented (TranslateConditionExpressionLast) |
| 476 | Fiscal period operators | Date | **FIXED** - InFiscalPeriod, InFiscalPeriodAndYear, This/Last/NextFiscalPeriod |
| 509 | LIKE wildcards [X-Y] | Query | **FIXED** - Underscore, [a-z] ranges, [abc] sets, [^abc] negation |
| NEW | Any/All related-record filtering | Query | **FIXED** - JoinOperator.Any/NotAny/All/NotAll, 26 tests |

### P3 — Developer Experience / Sustainability (COMPLETE)

| # | Title | Category | Status |
|---|-------|----------|--------|
| 557 | Expose Metadata generation | Metadata | **FIXED** - MetadataGenerator public with FromEarlyBoundEntity<T>() |
| 447 | PicklistAttributeMetadata options | Metadata | **FIXED** - RetrieveAttributeRequest populates OptionSet from context |
| NEW | ExecuteMultiple ContinueOnError | Messages | **FIXED** - Response key fix + proper fault extraction |
| NEW | README placeholder cleanup | Docs | **FIXED** - Removed YOUR_ORG placeholders |

---

## v1.2.0 Roadmap

### Phase 1 — Quick Wins & Cleanup (COMPLETE)

| # | Title | Category | Status |
|---|-------|----------|--------|
| 569 | ObjectTypeCode casting | Query | **FIXED** - SafeConvertToInt handles type mismatches gracefully |
| 566 | Upsert alt key copy | CRUD | **FIXED** - KeyAttributes copied to Attributes on create |

### Phase 2 — Any/All Filter Operators (COMPLETE)

| Feature | Category | Effort | Status |
|---------|----------|--------|--------|
| AnyAllFilterLinkEntity | Query | Medium | **COMPLETE** |

**Implementation (2026-01-07):**
- Added `JoinOperator.Any/NotAny/All/NotAll` support to `XrmFakedContext.Queries.cs`
- Created `TranslateAnyAllLinkedEntityToLinq()` method (~170 lines)
- Added helper methods: `NormalizeKey()`, `EvaluateAllOperator()`, `EvaluateNotAllOperator()`
- FetchXML `link-type="any|not any|all|not all"` parsing already supported
- Created 26 comprehensive tests in `AnyAllFilterTests.cs`
- All 1148 tests pass

**Constraints (verified via XrmToolbox testing and enforced in implementation):**
- ✅ Any/All LinkEntity CAN be direct child of `<entity>` (top level only)
- ❌ Any/All LinkEntity CANNOT have `<link-entity>` as parent (no nesting under joins)
- ❌ Any/All LinkEntity CANNOT have nested `<link-entity>` children
- ❌ No attributes/columns allowed on Any/All link-entities
- ✅ Filters ARE supported inside the Any/All LinkEntity
- Translates to SQL EXISTS/NOT EXISTS subqueries

**FetchXML structure:**
```xml
<fetch top="50">
  <entity name="product">
    <link-entity name="gli_productjurisdiction" from="productid" to="productid"
                 link-type="not any" intersect="true">
      <filter>
        <condition attribute="productid" operator="not-null" />
      </filter>
    </link-entity>
  </entity>
</fetch>
```

### Phase 3 — Key Constraints & Validation (COMPLETE)

| Feature | Category | Effort | Status |
|---------|----------|--------|--------|
| Alternate key limits | Metadata | Medium | **COMPLETE** |
| Key attribute validation | Metadata | Low | **COMPLETE** |
| Key type enforcement | Metadata | Low | **COMPLETE** |

**Implementation (2026-01-07):**
- Added `ValidateAlternateKeyConstraints()` method in `XrmFakedContext.Crud.cs`
- Maximum 10 alternate keys per entity (validated in `AddAlternateKey`)
- Maximum 16 columns/attributes per key (validated in `AddAlternateKey`)
- Attribute type enforcement - only allowed types:
  - Decimal, Integer, BigInt, String, DateTime
  - Lookup, Customer (EntityReference types)
  - Picklist, State, Status (OptionSetValue types)
- Removed Money support from `CompareKeyValues()` (not supported by Dataverse as key type)
- 26 comprehensive tests in `AlternateKeyConstraintTests.cs`
- All 1174 tests pass

**Constraints enforced (verified against Microsoft documentation):**
- FaultException thrown if entity exceeds 10 alternate keys
- FaultException thrown if key exceeds 16 attributes
- FaultException thrown if unsupported attribute type used in key
- Money attributes explicitly rejected with clear error message

### Phase 4 — Metadata CRUD

#### Phase 4a — OptionSet CRUD (COMPLETE)

| Message | Status | Notes |
|---------|--------|-------|
| CreateOptionSetRequest | ✅ COMPLETE | Global OptionSet creation with metadata storage |
| UpdateOptionSetRequest | ✅ COMPLETE | OptionSet label/description updates |
| DeleteOptionSetRequest | ✅ COMPLETE | Global OptionSet deletion with validation |

**Implementation (2026-01-07):**
- Added `CreateOptionSetRequestExecutor` - creates global OptionSets in metadata repository
- Added `UpdateOptionSetRequestExecutor` - updates DisplayName, Description, HasChanged
- Added `DeleteOptionSetRequestExecutor` - deletes global OptionSets with existence validation
- 24 new tests across three test files
- All 1198 tests pass

#### Phase 4b — Entity/Attribute CRUD (Future)

| Message | Priority | Notes |
|---------|----------|-------|
| CreateEntity, UpdateEntity, DeleteEntity | Medium | Entity metadata manipulation |
| CreateAttribute, UpdateAttribute, DeleteAttribute | Medium | Attribute metadata manipulation |
| RetrieveAllEntities | Medium | Bulk metadata retrieval |
| RetrieveEntityChanges | Medium | Change tracking / delta sync |

### Phase 5 — Coverage Map & Documentation (Ongoing)

Formalize the testing coverage into a structured map:
- ✅ Supported (with test coverage)
- ⚠️ Supported with differences (documented)
- ❌ Not supported (documented)
- 🧪 Needs verification

---

### Not Tracked - Lower Priority SDK Features

| Feature | Category | Notes |
|---------|----------|-------|
| Encryption messages | Security | IsDataEncryptionActive, etc. - rarely tested |
| Relationship validation | Metadata | CanBeReferenced, CanManyToMany, etc. |
| Managed properties | Metadata | RetrieveManagedProperty, etc. |

### SDK Message Implementation Status

Based on decompiled Microsoft.Xrm.Sdk.Messages (61 messages total).

**Coverage:** 26 implemented (43%) | 35 not implemented (57%)

#### CRUD Operations (15/15 implemented)

| Message | Status | Priority |
|---------|--------|----------|
| Create, CreateMultiple | ✅ | - |
| Retrieve, RetrieveMultiple | ✅ | - |
| Update, UpdateMultiple | ✅ | - |
| Delete, DeleteMultiple | ✅ | - |
| Upsert, UpsertMultiple | ✅ | - |
| Associate, Disassociate | ✅ | - |
| ExecuteMultiple, ExecuteTransaction | ✅ | - |
| ExecuteAsync | ✅ | COMPLETE |

#### Metadata Operations (4/21 implemented)

| Message | Status | Priority | Notes |
|---------|--------|----------|-------|
| RetrieveEntity | ✅ | - | |
| RetrieveAttribute | ✅ | - | |
| RetrieveRelationship | ✅ | - | |
| RetrieveMetadataChanges | ✅ | - | |
| CreateEntity, DeleteEntity, UpdateEntity | ❌ | MEDIUM | Entity metadata CRUD |
| CreateAttribute, DeleteAttribute, UpdateAttribute | ❌ | MEDIUM | Attribute metadata CRUD |
| CreateManyToMany, CreateOneToMany | ❌ | MEDIUM | Relationship creation |
| DeleteRelationship, UpdateRelationship | ❌ | MEDIUM | Relationship management |
| RetrieveAllEntities | ❌ | MEDIUM | Bulk metadata retrieval |
| RetrieveEntityChanges | ❌ | MEDIUM | Change tracking / delta sync |
| CreateEntityKey, RetrieveEntityKey | ❌ | LOW | Alternate key metadata |
| DeleteEntityKey, ReactivateEntityKey | ❌ | LOW | Alternate key metadata |
| CreateCustomerRelationships | ❌ | LOW | Special customer lookup |

#### OptionSet Operations (6/8 implemented)

| Message | Status | Priority |
|---------|--------|----------|
| RetrieveOptionSet | ✅ | - |
| InsertOptionValue, InsertStatusValue | ✅ | - |
| CreateOptionSet | ✅ | COMPLETE |
| UpdateOptionSet | ✅ | COMPLETE |
| DeleteOptionSet | ✅ | COMPLETE |
| DeleteOptionValue | ❌ | LOW |
| UpdateOptionValue, UpdateStateValue | ❌ | MEDIUM |
| OrderOption | ❌ | LOW |
| RetrieveAllOptionSets | ❌ | MEDIUM |

#### Encryption/Security (0/3) - LOW priority

| Message | Notes |
|---------|-------|
| IsDataEncryptionActive | Encryption status check |
| RetrieveDataEncryptionKey | Get encryption key |
| SetDataEncryptionKey | Set encryption key |

#### Relationship Validation (0/6) - LOW priority

| Message | Notes |
|---------|-------|
| CanBeReferenced, CanBeReferencing | Lookup validation |
| CanManyToMany | N:N validation |
| GetValidManyToMany | Valid N:N targets |
| GetValidReferencedEntities | Valid lookup targets |
| GetValidReferencingEntities | Valid referencing entities |

#### Other (1/8)

| Message | Status | Priority |
|---------|--------|----------|
| RetrieveTimestamp | ❌ | LOW |
| RetrieveManagedProperty | ❌ | LOW |
| RetrieveAllManagedProperties | ❌ | LOW |
| ConvertDateAndTimeBehavior | ❌ | LOW |
| CreateAsyncJobToRevokeInheritedAccess | ❌ | LOW |

#### Additional Implemented (from Microsoft.Crm.Sdk.Messages)

Security/Sharing: GrantAccess, ModifyAccess, RevokeAccess, RetrievePrincipalAccess, RetrieveSharedPrincipalsAndAccess

Queue: AddToQueue, RemoveFromQueue, PickFromQueue

Team: AddMembersTeam, RemoveMembersTeam, AddUserToRecordTeam, RemoveUserFromRecordTeam

Marketing: AddMemberList, AddListMembersList

Sales: WinOpportunity, LoseOpportunity, QualifyLead, CloseQuote, WinQuote, ReviseQuote, CloseIncident

Other: Assign, SetState, WhoAmI, InitializeFrom, BulkDelete, CalculateRollupField, SendEmail, FetchXmlToQueryExpression, ExecuteFetch, PublishXml, UtcTimeFromLocalTime, RetrieveVersion

**Alternate Key Notes:**
- `RetrieveAllEntityKeysRequest` does NOT exist in SDK
- Key definitions retrieved via `RetrieveEntityRequest` with `EntityFilters.Keys`
- **Supported types:** Decimal, Integer, String, DateTime, Lookup, OptionSet
- **Money is NOT supported** as alternate key type ([MS Docs](https://learn.microsoft.com/en-us/power-apps/developer/data-platform/define-alternate-keys-entity))

---

## Won't Fix

| # | Title | Reason |
|---|-------|--------|
| 414 | Assembly version error | Version-specific, not applicable |
| 453 | VS2019 unit test hang | We target VS2022+ |
| 523 | Microsoft.CrmSdk.Extensions | Xrm.Client namespace deprecated |

---

## Custom Additions (Community Enhancements)

The following features were developed independently by the community and are NOT fixes for upstream issues. These represent new capabilities added to enhance the testing framework beyond the original FakeXrmEasy.

**Total Custom Additions: 18**

### Message Executors (7)

| Feature | Version | Description |
|---------|---------|-------------|
| ExecuteAsync | v1.0.3 | Request executor with AsyncOperation entity tracking |
| CreateOptionSetRequest | v1.1.0 | Creates global OptionSets in metadata repository |
| UpdateOptionSetRequest | v1.1.0 | Updates OptionSet DisplayName, Description properties |
| DeleteOptionSetRequest | v1.1.0 | Deletes global OptionSets with validation |
| CreateEntityRequest | Planned | Entity metadata creation |
| UpdateEntityRequest | Planned | Entity metadata updates |
| DeleteEntityRequest | Planned | Entity metadata deletion |

### Query Engine (1)

| Feature | Version | Description |
|---------|---------|-------------|
| Any/All Filter Operators | v1.1.0 | JoinOperator.Any/NotAny/All/NotAll support with constraint validation |

### Validation & Constraints (1)

| Feature | Version | Description |
|---------|---------|-------------|
| Alternate Key Constraints | v1.1.0 | 10 keys max per entity, 16 attrs max per key, type validation |

### Bug Fixes (1)

| Feature | Version | Description |
|---------|---------|-------------|
| ExecuteMultiple ContinueOnError | v1.0.3 | Response key fix + proper fault extraction |

### Core Framework (8)

| Feature | Version | Description |
|---------|---------|-------------|
| IPluginExecutionContext4 | v1.0.0 | Full interface hierarchy with Azure AD object IDs |
| SDK-Style Projects | v1.0.0 | Modern project format, automatic binding redirects |
| CalculateRollupFieldRequest | v1.0.1 | New executor for rollup field testing |
| Auto-Populate Entity Images | v1.0.2 | ExecutePluginWithTarget auto-retrieves pre/post images |
| Filtering Attributes Validation | v1.0.2 | Plugins only execute when filtering attrs present |
| CreateMultiple/UpdateMultiple/DeleteMultiple/UpsertMultiple | v1.0.2 | Bulk operation executors |
| Auto-Register Relationships | v1.0.2 | InitializeMetadata auto-registers N:N, 1:N, N:1 relationships |

---

## Needs Evaluation

The following 34 open issues from the archived repository have not yet been evaluated for inclusion.

**Open Issues (sorted by issue number):**

#74, #183, #218, #220, #258, #279, #287, #288, #293, #332, #340, #342, #354, #359, #372, #407, #415, #439, #444, #445, #449, #462, #467, #473, #490, #497, #501, #505, #516, #532, #550, #560, #579, #584

**Notes:**
- Issue #293 is documented as a Known Limitation (see above) but remains open upstream
- All closed issues have been removed from this list
- Issues we've fixed (#255, #451, #458, #470, #472, #476, #479, #482, #485, #491, #500, #506, #508, #509, #514, #521, #524, #539, #543, #545, #547, #551, #553, #555, #562, #566, #569, #573, #587, #607, #608, #610, #612, #615) are tracked in the "Fixed Issues" section above
- Issues marked Won't Fix (#414, #453, #523) are tracked in the "Won't Fix" section above

---

## Needs Investigation

All items resolved - see v1.2.0 Phase 1 above.

| # | Title | Status |
|---|-------|--------|
| 569 | ObjectTypeCode casting | **FIXED** in v1.2.0 |
| 566 | Upsert alt key copy | **FIXED** in v1.2.0 |

---

## Open PRs Summary

### Consider for Implementation
| PR | Title | Author | Notes |
|----|-------|--------|-------|
| - | All prioritized PRs implemented | - | See P0-P3 roadmap sections |

**Note:** We implement the spirit of community PRs independently rather than directly copying code, to ensure proper ownership and avoid copyright concerns.

### Already Integrated/Fixed
| PR | Title | Status |
|----|-------|--------|
| 557 | Expose Metadata generation | FIXED (Part 10) |
| 447 | PicklistAttributeMetadata options | FIXED (Part 10) |
| 461 | next-x-timeperiod operators | FIXED (Part 9) |
| 460 | last-x-weeks operator | FIXED (Part 9) |
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
- VS2019/VS2022/VS2026

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

### 2026-01-07 (Part 13) - Phase 4a Complete: OptionSet CRUD Operations

- Added: CreateOptionSetRequest executor
  - Creates global OptionSets in metadata repository
  - Validates OptionSet name uniqueness
  - Supports OptionSetMetadata with options
  - Returns MetadataId on success
- Added: UpdateOptionSetRequest executor
  - Updates DisplayName, Description, HasChanged properties
  - Validates OptionSet existence before update
  - Throws FaultException for non-existent OptionSets
- Added: DeleteOptionSetRequest executor
  - Deletes global OptionSets from metadata repository
  - Validates existence before deletion
  - Throws FaultException for non-existent OptionSets
- Added: 24 new tests across three test files
  - CreateOptionSetRequestTests.cs
  - UpdateOptionSetRequestTests.cs
  - DeleteOptionSetRequestTests.cs
- SDK Message Coverage: 26/61 (43%) - OptionSet operations now 6/8
- All 1198 tests pass
- Phase 4a status: COMPLETE

### 2026-01-07 (Part 12) - Phase 3 Complete: Key Constraints & Validation

- Fixed: Alternate key limits enforcement
  - Maximum 10 alternate keys per entity
  - Maximum 16 columns/attributes per key
  - Validated in `AddAlternateKey()` method with FaultException on violation
- Fixed: Attribute type enforcement for alternate keys
  - Added `ValidateAlternateKeyConstraints()` method in XrmFakedContext.Crud.cs
  - Only allowed types: Decimal, Integer, BigInt, String, DateTime, Lookup, Customer, Picklist, State, Status
  - Money explicitly NOT supported (matches Dataverse behavior per MS documentation)
- Fixed: Removed Money support from `CompareKeyValues()` with documentation
  - Money is not a valid alternate key type in Dataverse
- Added: 26 comprehensive tests in AlternateKeyConstraintTests.cs
  - Tests for maximum key count limit
  - Tests for maximum attributes per key limit
  - Tests for each supported attribute type
  - Tests for rejected types (Money, Boolean, etc.)
- All 1174 tests pass
- Phase 3 status: COMPLETE

### 2026-01-07 (Part 11) - Phase 2 Complete: Any/All Filter Operators

- Fixed: Any/All related-record filtering (JoinOperator.Any/NotAny/All/NotAll)
  - Added `TranslateAnyAllLinkedEntityToLinq()` method (~170 lines) in XrmFakedContext.Queries.cs
  - Added helper methods: `NormalizeKey()`, `EvaluateAllOperator()`, `EvaluateNotAllOperator()`
  - FetchXML `link-type="any|not any|all|not all"` parsing already supported
  - Constraint validation enforced:
    - Any/All LinkEntity must be direct child of root entity (no nested parent)
    - Any/All LinkEntity cannot have nested link-entity children
    - No attributes/columns allowed on Any/All link-entities
    - Filters ARE supported inside Any/All link-entities
  - 26 comprehensive tests in AnyAllFilterTests.cs
  - All 1148 tests pass
- Phase 2 status: COMPLETE

### 2026-01-07 (Part 10) - P3 Complete + ExecuteAsync

- Fixed: ExecuteAsync request executor
  - Creates asyncoperation entity to track job
  - Executes wrapped request immediately (in-memory simulation)
  - Returns AsyncJobId matching Dataverse behavior
  - 9 new tests in ExecuteAsyncRequestTests.cs
- Fixed: #557 - MetadataGenerator now public
  - Added `FromEarlyBoundEntity<T>()` and `FromEarlyBoundEntity(Type)` methods
  - Made `CreateAttributeMetadataByType()` public for external use
  - Comprehensive XML documentation
  - 23 new tests in MetadataGeneratorTests.cs
- Fixed: #447 - PicklistAttributeMetadata options fidelity
  - RetrieveAttributeRequestExecutor populates OptionSet from OptionSetValuesMetadata
  - Works with both PicklistAttributeMetadata and StateAttributeMetadata
  - 6 new tests for OptionSet behavior
- Fixed: ExecuteMultiple ContinueOnError behavior
  - Fixed response key bug ("response.Responses" → "Responses")
  - Enhanced fault extraction with ErrorCode and TraceText preservation
  - Proper behavior for ReturnResponses=false with faults
  - 8 new tests for ContinueOnError scenarios
- Fixed: README placeholder cleanup
  - Removed GitHub Actions badge with YOUR_ORG placeholder
  - Updated Sponsorship section to Community/Support
- SDK Message Coverage: 23/61 (38%) - CRUD operations now 100%

### 2026-01-07 (Part 9) - P2 Complete
- Verified: #461 - next-x-timeperiod operators already implemented
  - TranslateConditionExpressionNext supports NextXDays, NextXWeeks, NextXMonths, NextXYears
- Verified: #460 - last-x-weeks operators already implemented
  - TranslateConditionExpressionLast supports LastXDays, LastXWeeks, LastXMonths, LastXYears
- Fixed: #476 - Fiscal Period Operators
  - Added 5 new operators: InFiscalPeriod, InFiscalPeriodAndYear, ThisFiscalPeriod, LastFiscalPeriod, NextFiscalPeriod
  - Added FiscalYearSettings with Template enum (Annually, SemiAnnually, Quarterly, Monthly, FourWeek)
  - Helper methods: GetPeriodsPerYear, GetFiscalPeriodBounds, GetCurrentFiscalPeriod, GetOffsetFiscalPeriod
  - Period rollover at fiscal year boundary handled correctly
  - FetchXML support for all new operators
  - 13 new tests in FiscalPeriodOperatorTests.cs
- Fixed: #509 - LIKE Wildcards [X-Y] Character Ranges
  - Added underscore (_) single-character wildcard support
  - Added [A-Z] and [0-9] character range patterns
  - Added [abc] character set patterns
  - Added [^abc] negated character set patterns
  - Mixed patterns supported (e.g., `[A-Z]_%` for "starts with letter, at least 2 chars")
  - Backward compatibility maintained for simple % patterns (uses StartsWith/EndsWith/Contains)
  - Advanced patterns converted to regex with ConvertLikePatternToRegex helper
  - 30 new tests in LikeAdvancedPatternTests.cs
- Deferred: Any/All related-record filtering to P3 (requires substantial LINQ changes)
- P2 status: 4 fixed/verified, 1 deferred

### 2026-01-07 (Part 8) - P1 Complete
- Fixed: #521 - Composite Alternate Keys + Uniqueness Enforcement
  - Added `FindViolatedAlternateKey()` method to check uniqueness on Create/Update
  - Added `CompareKeyValues()` for EntityReference/OptionSetValue/Money comparison
  - Added `AddAlternateKey()` helper methods for easier test setup
  - Throws FaultException when duplicate key values detected
  - 22 new tests in CompositeAlternateKeyTests.cs
- Fixed: #553 - RowVersion / Optimistic Concurrency
  - Added auto-incrementing `versionnumber` to all entities on Create/Update
  - Thread-safe `GetNextVersionNumber()` using `Interlocked.Increment`
  - `UpdateRequestExecutor` validates `ConcurrencyBehavior.IfRowVersionMatches`
  - Throws FaultException on version mismatch or missing RowVersion
  - 14 new tests in RowVersionTests.cs
- Fixed: #508 - Alternate Keys in Associate/Disassociate
  - `AssociateRequestExecutor` now resolves KeyAttributes to entity IDs
  - `DisassociateRequestExecutor` now resolves KeyAttributes to entity IDs
- Fixed: #472 - OwningBusinessUnit on Assign
  - `AssignRequestExecutor` now updates `owningbusinessunit`
  - Retrieves businessunitid from new owner's systemuser record
  - 2 new tests for business unit ownership transfer
- Fixed: #470 - GetRecordUniqueId Missing Throw
  - Now throws FaultException when no matching record found for alternate key
  - Consistent with Dataverse behavior for invalid alternate keys
- P1 status: 5 fixed

### 2026-01-07 (Part 7) - P0 Complete
- Documented: #293 - Added Known Limitation section with workaround for output parameters
- Fixed: #491 - DateTime Kind conversion now matches real Dataverse behavior
  - Created integration tests in `IntegrationTests/DataverseDateTimeInvestigation.cs`
  - Verified against real Dataverse: Local→UTC converted, Utc/Unspecified stored as-is
  - Updated `ConvertToUtc()` to properly handle DateTimeKind.Local
  - Fixed 5 tests that were using Local kind (now use UTC for Dataverse parity)
- P0 status: 5 fixed, 1 documented

### 2026-01-07 (Part 6) - P0 Parity Bug Fixes
- Fixed: #479 - Statecode on Create now matches Dataverse behavior
  - Allow explicit Active state (0) on Create
  - Throw FaultException for Inactive state (1+) - Dataverse requires Create+Update pattern
  - System still defaults to Active if not specified
- Fixed: #562 - Min date validation 01/01/1753
  - Added CrmMinDateTime constant and ValidateDateTime method
  - Throws FaultException for dates before 1753 (SQL Server limitation)
  - Validates during Create, Update, and Initialize operations
- Fixed: #573 - Pipeline NRE in RegisterPluginStep<TPlugin, TEntity>
  - Added null-safe EntityTypeCode field access
  - Provides descriptive error message with resolution steps
- Fixed: #458 - DateTime.Kind differences for DateOnly/TimeZoneIndependent fields
  - DateOnly fields now return DateTimeKind.Unspecified (was incorrectly UTC)
  - Added TimeZoneIndependent behavior support (also returns Unspecified)
  - UserLocal fields continue to return DateTimeKind.Utc
- Deferred: #491 - UTC conversion (needs migration planning to avoid breaking changes)
- Bug fix: QueryByAttributeTests - DateTime(1980) was creating ticks, not year

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
