using System;
using System.Text.RegularExpressions;
using System.Windows.Media.Imaging;
using System.Xml;

namespace TVProgram
{
    public class Program : BindableBase
    {
        public int StationID { get; }
        public string StationName { get; }
        public BitmapImage Source { get; }
        public string Title { get { return _Title; } set { SetProperty(ref _Title, value); } }
        private string _Title;
        //public string Link { get { return _Link; } set { SetProperty(ref _Link, value); } }
        //private string _Link;
        public DateTime? StartTime { get { return _StartTime; } set { SetProperty(ref _StartTime, value); } }
        private DateTime? _StartTime;
        public DateTime? EndTime { get { return _EndTime; } set { SetProperty(ref _EndTime, value); } }
        private DateTime? _EndTime;
        public string Category { get { return _Category; } set { SetProperty(ref _Category, value); } }
        private string _Category;

        public Program(int id, string name, string img)
        {
            StationID = id;
            StationName = name;
            Source = new BitmapImage(new Uri("Resources/" + img, UriKind.Relative));
        }

        private static Regex regex = new Regex(@".*～(?<EndTime>.*?)\s\[(?<Station>.*)\]");
        public void Update(XmlElement node)
        {
            if(node == null)
            {
                Title = "番組情報がありません";
                //Link = "";
                Category = "";
                StartTime = null;
                EndTime = null;
                return;
            }

            //RSSの例
            //  <item rdf:about="http://tv.so-net.ne.jp/schedule/101032201705250000.action?from=rss">
            //    <title>ETV特集　アンコール「人知れず　表現し続ける者たち」[字][再]</title>
            //    <link>http://tv.so-net.ne.jp/schedule/101032201705250000.action?from=rss</link>
            //    <description>5/25 0:00～1:00 [ＮＨＫＥテレ１・東京(Ch.2)]</description>
            //    <dc:subject>
            //        ドキュメンタリー／教養
            //    </dc:subject>
            //    <dc:date>2017-05-25T00:00+09:00</dc:date>
            //    <dc:relation>32737:1032:15423</dc:relation>
            //  </item>
            Title = node.GetInnerText("rss:title").RegexReplace(@"\[..?\]", "");
            //Link = node.GetInnerText("rss:link");
            StartTime = DateTime.Parse(node.GetInnerText("dc:date"));
            Category = node.GetInnerText("dc:subject").RemoveWhitespace();

            var description = node.GetInnerText("rss:description");
            var match = regex.Match(description);
            try
            {
                EndTime = DateTime.Parse(match.Groups["EndTime"].Value);
            }
            catch(Exception) //パース失敗時とりあえず1時間番組とする
            {
                EndTime = StartTime + TimeSpan.FromHours(1);
            }
            if(StartTime > EndTime)
                EndTime += TimeSpan.FromDays(1);
        }
    }
}
