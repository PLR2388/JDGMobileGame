using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JDG.Application.Services
{
    /// <summary>
    /// Types of message boxes that can be displayed.
    /// </summary>
    public enum MessageBoxType
    {
        YesNo,
        Ok,
        OkCancel,
        Custom
    }

    /// <summary>
    /// Configuration for card selector dialog.
    /// </summary>
    public class CardSelectorConfig
    {
        public string Title { get; set; }
        public List<Guid> CardIds { get; set; }
        public int MinSelection { get; set; }
        public int MaxSelection { get; set; }
        public bool AllowCancel { get; set; }
    }

    /// <summary>
    /// Service for showing dialogs and UI prompts.
    /// Replaces MessageBox and CardSelector singletons.
    /// </summary>
    public interface IDialogService
    {
        /// <summary>
        /// Shows a message box with the specified title, message, and type.
        /// Returns true if user confirms, false otherwise.
        /// </summary>
        Task<bool> ShowMessageBoxAsync(string title, string message, MessageBoxType type);

        /// <summary>
        /// Shows a card selector dialog and returns selected card IDs.
        /// Returns null if canceled.
        /// </summary>
        Task<List<Guid>> ShowCardSelectorAsync(CardSelectorConfig config);

        /// <summary>
        /// Shows a simple yes/no prompt.
        /// </summary>
        Task<bool> ShowConfirmAsync(string message);

        /// <summary>
        /// Shows an information message with OK button.
        /// </summary>
        Task ShowInfoAsync(string message);
    }
}
