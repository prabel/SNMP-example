using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

// tylko male liczby

namespace SNMP
{
    class Program
    {
        // Host details
        public const String IP_ADDRESS = "31.7.184.131";
        public const int PORT_NUMBER = 161;

        // Message details
        public const String OID = "1.3.6.1.2.1.1.1.0";
        public const String COMMUNITY = "public";

        static void Main(string[] args)
        {
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(Program.IP_ADDRESS), Program.PORT_NUMBER);
            UdpClient udpClient = new UdpClient(Program.PORT_NUMBER);

            udpClient.Connect(ipEndPoint);

            // Convert message to byte array
            byte[] request_packet = new Message().getMessageAsByteArray();
            udpClient.Send(request_packet, request_packet.Length);

            byte[] response_packet = udpClient.Receive(ref ipEndPoint);
            Console.WriteLine("Received data: " +
                Encoding.ASCII.GetString(response_packet)
                + " from address " + ipEndPoint.Address.ToString());

            udpClient.Close();
        
           
            Console.ReadLine();
        }


    }
    
}
