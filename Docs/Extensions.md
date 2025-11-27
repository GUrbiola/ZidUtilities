# `ZidUtilities.CommonCode.Extensions` — API Documentation

## Overview
`Extensions` is a large static class that provides many utility extension methods and helpers across common .NET types (strings, collections, DateTime, IO, drawing color, ADO.NET `DataTable`/SQL helpers, XML helpers, token parsing helpers for SQL-like text, and more). These methods are designed to be used as extension methods (where applicable) and as general helper functions.

Namespace: `ZidUtilities.CommonCode`  
Class: `public static class Extensions`

---

## Quick usage

````````

---

## Table of contents
- Fields & Constants
- Empty Extensions
- Boolean helpers
- Byte array helpers
- Char helpers & token character predicates
- `System.Drawing.Color` helpers
- `DataTable` helpers
- `DateTime` helpers
- `HttpSessionStateBase` helpers
- `ICollection`, `IList`, `IEnumerable` helpers
- `IDataRecord` helpers
- Integer helpers
- `IQueryable` ordering helpers
- `List<T>` helpers & bulk insert
- Long helpers (AD ticks)
- Object helpers & (de)serialization
- `Point`, `Random`, `Stream`, `StringBuilder` helpers
- String helpers (very extensive)
- EzSql token parsing helpers
- Generic type helpers
- `XmlWriter` helpers
- Notes

---

## Fields & Constants

- `public static HashSet<string> ReservedWords`
  - SQL reserved words used by the EzSql token helpers.
- `public const long NonExpiringTickValue`
  - Constant used as sentinel for "non-expiring" AD tick dates. Value: `9223372036854775807`.
- `public static HashSet<string> SqlDataTypes`
  - Common SQL Server types used by some SQL helpers.

---

## Empty Extensions

- `public static string GenerateToken()`
  - Returns a base64-encoded GUID-derived token (convenience).

---

## Extensions for booleans

- `public static string BoolAsString(this bool boolean)`
  - Returns `"true"` or `"false"`.

---

## Extensions for byte arrays

- `public static System.Drawing.Image AsImage(this byte[] rawData)`
  - Converts raw image bytes to a `System.Drawing.Image`.

---

## Extensions for char (predicates & converters)

- `public static int FromHexCharToDecInt(this char c)`
  - Convert a single hex character (`0`-`F`/`f`) to its integer value, or `-1` if invalid.

Private character tests (used throughout token parsing):
- `IsCloseBracket`, `IsComma`, `IsComparator`, `IsOpenBracket`, `IsOperator`, `IsSemmiColon`, `IsStringOperator`, `IsWhiteSpace`
  - Return boolean about the character category.

---

## Extensions for `System.Drawing.Color`

- `public static string ColorToString(this Color htmlColor)`
  - Convert a `Color` to its HTML hex string.
- `public static Color HalfMix(this Color one, Color two)`
  - Returns the half-average color of two inputs.
- `public static Color GetHighContrastColor(this Color color)`
  - Returns `Color.Black` or `Color.White` depending on perceived luminance to maximize contrast.
- `public static Color Variate(this Color color, int red, int green, int blue)`
  - Randomly vary the color by given max deltas (uses `Random` — note RNG seeded per call).

---

## Extensions for `DataTable`

- `public static void SaveToCsv(this DataTable dt, string fileName)`
  - Writes the table to a CSV file. Uses column names as header and `EnsureCsvField` to quote values.
- `public static string GetCreateTableSql(this DataTable table)`
  - Produces a SQL `CREATE TABLE` script (and `ALTER TABLE` default constraint scripts) based on `DataTable` schema. Useful for generating SQL Server DDL from a `DataTable`.

---

## Extensions for `DateTime`

A number of convenience and AD-tick conversion helpers:

- AD ticks conversions (Active Directory / filetime-like):
  - `public static long DateToAdTicks(this DateTime? date)`
  - `public static long DateToADTicks(this DateTime? date)`
  - `public static long DateToAdTicks(this DateTime date)`
  - `public static long DateToADTicks(this DateTime date)`
  - These convert `DateTime` / nullable `DateTime` into AD-tick integer. When date is invalid/unset returns `NonExpiringTickValue`.

- `public static DateTime? ADTicksToDate(this long ticks)` / `TicksToDate(this long ticks)`
  - Convert AD ticks back into `DateTime?`.

- Comparisons & checks:
  - `IsDateEqualTo` overloads compare by Date portion for `DateTime?` vs `DateTime?` or `DateTime`.
  - `IsEqualTo` overloads also compare date-only equality.

- Convenience:
  - `SafeStringDate(this DateTime? dt)` -> formatted "MM/dd/yyyy" or empty.
  - `BeginningOfTheMonth`, `ChangeTime`, `FirstSecondOfDay`, `LastSecondOfDay`, `GetLastDayOfMonth`
  - `Elapsed(this DateTime input)` -> `TimeSpan` since `input`.
  - `IsBetween(this DateTime dt, DateTime startDate, DateTime endDate, Boolean compareTime = false)`
  - `NextDayOfWeek`, `NextSunday`, `StartOfWeek`, `ThisWeekFriday`, `ThisWeekMonday`
  - `TimeElapsed()` -> formatted duration.
  - `ToIsoDate(this DateTime value)` -> "yyyyMMdd HHmmss.ff"
  - `IsWeekday(this DayOfWeek d)`, `IsWeekend(this DayOfWeek d)`
  - `HasPositiveValue(this double? number)`

---

## Extensions for `HttpSessionStateBase`

- `public static T GetValue<T>(this HttpSessionStateBase session, string key)`
- `public static T GetValue<T>(this HttpSessionStateBase session, string key, T defaultValue)`
  - Get and convert session value to `T` using `Convert.ChangeType` when available; returns provided default otherwise.

---

## Extensions for `ICollection<T>` and `IList<T>`

- `public static DataTable ConvertToDataTable<T>(this ICollection<T> list, string tableName = "")`
- `public static DataTable ConvertToDataTable<T>(this IList<T> list, string tableName = "")`
  - Build a `DataTable` from POCO objects using property descriptors.

- `public static void ExportToCSV<T>(this ICollection<T> data, string fileName)`
  - Export list to CSV (writes header and rows using public properties).

- `public static void Shuffle<T>(this IList<T> list)`
  - In-place Fisher–Yates shuffle.

---

## Extensions for `IDataRecord`

- `public static bool HasColumn(this IDataRecord dr, string columnName)`
  - Returns whether the record contains a column (case-insensitive).

---

## Extensions for `IEnumerable<T>`

- `public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)`
  - True if null or has no elements.
- `public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)`
  - Returns distinct elements using a projection key.

---

## Extensions for `int`

- `public static string EnsureCsvField(this int? integer)`
- `public static string FromDecIntToHexChar(this int c)` (0..15 -> hex char)
- `public static string FromDecIntToHexString(this int val)` -> base-16 string
- `public static string GenerateRandomString(int length)` -> GUID-based random hex string
- `public static int InRange(this int x, int lo, int hi)` -> clamps `x` between `lo` and `hi` (swaps limits if reversed)
- `public static bool IsInRange(this int x, int lo, int hi)` -> inclusive range check

---

## Extensions for `IQueryable<T>` (dynamic ordering)

- A set of `OrderBy`, `OrderByDescending`, `ThenBy`, `ThenByDescending` overloads accepting `string propertyName` and optional `direction`.
- Implementation uses LINQ expression tree building with `OrderingHelper<T>(...)` to create `OrderBy`/`ThenBy` calls dynamically.

Usage:
