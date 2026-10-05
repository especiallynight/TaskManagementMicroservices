using System.Diagnostics;

namespace Task_Management;

public static class Tracer
{
    public static TraceSource TaskManagerTrace =
        new TraceSource("TaskManagerTrace", SourceLevels.Verbose);
}