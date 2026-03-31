namespace PostService.IntegrationTests.Helpers;

public static class WaitHelpers
{
    public static async Task<T> UntilAsync<T>(
        Func<Task<T>> action,
        Func<T, bool> predicate,
        TimeSpan timeout,
        TimeSpan? pollInterval = null)
    {
        var interval = pollInterval ?? TimeSpan.FromMilliseconds(300);
        var expiresAt = DateTimeOffset.UtcNow.Add(timeout);
        Exception? lastException = null;

        while (DateTimeOffset.UtcNow < expiresAt)
        {
            try
            {
                var result = await action();
                if (predicate(result))
                {
                    return result;
                }
            }
            catch (Exception ex)
            {
                lastException = ex;
            }

            await Task.Delay(interval);
        }

        throw new TimeoutException(
            "No se obtuvo el resultado esperado dentro del tiempo limite.",
            lastException);
    }
}
