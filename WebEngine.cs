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
    internal static class WebEngine
    {
        public static string GetCnpjForAll(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return string.Empty;

            var companies = text.Split(new[] {'\n', '\r'}, StringSplitOptions.RemoveEmptyEntries);
            
            var result = new StringBuilder();
            foreach (var company in companies)
            {
                result.AppendLine(GetCnpj(company));
            }
            return result.ToString();
        }

        /// <summary>
        /// Obtem os dados de um CNPJ.
        /// </summary>
        /// <param name="names">Nome da empresa.</param>
        /// <returns>Lista de CNPJ relacionados.</returns>
        private static string GetCnpj(string names)
        {
            var html = GetHtml($"https://www.google.com/search?q=cnpj.rocks:+{names}");
            
            var result = new StringBuilder();
            result.AppendLine(names);

            foreach (var cnpj in ExtractAllCnpj(html).GroupBy(a => a).OrderByDescending(a => a.Count()).Select(a => a.FirstOrDefault()))
            {
                result.AppendLine(" > " + FormatCnpj(cnpj));
            }

            result.AppendLine();
            return result.ToString();
        }

        /// <summary>
        /// Formata a exibição de um CNPJ.
        /// </summary>
        /// <param name="cnpj">CNPJ.</param>
        /// <returns>CNPJ formatado.</returns>
        private static string FormatCnpj(string cnpj) =>
            !Regex.IsMatch(cnpj, "[0-9]{14}") ? 
                cnpj : 
                $"{cnpj.Substring(0, 2)}.{cnpj.Substring(2, 3)}.{cnpj.Substring(5, 3)}/{cnpj.Substring(8, 4)}-{cnpj.Substring(12, 2)}";

        /// <summary>
        /// Extrai todos os CNPJ contidos em um texto. 
        /// </summary>
        /// <param name="text">Texto.</param>
        /// <returns>Lista de CNPJ</returns>
        private static IEnumerable<string> ExtractAllCnpj(string text)
        {
            foreach (Match match in Regex.Matches(text, @"[0-9]{2}\.?[0-9]{3}\.?[0-9]{3}\/?[0-9]{4}-?[0-9]{2}", RegexOptions.Singleline))
                yield return Regex.Replace(match.Value, "[^0-9]", string.Empty);
        }

        /// <summary>
        /// Retorna o conteúdo HTML de uma página.
        /// </summary>
        /// <param name="url">Página URL.</param>
        /// <returns>Código HTML.</returns>
        private static string GetHtml(string url)
        {
            var webClient = new WebClient();
            var response = webClient.DownloadString(new Uri(url));
            return response;
        }
    }
}
