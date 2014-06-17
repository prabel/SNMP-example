using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace SNMP
{

    class Message
    {
        public Message()
        {               
        }

        private const int CONST_LENGTH = 29;
        private const int REDUCED_FIRST_BYTE_LENGTH = 1;

        private int mMibLength;

        public byte[] getMessageAsByteArray()
        {
            List<byte> list = new List<byte>();
            String[] mibValues = Program.OID.Split('.');
           
            byte[] community = Encoding.ASCII.GetBytes(Program.COMMUNITY);

            byte[] temp = new byte[32];
            mMibLength = mibValues.Length;
            temp = convertMessage(mibValues);
           
            int snmpLength = CONST_LENGTH + Program.COMMUNITY.Length + mMibLength - REDUCED_FIRST_BYTE_LENGTH;

            list.Add(0x30);                                //Sequence start
            list.Add(Convert.ToByte(snmpLength-2));       //Convert.ToByte(snmplen);   //ustaw dlugosc !

            //--------------SNMP VERSION ----------------
            list.Add(0x02);                            // integer type
            list.Add(0x01);                            // length
            list.Add(0x00);                            // version SNMPv1

            //--------------SNMP COMMUNITY STRING----------------

            list.Add(0x04); // typ string
            list.Add(Convert.ToByte(community.Length));     // length

            for (int i = 0; i < community.Length; i++)
            {
                list.Add(community[i]);
            }

            //--------------GET REQUEST -----------------------

            list.Add(0xA0);  // GET Request PDU
            list.Add(Convert.ToByte( 20 + mMibLength - REDUCED_FIRST_BYTE_LENGTH));  // length

            //--------------REQUEST ID -----------------------
            list.Add(0x02); //typ integer
            list.Add(0x04); //dlugosc tego


            byte[] rand_byte = new byte[4];
            Random rand = new Random();
            rand.NextBytes(rand_byte);

            for (int i = 0; i < rand_byte.Length; i++)
                list.Add(rand_byte[i]);

            //--------------ERROR STATUS ----------------
            list.Add(0x02);
            list.Add(0x01);
            list.Add(0x00);

            //--------------ERROR INDEX ----------------
            list.Add(0x02);
            list.Add(0x01);
            list.Add(0x00);

            //--------------VAR BINDINGS LIST ----------------

            list.Add(0x30); //sequence start
            list.Add(Convert.ToByte(6 + mMibLength - 1)); //sequence length

            //--------------VAR BINDINGS TYPE----------------

            list.Add(0x30); //sequence start
            list.Add(Convert.ToByte(4 + mMibLength - 1)); ; //sequence length


            list.Add(0x06); //identyfikator obiektu
            list.Add(Convert.ToByte(mMibLength - 1));
            list.Add(0x2B);
            //There are two more Basic
            //formula (40*x)+y. The first two numbers in an SNMP OID are always 1.3. Therefore,
            //the first two numbers of an SNMP OID are encoded as 43 or 0x2B, because (40*1)+3

            for (int i = 2; i < mMibLength; i++)
                list.Add(Convert.ToByte(temp[i]));


            list.Add(0x05); //Null object value
            list.Add(0x00); //Null

            byte[] snmp_packet = list.ToArray();
            Console.WriteLine(BitConverter.ToString(snmp_packet));
            Console.WriteLine(snmp_packet.Length);
            return snmp_packet;
        }

        public byte[] convertMessage(String[] mibValues)
        {
            byte[] temp = new byte[32];
            int value;
            int counter = 0;
            for (int i = 0; i < mibValues.Length; i++)
            {
                value = Convert.ToInt16(mibValues[i]);
                if(value > 127)
                {
                    temp[counter++] = Convert.ToByte(128 + (value / 128));
                    temp[counter++] = Convert.ToByte(value - ((value/128)*128));
                    mMibLength++;
                }
                else
                temp[counter++] = Convert.ToByte(mibValues[i]);
            }

            return temp;
        }
    }
}
