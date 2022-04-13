namespace Money_CLI.Controllers;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Money_CLI.Models;
using Money_CLI.Models.Enums;
using Serilog;

public class FileHandler : GenericController
{
    #region FileName based on month
    /// <summary>
    /// Returns the string file name for the selected month.
    /// </summary>
    public static string FileName(int month)
    {
        string monthFullName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);

        return $"{month.ToString("00")}-{monthFullName}";
    }
    #endregion

    /// <summary>
    /// Return the full path to the current file.
    /// If it does not exist, create it.
    /// </summary>
    public static string? GetFile(ChangeType changeType, int month, int year)
    {
        try {
            // Set the category folder
            string categoryFolder;
            switch (changeType) {
                case ChangeType.Income:
                    categoryFolder = "Income";
                    break;
                default:
                    categoryFolder = "Expenses";
                    break;
            }

            // Create the folder, if it doesn't exist
            string folderPath = ValidateDirectory(@$"{SystemVariables.ExportFolder}{categoryFolder}/{year}/");
            if (!Directory.Exists(folderPath)) {
                Directory.CreateDirectory(folderPath);
                Log.Information("Created folder {folderPath}", folderPath);
            }

            // Select the month from the input, only if it's valid
            string fileName = FileName(month);

            // Create the file, if it doesn't exist
            string fullPath = @$"{folderPath}{fileName}.md";
            if (!File.Exists(fullPath)) {
                File.Create(fullPath).Close();
                Log.Information("Created file {fullPath}", fullPath);
            }

            Log.Information("Your {changeType} will be exported to {fullPath}", (changeType == ChangeType.Expense ? "expenses" : "income"), fullPath);

            return fullPath;
        } catch (Exception) {
            return null;
        }
    }

    /// <summary>
    /// Exports all changes to the current file.
    /// </summary>
    public static bool Export(ChangeType changeType, int month, int year)
    {
        string? file = GetFile(changeType, month, year);

        if (file == null)
            return false;

        try {
            List<string> template;
            double total = 0;

            switch (changeType)
            {
                case ChangeType.Income:
                    // Get total amount
                    total = MoneyHandler.TotalMonthlyIncome(month, year);

                    // Load the file template
                    template = FileTemplates.FileTemplate(
                            "Income",
                            FileName(month),
                            total
                        ).ToList();

                    // Get all entries and add them to the list
                    foreach (Income income in MoneyHandler.AllMonthlyIncome(month, year))
                        template.Add(income.ToString());

                    break;
                case ChangeType.Expense:
                    // Get total amount
                    total = MoneyHandler.TotalMonthlyExpenses(month, year);

                    // Load the file template
                    template = FileTemplates.FileTemplate(
                            "Expenses",
                            FileName(month),
                            total
                        ).ToList();

                    // Get all entries and add them to the list
                    foreach (Expense expense in MoneyHandler.AllMonthlyExpenses(month, year))
                        template.Add(expense.ToString());

                    break;
                default:
                    return false;
            }

            // Write the file
            File.WriteAllLines(file, template);

            return true;
        } catch (Exception) {
            return false;
        }
    }
}
