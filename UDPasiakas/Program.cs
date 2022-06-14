using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using System.Linq;
public class httpAsiakasLuokka
{
    /// <summary>
    /// UDP protokollaa käyttävä chätti asiakas
    /// </summary>
    public static void Main()
    {
        
        Socket soketti = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        IPEndPoint iep = null;
        string viesti = "";
        byte[] rec = new byte[3000];
        int paljon = 0;
        Console.Write("kirjoita osoite johon yhdistetään:");
        string syote = Console.ReadLine();
        Console.Write("kirjoita portti:");
        string portti = Console.ReadLine();
        IPAddress[] addresses = Dns.GetHostAddresses(syote);
        IPAddress ip = (IPAddress)addresses.GetValue(1);
        iep = new IPEndPoint(ip, int.Parse(portti));
        EndPoint senderRemote = (EndPoint)iep;
        soketti.ReceiveTimeout = 1000;
        byte[] viestii = System.Text.Encoding.ASCII.GetBytes(viesti);
        while (true)
        {
            try
            {
                
                soketti.SendTo(viestii = System.Text.Encoding.ASCII.GetBytes(viesti = Console.ReadLine()), senderRemote);
                if (viesti.Equals("lopeta"))
                {
                    soketti.Close();
                } 
            }catch
            {
                continue;
            }
            try
            {
                
                paljon = soketti.Receive(rec);
                string vastaus = System.Text.Encoding.ASCII.GetString(rec, 0, paljon);
                Console.WriteLine(vastaus);
                
            }catch
            {
                continue;
            }
        }
        //soketti.Close();

    }


    
}
