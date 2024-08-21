using CommunityToolkit.Maui;
using Sentry.Extensibility;

namespace MauiApp1;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp() =>
        MauiApp.CreateBuilder()
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseSentry(options =>
            {
                options.Dsn = "[YOUR_SENTRY_DSN_HERE]";
                options.Debug = true;
                options.TracesSampleRate = 1.0;
                options.AttachScreenshot = true;
                options.AddEventProcessors([new MySentryEventProcessorWithHint(), new MySentryEventProcessor()]); // Doesn't catch native/Java crash events
                options.SetBeforeSend(EnrichEvent); // Doesn't catch native/Java crash events
                options.ConfigureScope(scope => scope.AddEventProcessors([new MySentryEventProcessorWithHint(), new MySentryEventProcessor()])); // Doesn't catch native/Java crash events
            })
            .Build();

    class MySentryEventProcessorWithHint : ISentryEventProcessorWithHint
    {
        public SentryEvent? Process(SentryEvent @event, SentryHint hint) => Process(@event);
        public SentryEvent? Process(SentryEvent @event) => EnrichEvent(@event);
    }

    class MySentryEventProcessor : ISentryEventProcessor
    {
        public SentryEvent? Process(SentryEvent @event) => EnrichEvent(@event);
    }

    public static SentryEvent EnrichEvent(SentryEvent sentryEvent)
    {
        sentryEvent.Release = "0.0.1";
        sentryEvent.Environment = "DEV";

        for (var i = 1; i <= 5; i++)
        {
            sentryEvent.SetTag($"Tag_{i}", i.ToString());
            sentryEvent.Contexts[$"Context_{i}"] = Enumerable.Range(1, 5).ToDictionary(i => $"Key_{i}", i => i.ToString());
        }

        return sentryEvent;
    }
}