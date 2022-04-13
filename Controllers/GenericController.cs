namespace Money_CLI.Controllers;

using System;
using System.IO;

// TODO: Change the name to anything more meaningful.

public class GenericController
{
    /// <summary>
    /// Validates the directory with the correct Separators.
    /// For more documentation, go to https://docs.microsoft.com/en-us/dotnet/api/system.io.path.directoryseparatorchar
    /// </summary>
    public static string ValidateDirectory(string directory)
    {
        string sepChar = Path.DirectorySeparatorChar.ToString();
        string altChar = Path.AltDirectorySeparatorChar.ToString();

        if (!directory.EndsWith(sepChar) && !directory.EndsWith(altChar))
            directory += sepChar;

        if (OperatingSystem.IsWindows())
            return directory.Replace(altChar, sepChar);

        return directory.Replace("\\", sepChar);
    }

    /// <summary>
    /// Validates the value of the day.
    /// </summary>
    public static int ValidateDay (int day) => (1 <= day && day <= 31) ? day : DateTime.Now.Day;

    /// <summary>
    /// Validates the value of the month.
    /// </summary>
    public static int ValidateMonth (int month) => (1 <= month && month <= 12) ? month : DateTime.Now.Month;

    /// <summary>
    /// Validates the value of the year.
    /// </summary>
    public static int ValidateYear (int year) => (1970 <= year && year <= DateTime.Now.Year) ? year : DateTime.Now.Year;
}