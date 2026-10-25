using Dalamud.Memory;
using ECommons.DalamudServices;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Client.UI;
using Lumina.Excel.Sheets;
using System;
using System.Text;
namespace ECommons.Automation;
#nullable disable

/// <summary>
/// A class containing chat functionality
/// </summary>
// from: https://github.com/Infiziert90/ChatTwo/blob/5b58513a5226a2d3b40aca96be4f51c779fab363/ChatTwo/GameFunctions/ChatBox.cs#L9
public static unsafe class Chat
{
    /// <summary>
    /// <para>
    /// Send a given message to the chat box. <b>This can send chat to the server.</b>
    /// </para>
    /// <para>
    /// <b>This method is unsafe.</b> This method does no checking on your input and
    /// may send content to the server that the normal client could not. You must
    /// verify what you're sending and handle content and length to properly use
    /// this.
    /// </para>
    /// </summary>
    /// <param name="message">Message to send</param>
    /// <exception cref="InvalidOperationException">If the signature for this function could not be found</exception>
    public static void SendMessageUnsafe(byte[] message)
    {
        var mes = Utf8String.FromSequence(message.NullTerminate());
        UIModule.Instance()->ProcessChatBoxEntry(mes);
        mes->Dtor(true);
    }

    /// <summary>
    /// <para>
    /// Send a given message to the chat box. <b>This can send chat to the server.</b>
    /// </para>
    /// <para>
    /// This method is slightly less unsafe than <see cref="SendMessageUnsafe"/>. It
    /// will throw exceptions for certain inputs that the client can't normally send,
    /// but it is still possible to make mistakes. Use with caution.
    /// </para>
    /// </summary>
    /// <param name="message">message to send</param>
    /// <exception cref="ArgumentException">If <paramref name="message"/> is empty, longer than 500 bytes in UTF-8, or contains invalid characters.</exception>
    /// <exception cref="InvalidOperationException">If the signature for this function could not be found</exception>
    public static void SendMessage(string message)
    {
        var bytes = Encoding.UTF8.GetBytes(message);
        if(bytes.Length == 0)
            throw new ArgumentException("message is empty", nameof(message));

        if(bytes.Length > 500)
            throw new ArgumentException("message is longer than 500 bytes", nameof(message));

        if(message.Length != SanitiseText(message).Length)
            throw new ArgumentException("message contained invalid characters", nameof(message));

        SendMessageUnsafe(bytes);
    }

    /// <summary>
    /// <para>
    /// Sanitises a string by removing any invalid input.
    /// </para>
    /// <para>
    /// The result of this method is safe to use with
    /// <see cref="SendMessage"/>, provided that it is not empty or too
    /// long.
    /// </para>
    /// </summary>
    /// <param name="text">text to sanitise</param>
    /// <returns>sanitised text</returns>
    /// <exception cref="InvalidOperationException">If the signature for this function could not be found</exception>
    public static string SanitiseText(string text)
    {
        var uText = Utf8String.FromString(text);

        uText->SanitizeString((AllowedEntities)0x27F);
        var sanitised = uText->ToString();
        uText->Dtor(true);

        return sanitised;
    }

    /// <summary>
    /// Executes command as if it was typed in chat box. 
    /// </summary>
    /// <param name="message">Full text of the command.</param>
    /// <exception cref="InvalidOperationException">If you didn't prefixed it with a slash.</exception>
    public static void ExecuteCommand(string message)
    {
        if(!message.StartsWith("/")) throw new InvalidOperationException($"Attempted to execute command but was not prefixed with a slash: {message}");
        SendMessage(message);
    }

    /// <summary>
    /// Executes General Action by ID via chat.
    /// </summary>
    /// <param name="generalActionId"></param>
    public static void ExecuteGeneralAction(uint generalActionId)
    {
        ExecuteCommand($"/generalaction \"{Svc.Data.GetExcelSheet<GeneralAction>().GetRowOrDefault(generalActionId)?.Name}\"");
    }

    /// <summary>
    /// Executes Action by ID via chat.
    /// </summary>
    /// <param name="actionId"></param>
    public static void ExecuteAction(uint actionId)
    {
        ExecuteCommand($"/action \"{Svc.Data.GetExcelSheet<Lumina.Excel.Sheets.Action>().GetRowOrDefault(actionId)?.Name}\"");
    }

    [Obsolete("Use Chat.<MethodName> directly instead of Chat.Instance.<MethodName>")]
    public static class Instance
    {
        public static void ExecuteCommand(string message) => Chat.ExecuteCommand(message);
        public static void SendMessageUnsafe(byte[] message) => Chat.SendMessageUnsafe(message);
        public static void SendMessage(string message) => Chat.SendMessage(message);
        public static void ExecuteGeneralAction(uint generalActionId) => Chat.ExecuteGeneralAction(generalActionId);
        public static void ExecuteAction(uint actionId) => Chat.ExecuteAction(actionId);
        public static string SanitiseText(string text) => Chat.SanitiseText(text);
    }
}