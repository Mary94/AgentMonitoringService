namespace AgentMonitoringService.Utils
{
    public class ConsoleManager : IConsoleManager
    {
        public string ReadLine()
        {
            return Console.ReadLine();
        }

        public void WriteLine(string value)
        {
            Console.WriteLine(value);
        }

        public bool IsInputEmpty(string line)
        {
            return string.IsNullOrWhiteSpace(line);
        }

        public bool IsResponseYes(string response)
        {
            return response.Equals("Y", StringComparison.OrdinalIgnoreCase);
        }
    }
}
