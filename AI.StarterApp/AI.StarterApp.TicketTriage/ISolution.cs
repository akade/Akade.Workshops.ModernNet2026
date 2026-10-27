namespace AI.StarterApp.TicketTriage;

/// <summary>
/// Static part of a solution. Kept separate because interfaces with static abstract members
/// cannot be used as type arguments (e.g. for <c>AddSingleton&lt;ISolution, T&gt;</c>).
/// </summary>
internal interface ISolutionRegistration
{
    /// <summary>
    /// Registers everything the solution needs (e.g. Qdrant collections) into DI.
    /// </summary>
    static abstract void RegisterServices(IServiceCollection services);
}

internal static class SolutionServiceCollectionExtensions
{
    public static IServiceCollection AddSolution<T>(this IServiceCollection services)
        where T : class, ISolution, ISolutionRegistration
    {
        T.RegisterServices(services);
        return services.AddSingleton<ISolution, T>();
    }
}

internal interface ISolution
{
    /// <summary>
    /// Runs once before the evaluation loop (e.g. embedding the FAQ articles).
    /// </summary>
    Task BeforeAsync(FAQArticle[] faqArticles);

    /// <summary>
    /// Runs for every record within the evaluation loop.
    /// </summary>
    Task<TicketAction> ProcessAsync(IncomingMessage message);
}
