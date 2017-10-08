using System.Text.RegularExpressions;

namespace GraphQLSpike
{
    public class TextAnalyzer
    {
        public AnalysisResponse SearchForEmail(string text)
        {
            Regex emailRegex = new Regex(@"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*",
                RegexOptions.IgnoreCase);
            var response = AnalyzeRegex(text, emailRegex, "#EmailNotAllowed#");
            return response;
        }

        private static AnalysisResponse AnalyzeRegex(string text, Regex emailRegex, string message)
        {
            bool emailFound = false;
            MatchCollection emailMatches = emailRegex.Matches(text);
            foreach (Match emailMatch in emailMatches) {
                emailFound = true;
                text = text.Replace(emailMatch.Value, message);
            }

            var response = new AnalysisResponse();
            response.PersonalInformationFound = emailFound;
            response.Text = text;
            return response;
        }

        public AnalysisResponse SearchForFiscalCode(string text)
        {

            Regex emailRegex = new Regex(@"/^[A-Z]{6}\d{2}[A-Z]\d{2}[A-Z]\d{3}[A-Z]$/i",
                RegexOptions.IgnoreCase);
            var response = AnalyzeRegex(text, emailRegex, "#FiscalCodeNotAllowed#");
            return response;
        }
    }

    public class AnalysisResponse
    {
        public bool PersonalInformationFound { get; set; }
        public string Text { get; set; }
    }
}