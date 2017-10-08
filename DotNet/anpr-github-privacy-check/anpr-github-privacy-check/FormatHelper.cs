namespace GraphQLSpike
{
    public static class FormatHelper
    {
        public static string FormatWithQuotes(string text)
        {
            if (text == null) {
                return null;
            }
            text = "\"" + text + "\"";
            return text;
        }
    }
}