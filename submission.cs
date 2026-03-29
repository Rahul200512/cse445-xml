using System;
using System.Xml.Schema;
using System.Xml;
using Newtonsoft.Json;
using System.IO;
using System.Xml.Linq;
using System.Xml.Xsl;
using System.Xml.XPath;
using System.Linq;



/**
 * This template file is created for ASU CSE445 Distributed SW Dev Assignment 4.
 * Please do not modify or delete any existing class/variable/method names. However, you can add more variables and functions.
 * Uploading this file directly will not pass the autograder's compilation check, resulting in a grade of 0.
 * **/


namespace ConsoleApp1
{


    public class Submission
    {
        public static string xmlURL = "https://rahul200512.github.io/cse445-xml/Hotels.xml";
        public static string xmlErrorURL = "https://rahul200512.github.io/cse445-xml/HotelsError.xml";
        public static string xsdURL = "https://rahul200512.github.io/cse445-xml/Hotels.xsd";
        public static string xsltURL = "https://rahul200512.github.io/cse445-xml/Hotels.xslt";

        public static void Main(string[] args)
        {
            // Q2 Test
            string result = Verification(xmlURL, xsdURL);
            Console.WriteLine("Q2 Valid XML: " + result);

            result = Verification(xmlErrorURL, xsdURL);
            Console.WriteLine("Q2 Error XML: " + result);

            // Q3 Test
            result = Transformation(xmlURL, xsltURL);
            Console.WriteLine("Q3 Transformation:\n" + result);

            // Q4 Test
            result = XPathSearching(xmlURL, 4);
            Console.WriteLine("Q4 XPath (>=4 stars): " + result);

            result = XmlSearching(xmlURL, 4);
            Console.WriteLine("Q4 XmlSearch (>=4 stars): " + result);

            // Q5 Test
            result = Xml2Json(xmlURL);
            Console.WriteLine("Q5 Xml2Json:\n" + result);

            result = JsonSearching(xmlURL, 4);
            Console.WriteLine("Q5 JsonSearch (>=4 stars): " + result);
        }

        // Q2.1 - Validate XML against XSD schema
        public static string Verification(string xmlUrl, string xsdUrl)
        {
            try
            {
                string xsdContent = DownloadContent(xsdUrl);
                XmlSchemaSet schemas = new XmlSchemaSet();
                schemas.Add(null, XmlReader.Create(new StringReader(xsdContent)));

                string xmlContent = DownloadContent(xmlUrl);
                XDocument doc = XDocument.Parse(xmlContent);

                string errorMessage = "No Error";
                doc.Validate(schemas, (sender, e) =>
                {
                    errorMessage = e.Message;
                });

                return errorMessage;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        // Q3 - Transform XML using XSLT into HTML
        public static string Transformation(string xmlUrl, string xsltUrl)
        {
            try
            {
                string xmlContent = DownloadContent(xmlUrl);
                string xsltContent = DownloadContent(xsltUrl);

                XslCompiledTransform xslt = new XslCompiledTransform();
                using (StringReader sr = new StringReader(xsltContent))
                using (XmlReader xr = XmlReader.Create(sr))
                {
                    xslt.Load(xr);
                }

                StringWriter resultWriter = new StringWriter();
                using (StringReader sr = new StringReader(xmlContent))
                using (XmlReader xr = XmlReader.Create(sr))
                {
                    xslt.Transform(xr, null, resultWriter);
                }

                return resultWriter.ToString();
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        // Q4.1 - Search hotels using XPath by star rating
        public static string XPathSearching(string xmlUrl, int stars)
        {
            try
            {
                string xmlContent = DownloadContent(xmlUrl);
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xmlContent);

                XmlNamespaceManager nsMgr = new XmlNamespaceManager(doc.NameTable);
                nsMgr.AddNamespace("h", "https://rahul200512.github.io/cse445-xml");
                XmlNodeList nodes = doc.SelectNodes("//h:Hotel[@Rating >= " + stars + "]", nsMgr);

                string result = "";
                foreach (XmlNode node in nodes)
                {
                    if (result != "") result += "\n";
                    string name = node.SelectSingleNode("h:Name", nsMgr).InnerText;
                    string phone = node.SelectSingleNode("h:Phone", nsMgr).InnerText;
                    string street = node.SelectSingleNode("h:Address/h:Street", nsMgr).InnerText;
                    string city = node.SelectSingleNode("h:Address/h:City", nsMgr).InnerText;
                    string state = node.SelectSingleNode("h:Address/h:State", nsMgr).InnerText;
                    string zip = node.SelectSingleNode("h:Address/h:Zip", nsMgr).InnerText;
                    int rating = int.Parse(node.Attributes["Rating"].Value);
                    result += name + ", " + phone + ", " + street + ", " + city + ", " + state + ", " + zip + ", " + rating + " Stars";
                }

                return result;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        // Q4.2 - Search hotels element-by-element by star rating
        public static string XmlSearching(string xmlUrl, int stars)
        {
            try
            {
                string xmlContent = DownloadContent(xmlUrl);
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xmlContent);

                string ns = "https://rahul200512.github.io/cse445-xml";
                XmlNodeList hotels = doc.GetElementsByTagName("Hotel", ns);
                string result = "";

                foreach (XmlNode hotel in hotels)
                {
                    int rating = int.Parse(hotel.Attributes["Rating"].Value);
                    if (rating >= stars)
                    {
                        if (result != "") result += "\n";
                        string name = hotel["Name", ns].InnerText;
                        string phone = hotel["Phone", ns].InnerText;
                        XmlNode addr = hotel["Address", ns];
                        string street = addr["Street", ns].InnerText;
                        string city = addr["City", ns].InnerText;
                        string state = addr["State", ns].InnerText;
                        string zip = addr["Zip", ns].InnerText;
                        result += name + ", " + phone + ", " + street + ", " + city + ", " + state + ", " + zip + ", " + rating + " Stars";
                    }
                }

                return result;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        // Q5 helper - Convert XML to JSON
        public static string Xml2Json(string xmlUrl)
        {
            try
            {
                string xmlContent = DownloadContent(xmlUrl);
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xmlContent);

                string jsonText = JsonConvert.SerializeXmlNode(doc);
                return jsonText;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        // Q5.1 - Search hotels from JSON by star rating
        public static string JsonSearching(string xmlUrl, int stars)
        {
            try
            {
                string jsonText = Xml2Json(xmlUrl);
                dynamic jsonObj = JsonConvert.DeserializeObject(jsonText);

                var hotels = jsonObj["Hotels"]["Hotel"];
                string result = "";

                foreach (var hotel in hotels)
                {
                    int rating = int.Parse((string)hotel["@Rating"]);
                    if (rating >= stars)
                    {
                        if (result != "") result += "\n";
                        string name = (string)hotel["Name"];
                        string phone = (string)hotel["Phone"];
                        string street = (string)hotel["Address"]["Street"];
                        string city = (string)hotel["Address"]["City"];
                        string state = (string)hotel["Address"]["State"];
                        string zip = (string)hotel["Address"]["Zip"];
                        result += name + ", " + phone + ", " + street + ", " + city + ", " + state + ", " + zip + ", " + rating + " Stars";
                    }
                }

                return result;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        // Q5.2 - Custom student service: Find hotels in a specific city
        public static string StudentService(string xmlUrl, string city)
        {
            try
            {
                string xmlContent = DownloadContent(xmlUrl);
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xmlContent);

                string ns = "https://rahul200512.github.io/cse445-xml";
                XmlNodeList hotels = doc.GetElementsByTagName("Hotel", ns);
                string result = "";

                foreach (XmlNode hotel in hotels)
                {
                    string hotelCity = hotel["Address", ns]["City", ns].InnerText;
                    if (hotelCity.Equals(city, StringComparison.OrdinalIgnoreCase))
                    {
                        if (result != "") result += "\n";
                        string name = hotel["Name", ns].InnerText;
                        string phone = hotel["Phone", ns].InnerText;
                        int rating = int.Parse(hotel.Attributes["Rating"].Value);
                        result += name + ", " + phone + ", " + rating + " Stars";
                    }
                }

                if (string.IsNullOrEmpty(result))
                    return "No hotels found in " + city;

                return result;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        // Helper method to download content from URL
        private static string DownloadContent(string url)
        {
            using (System.Net.WebClient client = new System.Net.WebClient())
            {
                return client.DownloadString(url);
            }
        }
    }

}
