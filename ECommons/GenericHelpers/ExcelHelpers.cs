using Dalamud.Game;
using ECommons.DalamudServices;
using ECommons.ExcelServices.Sheets;
using Lumina.Excel;
using Lumina.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace ECommons;
public static unsafe partial class GenericHelpers
{
    public static ExcelSheet<T> GetSheet<T>(ClientLanguage? language = null) where T : struct, IExcelRow<T>
        => Svc.Data.GetExcelSheet<T>(language ?? Svc.ClientState.ClientLanguage)!;

    public static ExcelSheet<T> GetSheet<T>(string sheetName, ClientLanguage? language = null) where T : struct, IExcelRow<T>
        => Svc.Data.GetExcelSheet<T>(language ?? Svc.ClientState.ClientLanguage, sheetName)!;

    public static SubrowExcelSheet<T> GetSubrowSheet<T>(ClientLanguage? language = null) where T : struct, IExcelSubrow<T>
        => Svc.Data.GetSubrowExcelSheet<T>(language ?? Svc.ClientState.ClientLanguage)!;

    public static int GetRowCount<T>() where T : struct, IExcelRow<T>
        => GetSheet<T>().Count;

    #region Rows
    public static T? GetRow<T>(uint rowId, ClientLanguage? language = null) where T : struct, IExcelRow<T>
        => GetSheet<T>(language).GetRowOrDefault(rowId);

    public static T? FindRow<T>(Func<T, bool> predicate) where T : struct, IExcelRow<T>
         => GetSheet<T>().FirstOrNull(predicate);

    public static T[] FindRows<T>(Func<T, bool> predicate) where T : struct, IExcelRow<T>
        => GetSheet<T>().Where(predicate).ToArray();

    public static bool TryGetRow<T>(uint rowId, [NotNullWhen(returnValue: true)] out T row) where T : struct, IExcelRow<T>
        => GetSheet<T>().TryGetRow(rowId, out row);

    public static bool TryGetRow<T>(uint rowId, ClientLanguage? language, [NotNullWhen(returnValue: true)] out T row) where T : struct, IExcelRow<T>
        => GetSheet<T>(language).TryGetRow(rowId, out row);

    public static bool TryGetRow<T>(string sheetName, uint rowId, out T row) where T : struct, IExcelRow<T>
        => TryGetRow(sheetName, rowId, null, out row);

    public static bool TryGetRow<T>(string sheetName, uint rowId, ClientLanguage? language, out T row) where T : struct, IExcelRow<T>
        => GetSheet<T>(sheetName).TryGetRow(rowId, out row);

    public static bool TryFindRow<T>(Predicate<T> predicate, out T row) where T : struct, IExcelRow<T>
        => GetSheet<T>().TryGetFirst(predicate, out row);

    public static bool TryFindRow<T>(Predicate<T> predicate, ClientLanguage? language, out T row) where T : struct, IExcelRow<T>
        => GetSheet<T>(language).TryGetFirst(predicate, out row);
    #endregion

    #region Subrows
    public static T? GetRow<T>(uint rowId, ushort subRowId, ClientLanguage? language = null) where T : struct, IExcelSubrow<T>
        => GetSubrowSheet<T>(language).GetSubrowOrDefault(rowId, subRowId);

    public static SubrowCollection<T>? GetSubRow<T>(uint rowId, ClientLanguage? language = null) where T : struct, IExcelSubrow<T>
        => GetSubrowSheet<T>(language).GetRowOrDefault(rowId);

    public static bool TryGetRow<T>(uint rowId, ushort subRowId, [NotNullWhen(returnValue: true)] out T row) where T : struct, IExcelSubrow<T>
        => GetSubrowSheet<T>().TryGetSubrow(rowId, subRowId, out row);

    public static bool TryGetRow<T>(uint rowId, ushort subRowId, ClientLanguage? language, [NotNullWhen(returnValue: true)] out T row) where T : struct, IExcelSubrow<T>
        => GetSubrowSheet<T>(language).TryGetSubrow(rowId, subRowId, out row);

    public static T? FindRow<T>(Func<T, bool> predicate, ClientLanguage? language = null) where T : struct, IExcelSubrow<T>
        => GetSubrowSheet<T>(language).SelectMany(m => m).Cast<T?>().FirstOrDefault(t => predicate(t.Value), null);
    #endregion

    #region Rawrows
    public static bool TryGetRawRow(string sheetName, uint rowId, out RawRow rawRow)
        => TryGetRow(sheetName, rowId, out rawRow);

    public static bool TryGetRawRow(string sheetName, uint rowId, ClientLanguage? language, out RawRow rawRow)
        => TryGetRow(sheetName, rowId, language, out rawRow);
    #endregion

    public static IEnumerable<T> AllRows<T>(this SubrowExcelSheet<T> subrowSheet) where T : struct, IExcelSubrow<T>
    {
        foreach(var x in subrowSheet)
        {
            foreach(var z in x)
            {
                yield return z;
            }
        }
    }

    public static bool TryGetValue<T>(this RowRef<T> rowRef, out T value) where T : struct, IExcelRow<T>
    {
        if(rowRef.ValueNullable != null)
        {
            value = rowRef.Value;
            return true;
        }
        value = default;
        return false;
    }

    public static TExtension GetExtension<TExtension, TBase>(this TBase row) where TExtension : struct, IExcelRow<TExtension>, IRowExtension<TExtension, TBase> where TBase : struct, IExcelRow<TBase>
        => TExtension.GetExtended(row);
}
