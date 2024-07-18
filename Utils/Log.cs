namespace Utils
{
    public static class Log
    {
        public static int FileNumber { get; set; }
        public static object LockObject { get; set; } = new();
        public static string DirectoryLog { get; set; } = "logs";
        
        public static void Write(string message, bool console = true)
        {
            try
            {
                lock (LockObject)
                {
                    if (!Directory.Exists(DirectoryLog))
                    { 
                        Directory.CreateDirectory(DirectoryLog); 
                    }
                    File.WriteAllText(Path.Combine(DirectoryLog, $"{DateTime.Now:MM-dd-hh-mm}-{FileNumber++}.txt"), string.Join("\n", [$"---{DateTime.Now}", message, "\n"]));
                }

                if (console)
                {
                    Console.WriteLine(message);
                }
            }
            catch(Exception ex) 
            {
                // ignored
                Console.WriteLine(ex.Message);
            }
        }
    }
}
