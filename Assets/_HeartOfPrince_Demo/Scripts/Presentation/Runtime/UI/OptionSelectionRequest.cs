#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;

namespace HeartOfPrince.Presentation
{
    public sealed class OptionSelectionRequest
    {
        public OptionSelectionRequest(
            IEnumerable<OptionSelectionItem> options,
            string? title = null,
            string? emptyMessage = null,
            string? initialOptionId = null,
            bool closeOnSelection = true,
            Action<OptionSelectionItem>? onSelected = null)
        {
            Options = options?.ToList() ??
                throw new ArgumentNullException(nameof(options));

            Title = string.IsNullOrWhiteSpace(title)
                ? "Choose"
                : title;

            EmptyMessage = string.IsNullOrWhiteSpace(emptyMessage)
                ? "No options are available."
                : emptyMessage;

            InitialOptionId = initialOptionId;
            CloseOnSelection = closeOnSelection;
            OnSelected = onSelected;
        }

        public IReadOnlyList<OptionSelectionItem> Options { get; }
        public string Title { get; }
        public string EmptyMessage { get; }
        public string? InitialOptionId { get; }
        public bool CloseOnSelection { get; }
        public Action<OptionSelectionItem>? OnSelected { get; }
    }
}
