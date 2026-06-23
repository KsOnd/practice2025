namespace task01;

public static class StringExtentions
{
    public static bool IsPalindrome(this string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return false;
        }

        var cleanedStr =string.Concat(input.ToLower().Where(ch => !char.IsPunctuation(ch) && !char.IsWhiteSpace(ch)));

        if (cleanedStr.Length == 0)
        {
            return false;
        }
        
        var reversedStr = string.Concat(cleanedStr.Reverse());

        return cleanedStr == reversedStr;
    }
}