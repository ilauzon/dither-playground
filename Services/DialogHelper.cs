using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;

namespace dither_playground.Services;

/// <summary>
/// A helper class to manage dialogs via extension methods. Add more on your own
/// </summary>
public static class DialogHelper
{
    /// <summary>
    /// Shows an open file dialog for a registered context, most likely a ViewModel
    /// </summary>
    /// <param name="context">The context</param>
    /// <param name="title">The dialog title or a default is null</param>
    /// <param name="selectMany">Is selecting many files allowed?</param>
    /// <returns>An array of file names</returns>
    /// <exception cref="ArgumentNullException">if context was null</exception>
    public static async Task<List<IStorageFile>?> OpenFileDialogAsync(this IDialogParticipant? context,
        string? title = null,
        bool selectMany = true)
    {
        ArgumentNullException.ThrowIfNull(context);

        // lookup the TopLevel for the context. If no TopLevel was found, we throw an exception
        var topLevel = DialogManager.GetTopLevelForContext(context)
                       ?? throw new InvalidOperationException("No TopLevel was resolved for the given context.");

        // Open the file dialog
        var storageFiles = await topLevel.StorageProvider.OpenFilePickerAsync(
            new FilePickerOpenOptions
            {
                AllowMultiple = selectMany,
                FileTypeFilter = [FilePickerFileTypes.ImageAll],
                Title = title ?? "Select any file(s)"
            });

        // return the result
        return storageFiles.ToList();
    }

    /// <summary>
    /// Shows a save file dialog for a registered context, most likely a ViewModel
    /// </summary>
    /// <param name="context">The context</param>
    /// <param name="image">The image to be saved</param>
    /// <param name="filename">The suggested name of the file to be saved</param>
    /// <exception cref="ArgumentNullException">if context was null</exception>
    public static async Task SaveFileDialogAsync(this IDialogParticipant? context, Bitmap image,
        string? filename = null)
    {
        ArgumentNullException.ThrowIfNull(context);

        // lookup the TopLevel for the context. If no TopLevel was found, we throw an exception
        var topLevel = DialogManager.GetTopLevelForContext(context)
                       ?? throw new InvalidOperationException("No TopLevel was resolved for the given context.");

        // get current date for use in filename
        var dateTime = DateTime.Now.ToString("/H/mm/ss/dd/MM/yyyy");

        // Open the file dialog
        var file = await topLevel.StorageProvider.SaveFilePickerAsync(
            new FilePickerSaveOptions
            {
                Title = "Save creation",
                FileTypeChoices = [FilePickerFileTypes.ImagePng],
                SuggestedFileName = filename ?? $"dithered{dateTime}.png"
            });

        if (file is not null)
        {
            await using var stream = await file.OpenWriteAsync();
            image.Save(stream);
        }
    }
}