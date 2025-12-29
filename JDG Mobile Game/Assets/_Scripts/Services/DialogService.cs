using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JDG.Application.Services;
using UnityEngine;

// Alias to avoid conflict with global CardSelectorConfig in MessageBox/Config.cs
using DomainCardSelectorConfig = JDG.Application.Services.CardSelectorConfig;

namespace JDG.Infrastructure.Services
{
    /// <summary>
    /// Infrastructure implementation of IDialogService.
    /// Wraps the existing MessageBox and CardSelector singletons during migration.
    /// Uses Strangler Fig pattern - delegates to old systems temporarily.
    /// Phase 64: Changed to lazy resolution to avoid constructor singleton access.
    /// </summary>
    public class DialogService : IDialogService
    {
        private Transform _canvas;

        // Phase 64: Lazy resolution - access singletons when needed, not in constructor
        // This allows DialogService to be constructed before MessageBox/CardSelector exist
        #pragma warning disable CS0618 // Suppress obsolete warning - adapter service wraps singletons
        private MessageBox MessageBox => MessageBox.Instance;
        private CardSelector CardSelector => CardSelector.Instance;
        #pragma warning restore CS0618

        /// <summary>
        /// Phase 64: Empty constructor - singletons accessed lazily via properties.
        /// </summary>
        public DialogService()
        {
        }

        /// <summary>
        /// Sets the canvas where dialogs will be displayed.
        /// </summary>
        public void SetCanvas(Transform canvas)
        {
            _canvas = canvas;
        }

        public async Task<bool> ShowMessageBoxAsync(string title, string message, MessageBoxType type)
        {
            if (_canvas == null)
            {
                Debug.LogError("DialogService: Canvas not set. Call SetCanvas() first.");
                return false;
            }

            var tcs = new TaskCompletionSource<bool>();

            MessageBoxConfig config;

            switch (type)
            {
                case MessageBoxType.Ok:
                    config = new MessageBoxConfig(
                        title: title,
                        description: message,
                        showOkButton: true,
                        okAction: () => tcs.TrySetResult(true)
                    );
                    break;

                case MessageBoxType.YesNo:
                case MessageBoxType.OkCancel:
                    config = new MessageBoxConfig(
                        title: title,
                        description: message,
                        showPositiveButton: true,
                        positiveAction: () => tcs.TrySetResult(true),
                        showNegativeButton: true,
                        negativeAction: () => tcs.TrySetResult(false)
                    );
                    break;

                case MessageBoxType.Custom:
                    config = new MessageBoxConfig(
                        title: title,
                        description: message,
                        showOkButton: true,
                        okAction: () => tcs.TrySetResult(true)
                    );
                    break;

                default:
                    config = new MessageBoxConfig(
                        title: title,
                        description: message,
                        showOkButton: true,
                        okAction: () => tcs.TrySetResult(true)
                    );
                    break;
            }

            MessageBox.CreateMessageBox(_canvas, config);

            return await tcs.Task;
        }

        public async Task<List<Guid>> ShowCardSelectorAsync(DomainCardSelectorConfig config)
        {
            if (_canvas == null)
            {
                Debug.LogError("DialogService: Canvas not set. Call SetCanvas() first.");
                return null;
            }

            var tcs = new TaskCompletionSource<List<Guid>>();

            var cards = config.CardIds != null ? ConvertCardIds(config.CardIds) : new List<Cards.InGameCard>();

            var cardSelectorConfig = new global::CardSelectorConfig(
                title: config.Title,
                cards: cards,
                showPositiveButton: true,
                positiveMultipleAction: (selectedCards) =>
                {
                    // TODO: InGameCard doesn't have Id property yet
                    // This is a temporary limitation during migration
                    // For now, return empty list as card selection isn't fully integrated
                    var selectedIds = new List<Guid>();
                    Debug.LogWarning($"DialogService: Card selection returned {selectedCards.Count} cards, but ID mapping not implemented yet.");
                    tcs.TrySetResult(selectedIds);
                },
                showNegativeButton: config.AllowCancel,
                negativeAction: config.AllowCancel ? () => tcs.TrySetResult(null) : null,
                numberCardSelection: config.MaxSelection
            );

            CardSelector.CreateCardSelection(_canvas, cardSelectorConfig);

            return await tcs.Task;
        }

        public async Task<bool> ShowConfirmAsync(string message)
        {
            return await ShowMessageBoxAsync("Confirm", message, MessageBoxType.YesNo);
        }

        public async Task ShowInfoAsync(string message)
        {
            await ShowMessageBoxAsync("Information", message, MessageBoxType.Ok);
        }

        // Phase 35: Synchronous callback-based methods for legacy pattern support

        /// <summary>
        /// Shows a message box with callback actions.
        /// Phase 35: Synchronous version for legacy code migration.
        /// </summary>
        /// <param name="canvas">The Unity Transform canvas (passed as object to avoid Unity dependency in interface)</param>
        /// <param name="options">Message box configuration</param>
        public void ShowMessageBox(object canvas, MessageBoxOptions options)
        {
            var canvasTransform = canvas as Transform;
            if (canvasTransform == null)
            {
                Debug.LogError("DialogService: Invalid canvas. Expected UnityEngine.Transform.");
                return;
            }

            var config = new MessageBoxConfig(
                title: options.Title,
                description: options.Message,
                showOkButton: options.ShowOkButton,
                okAction: options.OnOk != null ? new UnityEngine.Events.UnityAction(options.OnOk) : null,
                showPositiveButton: options.ShowPositiveButton,
                positiveAction: options.OnPositive != null ? new UnityEngine.Events.UnityAction(options.OnPositive) : null,
                showNegativeButton: options.ShowNegativeButton,
                negativeAction: options.OnNegative != null ? new UnityEngine.Events.UnityAction(options.OnNegative) : null
            );

            MessageBox.CreateMessageBox(canvasTransform, config);
        }

        /// <summary>
        /// Shows an OK-only message box (warning style) with callback.
        /// Phase 35: Synchronous version for legacy code migration.
        /// Note: Using System.Action to avoid conflict with OnePlayer.Action enum.
        /// </summary>
        public void ShowWarning(object canvas, string title, string message, System.Action onOk)
        {
            ShowMessageBox(canvas, new MessageBoxOptions
            {
                Title = title,
                Message = message,
                ShowOkButton = true,
                OnOk = onOk
            });
        }

        /// <summary>
        /// Shows an OK-only message box (warning style) without callback.
        /// Phase 35: Synchronous version for legacy code migration.
        /// </summary>
        public void ShowWarning(object canvas, string title, string message)
        {
            ShowWarning(canvas, title, message, (System.Action)null);
        }

        /// <summary>
        /// Shows a Yes/No confirmation dialog.
        /// Phase 35: Synchronous version for legacy code migration.
        /// Note: Using System.Action to avoid conflict with OnePlayer.Action enum.
        /// </summary>
        public void ShowConfirm(object canvas, string title, string message, System.Action onYes, System.Action onNo)
        {
            ShowMessageBox(canvas, new MessageBoxOptions
            {
                Title = title,
                Message = message,
                ShowPositiveButton = true,
                OnPositive = onYes,
                ShowNegativeButton = true,
                OnNegative = onNo
            });
        }

        /// <summary>
        /// Shows a card selector dialog with callback actions.
        /// Phase 35: Synchronous version for legacy code migration.
        /// </summary>
        public void ShowCardSelector(object canvas, CardSelectorOptions options)
        {
            var canvasTransform = canvas as Transform;
            if (canvasTransform == null)
            {
                Debug.LogError("DialogService: Invalid canvas. Expected UnityEngine.Transform.");
                return;
            }

            // Convert List<object> to List<InGameCard>
            var cards = new List<Cards.InGameCard>();
            if (options.Cards != null)
            {
                foreach (var card in options.Cards)
                {
                    if (card is Cards.InGameCard inGameCard)
                    {
                        cards.Add(inGameCard);
                    }
                }
            }

            // Create callbacks that convert InGameCard back to object
            UnityEngine.Events.UnityAction<Cards.InGameCard> okSingle = null;
            UnityEngine.Events.UnityAction<List<Cards.InGameCard>> okMultiple = null;
            UnityEngine.Events.UnityAction<Cards.InGameCard> positiveSingle = null;
            UnityEngine.Events.UnityAction<List<Cards.InGameCard>> positiveMultiple = null;

            if (options.OnOkSingle != null)
            {
                okSingle = (card) => options.OnOkSingle(card);
            }
            if (options.OnOkMultiple != null)
            {
                okMultiple = (selectedCards) =>
                {
                    var objects = new List<object>();
                    foreach (var card in selectedCards) objects.Add(card);
                    options.OnOkMultiple(objects);
                };
            }
            if (options.OnPositiveSingle != null)
            {
                positiveSingle = (card) => options.OnPositiveSingle(card);
            }
            if (options.OnPositiveMultiple != null)
            {
                positiveMultiple = (selectedCards) =>
                {
                    var objects = new List<object>();
                    foreach (var card in selectedCards) objects.Add(card);
                    options.OnPositiveMultiple(objects);
                };
            }

            var config = new global::CardSelectorConfig(
                title: options.Title,
                cards: cards,
                showOkButton: options.ShowOkButton,
                showPositiveButton: options.ShowPositiveButton,
                showNegativeButton: options.ShowNegativeButton,
                okAction: okSingle,
                okMultipleAction: okMultiple,
                positiveAction: positiveSingle,
                positiveMultipleAction: positiveMultiple,
                negativeAction: options.OnNegative != null ? new UnityEngine.Events.UnityAction(options.OnNegative) : null,
                numberCardSelection: options.NumberCardSelection,
                showOrder: options.ShowOrder
            );

            CardSelector.CreateCardSelection(canvasTransform, config);
        }

        // Helper method to convert card GUIDs to InGameCard instances
        // TODO: This is a temporary bridge - should be improved
        private List<Cards.InGameCard> ConvertCardIds(List<Guid> cardIds)
        {
            var cards = new List<Cards.InGameCard>();

            // For now, we need to find the actual InGameCard instances
            // This is a limitation of the current CardSelector design
            // In the future, CardSelector should work with IDs directly
            Debug.LogWarning($"DialogService: Card conversion not fully implemented. Returning empty list.");

            return cards;
        }

        // Phase 38: Legacy config support methods for Ability base class migration

        /// <summary>
        /// Shows a message box using the legacy MessageBoxConfig type.
        /// Phase 38: Added for legacy Ability class migration.
        /// Delegates directly to MessageBox.Instance.CreateMessageBox.
        /// </summary>
        public void ShowMessageBoxLegacy(object canvas, object config)
        {
            var canvasTransform = canvas as Transform;
            if (canvasTransform == null)
            {
                Debug.LogError("DialogService: Invalid canvas. Expected UnityEngine.Transform.");
                return;
            }

            if (config is MessageBoxConfig messageBoxConfig)
            {
                MessageBox.CreateMessageBox(canvasTransform, messageBoxConfig);
            }
            else
            {
                Debug.LogError($"DialogService: Invalid config type. Expected MessageBoxConfig, got {config?.GetType().Name ?? "null"}.");
            }
        }

        /// <summary>
        /// Shows a card selector using the legacy CardSelectorConfig type.
        /// Phase 38: Added for legacy Ability class migration.
        /// Delegates directly to CardSelector.Instance.CreateCardSelection.
        /// </summary>
        public void ShowCardSelectorLegacy(object canvas, object config)
        {
            var canvasTransform = canvas as Transform;
            if (canvasTransform == null)
            {
                Debug.LogError("DialogService: Invalid canvas. Expected UnityEngine.Transform.");
                return;
            }

            if (config is global::CardSelectorConfig cardSelectorConfig)
            {
                CardSelector.CreateCardSelection(canvasTransform, cardSelectorConfig);
            }
            else
            {
                Debug.LogError($"DialogService: Invalid config type. Expected CardSelectorConfig, got {config?.GetType().Name ?? "null"}.");
            }
        }
    }
}
