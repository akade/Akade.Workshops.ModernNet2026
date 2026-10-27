using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace AI.StarterApp.TicketTriage;

[JsonPolymorphic(InferClosedTypePolymorphism = true)]
closed record TicketAction;

/// <summary>
/// The incoming message can be fully answered based on the knowledge base. If not, the message should be classified as NewTicket
/// </summary>
/// <param name="RelevantFAQArticles">The set of FAQ articles that are relevant to the incoming message</param>
internal sealed record FAQ(ImmutableHashSet<FAQArticleId> RelevantFAQArticles) : TicketAction
{
    public override int GetHashCode()
    {
        HashCode hashCode = new();
        foreach (var articleId in RelevantFAQArticles)
        {
            hashCode.Add(articleId);
        }
        return hashCode.ToHashCode();
    }

    public bool Equals(FAQ? other)
    {
        return other is not null && RelevantFAQArticles.SetEquals(other.RelevantFAQArticles);
    }
}

/// <summary>
/// The incoming message is a duplicate of an existing ticket, i.e. solving or answering the existing ticket will
/// also fully answer the incoming message. If not, the message should be classified as NewTicket.
/// </summary>
/// <param name="TicketNr">The existing ticket number</param>
internal sealed record Duplicate(Guid TicketNr) : TicketAction;

/// <summary>
/// The incoming message is a new ticket that needs to be created for the given category, severity, and affected devices. 
/// </summary>
/// <param name="Category">The category of the new ticket</param>
/// <param name="Kind">The severity of the new ticket</param>
/// <param name="AffectedDevices">The set of affected devices for the new ticket</param>
internal sealed record NewTicket(Category Category, Kind Kind, ImmutableHashSet<AffectedDevices> AffectedDevices) : TicketAction
{
    public override int GetHashCode()
    {
        HashCode hashCode = new();
        hashCode.Add(Category);
        hashCode.Add(Kind);
        foreach (var device in AffectedDevices)
        {
            hashCode.Add(device);
        }
        return hashCode.ToHashCode();
    }

    public bool Equals(NewTicket? other)
    {
        return other is not null && Category == other.Category && Kind == other.Kind && AffectedDevices.SetEquals(other.AffectedDevices);
    }
}

internal record struct FAQArticleId(int Id);

internal sealed record FAQArticle(FAQArticleId Id, string Title, string Content);

internal enum Category
{
    Other,
    Connectivity,
    Mounting,
    Power,
    Accuracy,

}

internal enum Kind
{
    /// <summary>
    /// An inquire or a question
    /// </summary>
    Question,
    /// <summary>
    /// The enquiry is about an isolated fault
    /// </summary>
    MinorFault,
    /// <summary>
    /// The enquiry is about a real fault that still seems to be of limited scope.
    /// </summary>
    Fault,
    /// <summary>
    /// The ticket indicates widespread problems that need to be addressed as fast as possible
    /// </summary>
    Critical,
}

internal enum AffectedDevices
{
    TemperatureSensor,
    MovementSensor,
    HumiditySensor
}

internal record EvaluationRecord(
    IncomingMessage Message,
    TicketAction Action
);

internal record IncomingMessage(string Message)
{
    public static implicit operator IncomingMessage(string message) => new(message);

    public Guid Id { get; init; }
}

internal record EvaluationSet(
    ImmutableHashSet<EvaluationRecord> Records,
    ImmutableHashSet<FAQArticle> FAQArticles
);


internal static class TicketActionExtensions
{
    extension(TicketAction)
    {
        public static double Score(TicketAction a, TicketAction b)
        {
            if (a.GetType() != b.GetType())
            {
                return 0;
            }

            return (a, b) switch
            {
                (FAQ faqA, FAQ faqB) => Jaccard(faqA.RelevantFAQArticles, faqB.RelevantFAQArticles),
                (NewTicket newTicketA, NewTicket newTicketB) =>
                      0.4 * Score(newTicketA.Category, newTicketB.Category)
                    + 0.4 * Score(newTicketA.Kind, newTicketB.Kind)
                    + 0.2 * Jaccard(newTicketA.AffectedDevices, newTicketB.AffectedDevices),
                (_, _) => Score(a, b)
            };
        }
    }

    private static double Score<T>(T a, T b) => a.Equals(b) ? 1.0 : 0.0;

    private static double Jaccard<T>(ImmutableHashSet<T> a, ImmutableHashSet<T> b)
    {
        if (a.Count == 0 && b.Count == 0)
        {
            return 1;
        }

        return (double)a.Intersect(b).Count / a.Union(b).Count;
    }
}