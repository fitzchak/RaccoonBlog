using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace RaccoonBlog.Web.Common
{
    public class SlugConverter
    {
        public static string TitleToSlug(string title)
        {
            // 2 - Strip diacritical marks using Michael Kaplan's function or equivalent
            title = RemoveDiacritics(title);

            // 3 - Lowercase the string for canonicalization
            title = title.ToLowerInvariant();

            // 4 - Replace all the non-word characters with dashes
            title = ReplaceNonWordWithDashes(title);

            // 1 - Trim the string of leading/trailing whitespace
            title = title.Trim(' ', '-');

            return title;
        }


        // http://blogs.msdn.com/michkap/archive/2007/05/14/2629747.aspx
        /// <summary>
        /// Strips the value from any non English character by replacing those with their English equivalent.
        /// </summary>
        /// <param name="value">The string to normalize.</param>
        /// <returns>A string where all characters are part of the basic English ANSI encoding.</returns>
        /// <seealso cref="http://stackoverflow.com/questions/249087/how-do-i-remove-diacritics-accents-from-a-string-in-net"/>
        private static string RemoveDiacritics(string value)
        {
            string stFormD = value.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            for (int ich = 0; ich < stFormD.Length; ich++)
            {
                UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(stFormD[ich]);
                if (uc != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(stFormD[ich]);
                }
            }

            return (sb.ToString().Normalize(NormalizationForm.FormC));
        }

        private static string ReplaceNonWordWithDashes(string title)
        {
            // Remove Apostrophe Tags
            title = Regex.Replace(title, "[’'“”\"&]{1,}", "", RegexOptions.None);

            // Replaces all non-alphanumeric character by a space
            var builder = new StringBuilder();
            for (int i = 0; i < title.Length; i++)
            {
                builder.Append(char.IsLetterOrDigit(title[i]) ? title[i] : ' ');
            }

            title = builder.ToString();

            // Replace multiple spaces into a single dash
            title = Regex.Replace(title, "[ ]{1,}", "-", RegexOptions.None);

            return title;
        }
    }
}
