using Serilog.Events;
using System;

namespace ECommons.Logging;

public record struct InternalLogMessage
{
    public string Message;
    public LogEventLevel Level;
    public DateTimeOffset Time;

    public InternalLogMessage(string Message, LogEventLevel Level = LogEventLevel.Information)
    {
        this.Message = Message;
        this.Level = Level;
        this.Time = DateTimeOffset.Now;
    }
}
