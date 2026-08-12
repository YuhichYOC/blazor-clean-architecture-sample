using Sample.Domain;
using Sample.Application.Abstractions;

namespace Sample.Persistence;

/*
 * 機能追加時にパーシステント層でやること
 * 5. リポジトリアダプターの追加
 * アプリケーション層で定義した IUserRepository, パーシステント層で定義した IUserDataAccess に対応する「逆転」を記載したクラス
 *
 * これも DIP のために DI で使用する
 * 後でパーシステント層の ServiceCollectionExtensions にて使用する
 *
 * クラスの宣言は IUserDataAccess を使って IUserRepository を実装する形になっている
 */
public sealed class UserRepositoryAdapter(IUserDataAccess inner) : IUserRepository
{
    /*
     * IUserRepository, IUserDataAccess, つまり UserRepository に定義したメソッドをここでも定義する
     * IUserDataAccess ( DI で渡された UserRepository ) の FindByIdAsync を呼び出し、結果を返すという内容
     */
    public async Task<User?> FindByIdAsync(string userId, CancellationToken ct = default)
        => await inner.FindByIdAsync(userId, ct);
}
