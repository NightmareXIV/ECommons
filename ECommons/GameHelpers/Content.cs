using System.Linq;
using ECommons.DalamudServices;
using ECommons.ExcelServices;
using Lumina.Data;
using Lumina.Excel.Sheets;
using TerritoryHelper = ECommons.TerritoryName;
using SheetContentType = Lumina.Excel.Sheets.ContentType;

#nullable disable

namespace ECommons.GameHelpers;

/// <summary>
///     Primary types of actual (in regard to combat) content in the game.
/// </summary>
/// <seealso cref="Content.DetermineContentType" />
public enum ContentType
{
    Unknown,
    /// <summary>
    ///     This encompasses anything that isn't in a roulette or a field operation,
    ///     quest instances, actual over world content, housing, for-fun content,
    ///     etc.
    /// </summary>
    OverWorld,
    PVP,
    Dungeon,
    DeepDungeon,
    Variant,
    /// This includes Criterion Savage
    Criterion,
    Trial,
    /// Bozja, Eureka, Diadem, etc.
    FieldOperations,
    /// Delubrum Reginae, Dalriada, etc.
    FieldRaid,
    ARaid,
    Raid,
}

/// <summary>
///     All the difficulties of content in the game.
/// </summary>
/// <seealso cref="Content.DetermineContentDifficulty" />
public enum ContentDifficulty
{
    Unknown,
    /// <summary>
    ///     This encompasses anything that doesn't otherwise have an explicit
    ///     difficulty, or is the lowest difficulty, variant dungeons, field raids,
    ///     etc.
    /// </summary>
    Normal,
    Hard,
    Unreal,
    /// Only Delubrum Reginae Savage
    FieldRaidsSavage,
    Extreme,
    Chaotic,
    Criterion,
    Savage,
    CriterionSavage,
    Ultimate,
}

/// <summary>
///     Organization of the biggest pieces of data about the content the user is
///     currently engaged in.
/// </summary>
/// <remarks>
///     This entire class is skewed towards identifying 'standard' PvE combat content,
///     as in content in roulettes and the higher end.
/// </remarks>
public static class Content
{
    /// <summary>
    ///     The ID of the current territory the player is in.
    /// </summary>
    public static uint TerritoryID => Svc.ClientState.TerritoryType;

    /// <summary>
    ///     The result of the TerritoryName builder.
    /// </summary>
    /// <seealso cref="TerritoryHelper.GetTerritoryName" />
    private static string TerritoryNameResult =>
        TerritoryHelper.GetTerritoryName(TerritoryID);

    /// <summary>
    ///     Whether the TerritoryName came out successfully from the builder.
    /// </summary>
    /// <seealso cref="TerritoryNameResult" />
    /// <seealso cref="TerritoryName" />
    private static bool TerritoryNameResolved =>
        TerritoryNameResult.Contains('|');

    /// <summary>
    ///     The zone name of the current territory the player is in.
    /// </summary>
    /// <value><c>null</c> when not resolved.</value>
    /// <seealso cref="TerritoryHelper.GetTerritoryName" />
    /// <seealso cref="TerritoryNameResolved" />
    /// <seealso cref="TerritoryNameResult" />
    public static string? TerritoryName =>
        TerritoryNameResolved
            ? TerritoryNameResult.Split('|')[1].Split('(')[0].Trim()
            : null;

    /// <summary>
    ///     The Sheet row for the current <see cref="TerritoryType" />.
    /// </summary>
    public static TerritoryType? TerritoryTypeRow =>
        Svc.Data.Excel.GetSheet<TerritoryType>(Language.English)!
            .GetRowOrDefault(Svc.ClientState.TerritoryType);

    /// <summary>
    ///     The ID of the current map the player is in.
    /// </summary>
    public static uint MapID =>
        Svc.ClientState.MapId;

    /// <summary>
    ///     The intended use of the current territory the player is in.
    /// </summary>
    /// <seealso cref="TerritoryIntendedUseEnum" />
    public static TerritoryIntendedUseEnum? TerritoryIntendedUse
    {
        get
        {
            var intendedUseRow = TerritoryTypeRow?
                .TerritoryIntendedUse
                .ValueNullable?.RowId;

            if (intendedUseRow != null)
                return (TerritoryIntendedUseEnum)intendedUseRow;

            return null;
        }
    }

    /// <summary>
    ///     The Sheet row for the current <see cref="ContentFinderCondition" />.
    /// </summary>
    public static ContentFinderCondition? ContentFinderConditionRow =>
        TerritoryTypeRow?.ContentFinderCondition.ValueNullable;

    /// <summary>
    ///     The content name of the current territory the player is in.
    /// </summary>
    /// <value>
    ///     Falls back to <see cref="TerritoryName" /> when
    ///     <see cref="ContentFinderCondition">CFC Data</see> is not resolved.<br />
    ///     <c>null</c> when <see cref="TerritoryName" /> is also not
    ///     resolved.
    /// </value>
    /// <seealso cref="ContentFinderCondition" />
    /// <seealso cref="ContentFinderConditionRow" />
    public static string? ContentName =>
        TerritoryNameResolved
            ? ContentFinderConditionRow != null
                ? ContentFinderConditionRow?.Name.ToString()
                : TerritoryName
            : null;

    /// <summary>
    ///     If the content allows Undersized (Unrestricted) Parties.
    /// </summary>
    public static bool? AllowUndersized =>
        ContentFinderConditionRow?.AllowUndersized;

    /// <summary>
    ///     If the content is listed under High-End Content in the Duty Finder.
    /// </summary>
    public static bool? HighEndDuty =>
        ContentFinderConditionRow?.HighEndDuty;

    /// <summary>
    ///     Whether the difficulty was found in the <see cref="ContentName" />.
    /// </summary>
    private static bool ContentDifficultyFromNameResolved
    {
        get
        {
            if (ContentFinderConditionRow is null)
                return false;

            var contentName =
                ContentFinderConditionRow.Value.Name.ToString().ToLower();

            return contentName.Contains(" (hard") ||
                   contentName.Contains(" (extreme") ||
                   contentName.Contains(" (savage");
        }
    }

    /// <summary>
    ///     The title case difficulty of the content as found in the
    ///     <see cref="ContentName" />.
    /// </summary>
    /// <value>
    ///     <c>null</c> when not
    ///     <see cref="ContentDifficultyFromNameResolved">resolved</see> or when
    ///     <see cref="ContentFinderConditionRow" /> is null.
    /// </value>
    public static string? ContentDifficultyFromName =>
        ContentFinderConditionRow is null
            ? null
            : ContentDifficultyFromNameResolved
                ? ContentFinderConditionRow?.Name.ToString().Split('(').Last()
                    .TrimEnd(')').Trim()
                : null;

    /// <summary>
    ///     The Sheet row for the current <see cref="InstanceContent" />.
    /// </summary>
    public static InstanceContent? InstanceContentRow
    {
        get
        {
            var instanceContentRow = ContentFinderConditionRow?.Content.RowId;

            if (instanceContentRow != null)
                return Svc.Data.Excel.GetSheet<InstanceContent>(Language.English)!
                    .GetRowOrDefault((uint)instanceContentRow);

            return null;
        }
    }

    /// <summary>
    ///     The number of minutes the current piece of content is restricted to.
    /// </summary>
    public static ushort? TimeLimit =>
        InstanceContentRow?.TimeLimitmin;

    /// <summary>
    ///     The Sheet row for the current <see cref="ContentType" />.
    /// </summary>
    public static SheetContentType? ContentTypeRow
    {
        get
        {
            var contentTypeRowId = ContentFinderConditionRow?.ContentType.RowId;

            if (contentTypeRowId != null)
                return Svc.Data.Excel.GetSheet<SheetContentType>(Language.English)!
                    .GetRowOrDefault((uint)contentTypeRowId);

            return null;
        }
    }

    /// <summary>
    ///     The Row ID of the current <see cref="SheetContentType" />.
    /// </summary>
    private static uint? ContentTypeRowId =>
        ContentTypeRow?.RowId;

    /// <summary>
    ///     The name of the current <see cref="SheetContentType" />.
    /// </summary>
    public static string? ContentTypeName =>
        ContentTypeRowId is not null && ContentTypeRowId != 0
            ? ContentTypeRow?.Name.ToString()
            : ContentTypeRowId == 0
                ? "OverWorld"
                : null;

    /// <summary>
    ///     The determined <see cref="ContentType" /> of the current content.
    /// </summary>
    /// <seealso cref="DetermineContentType" />
    public static ContentType? ContentType => DetermineContentType();

    /// <summary>
    ///     The determined <see cref="ContentDifficulty" /> of the current content.
    /// </summary>
    /// <seealso cref="DetermineContentDifficulty" />
    public static ContentDifficulty? ContentDifficulty =>
        DetermineContentDifficulty();

    /// <summary>
    ///     A rigorous switch to categorize the (combat-focused) type of content that
    ///     the user is currently in; primarily using
    ///     <see cref="TerritoryIntendedUse" />.
    /// </summary>
    /// <param name="default">
    ///     The default content type to return if the switch doesn't resolve to anything.
    ///     <br />
    ///     Primarily here to make it easier in the future if this method is to get
    ///     more rigorous in regard to what returns as
    ///     <see cref="ContentType.OverWorld" />.
    /// </param>
    /// <returns>The determined <see cref="ContentType" />.</returns>
    private static ContentType? DetermineContentType
        (ContentType @default = GameHelpers.ContentType.OverWorld)
    {
        return TerritoryIntendedUse switch
        {
            TerritoryIntendedUseEnum.Barracks or
                TerritoryIntendedUseEnum.Rival_Wings or
                TerritoryIntendedUseEnum.Crystalline_Conflict or
                TerritoryIntendedUseEnum.Frontline =>
                GameHelpers.ContentType.PVP,

            TerritoryIntendedUseEnum.Dungeon or
                TerritoryIntendedUseEnum.Treasure_Map_Duty =>
                GameHelpers.ContentType.Dungeon,

            TerritoryIntendedUseEnum.Deep_Dungeon =>
                GameHelpers.ContentType.DeepDungeon,

            TerritoryIntendedUseEnum.Variant_Dungeon =>
                GameHelpers.ContentType.Variant,

            TerritoryIntendedUseEnum.Criterion_Duty or
                TerritoryIntendedUseEnum.Criterion_Savage_Duty =>
                GameHelpers.ContentType.Criterion,

            TerritoryIntendedUseEnum.Trial =>
                GameHelpers.ContentType.Trial,

            TerritoryIntendedUseEnum.Large_Scale_Raid or
                TerritoryIntendedUseEnum.Large_Scale_Savage_Raid =>
                GameHelpers.ContentType.FieldRaid,

            _ when
                (ContentName?.Contains("Delubrum") ?? false) ||
                (ContentName?.Contains("Lacus") ?? false) ||
                (ContentName?.Contains("Dalriada") ?? false) ||
                MapID is >= 520 and <= 527 =>
                GameHelpers.ContentType.FieldRaid,

            TerritoryIntendedUseEnum.Eureka or
                TerritoryIntendedUseEnum.Bozja or
                TerritoryIntendedUseEnum.Diadem or
                TerritoryIntendedUseEnum.Diadem_2 or
                TerritoryIntendedUseEnum.Diadem_3 =>
                GameHelpers.ContentType.FieldOperations,

            TerritoryIntendedUseEnum.Alliance_Raid =>
                GameHelpers.ContentType.ARaid,

            TerritoryIntendedUseEnum.Raid or
                TerritoryIntendedUseEnum.Raid_2 =>
                GameHelpers.ContentType.Raid,

            _ => @default,
        };
    }

    /// <summary>
    ///     A rigorous switch to categorize the difficulty of the content the user is
    ///     currently in; primarily using <see cref="ContentFinderConditionRow" />.
    /// </summary>
    /// <param name="default">
    ///     The default content difficulty to return if the switch doesn't resolve to
    ///     anything.<br />
    ///     Primarily here to make it easier in the future if this method is to get
    ///     more rigorous in regard to what returns as
    ///     <see cref="ContentDifficulty.Normal" />.
    /// </param>
    /// <returns>The determined <see cref="ContentDifficulty" />.</returns>
    private static ContentDifficulty? DetermineContentDifficulty
        (ContentDifficulty @default = GameHelpers.ContentDifficulty.Normal)
    {
        return ContentFinderConditionRow switch
        {
            _ when ContentDifficultyFromNameResolved =>
                ContentDifficultyFromName switch
                {
                    "Hard" => GameHelpers.ContentDifficulty.Hard,
                    "Extreme" => GameHelpers.ContentDifficulty.Extreme,
                    "Savage" => GameHelpers.ContentDifficulty.Savage,
                    _ => @default,
                },

            { ContentType.RowId: 1 } =>
                GameHelpers.ContentDifficulty.Normal,

            { ContentType.RowId: 2 } when
                ContentDifficultyFromName == "Hard" =>
                GameHelpers.ContentDifficulty.Hard,
            { ContentType.RowId: 4, HighEndDuty: false } when
                ContentDifficultyFromName == "Hard" =>
                GameHelpers.ContentDifficulty.Hard,

            { ContentType.RowId: 29 } when ContentDifficultyFromName == "Savage" =>
                GameHelpers.ContentDifficulty.FieldRaidsSavage,

            { ContentType.RowId: 4 } when
                ContentDifficultyFromName == "Extreme" ||
                (ContentName?.Contains("Minstrel") ?? false) =>
                GameHelpers.ContentDifficulty.Extreme,

            { ContentType.RowId: 4, HighEndDuty: true } =>
                GameHelpers.ContentDifficulty.Unreal,

            { ContentType.RowId: 37 } =>
                GameHelpers.ContentDifficulty.Chaotic,

            { ContentType.RowId: 30, AllowUndersized: true } =>
                GameHelpers.ContentDifficulty.Criterion,

            { ContentType.RowId: 5 } when ContentDifficultyFromName == "Savage" =>
                GameHelpers.ContentDifficulty.Savage,

            { ContentType.RowId: 30, AllowUndersized: false } =>
                GameHelpers.ContentDifficulty.CriterionSavage,

            { ContentType.RowId: 28 } =>
                GameHelpers.ContentDifficulty.Ultimate,

            _ => @default,
        };
    }
}
