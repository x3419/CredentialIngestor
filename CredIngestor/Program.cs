using Json.Net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CredIngestor
{ 

    class Source
    {
        string Domain;
        string Email;
        string IP;
        string Name;
        string Password;
    }

    class Cred {

        string _id;
        string _index;
        int _score;
        Source _source;
        string _type;

    }

    class CredResult {
        Cred []cred;
    }

    class Program
    {
        static void Main(string[] args)
        {

            int MAX_QUERY_RESULTS = 1000000;

            //// Below details how we will iterate through entire db by checking every printable character
            List<Char> printableChars = new List<char>();
            int from = 0;



            for (int i = char.MinValue+40; i <= char.MaxValue; i++)
            {
                try
                {
                    
                    for(int j = 0; j < MAX_QUERY_RESULTS; j+= 1000)
                    {

                        String url = @"https://scylla.sh/search?q=Password:*" + (char)i + "*&from=" + j + "&size=1000";
                        Console.WriteLine(url);
                        string html = string.Empty;
                        //string url = @"https://scylla.sh/search?q=Password:a*&from=0&size=100";

                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                        request.AutomaticDecompression = DecompressionMethods.GZip;
                        System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                        request.Accept = "application/json";
                        request.Timeout = 40000;
                        request.KeepAlive = true;


                        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                        using (Stream stream = response.GetResponseStream())
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            html = reader.ReadToEnd();
                        }


                        html = html.Replace("\r\n", "");
                        html = html.Replace("\n", "");
                        Console.WriteLine(html);

                        try
                        {

                            String str = html;
                            dynamic stuff = JsonConvert.DeserializeObject(str);

                            try
                            {
                                foreach (dynamic thing in stuff)
                                {
                                    foreach (dynamic thing2 in thing)
                                    {
                                        try
                                        {
                                            if (thing2.Name == "_source")
                                            {

                                                TcpClient tcpclnt = new TcpClient();

                                                tcpclnt.Connect("localhost", 12345);
                                                // use the ipaddress as in the server program

                                                Console.WriteLine("Connected");

                                                Stream stm = tcpclnt.GetStream();

                                                Console.WriteLine(thing2.Value);
                                                str = thing2.Value.ToString();


                                                ASCIIEncoding asen = new ASCIIEncoding();
                                                String strnew = str.Replace("\r\n", "");

                                                byte[] ba = asen.GetBytes(strnew);
                                                Console.WriteLine("Transmitting.....");

                                                stm.Write(ba, 0, ba.Length);

      
                                                tcpclnt.Close();


                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            Console.WriteLine("Problemo.. " + e.ToString());
                                        }


                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("Problem with " + e.ToString());
                            }



                        }

                        catch (Exception e)
                        {
                            Console.WriteLine("Error..... " + e.StackTrace);
                        }
                    }
                    
                } catch(Exception e)
                {
                    Console.WriteLine("Problem: " + e.ToString());
                }
                


            }





            Console.ReadLine();
        }
    }
}


