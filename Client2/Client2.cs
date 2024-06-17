using System;
using System.IO;
using System.Net.Sockets;
using System.Text;




namespace Client2
{
    public class Client2
    {
        const int SizeOfBuffer = 256;
        const int SizeOfHeader = 128;

        static void Main(string[] args)
        {
            try
            {
                TcpClient client2 = new TcpClient("127.0.0.1", 6238); 
                NetworkStream stream2 = client2.GetStream();

                Console.WriteLine("Connected To The Server Correctlyy. Receiving file...");

                
                byte[] HeaderBuffer = new byte[SizeOfHeader];
                int ReadingHeaderBytes = stream2.Read(HeaderBuffer, 0, SizeOfHeader);
                if (ReadingHeaderBytes > 0)
                {
                    string header2 = Encoding.UTF8.GetString(HeaderBuffer).TrimEnd('\0');
                    string[] PartsOfHeader = header2.Split(',');
                    string nameOfFile = PartsOfHeader[0];
                    long SizeOfFile = long.Parse(PartsOfHeader[1]);

                    
                    string PathernDesc = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    string pathernfFile = Path.Combine(PathernDesc, $"receivedFile_{nameOfFile}");

                    
                    using (FileStream filestrm = new FileStream(pathernfFile, FileMode.Create, FileAccess.Write))
                    {
                        byte[] DataBuffer = new byte[SizeOfBuffer];
                        int ReadingBytes;
                        long ReadingTotalBytes = 0;

                        while (ReadingTotalBytes < SizeOfFile && (ReadingBytes = stream2.Read(DataBuffer, 0, SizeOfBuffer)) > 0)
                        {
                            filestrm.Write(DataBuffer, 0, ReadingBytes);
                            ReadingTotalBytes += ReadingBytes;
                            Console.WriteLine($"Packet Received Correctly And Written To File. Total bytes received: {ReadingTotalBytes}/{SizeOfFile}");
                        }
                    }

                    Console.WriteLine($"File Received Correctly And Saved To: {pathernfFile}");
                    Console.WriteLine("All Packets Received Successfully Nice!!");
                }
                else
                {
                    Console.WriteLine("Failed To Read Header From Server.");
                }

                stream2.Close();
                client2.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Client2 error: {ex.Message}");
            }
            Console.ReadLine();
        }
    }


}
