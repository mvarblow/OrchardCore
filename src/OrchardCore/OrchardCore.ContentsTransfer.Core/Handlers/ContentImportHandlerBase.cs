using System;
using System.Data;

namespace OrchardCore.ContentsTransfer.Handlers;

public class ContentImportHandlerBase
{
    protected static bool Is(string columnName, params string[] terms)
    {
        foreach (var term in terms)
        {
            if (string.Equals(columnName, term, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    protected static bool Is(string columnName, ImportColumn importColumn)
    {
        if (string.Equals(columnName, importColumn.Name, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        foreach (var term in importColumn.AdditionalNames ?? Array.Empty<string>())
        {
            if (string.Equals(columnName, term, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    protected static string[] SplitCellValues(DataRow row, DataColumn column, string seperator = ",")
        => row[column]?.ToString()?.Split(seperator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries) ?? Array.Empty<string>();
}