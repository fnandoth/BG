using Npgsql;

namespace PostService.IntegrationTests.Helpers;

public sealed class TestEnvironment
{
    private TestEnvironment(
        string solutionRoot,
        Uri userServiceBaseUri,
        Uri postServiceBaseUri,
        Uri notificationServiceBaseUri,
        string userDbConnectionString,
        string postDbConnectionString,
        string notificationDbConnectionString)
    {
        SolutionRoot = solutionRoot;
        UserServiceBaseUri = userServiceBaseUri;
        PostServiceBaseUri = postServiceBaseUri;
        NotificationServiceBaseUri = notificationServiceBaseUri;
        UserDbConnectionString = userDbConnectionString;
        PostDbConnectionString = postDbConnectionString;
        NotificationDbConnectionString = notificationDbConnectionString;
    }

    public string SolutionRoot { get; }
    public Uri UserServiceBaseUri { get; }
    public Uri PostServiceBaseUri { get; }
    public Uri NotificationServiceBaseUri { get; }
    public string UserDbConnectionString { get; }
    public string PostDbConnectionString { get; }
    public string NotificationDbConnectionString { get; }

    public static TestEnvironment Load()
    {
        var solutionRoot = FindSolutionRoot();
        var env = ReadEnvFile(Path.Combine(solutionRoot, ".env"));

        return new TestEnvironment(
            solutionRoot: solutionRoot,
            userServiceBaseUri: ReadUri("BG_USER_SERVICE_URL", "http://localhost:5050"),
            postServiceBaseUri: ReadUri("BG_POST_SERVICE_URL", "http://localhost:5137"),
            notificationServiceBaseUri: ReadUri("BG_NOTIFICATION_SERVICE_URL", "http://localhost:5281"),
            userDbConnectionString: BuildConnectionString(
                host: ReadString("BG_USER_DB_HOST", "localhost"),
                port: ReadInt("BG_USER_DB_PORT", 5433),
                database: "user_db",
                username: "user_svc",
                password: ReadRequired("USER_DB_PASSWORD", env)),
            postDbConnectionString: BuildConnectionString(
                host: ReadString("BG_POST_DB_HOST", "localhost"),
                port: ReadInt("BG_POST_DB_PORT", 5434),
                database: "post_db",
                username: "post_svc",
                password: ReadRequired("POST_DB_PASSWORD", env)),
            notificationDbConnectionString: BuildConnectionString(
                host: ReadString("BG_NOTIFICATION_DB_HOST", "localhost"),
                port: ReadInt("BG_NOTIFICATION_DB_PORT", 5435),
                database: "notification_db",
                username: "notification_svc",
                password: ReadRequired("NOTIFICATION_DB_PASSWORD", env)));

        static Uri ReadUri(string key, string fallback)
        {
            return new Uri(ReadString(key, fallback), UriKind.Absolute);
        }

        static string ReadString(string key, string fallback)
        {
            return Environment.GetEnvironmentVariable(key) ?? fallback;
        }

        static int ReadInt(string key, int fallback)
        {
            return int.TryParse(Environment.GetEnvironmentVariable(key), out var parsed)
                ? parsed
                : fallback;
        }
    }

    private static string FindSolutionRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null)
        {
            var hasSolution = File.Exists(Path.Combine(directory.FullName, "BG.Microservices.sln"));
            var hasEnv = File.Exists(Path.Combine(directory.FullName, ".env"));

            if (hasSolution && hasEnv)
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new InvalidOperationException(
            "No se pudo encontrar la raiz de la solucion. Asegurate de ejecutar las pruebas dentro del workspace del proyecto.");
    }

    private static Dictionary<string, string> ReadEnvFile(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException("No se encontro el archivo .env requerido para las pruebas.", path);
        }

        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var rawLine in File.ReadAllLines(path))
        {
            var line = rawLine.Trim();

            if (string.IsNullOrWhiteSpace(line) || line.StartsWith('#'))
            {
                continue;
            }

            var separatorIndex = line.IndexOf('=');
            if (separatorIndex <= 0)
            {
                continue;
            }

            var key = line[..separatorIndex].Trim();
            var value = line[(separatorIndex + 1)..].Trim();
            result[key] = value;
        }

        return result;
    }

    private static string ReadRequired(string key, IReadOnlyDictionary<string, string> env)
    {
        return Environment.GetEnvironmentVariable(key)
            ?? (env.TryGetValue(key, out var value) ? value : null)
            ?? throw new InvalidOperationException($"No se encontro la variable requerida '{key}' ni en el entorno ni en el archivo .env.");
    }

    private static string BuildConnectionString(string host, int port, string database, string username, string password)
    {
        return new NpgsqlConnectionStringBuilder
        {
            Host = host,
            Port = port,
            Database = database,
            Username = username,
            Password = password,
            Pooling = false
        }.ConnectionString;
    }
}
