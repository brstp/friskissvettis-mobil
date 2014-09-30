using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class StringExtensions
{
    /// <summary>
    /// Sets first letter to upper case
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string UppercaseFirst(this string s)
    {
        // Check for empty string.
        if (string.IsNullOrEmpty(s))
        {
            return string.Empty;
        }

        // Return char and concat substring.
        return char.ToUpper(s[0]) + s.Substring(1);
    }
}
