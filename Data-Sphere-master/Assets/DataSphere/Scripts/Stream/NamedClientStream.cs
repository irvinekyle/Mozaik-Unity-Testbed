//Source: https://msdn.microsoft.com/en-us/library/system.net.sockets.tcplistener(v=vs.80).aspx?f=255&MSPPError=-2147217396

using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

using System.Net;
using System.IO;
using System.Threading;

public class NamedClientStream : MonoBehaviour {

    
        public string server;
        public int port;
        public string message;

        //private byte[] stringEnd = new byte[] { 0x0D, 0x0A };
        private byte[] mybyte = new byte[] { 13, 0, 1 };

        // Use this for initialization
        void Start () {
           SendMessageToServer("<Asset><Sources>18,25,26,27</Sources><@R10><@R10.Display.5>1,0,0,1</@R10.Display.5><@R10.Amp.6>11,0,0,6</@R10.Amp.6><@R10.Switcher.7>13,0,0,16</@R10.Switcher.7></@R10><@R7><@R7.Amp.2>4,0,0,0</@R7.Amp.2><@R7.Amp.9>0,0,0,0</@R7.Amp.9><@R7.Amp.10>0,0,0,0</@R7");
        }

        // Update is called once per frame
        void Update () {
            if (Input.GetMouseButton(0)) {
                int myNumber = 13;
                String myHexNumber = myNumber.ToString("X");
                SendMessageToServer("@R10_src_18\x0D\x0A");
            }
            else if (Input.GetMouseButton(1)) {
                SendMessageToServer("@R10_src_27\x0D\x0A");
            }
        }

        public void SendMessageToServer(string message)
        {
            Connect(server, port, message);
        }

        void Connect(String server, int port, String message)
        {
            try
            {
                // Create a TcpClient.
                // Note, for this client to work you need to have a TcpServer 
                // connected to the same address as specified by the server, port
                // combination.
                TcpClient client = new TcpClient(server, port);


                // Translate the passed message into ASCII and store it as a Byte array.
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);

                // Get a client stream for reading and writing.
                //Stream stream = client.GetStream();

                NetworkStream stream = client.GetStream();

                // Send the message to the connected TcpServer. 
                stream.Write(data, 0, data.Length);

               Debug.Log("Sent: {0}" + message);

                // Receive the TcpServer.response.

                // Buffer to store the response bytes.
                data = new Byte[256];

                // String to store the response ASCII representation.
                String responseData = String.Empty;

                // Read the first batch of the TcpServer response bytes.
                Int32 bytes = stream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                Debug.Log("Received: {0}" + responseData);

                // Close everything.
                stream.Close();
                client.Close();
            }
            catch (ArgumentNullException e)
            {
                Debug.Log("ArgumentNullException: {0}" + e);
            }
            catch (SocketException e)
            {
                Debug.Log("SocketException: {0}" + e);
            }

            Console.WriteLine("\n Press Enter to continue...");
            Console.Read();
        }
}