using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Syndication;
using System.Xml;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            XmlReader reader = XmlReader.Create("http://pipes.yahoo.com/pipes/pipe.run?_id=81d29ca6281bbffdae866bf3aed05203&_render=rss");

            SyndicationFeed feed = SyndicationFeed.Load(reader);

            foreach (var item in feed.Items)
            {
                Console.WriteLine(item.Title.Text);
            }

            Console.ReadLine();
        }
    }
}
