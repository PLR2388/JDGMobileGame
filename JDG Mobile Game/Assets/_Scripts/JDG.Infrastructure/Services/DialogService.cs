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
    /// </summary>
    public class DialogService : IDialogService
    {
        private readonly MessageBox _messageBox;
        private readonly CardSelector _cardSelector;
        private Transform _canvas;

        public DialogService()
        {
            // During migration, get the existing singletons
            // TODO: Later, inject dialog dependencies directly
            _messageBox = MessageBox.Instance;
            _cardSelector = CardSelector.Instance;
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

            _messageBox.CreateMessageBox(_canvas, config);

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
                    var selectedIds = new List<Guid>();
                    foreach (var card in selectedCards)
                    {
                        selectedIds.Add(card.Id);
                    }
                    tcs.TrySetResult(selectedIds);
                },
                showNegativeButton: config.AllowCancel,
                negativeAction: config.AllowCancel ? () => tcs.TrySetResult(null) : null,
                numberCardSelection: config.MaxSelection
            );

            _cardSelector.CreateCardSelection(_canvas, cardSelectorConfig);

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
    }
}
