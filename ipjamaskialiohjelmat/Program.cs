using System;

namespace Maski
{
    class Program
    {
        static void Main(string[] args)
        {
            xorOfSubnets("192.168.5.0", "192.168.7.0");
            
            string[] tiedot = forwardoi("192.168.0.0/22   192.168.100.2\r\n192.163.0.0/21   192.168.100.4\r\n192.163.5.0/24   192.168.100.6\r\n192.168.6.0/23   192.168.100.8\r\n192.168.8.0/24   192.168.100.10\r\n192.168.7.0/25   192.168.100.12\r\n192.168.7.128/25   192.168.100.14\r\n192.168.9.128/25   192.168.100.16\r\n0.0.0.0/0        192.168.100.18", "192.168.5.191");
            Console.WriteLine(tiedot[0] + " " + tiedot[1] + " " + tiedot[2]);
            /* "192.168.0.0/22   192.168.100.2\r\n192.168.0.0/21   192.168.100.4\r\n192.168.5.0/24   192.168.100.6\r\n192.168.6.0/23   192.168.100.8\r\n192.168.8.0/24   192.168.100.10\r\n192.168.7.0/25   192.168.100.12\r\n192.168.7.128/25   192.168.100.14\r\n192.168.9.128/25   192.168.100.16\r\n0.0.0.0/0        192.168.100.18"   */


            Console.ReadLine();

        }

        /// <summary>
        /// Funktio palauttaa aliverkkomaskia /x vastaaavan
        /// binääriluvun string-muodossa
        /// <param name="maski">string, joka on luku väliltä 2-30</param>
        /// <returns>binääriluku string-muodossa</returns>
        public static string binmask(string maski)
        {
            string binmaski = "";
            string[] luvut = new string[32];
            int maara = int.Parse(maski);
            for (int i = 0; i < 32; i++)
            {
                if (i < maara )
                {
                    luvut[i] = "1";
                }
                else
                {
                    luvut[i] = "0";
                }
                
            }
            binmaski = string.Join("", luvut);
            return binmaski;
        }

        /// <summary>
        /// Funktio palauttaa pistedesimaalimuodossa esitettyä aliverkkomaskia
        /// vastaavan binääriluvun string-muodossa.
        /// <param name="maski">string, joka on esimerkiksi 255.255.0.0</param>
        /// <returns>binääriluku string-muodossa</returns>
        public static string binmask2(string maski)
        {

            
            string[] numerot = maski.Split('.');
            int[] luvut = new int[numerot.Length];
            for (int i = 0; i < numerot.Length; i++)
            {
                luvut[i] = int.Parse(numerot[i]);
            }
            for (int i = 0; i < numerot.Length; i++)
            {
                numerot[i] = Convert.ToString(luvut[i], 2).PadLeft(8,'0');
            }
            return string.Join("", numerot);
            
        }

        public static string andFromIPandMask(string ipJaMaski)
        {
            char[] andIP = new char[32];
            
            string[] erotettu = ipJaMaski.Split('/');
            char[] binip = binmask2(erotettu[0]).ToCharArray();
            char[] binmaski = binmask(erotettu[1]).ToCharArray();
            for (int i = 0; i < andIP.Length; i++)
            {
                if (binip[i].Equals(binmaski[i]) && binip[i].Equals('1'))
                {
                    andIP[i] = '1';
                }
                else
                {
                    andIP[i] = '0';
                }
            }
            string[] jaettu = new string[4];
            string and = string.Join("", andIP);
            jaettu[0] = and.Substring(0, 8);
            jaettu[1] = and.Substring(8, 8);
            jaettu[2] = and.Substring(16, 8);
            jaettu[3] = and.Substring(24, 8);
            and = Convert.ToByte(jaettu[0],2).ToString() + '.' + Convert.ToByte(jaettu[1],2).ToString() + '.' + Convert.ToByte(jaettu[2],2).ToString() + '.' + Convert.ToByte(jaettu[3],2).ToString();
            return and;
        }


        /// <summary>
        /// Funktio kahden aliverkon XOR operaation tuloksen binäärilukuna string-muodossa
        /// <param name="subnet">aliverkko, muodossa x.x.x.x</param>
        /// <param name="andresult">aliverkko, tulos AND-operaatiosta, muodossa x.x.x.x</param>
        /// <returns>binääriluku string-muodossa</returns>
        public static string xorOfSubnets(string subnet, string andresult)
        {
            
            char[] binsubnet = binmask2(subnet).ToCharArray();
            char[] binand = binmask2(andresult).ToCharArray();
            char[] xor = new char[32];
            for (int i = 0; i < binsubnet.Length; i++)
            {
                if (binsubnet[i].Equals('1') && binand[i].Equals('1'))
                {
                    xor[i] = '0';
                }
                else if(binand[i].Equals('1') || binsubnet[i].Equals('1'))
                {
                    xor[i] = '1';
                }
                else
                {
                    xor[i] = '0';
                }
            }

            return string.Join("",xor);
        }

        /// <summary>
        /// Funktio palauttaa IP-osoitteen ja sen mihin verkkoon (kohdeverkko/maski)
        /// paketti tulee välittää sekä seuraavan laitteen osoitteen,
        /// jota kautta paketti tulee välittää kohdeverkkoon.
        /// </summary>
        /// <param name="reititystaulu">reititystaulu, string, muodossa esim.
        /// 135.46.56.0/22   192.168.0.2
        /// 135.46.56.0/21   192.168.0.6
        /// 192.53.40.0/23   192.168.0.10
        /// 0.0.0.0/0        192.168.0.14
        /// eli rivit päättyvät rivinvaihtoon ja rivillä erottimena voi olla
        /// yksi tai useampi välilyönti</param>
        /// <param name="iposoite">IP-osoite, string, jonka kohde määritetään</param>
        /// <returns>parametreista muodostetut informaatiot string:inä
        /// iposoite, string, sama mikä tuli parametrina
        /// kohdeverkko, string, kohdeverkko ja mask, muodossa x.x.x.x/y
        /// nexthop, string, reititystaulun nex_hop IP osoite
        /// </returns>
        public static string[] forwardoi(string reititystaulu,
                                        string iposoite)
        {
            
            int parasmaski = 0;
            string[] tiedot = new string[3];
            string kohdeverkko = ""; // kohdeverkko/maski
            string nexthop = ""; // seuraavan laitteen osoitte
            string[] pilkottu = reititystaulu.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            for (int i = 0; i < pilkottu.Length; i++)
            {
                string[] temp = pilkottu[i].Split(' ');
                string[] temp2 = temp[0].Split('/');
                string and = andFromIPandMask(iposoite + "/" + temp2[1]);
                string xor = xorOfSubnets(temp2[0], and);
                if (Convert.ToInt32(xor,2) == 0)
                {

                    int maski = Convert.ToInt32(temp2[1]);
                    if (maski >= parasmaski) {
                        parasmaski = maski;
                        kohdeverkko = temp[0];
                        nexthop = temp[temp.Length - 1];
                        tiedot[0] = iposoite;
                        tiedot[1] = kohdeverkko;
                        tiedot[2] = nexthop;
                    }
                    
                }
            }

            /*if (parasmaski == 0)
            {
                string[] temp3 = pilkottu[pilkottu.Length - 1].Split(' ');
                tiedot[0] = iposoite;
                tiedot[1] = temp3[0];
                tiedot[2] = temp3[1];

                return tiedot;
            }*/
            
            
            return tiedot; //iposoite,kohdevekko = temp3[0],nexthop = temp3[temp.Length-1];
        }

    }
}
