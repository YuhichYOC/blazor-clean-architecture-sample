namespace Sample.Api.Entities;

/*
 * baseline との差
 * エンティティは API 側に定義する
 */
public class User
{
    public string UserId { get; set; } = default!;

    public string Password { get; set; } = default!;

    public string UserName { get; set; } = default!;
}
