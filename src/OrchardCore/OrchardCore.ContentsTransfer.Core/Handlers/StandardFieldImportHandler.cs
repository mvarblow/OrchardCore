using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace OrchardCore.ContentsTransfer.Handlers;

public abstract class StandardFieldImportHandler : ContentImportHandlerBase, IContentFieldImportHandler
{
    public IReadOnlyCollection<ImportColumn> GetColumns(ImportContentFieldContext context)
    {
        return new[]
        {
            new ImportColumn()
            {
                Name = $"{context.PartName}_{context.ContentPartFieldDefinition.Name}_{BindingPropertyName}",
                Description = Description(context),
                IsRequired = IsRequired(context),
                ValidValues = GetValidValues(context),
            }
        };
    }

    public async Task ImportAsync(ContentFieldImportMapContext context)
    {
        if (context.ContentItem == null)
        {
            throw new ArgumentNullException(nameof(context.ContentItem));
        }

        if (context.Columns == null)
        {
            throw new ArgumentNullException(nameof(context.Columns));
        }

        if (context.Row == null)
        {
            throw new ArgumentNullException(nameof(context.Row));
        }

        var knownColumns = GetColumns(context);

        foreach (DataColumn column in context.Columns)
        {
            var firstColumn = knownColumns.FirstOrDefault(x => Is(column.ColumnName, x));

            if (firstColumn == null)
            {
                continue;
            }

            var text = context.Row[column]?.ToString();

            await SetValueAsync(context, text);
        }
    }

    public async Task ExportAsync(ContentFieldExportMapContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context.ContentItem));
        }

        if (context.ContentItem == null)
        {
            throw new ArgumentNullException(nameof(context.ContentItem));
        }

        if (context.Row == null)
        {
            throw new ArgumentNullException(nameof(context.Row));
        }

        var firstColumn = GetColumns(context).FirstOrDefault();

        if (firstColumn != null)
        {
            context.Row[firstColumn.Name] = await GetValueAsync(context);
        }
    }

    protected virtual string Description(ImportContentFieldContext context)
        => string.Empty;

    protected virtual bool IsRequired(ImportContentFieldContext context)
        => false;

    protected virtual string[] GetValidValues(ImportContentFieldContext context)
        => Array.Empty<string>();

    protected abstract Task<object> GetValueAsync(ContentFieldExportMapContext context);

    protected abstract Task SetValueAsync(ContentFieldImportMapContext context, string value);

    protected abstract string BindingPropertyName { get; }
}