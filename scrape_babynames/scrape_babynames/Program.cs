using System;
using System.IO;
using System.Net;
using System.Text;
using System.Collections.Generic;

namespace scrape_babynames
{
    class Program
    {
        static void Main(string[] args)
        {
            ScrapeForenames(((args.Length > 0) && (args[0] == "p" || args[0] == "P")) ? true : false);
        }

        static void ScrapeForenames(bool showProgress)
        { 
            List<string> boysnames = new List<string>();
            List<string> girlsnames = new List<string>();
            string url;

            for ( int year = 2011; year <= 2018; year++ )
            {
                try
                {
                    url = "https://www.britishbabynames.com/blog/top-1000-names-in-england-and-wales-" + year + ".html";

                    if ( showProgress )
                        Console.WriteLine(url);

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    request.Method = "GET";
                    WebResponse response = request.GetResponse();
                    StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                    string result = reader.ReadToEnd();

                    string boysSearch  = "#0060bf;"; // boys are blue
                    string girlsSearch = "#ff007f;"; // girls are pink
                    string line;

                    StringReader sr = new StringReader(result);
                    
                    do
                    {
                        line = sr.ReadLine();
                        if ( line != null )
                        {
                            if ( line.Contains(boysSearch) || line.Contains(girlsSearch) )
                            {
                                bool boy = (line.Contains(boysSearch));
                                string[] arr = line.Split('>');
                                foreach (string item in arr)
                                {
                                    if ( item.Contains("&") )
                                    {
                                        string[] names = item.Split('&');                                        
                                        if (names[0].Length != 0)
                                        {
                                            string name = names[0].ToLower();
                                            name = name.Substring(0, 1).ToUpper() + name.Substring(1, name.Length - 1);
                                            if (name.Contains("-"))
                                            {
                                                string[] s = name.Split('-');
                                                name = s[0] + "-" + s[1].Substring(0, 1).ToUpper() + s[1].Substring(1, s[1].Length - 1);
                                            }
                                            if (showProgress)
                                                Console.WriteLine(name);
                                            if (boy && !boysnames.Contains(name))                                                
                                                boysnames.Add(name);
                                            if (!boy && !girlsnames.Contains(name))                                                
                                                girlsnames.Add(name);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    while ( line != null );

                    boysnames.Sort();
                    girlsnames.Sort();

                    string filename = "UKBoysNames.txt";
                    using (StreamWriter file = new StreamWriter(filename))
                    {
                        foreach (string item in boysnames)
                            file.WriteLine(item);
                    }

                    filename = "UKGirlsNames.txt";
                    using (StreamWriter file = new StreamWriter(filename))
                    {
                        foreach (string item in girlsnames)
                            file.WriteLine(item);
                    }

                }
                catch (Exception ex)
                {
                    string errorfilename = "scrape_babynames.log";
                    using (StreamWriter file = new StreamWriter(errorfilename))
                    {
                        file.WriteLine(DateTime.Now + " : Application Error : " + ex.Message);
                    }
                }
            }

            if (showProgress)
            {
                Console.WriteLine("Press any key to continue");
                Console.ReadLine();
            }

        }

    }
}
