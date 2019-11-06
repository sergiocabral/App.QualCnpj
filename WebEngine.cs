using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace QualCnpj
{
    /// <summary>
    /// Agrupa as funcionalidade Web.
    /// </summary>
    internal class WebEngine
    {
        public string GetCnpjForAll(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return string.Empty;

            var list = text.Split(new[] {'\n', '\r'}, StringSplitOptions.RemoveEmptyEntries);
            var result = new StringBuilder();
            foreach (var item in list)
            {
                result.AppendLine(GetCnpj(item));
            }
            return result.ToString();
        }

        private string GetCnpj(string item)
        {
            var html = GetHtml($"https://www.google.com/search?q=cnpj.rocks:+{item}");
            
            var result = new StringBuilder();
            result.AppendLine(item);

            foreach (var cnpj in ExtractAllCnpj(html).GroupBy(a => a).OrderByDescending(a => a.Count()).Select(a => a.FirstOrDefault()))
            {
                result.AppendLine(" > " + FormatCnpj(cnpj));
            }

            result.AppendLine();
            return result.ToString();
        }

        private string FormatCnpj(string cnpj) =>
            !Regex.IsMatch(cnpj, "[0-9]{14}") ? 
                cnpj : 
                $"{cnpj.Substring(0, 2)}.{cnpj.Substring(2, 3)}.{cnpj.Substring(5, 3)}/{cnpj.Substring(8, 4)}-{cnpj.Substring(12, 2)}";

        private IEnumerable<string> ExtractAllCnpj(string html)
        {
            foreach (Match match in Regex.Matches(html, @"[0-9]{2}\.?[0-9]{3}\.?[0-9]{3}\/?[0-9]{4}-?[0-9]{2}", RegexOptions.Singleline))
                yield return Regex.Replace(match.Value, "[^0-9]", string.Empty);
        }

        /// <summary>
        /// Retorna o conteúdo HTML de uma página.
        /// </summary>
        /// <param name="url">Página URL.</param>
        /// <returns>Código HTML.</returns>
        public static string GetHtml(string url)
        {
            var webClient = new WebClient();
            var response = webClient.DownloadString(new Uri(url));
            return response;
        }
    }
}
