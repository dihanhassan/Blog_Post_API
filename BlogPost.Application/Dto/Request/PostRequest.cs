using System.Text;
using System.Text.RegularExpressions;

namespace BlogPost.Application.Dto.Request
{
    public class PostRequest
    {
        public string? Title { get; set; }
        public string? Content { get; set; }
        public List<int>? CategoryIds { get; set; }
        public string Route
        {
            get
            {
                return GenerateSlug(Title);
            }
        }
        private static string GenerateSlug(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                return string.Empty;

            // Step 1: Transliterate Bengali to Banglish
            StringBuilder banglishSlug = new StringBuilder();
            foreach (char c in title)
            {
                banglishSlug.Append(Transliterate(c));
            }

            // Step 2: Normalize and format the slug
            string slug = banglishSlug.ToString().ToLowerInvariant();

            // Step 3: Remove any character that is not a letter, number, space, or dash
            slug = Regex.Replace(slug, @"[^\p{L}\p{N}\s-]", "");

            // Step 4: Replace multiple spaces or hyphens with a single space
            slug = Regex.Replace(slug, @"[\s-]+", " ").Trim();

            // Step 5: Replace spaces with hyphens
            slug = slug.Replace(" ", "-");

            return slug;
        }
        private static string Transliterate(char c)
        {
            // Mapping of Bengali characters to Banglish (Roman) characters
            var bengaliToBanglishMap = new Dictionary<char, string>
            {
                { 'অ', "o" }, { 'আ', "a" }, { 'ই', "i" }, { 'ঈ', "ee" },
                { 'উ', "u" }, { 'ঊ', "oo" }, { 'ঋ', "ri" }, { 'এ', "e" },
                { 'ঐ', "oi" }, { 'ও', "o" }, { 'ঔ', "ou" },

                { 'ক', "k" }, { 'খ', "kh" }, { 'গ', "g" }, { 'ঘ', "gh" },
                { 'ঙ', "ng" }, { 'চ', "ch" }, { 'ছ', "chh" }, { 'জ', "j" },
                { 'ঝ', "jh" }, { 'ঞ', "ny" }, { 'ট', "t" }, { 'ঠ', "th" },
                { 'ড', "d" }, { 'ঢ', "dh" }, { 'ণ', "n" }, { 'ত', "t" },
                { 'থ', "th" }, { 'দ', "d" }, { 'ধ', "dh" }, { 'ন', "n" },
                { 'প', "p" }, { 'ফ', "f" }, { 'ব', "b" }, { 'ভ', "bh" },
                { 'ম', "m" }, { 'য', "j" }, { 'র', "r" }, { 'ল', "l" },
                { 'শ', "sh" }, { 'ষ', "sh" }, { 'স', "s" }, { 'হ', "h" },
                { 'ড়', "r" }, { 'ঢ়', "rh" }, { 'য়', "y" }, { 'ঌ', "l" },

                // Signs
                { 'ং', "ng" }, // Anusvara
                { 'ঃ', "h" },  // Visarga
                { 'ঁ', "m" },  // Chandrabindu
                { '্', "" },     // Halanta (no transliteration)
                { 'া', "a" }
            };

            return bengaliToBanglishMap.TryGetValue(c, out string banglishChar) ? banglishChar : c.ToString();
        }
    }
}
