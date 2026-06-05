# RepositoryManager – Test Scenarios

## 1. Register

| # | Scenario | Input | Expected Result |
|---|----------|-------|-----------------|
| 1.1 | Register a valid JSON item | `itemName="config"`, `itemContent="{\"key\":\"value\"}"`, `itemType=1` | No exception; item stored |
| 1.2 | Register a valid XML item | `itemName="doc"`, `itemContent="<root><node/></root>"`, `itemType=2` | No exception; item stored |
| 1.3 | Register duplicate name | Call Register twice with the same `itemName` | Second call throws `InvalidOperationException` (no overwrite) |
| 1.4 | Invalid JSON content with itemType=1 | `itemContent="not json"`, `itemType=1` | Throws `ArgumentException` |
| 1.5 | Invalid XML content with itemType=2 | `itemContent="<unclosed>"`, `itemType=2` | Throws `ArgumentException` |
| 1.6 | Wrong type for content (JSON string but itemType=2) | `itemContent="{}"`, `itemType=2` | Throws `ArgumentException` |
| 1.7 | Null item name | `itemName=null` | Throws `ArgumentException` |
| 1.8 | Empty item name | `itemName=""` | Throws `ArgumentException` |
| 1.9 | Null content | `itemContent=null` | Throws `ArgumentNullException` |
| 1.10 | Unknown itemType | `itemType=99` | Throws `ArgumentException` |

## 2. Retrieve

| # | Scenario | Input | Expected Result |
|---|----------|-------|-----------------|
| 2.1 | Retrieve a registered JSON item | `itemName="config"` (previously registered) | Returns the original JSON string |
| 2.2 | Retrieve a registered XML item | `itemName="doc"` (previously registered) | Returns the original XML string |
| 2.3 | Retrieve non-existent item | `itemName="ghost"` | Throws `KeyNotFoundException` |
| 2.4 | Retrieve after Deregister | Register then Deregister an item, then Retrieve | Throws `KeyNotFoundException` |

## 3. GetType

| # | Scenario | Input | Expected Result |
|---|----------|-------|-----------------|
| 3.1 | GetType of a JSON item | `itemName` registered with `itemType=1` | Returns `1` |
| 3.2 | GetType of an XML item | `itemName` registered with `itemType=2` | Returns `2` |
| 3.3 | GetType of non-existent item | `itemName="ghost"` | Throws `KeyNotFoundException` |

## 4. Deregister

| # | Scenario | Input | Expected Result |
|---|----------|-------|-----------------|
| 4.1 | Deregister an existing item | Register an item, then Deregister it | No exception; Retrieve afterwards throws `KeyNotFoundException` |
| 4.2 | Deregister non-existent item | `itemName="ghost"` | Throws `KeyNotFoundException` |
| 4.3 | Re-register after Deregister | Deregister an item, then Register it again with same name | Second Register succeeds (slot freed) |

## 5. Initialize

| # | Scenario | Expected Result |
|---|----------|-----------------|
| 5.1 | Call Initialize once | No exception |
| 5.2 | Call Initialize twice | Second call throws `InvalidOperationException` |
| 5.3 | Use repository without calling Initialize | All operations work normally (constructor initialises state) |

## 6. Thread Safety (concurrent scenarios)

| # | Scenario | Expected Result |
|---|----------|-----------------|
| 6.1 | Concurrent Register with different names | All items stored without data corruption |
| 6.2 | Concurrent Register with the same name | Exactly one succeeds; the rest throw `InvalidOperationException` |
| 6.3 | Concurrent Retrieve while Register is in progress | Retrieve either finds the item or throws `KeyNotFoundException`; no corrupt state |
| 6.4 | Concurrent Deregister of the same item | Exactly one succeeds; the rest throw `KeyNotFoundException` |
| 6.5 | Concurrent Initialize calls | Exactly one succeeds; the rest throw `InvalidOperationException` |
