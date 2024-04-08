using AgentMonitoringService.Utils;
using System.Text.Json;

namespace AgentMonitoringService
{
    public class EventProcessor
    {
        private readonly IConsoleManager _consoleManager;
        public EventProcessor(IConsoleManager consoleManager)
        {
            _consoleManager = consoleManager;
        }

        public List<EventInput> GetEventInputs()
        {
            try
            {
                List<EventInput> eventInputs = new List<EventInput>();
                string line;
                // Read lines until an empty line is encountered
                while (!_consoleManager.IsInputEmpty(line = _consoleManager.ReadLine()))
                {
                    EventInput obj = JsonSerializer.Deserialize<EventInput>(line)!;
                    eventInputs.Add(obj);
                }

                return eventInputs;
            }
            catch (Exception ex)
            {
                _consoleManager.WriteLine($"An error occurred: {ex.Message}");
                _consoleManager.WriteLine("Would you like to try again? (Y/N)");
                string response = _consoleManager.ReadLine();

                if (response.Equals("Y", StringComparison.OrdinalIgnoreCase))
                {
                    // Retry the operation
                    return GetEventInputs();
                }
                else
                {
                    // User chose not to retry, rethrow the exception
                    throw ex;
                }
            }
        }

        public void ProcessEvents(List<EventInput> events)
        {
            // Dictionary to keep track of the count of SUSPICIOUS events for each agent on each day
            Dictionary<string, Dictionary<string, int>> suspiciousCountsByAgentAndDay = new Dictionary<string, Dictionary<string, int>>();

            DateTime? previousDay = null;

            foreach (EventInput ev in events)
            {
                DateTime currentDay = ev.Timestamp.Date;

                // Update suspicious count for agent and day
                if (ev.Status == "SUSPICIOUS")
                {
                    if (!suspiciousCountsByAgentAndDay.ContainsKey(ev.AgentId))
                        suspiciousCountsByAgentAndDay[ev.AgentId] = new Dictionary<string, int>();

                    string currentDayString = currentDay.ToString("yyyy-MM-dd");

                    if (!suspiciousCountsByAgentAndDay[ev.AgentId].ContainsKey(currentDayString))
                        suspiciousCountsByAgentAndDay[ev.AgentId][currentDayString] = 0;

                    suspiciousCountsByAgentAndDay[ev.AgentId][currentDayString]++;

                    // Emit event for agent with status SUSPICIOUS above frequency threshold
                    if (suspiciousCountsByAgentAndDay.ContainsKey(ev.AgentId) && suspiciousCountsByAgentAndDay[ev.AgentId].Values.Sum() > 1)
                    {
                        // timestamp format is different in the test output and description ??
                        _consoleManager.WriteLine($"{{\"event_type\": \"suspicious_threshold_met\", \"agent_id\": \"{ev.AgentId}\", \"timestamp\": \"{ev.Timestamp.ToString("yyyy-MM-ddTHH:mm:ss")}\"}}");
                    }
                }
                if (previousDay != currentDay)
                {
                    if (previousDay.HasValue)
                        SummarizePreviousDay(previousDay.Value, events);
                    previousDay = currentDay;
                }
            }
        }

        void SummarizePreviousDay(DateTime previousDay, List<EventInput> events)
        {
            var currentDayEvents = events.Where( e=> e.Timestamp.Date == previousDay.Date);
            DateTime thirtyDaysAgo = previousDay.Date - TimeSpan.FromDays(30);
            Dictionary<string, string> imageStatusByDay = new Dictionary<string, string>();

            foreach (var ev in currentDayEvents)
            {
                string status = "NORMAL";
                var eventsLast30Days = events.Where(e => e.Timestamp.Date >= thirtyDaysAgo && e.ImageId == ev.ImageId && e.Timestamp.Date <= previousDay.Date).ToList();
                if (eventsLast30Days.Any() && (eventsLast30Days.Count(e => e.Status == "SUSPICIOUS") / (double)eventsLast30Days.Count()) > 0.5)
                {
                    status = "SUSPICIOUS";
                }
                if (!imageStatusByDay.ContainsKey(ev.ImageId))
                    imageStatusByDay.Add(ev.ImageId, status);
                else
                    imageStatusByDay[ev.ImageId] = status;
            }

            EmitDailySummary(previousDay, imageStatusByDay);
        }

        void EmitDailySummary(DateTime day, Dictionary<string, string> imageStatusByDay)
        {
            string dayString = day.ToString("yyyy-MM-dd");

            _consoleManager.WriteLine($"{{\"event_type\": \"daily_summary\", \"day\": \"{dayString}\", \"images\": {ToJson(imageStatusByDay)}}}");
        }

        string ToJson(Dictionary<string, string> dict)
        {
            return "{" + string.Join(", ", dict.Select(kv => $"\"{kv.Key}\": \"{kv.Value}\"")) + "}";
        }
    }
}
