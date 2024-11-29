namespace ReminderBot.Extensions;

public static class StringStylesExtension
{
    public static string Bold(this string text) => $"<b>{text}</b>";
    public static string Italic(this string text) => $"<i>{text}</i>";
    public static string Underline(this string text) => $"<u>{text}</u>";
    public static string Strike(this string text) => $"<s>{text}</s>";
    public static string Code(this string text) => $"<code>{text}</code>";
    public static string Pre(
        this string text, string language
    ) => $"<pre language=\"{language}\">{text}</pre>";
    public static string Page(this string text) => $"***\n{text}\n***";
}