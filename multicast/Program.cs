using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
/// <summary>
/// multicast protokollaa käyttävä chätti ohjelma käyttäen soketteja
/// </summary>
namespace Multicastapp
{
    class Program
    {

        

        public static void Main()
        {

           bool toimiko = vali();


             
        }

            public static string dataOtsikkosta(byte[] tavut)
            {
                string nimi = Encoding.UTF8.GetString(tavut, 6 + tavut[4], tavut[5 + tavut[4]]) + Encoding.UTF8.GetString(tavut, 5 + tavut[4] + tavut[5 + tavut[4]] + 2, tavut[5 + tavut[4] + tavut[5 + tavut[4]] + 1]);
            
            
                return nimi;
            }


        public static bool vali()
        {


            //Console.WriteLine("anna soketille portti");
            //int portti = int.Parse(Console.ReadLine());
            IPAddress address = IPAddress.Parse("239.0.0.1");
            IPEndPoint kohde = new IPEndPoint(address, 42000);
            IPEndPoint asd = new IPEndPoint(IPAddress.Any, 42000);
            EndPoint remoteEP = (EndPoint)new IPEndPoint(IPAddress.Any, 42000);

            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            Socket vast = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            //UdpClient s = new UdpClient(42000,AddressFamily.InterNetwork);
            //MulticastOption Multicast = new MulticastOption(address);
            vast.Bind(asd);
            //s.JoinMulticastGroup(address);
            s.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(address));
            vast.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(address));
            //s.SetSocketOption(SocketOptionLevel.IP,SocketOptionName.MulticastTimeToLive, 1);
            s.Connect(kohde);
            vast.ReceiveTimeout = 1000;
            //s.SendTimeout = 1000;
            //Thread t = new Thread(new ThreadStart(otto));
            //t.Start();
            Console.WriteLine("kirjoita käyttäjänimi");
            string kayttaja = Console.ReadLine();
            Console.WriteLine("kirjoita päivä");
            int paiva = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("kirjoita kuukausi");
            int kk = Convert.ToInt32(Console.ReadLine());
            
            Console.WriteLine("kirjoita vuosi");
            int vuosi = Convert.ToInt32(Console.ReadLine());
            string viesti = "";

            List<string> listakayttajista = new List<string> { };


            //viesti = Convert.ToString(udpClient.Receive(ref kohde));
            //Console.WriteLine("\nThe ClientTarget received: " + "\n\n" + viesti + "\n");



            byte[] rec = new byte[256];


            while (true)
            {
                try
                {
                    if (Console.KeyAvailable)
                    {
                        rec = asetaTavut(1, 3, paiva, kk, vuosi, "TIEA322saseilmo", kayttaja, viesti = Console.ReadLine()); // Reader.ReadLine(5000));

                        if (viesti.Equals("lopeta"))
                        {
                            s.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.DropMembership, new MulticastOption(address));
                            //s.DropMulticastGroup(address);
                            s.Close();

                            break;

                        }

                        s.Send(rec, rec.Length, SocketFlags.None);
                        continue;
                    }

                    try
                    {
                        byte[] rec2 = new byte[256];
                        vast.Receive(rec2);
                        if (Versio(rec) > 3 | Versio(rec) < 1)
                        {
                            continue;
                        }
                        //update viesti
                        /*if()
                        {

                        }*/

                        string vastaus = dataOtsikkosta(rec2);
                        Console.WriteLine(vastaus);
                        continue;
                    }
                    catch
                    {
                        continue;
                    }



                }
                catch
                {
                    continue;
                }
                 

                
            }
            //soketti.Close();
            
            return true;








        }


        public static int Versio(byte[] tavut)
        {
            int versio = (byte)(tavut[0] >> 4);
            

            return versio;
        }


        public static void otto()
        {



            Socket vastaanotto;
            //UdpClient vastaanotto = new UdpClient(2000, AddressFamily.InterNetwork);
            IPAddress address = IPAddress.Parse("224.0.0.2");
            IPEndPoint tulo = new IPEndPoint(IPAddress.Any, 42000);
            EndPoint remoteEP = (EndPoint)new IPEndPoint(IPAddress.Any, 42000);
            EndPoint senderRemote = (EndPoint)tulo;
            IPEndPoint kohde = new IPEndPoint(address, 42000);
            //vastaanotto.JoinMulticastGroup(address);
            vastaanotto = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            //vastaanotto.Bind(tulo);
            vastaanotto.Connect(kohde);
            vastaanotto.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(address, IPAddress.Any));

            var saapuva = new byte[256];
            vastaanotto.ReceiveFrom(saapuva,ref remoteEP); //.Receive(saapuva);
            if (Versio(saapuva) <= 3 & Versio(saapuva) >= 1)
            {
                string vastaus = dataOtsikkosta(saapuva);
                Console.WriteLine(vastaus);
            }
            //vastaanotto.DropMulticastGroup(address);
            vastaanotto.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.DropMembership, new MulticastOption(address));
            vastaanotto.Close();
        }




        public static byte[] asetaTavut(int versio, int viesti,
                                    int day, int month, int year,
                                    string asiakasnimi,
                                    string usernimi, string teksti)
        {
            // selvitä string-kenttien pituudet UTF-8 tavuina
            byte[] buffer = Encoding.UTF8.GetBytes(asiakasnimi);
            byte[] buffer2 = Encoding.UTF8.GetBytes(usernimi);
            byte[] buffer3 = Encoding.UTF8.GetBytes(teksti);
            int clientLength = buffer.Length; // UTF-8 koodattujen tavujen määrä
            int userLength = buffer2.Length; // UTF-8 koodattujen tavujen määrä
            int dataLength = buffer3.Length; // UTF-8 koodattujen tavujen määrä
                                             // Otsikon vakiopituiset kentät on 7 tavua
            int constLength = 7;
            byte[] tavut = new byte[constLength + clientLength + userLength + dataLength];
            byte ekat = (byte)(versio << 4);
            ekat = (byte)(ekat ^ viesti);
            tavut[0] = ekat;

            int indeksi = 5;
            byte pai = 0;
            byte kk = 0;
            byte vuo = 0;
            pai = (byte)(day << 3);
            pai = (byte)(pai ^ (month >> 1));
            kk = (byte)(month << 7);
            kk = (byte)(kk ^ (year >> 4));
            vuo = (byte)(year << 4);
            tavut[1] = pai;
            tavut[2] = kk;
            tavut[3] = vuo;
            tavut[4] = Convert.ToByte(buffer.Length);

            for (int i = 0; i < buffer.Length; i++)
            {
                tavut[5 + i] = buffer[i];
                indeksi++;
            }
            tavut[4 + buffer.Length + 1] = Convert.ToByte(buffer2.Length);
            int aloitus = indeksi;
            for (int i = 0; i < buffer2.Length; i++)
            {
                tavut[aloitus + i + 1] = buffer2[i];
                indeksi++;
            }
            tavut[indeksi + 1] = Convert.ToByte(buffer3.Length);
            indeksi++;
            aloitus = indeksi;
            for (int i = 0; i < buffer3.Length; i++)
            {
                tavut[aloitus + i + 1] = buffer3[i];
                indeksi++;
            }
            return tavut;
        }


        public static string[] nimettauluna(byte[] tavut)
        {

            //kaksi ekaa tavua turhat
            int indeksi = 4;
            int pituus = 0;
            var nimetList = new List<string>();
            string nimi;
            indeksi = indeksi + Convert.ToInt32(tavut[indeksi]);
            indeksi = indeksi + 1;
            indeksi = indeksi + Convert.ToInt32(tavut[indeksi]);
            
            while (indeksi < 255 | (indeksi + pituus) < tavut.Length)
            {
                pituus = Convert.ToInt32(tavut[indeksi]);
                if (indeksi + pituus > 255 | indeksi + pituus > tavut.Length)
                {
                    break;
                }

                pituus = Convert.ToInt32(tavut[indeksi]);
                nimi = Encoding.UTF8.GetString(tavut, indeksi, pituus);
                nimetList.Add(nimi);
                indeksi = indeksi + pituus;
            }


           
            string[] nimet = nimetList.ToArray();
            return nimet;
        }


    }



}
