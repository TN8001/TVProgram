using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading;
using System.Xml;

namespace TVProgram
{
    class ViewModel
    {
        public IList<Program> Programs { get; }

        private DispatcherTimer timer;
        public ViewModel()
        {
            Programs = new List<Program>()
            {
                //icon.png 手元では各局のファビコンを設定していますが自粛
                new Program(1024, "NHK総合", "icon.png"),
                new Program(1032, "NHKＥテレ", "icon.png"),
                new Program(24632, "ｔｖｋ", "icon.png"),
                new Program(1040, "日テレ", "icon.png"),
                new Program(1064, "テレビ朝日", "icon.png"),
                new Program(1048, "ＴＢＳ", "icon.png"),
                new Program(1072, "テレビ東京", "icon.png"),
                new Program(1056, "フジテレビ", "icon.png"),

                new Program(101, "NHKＢＳ１", "icon.png"),
                new Program(103, "NHKＢＳＰ", "icon.png"),
                new Program(141, "ＢＳ日テレ", "icon.png"),
                new Program(151, "ＢＳ朝日", "icon.png"),
                new Program(161, "ＢＳ-ＴＢＳ", "icon.png"),
                new Program(171, "ＢＳジャパン", "icon.png"),
                new Program(181, "ＢＳフジ", "icon.png"),
                new Program(211, "BSイレブン", "icon.png"),
                new Program(222, "トゥエルビ", "icon.png"),
            };

            timer = new DispatcherTimer();
            timer.Tick += (s, e) => GetPrograms();
            GetPrograms();
        }
        private void GetPrograms()
        {
            timer.Stop();
            var l = new List<XmlElement>();
            l.AddRange(GetRssItems("http://tv.so-net.ne.jp/rss/schedulesByCurrentTime.action?group=10&stationAreaId=24"));
            l.AddRange(GetRssItems("http://tv.so-net.ne.jp/rss/schedulesByCurrentTime.action?group=21"));
            l.AddRange(GetRssItems("http://tv.so-net.ne.jp/rss/schedulesByCurrentTime.action?group=22"));
            var firstEnd = UpdateAll(l);

            timer.Interval = firstEnd - DateTime.Now + TimeSpan.FromSeconds(5);
            timer.Start();
        }
        private IList<XmlElement> GetRssItems(string url)
        {
            var xml = new XmlDocument();
            xml.Load(url);
            return xml.GetElements("/rdf:RDF/rss:item");
        }
        private DateTime UpdateAll(IEnumerable<XmlElement> items)
        {
            var firstEnd = DateTime.Now + TimeSpan.FromMinutes(30);

            foreach(var p in Programs)
            {
                var n = items.FirstOrDefault((x) =>
                {
                    var id = int.Parse(x.GetInnerText("dc:relation").Split(':')[1]);
                    return p.StationID == id;
                });

                p.Update(n);

                if(firstEnd > p.EndTime && p.StartTime < p.EndTime)
                    firstEnd = (DateTime)p.EndTime;
            }

            return firstEnd;
        }
    }
}
