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
    /// Configuration for synchronous message box with callbacks.
    /// Phase 35: Added to support callback-based dialog pattern.
    /// </summary>
    public class MessageBoxOptions
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public bool ShowOkButton { get; set; }
        public bool ShowPositiveButton { get; set; }
        public bool ShowNegativeButton { get; set; }
        public Action OnOk { get; set; }
        public Action OnPositive { get; set; }
        public Action OnNegative { get; set; }
    }

    /// <summary>
    /// Configuration for synchronous card selector with callbacks.
    /// Phase 35: Added to support callback-based card selection pattern.
    /// Uses object type for cards to avoid Unity dependency in Application layer.
    /// </summary>
    public class CardSelectorOptions
    {
        public string Title { get; set; }
        public List<object> Cards { get; set; }
        public bool ShowOkButton { get; set; }
        public bool ShowPositiveButton { get; set; }
        public bool ShowNegativeButton { get; set; }
        public Action<object> OnOkSingle { get; set; }
        public Action<List<object>> OnOkMultiple { get; set; }
        public Action<object> OnPositiveSingle { get; set; }
        public Action<List<object>> OnPositiveMultiple { get; set; }
        public Action OnNegative { get; set; }
        public int NumberCardSelection { get; set; } = 1;
        public bool ShowOrder { get; set; }
    }

    /// <summary>
    /// Service for showing dialogs and UI prompts.
    /// Replaces MessageBox and CardSelector singletons.
    /// Phase 35: Added synchronous callback methods to support legacy patterns.
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

        // Phase 35: Synchronous callback-based methods for legacy pattern support
        // These methods accept an object canvas parameter to support UnityEngine.Transform
        // without adding Unity dependency to JDG.Application layer

        /// <summary>
        /// Shows a message box with callback actions.
        /// Synchronous version for legacy code migration.
        /// </summary>
        /// <param name="canvas">The Unity Transform canvas (passed as object to avoid Unity dependency)</param>
        /// <param name="options">Message box configuration</param>
        void ShowMessageBox(object canvas, MessageBoxOptions options);

        /// <summary>
        /// Shows a card selector dialog with callback actions.
        /// Synchronous version for legacy code migration.
        /// </summary>
        /// <param name="canvas">The Unity Transform canvas</param>
        /// <param name="options">Card selector configuration</param>
        void ShowCardSelector(object canvas, CardSelectorOptions options);
    }
}
