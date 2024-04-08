# AgentMonitoringService

The goal of this assignment is to create a command line program that will function as the core of
a summarization service for a stream of events. This summarization program tracks the state of
an imaginary fleet of agents spread across the globe, with the goal of identifying suspicious
patterns. Each event is a status update with various metadata fields. As data is fed into the
program, the program should emit summary events according to the logic below.
# Input:
A stream of events sent over stdin json lines format (https://jsonlines.org/). Each event is
formatted like the example below:
```
{
"agent_id": "abcdef",
"image_id": "release-1",
"timestamp": "2024-01-01T12:12:31",
"status": "SUSPICIOUS"
}
```
Events are sent in time order - an event received in stdin will always have a timestamp equal to
or greater than the event before it.
Output:
A stream of summary events sent to stdout, also in json lines format. Summary events specified
below
# What to summarize:
If more than one event for any value of agent_id with status SUSPICIOUS is observed within
the same calendar day, the program must emit an event. Whenever an event is observed for an
Unset
Unset
agent above this frequency threshold, the program must emit an event formatted like the
example below:
```
{
"event_type": "suspicious_threshold_met",
"agent_id": "abcdef",
"timestamp": "2024-01-01T12:12:31"
}
```
Whenever the first event for a new calendar day is observed, the program must emit an event
summarizing the previous calendar day. It must include the calendar day being summarized, list
each unique value of image_id seen during that day, and a calculated status for each one.
The status for an image is one of:
- SUSPICIOUS, if more than 50% of the events seen with this image_id in the last 30
days had the value SUSPICIOUS for the status field.
- NORMAL, otherwise
For output summary event should be formatted like the example below:
```
{
"event_type”: "daily_summary",
"day”: "2024-01-01",
"images": {
            "release-1": "NORMAL",
            "release-2": "SUSPICIOUS",
            "release-3": "NORMAL"
          }
}
```
