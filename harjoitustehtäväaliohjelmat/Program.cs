using System;
using System.Text;

namespace ConsoleApp2
{
    class Program
    {
        public static void Main()
        {
            Console.WriteLine("Arvoilla 2,0,0,0 koodisi antaa tavun " + Convert.ToString(EkaTavu(2, 0, 0, 0), 2).PadLeft(8, '0'));

            byte[] tavut = SsrcTavuiksi(1975262257);
            System.Console.WriteLine("Arvolla 1975262257 koodisi antaa tavut ");
            string s = "";
            for (int i = 0; i < 4; i++)
            {
                s += Convert.ToString(tavut[i], 2).PadLeft(8, '0') + " ";
            }
            System.Console.WriteLine(s);

            byte[] ll = { 192, 208 };
            pvmFromByte(ll);
            Console.ReadLine();
        }


        /// <summary>
        /// Funktio palauttaa RTP paketin ensimmäisen tavun.
        /// </summary>
        /// <param name="Version">RTP versio</param>
        /// <param name="Padding">Onko täytebittejä</param>
        /// <param name="Extension">Onko laajennettu otsikko</param>
        /// <param name="CC">CSRC tunnusten lukumäärä</param>
        /// <returns>parametreista muodostetun tavun</returns>
        public static byte EkaTavu(int Version, int Padding, int Extension, int CC)
        {
            byte ekatavu = 0;
           /* string version = Convert.ToString(Version, 2).PadLeft(2, '0');
            string padding = Convert.ToString(Padding,2);
            string extension = Convert.ToString(Extension,2);
            string cc = Convert.ToString(CC,2).PadLeft(4, '0');
            string koko = version + padding + extension + cc;

            for (int i = 7,j = 0; i >= 0; i--,j++)
            {
                if (int.Parse(koko[j].ToString()) == 1)
                {
                    ekatavu = (byte)(ekatavu | 1 << i);
                }
                else ekatavu = (byte)(ekatavu | 0 << i);

            }*/
            
             ekatavu = (byte) (ekatavu ^ (Version << 6));
             ekatavu = (byte)(ekatavu ^ (Padding << 5));
             ekatavu = (byte)(ekatavu ^ (Extension << 4));
             ekatavu = (byte) (ekatavu ^ (CC << 0));
            return ekatavu;
        }

        /// <summary>
        /// Funktio palauttaa 32- bittisen int-arvon jaettuna neljään tavuun.
        /// </summary>
        /// <param name="ssrc">32-bittinen int</param>
        /// <returns>32-bittinen int jaettuna tavuihin</returns>
        public static byte[] SsrcTavuiksi(int ssrc)
        {
            byte[] tavut = new byte[4];
            string koko = Convert.ToString(ssrc, 2).PadLeft(32, '0');

            tavut[0] = tavuiksi(koko.Substring(0,8));
            tavut[1] = tavuiksi(koko.Substring(8, 8));
            tavut[2] = tavuiksi(koko.Substring(16, 8));
            tavut[3] = tavuiksi(koko.Substring(24, 8));






            return tavut;
        }

        public static byte tavuiksi(string tavu)
        {
            byte ekatavu = 0;


            for (int i = 7, j = 0; i >= 0; i--, j++)
            {
                if (int.Parse(tavu[j].ToString()) == 1)
                {
                    ekatavu = (byte)(ekatavu | 1 << i);
                }
                else ekatavu = (byte)(ekatavu | 0 << i);

            }


            return ekatavu;
        }

        /// <summary>
        /// Funktio palauttaa kaksi tavua, joihin asetettu kentät:
        /// Day - 5 bittiä
        /// Month - 4 bittiä
        /// Year - 7 bittiä
        /// </summary>
        /// <param name="Day">Päivä kuukaudessa 1-31</param>
        /// <param name="Month">Kuukausi vuodessa 1-12</param>
        /// <param name="Year">Vuosi, kahdella merkillä 00-99</param>
        /// <returns>parametreista muodostetut tavut</returns>
        public static byte[] pvm(int Day, int Month, int Year)
        {
            byte[] tavut = new byte[2];
            //string koko = Convert.ToString(Day, 2) + Convert.ToString(Month, 2) + Convert.ToString(Year, 2);
            //tavut[0] = tavuiksi(koko.Substring(0, 8));
            //tavut[1] = tavuiksi(koko.Substring(8, 8));

            short bitit = 0;
            bitit = (short)(bitit ^ (Day << 11));
            bitit = (short)(bitit ^ (Month << 7));
            bitit = (short)(bitit ^ (Year << 0));

            tavut[0] = (byte)(bitit >> 8);
            tavut[1] = (byte)(bitit & 0xff);
            return tavut;
        }
        /// <summary>
        /// aliohjelma joka palauttaa päivämäärän tavuista
        /// </summary>
        /// <param name="tavut"></param>
        /// <returns></returns>
        public static int pvmFromByte(byte[] tavut)
        {
            byte Day = 0;
            byte Month = 0;
            byte Year = 0;
            
            byte temp = tavut[0];

            Day = (byte)(Day ^ (temp >> 3));
            temp = tavut[0];
            temp = (byte)(temp & 0x7);
            temp = (byte)(temp << 1);
            Month = temp;
            temp = 0;
            temp = (byte) (tavut[1] >> 7);
            Month = (byte)(Month ^ temp);
            Year = (byte)(tavut[1] & 0x7F);
            int Day2 = Convert.ToInt32(Day);
            int Month2 = Convert.ToInt32(Month);
            int Year2 = Convert.ToInt32(Year);
            return Day;
        }

        /// <summary>
        /// Funktio palauttaa informaatiota TIEA322 Multicastchat -sovelluksen otsikkokentistä.
        /// Tämä funktio palauttaa asiakassovellus -kentän tekstinä
        /// </summary>
        /// <param name="tavut">byte taulukko, vaihtuva koko</param>
        /// <returns>kentän arvo string-tyyppeinä</returns>
        public static string dataOtsikkosta(byte[] tavut)
        {

            // ennen tekstiä on heksaluku joka kertoo monta kirjainta on otettava. heksalukujen määrä on tavu 5
            int maara = tavut[5];
            byte[] nimi = new byte[maara];
            for (int i = 0; i < nimi.Length; i++)
            {
                nimi[i] = tavut[i + 6];
            }
            
            string asiakassovellus = Encoding.UTF8.GetString(nimi, 0, Convert.ToInt32(tavut[5]));
            



            return asiakassovellus;
        }
    }
}
