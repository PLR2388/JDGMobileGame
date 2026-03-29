using System.Globalization;
using System.Text;

namespace JDG.Application.Services
{
    /// <summary>
    /// Generates card IDs from asset names for localization lookup.
    /// Phase 166: Extracted from LocalizationService to enable use from JDG.Infrastructure.
    /// </summary>
    public static class CardIdGenerator
    {
        public static string GenerateCardId(string assetName)
        {
            if (string.IsNullOrEmpty(assetName))
                return string.Empty;

            var normalized = assetName
                .ToLowerInvariant()
                .Normalize(NormalizationForm.FormD);

            var sb = new StringBuilder();
            foreach (var c in normalized)
            {
                var category = CharUnicodeInfo.GetUnicodeCategory(c);
                if (category != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }

            return sb.ToString()
                .Replace(" ", "-")
                .Replace("'", "")
                .Replace("!", "")
                .Replace(".", "")
                .Replace(",", "")
                .Replace("?", "");
        }
    }
}
