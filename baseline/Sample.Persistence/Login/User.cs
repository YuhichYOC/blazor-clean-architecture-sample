namespace Sample.Persistence.Entities;

/*
 * 機能追加時にパーシステント層でやること
 * 1. データベース上でのレコードの受け皿を用意
 * 以下に定義するクラスは UserDbContext, UserRepository で使用する
 * データベースとの I/O の単位となる
 */
public class User
{
    public string UserId { get; set; } = default!;

    public string Password { get; set; } = default!;

    public string UserName { get; set; } = default!;
}
