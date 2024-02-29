using System.Diagnostics;
using System.Net.Sockets;

namespace Utils
{
    public static class HostInfo
    {
        public static Dictionary<string, string> GetArpResult()
        {
            Dictionary<string, string> arpTable = new Dictionary<string, string>();

            ProcessStartInfo psi = new ProcessStartInfo("arp", "-a")
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            Process process = new Process
            {
                StartInfo = psi
            };

            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            string[] lines = output.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string line in lines)
            {
                string[] parts = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 3 && parts[1] != "dynamic") // Формат вывода ARP-кэша
                {
                    arpTable[parts[0]] = parts[1]; // IP-адрес и соответствующий MAC-адрес
                }
            }

            return arpTable;
        }


        public static bool PingHost(string hostUri, int portNumber)
        {
            try
            {
                using var client = new TcpClient(hostUri, portNumber);
                return true;
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"Error pinging host: {hostUri}:{portNumber}. {ex.Message}");
                return false;
            }
        }
    }
}