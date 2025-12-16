using Dalamud.Game;
using ECommons.DalamudServices;
using ECommons.ExcelServices.Sheets;
using FFXIVClientStructs;
using Lumina.Data;
using Lumina.Excel;
using Lumina.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace ECommons;
public static unsafe partial class GenericHelpers
{
    /// <summary>
    /// Converts RowRef into RowRef with another language
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="rowRef"></param>
    /// <param name="language"></param>
    /// <returns></returns>
    public static RowRef<T> WithLanguage<T>(this RowRef<T> rowRef, ClientLanguage? language) where T : struct, IExcelRow<T>
    {
        return new RowRef<T>(Svc.Data.Excel, rowRef.RowId, language switch
        {
            ClientLanguage.Japanese => Lumina.Data.Language.Japanese,
            ClientLanguage.German => Lumina.Data.Language.German,
            ClientLanguage.French => Lumina.Data.Language.French,
            ClientLanguage.English => Lumina.Data.Language.English,
            _ => null
        });
    }

    /// <summary>
    /// Converts RowRef into RowRef with another language
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="rowRef"></param>
    /// <param name="language"></param>
    /// <returns></returns>
    public static RowRef<T> WithLanguage<T>(this RowRef<T> rowRef, Lumina.Data.Language? language) where T : struct, IExcelRow<T>
    {
        return new RowRef<T>(Svc.Data.Excel, rowRef.RowId, language);
    }

    /// <summary>
    /// TODO: document
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="language"></param>
    /// <returns></returns>
    public static ExcelSheet<T> GetSheet<T>(ClientLanguage? language = null) where T : struct, IExcelRow<T>
        => Svc.Data.GetExcelSheet<T>(language ?? Svc.ClientState.ClientLanguage)!;

    /// <summary>
    /// TODO: document
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="sheetName"></param>
    /// <param name="language"></param>
    /// <returns></returns>
    public static ExcelSheet<T> GetSheet<T>(string sheetName, ClientLanguage? language = null) where T : struct, IExcelRow<T>
        => Svc.Data.GetExcelSheet<T>(language ?? Svc.ClientState.ClientLanguage, sheetName)!;

    /// <summary>
    /// TODO: document
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="language"></param>
    /// <returns></returns>
    public static SubrowExcelSheet<T> GetSubrowSheet<T>(ClientLanguage? language = null) where T : struct, IExcelSubrow<T>
        => Svc.Data.GetSubrowExcelSheet<T>(language ?? Svc.ClientState.ClientLanguage)!;

    /// <summary>
    /// TODO: document
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static int GetRowCount<T>() where T : struct, IExcelRow<T>
        => GetSheet<T>().Count;

    #region Rows
    /// <summary>
    /// TODO: document
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="rowId"></param>
    /// <param name="language"></param>
    /// <returns></returns>
    public static T? GetRow<T>(uint rowId, ClientLanguage? language = null) where T : struct, IExcelRow<T>
        => GetSheet<T>(language).GetRowOrDefault(rowId);

    /// <summary>
    /// TODO: document
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static T? FindRow<T>(Func<T, bool> predicate) where T : struct, IExcelRow<T>
         => GetSheet<T>().FirstOrNull(predicate);

    /// <summary>
    /// TODO: document
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static T[] FindRows<T>(Func<T, bool> predicate) where T : struct, IExcelRow<T>
        => GetSheet<T>().Where(predicate).ToArray();

    /// <summary>
    /// TODO: document
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="rowId"></param>
    /// <param name="row"></param>
    /// <returns></returns>
    public static bool TryGetRow<T>(uint rowId, [NotNullWhen(returnValue: true)] out T row) where T : struct, IExcelRow<T>
        => GetSheet<T>().TryGetRow(rowId, out row);

    /// <summary>
    /// TODO: document
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="rowId"></param>
    /// <param name="language"></param>
    /// <param name="row"></param>
    /// <returns></returns>
    public static bool TryGetRow<T>(uint rowId, ClientLanguage? language, [NotNullWhen(returnValue: true)] out T row) where T : struct, IExcelRow<T>
        => GetSheet<T>(language).TryGetRow(rowId, out row);

    /// <summary>
    /// TODO: document
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="sheetName"></param>
    /// <param name="rowId"></param>
    /// <param name="row"></param>
    /// <returns></returns>
    public static bool TryGetRow<T>(string sheetName, uint rowId, out T row) where T : struct, IExcelRow<T>
        => TryGetRow(sheetName, rowId, null, out row);

    /// <summary>
    /// TODO: document
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="sheetName"></param>
    /// <param name="rowId"></param>
    /// <param name="language"></param>
    /// <param name="row"></param>
    /// <returns></returns>
    public static bool TryGetRow<T>(string sheetName, uint rowId, ClientLanguage? language, out T row) where T : struct, IExcelRow<T>
        => GetSheet<T>(sheetName).TryGetRow(rowId, out row);

    /// <summary>
    /// TODO: document
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="predicate"></param>
    /// <param name="row"></param>
    /// <returns></returns>
    public static bool TryFindRow<T>(Predicate<T> predicate, out T row) where T : struct, IExcelRow<T>
        => GetSheet<T>().TryGetFirst(predicate, out row);

    /// <summary>
    /// TODO: document
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="predicate"></param>
    /// <param name="language"></param>
    /// <param name="row"></param>
    /// <returns></returns>
    public static bool TryFindRow<T>(Predicate<T> predicate, ClientLanguage? language, out T row) where T : struct, IExcelRow<T>
        => GetSheet<T>(language).TryGetFirst(predicate, out row);
    #endregion

    #region Subrows
    /// <summary>
    /// TODO: document
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="rowId"></param>
    /// <param name="subRowId"></param>
    /// <param name="language"></param>
    /// <returns></returns>
    public static T? GetRow<T>(uint rowId, ushort subRowId, ClientLanguage? language = null) where T : struct, IExcelSubrow<T>
        => GetSubrowSheet<T>(language).GetSubrowOrDefault(rowId, subRowId);

    /// <summary>
    /// TODO: document
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="rowId"></param>
    /// <param name="language"></param>
    /// <returns></returns>
    public static SubrowCollection<T>? GetSubRow<T>(uint rowId, ClientLanguage? language = null) where T : struct, IExcelSubrow<T>
        => GetSubrowSheet<T>(language).GetRowOrDefault(rowId);

    /// <summary>
    /// TODO: document
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="rowId"></param>
    /// <param name="subRowId"></param>
    /// <param name="row"></param>
    /// <returns></returns>
    public static bool TryGetRow<T>(uint rowId, ushort subRowId, [NotNullWhen(returnValue: true)] out T row) where T : struct, IExcelSubrow<T>
        => GetSubrowSheet<T>().TryGetSubrow(rowId, subRowId, out row);

    /// <summary>
    /// TODO: document
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="rowId"></param>
    /// <param name="subRowId"></param>
    /// <param name="language"></param>
    /// <param name="row"></param>
    /// <returns></returns>
    public static bool TryGetRow<T>(uint rowId, ushort subRowId, ClientLanguage? language, [NotNullWhen(returnValue: true)] out T row) where T : struct, IExcelSubrow<T>
        => GetSubrowSheet<T>(language).TryGetSubrow(rowId, subRowId, out row);

    /// <summary>
    /// TODO: document
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="predicate"></param>
    /// <param name="language"></param>
    /// <returns></returns>
    public static T? FindRow<T>(Func<T, bool> predicate, ClientLanguage? language = null) where T : struct, IExcelSubrow<T>
        => GetSubrowSheet<T>(language).SelectMany(m => m).Cast<T?>().FirstOrDefault(t => predicate(t.Value), null);
    #endregion

    #region Rawrows
    /// <summary>
    /// TODO: document
    /// </summary>
    /// <param name="sheetName"></param>
    /// <param name="rowId"></param>
    /// <param name="rawRow"></param>
    /// <returns></returns>
    public static bool TryGetRawRow(string sheetName, uint rowId, out RawRow rawRow)
        => TryGetRow(sheetName, rowId, out rawRow);

    /// <summary>
    /// TODO: document
    /// </summary>
    /// <param name="sheetName"></param>
    /// <param name="rowId"></param>
    /// <param name="language"></param>
    /// <param name="rawRow"></param>
    /// <returns></returns>
    public static bool TryGetRawRow(string sheetName, uint rowId, ClientLanguage? language, out RawRow rawRow)
        => TryGetRow(sheetName, rowId, language, out rawRow);
    #endregion

    /// <summary>
    /// TODO: document
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="subrowSheet"></param>
    /// <returns></returns>
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

    /// <summary>
    /// TODO: document
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="rowRef"></param>
    /// <param name="value"></param>
    /// <returns></returns>
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
    
    /// <summary>
    /// TODO: document
    /// </summary>
    /// <typeparam name="TExtension"></typeparam>
    /// <typeparam name="TBase"></typeparam>
    /// <param name="row"></param>
    /// <returns></returns>
    public static TExtension GetExtension<TExtension, TBase>(this TBase row) where TExtension : struct, IExcelRow<TExtension>, IRowExtension<TExtension, TBase> where TBase : struct, IExcelRow<TBase>
        => TExtension.GetExtended(row);

    extension<T>(IExcelRow<T> sheet) where T : struct, IExcelRow<T>
    {
        /// <summary>
        /// Gets RowRef of an Excel sheet. 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public static RowRef<T> GetRef(uint id, Lumina.Data.Language? language = null)
        {
            return new(Svc.Data.Excel, id, language);
        }

        public static T Get(uint id, Dalamud.Game.ClientLanguage? language = null)
        {
            return Svc.Data.GetExcelSheet<T>(language: language).GetRow(id);
        }

        public static ExcelSheet<T> Values
        {
            get
            {
                return Svc.Data.GetExcelSheet<T>();
            }
        }
    }
}