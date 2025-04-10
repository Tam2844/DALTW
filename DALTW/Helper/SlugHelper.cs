using System.Globalization;
using System.Text.RegularExpressions;
using System.Text;

namespace DALTW.Helper
{
    public static class SlugHelper
    {
        public static string GenerateSlug(string phrase)
        {
            string str = RemoveDiacritics(phrase).ToLower();

            // Xóa ký tự không hợp lệ và thay thế khoảng trắng bằng dấu gạch ngang
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
            str = Regex.Replace(str, @"\s+", "-").Trim('-');
            str = Regex.Replace(str, @"-+", "-");

            return str;
        }

        private static string RemoveDiacritics(string text)
        {
            var normalized = text.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (var ch in normalized)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(ch);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(ch);
                }
            }

            return sb.ToString().Normalize(NormalizationForm.FormC)
                .Replace("đ", "d")
                .Replace("Đ", "D");
        }
    }

}
