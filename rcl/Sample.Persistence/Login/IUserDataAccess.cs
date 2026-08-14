using Sample.Domain;

namespace Sample.Persistence;

/*
 * 機能追加時にパーシステント層でやること
 * 3. リポジトリのインターフェースを追加
 * アプリケーション層で作成した IUserRepository と同じものを定義する
 *     便宜上名前だけは変えておく
 *
 * これも DIP のために DI で使用するインターフェース
 * 後でパーシステント層の ServiceCollectionExtensions にて使用する
 */
public interface IUserDataAccess
{
    Task<User?> FindByIdAsync(string userId, CancellationToken ct = default);
}
