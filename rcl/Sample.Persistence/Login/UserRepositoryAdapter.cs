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
 *
 * 2026-8-12 注記 ... ログイン処理はやることが単純で、実はリポジトリアダプターを間に挟む必要がない
 * 一か月後の自分自身が DIP に向き合ったとき、この部分でどうすべきか必ず悩む。DIP 実装のバリエーションを暫定的に一つに絞るため、敢えてこの形にした
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
