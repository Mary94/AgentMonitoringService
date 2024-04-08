using AgentMonitoringService;
using AgentMonitoringService.Utils;

ConsoleManager consoleManager = new ConsoleManager();
consoleManager.WriteLine("Enter JSON Lines data (press Enter twice to finish):");
EventProcessor eventProcessor = new EventProcessor(consoleManager);
var events = eventProcessor.GetEventInputs();
eventProcessor.ProcessEvents(events);
