using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

namespace TVProgram
{
    public static class RssExtensions
    {
        private static class XmlNamespace
        {
            public static XmlNamespaceManager Rss;
            static XmlNamespace()
            {
                Rss = new XmlNamespaceManager(new NameTable());
                Rss.AddNamespace("rdf", "http://www.w3.org/1999/02/22-rdf-syntax-ns#");
                Rss.AddNamespace("rss", "http://purl.org/rss/1.0/");
                Rss.AddNamespace("dc", "http://purl.org/dc/elements/1.1/");
            }
        }
        ///<summary>拡張メソッド RSS1決め打ちで要素を取得</summary>
        public static IList<XmlElement> GetElements(this XmlDocument node, string xpath)
            => node.SelectNodes(xpath, XmlNamespace.Rss).Cast<XmlElement>().ToList();
        ///<summary>拡張メソッド RSS1決め打ちでインナーテキストを取得</summary>
        public static string GetInnerText(this XmlElement node, string xpath)
            => node.SelectNodes(xpath, XmlNamespace.Rss)[0].InnerText;
    }

    public static class StringExtensions
    {
        public static string RemoveWhitespace(this string input)
            => new string(input.ToCharArray().Where(c => !char.IsWhiteSpace(c)).ToArray());
        public static string RegexReplace(this string input, string pattern, string replacement)
            => Regex.Replace(input, pattern, replacement);
    }
}
