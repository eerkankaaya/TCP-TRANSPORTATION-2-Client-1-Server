using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;


namespace Client1
{
    public class Client1
    {
        const int SizeOfBuffer = 256;
        const int SizeOfHeader = 128;
        const int MinimumFileSize = 100 * 1024;

        static void Main(string[] args)
        {
            try
            {
                string PathOfFile = "C:\\Users\\Desktop\\Test2.txt";
                byte[] BytesOfFile = File.ReadAllBytes(PathOfFile);
                long SizeOfFile = BytesOfFile.Length;

                if (SizeOfFile < MinimumFileSize)
                {
                    Console.WriteLine("Error: The Size Of File Must Be Greater Than 100 KB.");
                    Console.ReadLine(); 
                    return;
                }

                TcpClient client1 = new TcpClient("127.0.0.1", 6237);
                NetworkStream stream1 = client1.GetStream();

                Console.WriteLine("Connected To The Server Correctlyy. File is sending.......");

                string NameOfFile = Path.GetFileName(PathOfFile);
                int PacketsTotally = (int)Math.Ceiling((double)BytesOfFile.Length / SizeOfBuffer);

                string header1 = $"{NameOfFile},{SizeOfFile}";
                byte[] BytesOfHeader = Encoding.UTF8.GetBytes(header1);
                byte[] BufferHeader = new byte[SizeOfHeader];
                Array.Copy(BytesOfHeader, BufferHeader, BytesOfHeader.Length);
                stream1.Write(BufferHeader, 0, SizeOfHeader);

                for (int j = 0; j < PacketsTotally; j++)
                {
                    int SizeOfData = Math.Min(SizeOfBuffer, BytesOfFile.Length - j * SizeOfBuffer);
                    byte[] packet1 = new byte[SizeOfBuffer];
                    Array.Copy(BytesOfFile, j * SizeOfBuffer, packet1, 0, SizeOfData);

                    stream1.Write(packet1, 0, SizeOfBuffer);
                    Console.WriteLine($"Packet {j + 1}/{PacketsTotally} sent.");

                    Thread.Sleep(10); 
                }

                Console.WriteLine("File Sent Correctly. Nice!!!!.");
                stream1.Close();
                client1.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Client1 error: {ex.Message}");
            }
            Console.ReadLine(); 
        }
    }
}
