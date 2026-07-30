#nullable enable
using System;
using UnityEngine;

namespace HeartOfPrince.Presentation
{
    /// <summary>
    /// General option data for focused option selection. This type is intentionally
    /// independent from Yarn so dialogue, activities, topics, and other gameplay
    /// systems can all use the same carousel.
    /// </summary>
    public sealed class OptionSelectionItem
    {
        public OptionSelectionItem(
            string id,
            string displayText,
            string? category = null,
            Texture2D? icon = null,
            bool isEnabled = true,
            string? requirementLabel = null,
            object? payload = null,
            Action<OptionSelectionItem>? onSelected = null)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException(
                    "An option requires a stable identifier.",
                    nameof(id));
            }

            Id = id;
            DisplayText = displayText ?? string.Empty;
            Category = category;
            Icon = icon;
            IsEnabled = isEnabled;
            RequirementLabel = requirementLabel;
            Payload = payload;
            OnSelected = onSelected;
        }

        public string Id { get; }
        public string DisplayText { get; }
        public string? Category { get; }
        public Texture2D? Icon { get; }
        public bool IsEnabled { get; }
        public string? RequirementLabel { get; }
        public object? Payload { get; }
        public Action<OptionSelectionItem>? OnSelected { get; }
    }
}
