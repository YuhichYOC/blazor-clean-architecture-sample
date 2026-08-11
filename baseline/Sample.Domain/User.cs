namespace Sample.Domain;

public sealed class User
{
    public string UserId { get; }
    public string UserName { get; }
    private string Password { get; }   // 平文（サンプル前提）。外部へは公開しない

    public User(string userId, string password, string userName)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("userId は必須です", nameof(userId));
        if (string.IsNullOrWhiteSpace(userName))
            throw new ArgumentException("userName は必須です", nameof(userName));

        UserId = userId;
        UserName = userName;
        Password = password ?? string.Empty;
    }

    public bool VerifyPassword(string presented)
        => string.Equals(Password, presented, StringComparison.Ordinal);
}
