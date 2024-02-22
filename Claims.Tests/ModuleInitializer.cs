using System.Runtime.CompilerServices;

namespace Claims.Tests;

public class ModuleInitializer
{
    [ModuleInitializer]
    public static void Initialize()
    {
        VerifyHttp.Initialize();

        VerifierSettings.IgnoreMembers(
            "Content-Length",
            "traceparent",
            "Traceparent",
            "X-Amzn-Trace-Id",
            "traceId",
            "origin");
        VerifierSettings
            .ScrubInlineGuids();
        VerifierSettings
            .ScrubLinesContaining("Traceparent", "X-Amzn-Trace-Id", "Content-Length");
    }
}