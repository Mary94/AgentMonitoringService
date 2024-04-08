namespace AgentMonitoringService.Utils
{
    public interface IConsoleManager
    {
        string ReadLine();
        void WriteLine(string value);
        bool IsInputEmpty(string line);
        bool IsResponseYes(string response);
    }
}
