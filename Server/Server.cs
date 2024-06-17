using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Concurrent;

namespace Server
{
    public class Server
    {
        const int SizeOfBuffer = 256;
        const int BufferMaximum = 12;
        static BlockingCollection<byte[]> buffer1 = new BlockingCollection<byte[]>(BufferMaximum);
        static byte[] BufferHeader = new byte[128];

        static void Main(string[] args)
        {
            TcpListener listenerc1 = new TcpListener(IPAddress.Any, 6237); 
            TcpListener listenerc2 = new TcpListener(IPAddress.Any, 6238);
            listenerc1.Start();
            listenerc2.Start();

            Console.WriteLine("Server Started Correctlyy On Ports 6237 And 6238...");
            Console.WriteLine("Waiting For Connections...");


            Thread client1ListenerThread = new Thread(() =>
            {
                while (true)
                {
                    TcpClient client1 = listenerc1.AcceptTcpClient();
                    Console.WriteLine("Client 1 Connected To The Server Correctlyy On Port 6237");
                    Thread clientThread1 = new Thread(() => Client1Handling(client1));
                    clientThread1.Start();
                }
            });
            client1ListenerThread.Start();

            Thread client2ListenerThread = new Thread(() =>
            {
                while (true)
                {
                    TcpClient client2 = listenerc2.AcceptTcpClient();
                    Console.WriteLine("Client 2 Connected To The Server Correctly On Port 6238");
                    Thread clientThread2 = new Thread(() => Client2Handling(client2));
                    clientThread2.Start();
                }
            });
            client2ListenerThread.Start();
        }

        static void Client1Handling(TcpClient cl1)
        {
            try
            {
                NetworkStream stream3 = cl1.GetStream();
                byte[] DataOfBuffer = new byte[SizeOfBuffer];

           
                int ReadingHeaderBytes = stream3.Read(BufferHeader, 0, BufferHeader.Length);
                if (ReadingHeaderBytes > 0)
                {
                    Console.WriteLine("Header Received Correctlyy from Client 1.");
                }

                while (true)
                {
                    int ReadingBytes = stream3.Read(DataOfBuffer, 0, SizeOfBuffer);
                    if (ReadingBytes == 0)
                        break;

                    byte[] packet = new byte[ReadingBytes];
                    Array.Copy(DataOfBuffer, packet, ReadingBytes);

                    buffer1.Add(packet);
                    Console.WriteLine($"Packet Received From Client 1 And Added To Buffer. Buffer size: {buffer1.Count}");

                    while (buffer1.Count >= BufferMaximum)
                    {
                        Thread.Sleep(10); 
                    }
                }

                stream3.Close();
                cl1.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Client1 connection failed: {ex.Message}");
            }
        }

        static void Client2Handling(TcpClient cl2)
        {
            try
            {
                NetworkStream stream = cl2.GetStream();

                
                stream.Write(BufferHeader, 0, BufferHeader.Length);
                Console.WriteLine("Header Sent To Client 2 Correctlyy.");

                while (true)
                {
                    byte[] packet;
                    if (buffer1.TryTake(out packet, Timeout.Infinite))
                    {
                        stream.Write(packet, 0, packet.Length);
                        Console.WriteLine("Packet Sent To Client 2 From Buffer Correctlyy.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Client2 connection failed: {ex.Message}");
            }
            finally
            {
                cl2.Close();
            }
        }
    }
}
