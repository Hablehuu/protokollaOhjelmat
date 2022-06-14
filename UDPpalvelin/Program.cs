using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using System.Linq;


/// <summary>
/// UDP protokollaa käyttävä viesti palvelin joka toimii soketeilla.
/// palvelin tallentaa käyttäjät ja toimii viestin välittäjänä
///
/// </summary>
namespace UDPPalvelin
{
    class Program
    {
        static void Main(string[] args)
        {
            Socket palvelin = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint iep = new IPEndPoint(IPAddress.Any, 25000);
            palvelin.Bind(iep);
            byte[] rec = new byte[3000];
            int paljon = 0;
            IPEndPoint iap = new IPEndPoint(IPAddress.Any, 0);
            EndPoint senderRemote = (EndPoint)iap;
            EndPoint[] kayttajatIP = new EndPoint[20];
            int maara = 0;
            string viesti = "";
            while (true)
            {

                paljon = palvelin.ReceiveFrom(rec, ref senderRemote);
                if (!kayttajatIP.Contains(senderRemote))
                {
                    kayttajatIP[maara] = senderRemote;
                    maara++;
                }
                Console.WriteLine("yhteys osoitteesta: " + senderRemote.ToString());
                viesti = System.Text.Encoding.ASCII.GetString(rec, 0, paljon);
                Console.WriteLine(viesti);
                if (viesti.Contains(";"))
                {

                    byte[] viestii = System.Text.Encoding.ASCII.GetBytes(viesti);
                    for (int i = 0; i < maara; i++)
                    {
                        palvelin.SendTo(viestii, kayttajatIP[i]);
                    }
                }
                else
                {
                    string virhe = "viesti oli väärässä moudossa";
                    byte[] viestii = System.Text.Encoding.ASCII.GetBytes(virhe);
                    palvelin.SendTo(viestii, senderRemote);
                }


            }
            //Console.ReadKey();
            //palvelin.Close();
        }


    }


}
