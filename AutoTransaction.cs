using System.Data.Common;
using System.Transactions;

namespace StilettoSQL.Query;


public sealed class AutoTransaction : IDisposable {

    internal DbConnection? Connection;
    internal DbTransaction? transact;

    public AutoTransaction() {
        if (StProfile.CurrentTransaction_.Value != null) {
            throw new Exception("recursive transactions not supported");
        }

        // регистрируем себя.
        StProfile.CurrentTransaction_.Value = this;
    }

    async public Task CommitAsync() {
        if (transact == null) {
            throw new Exception("no connetions during transaction");
        }
        await transact.CommitAsync();
    }

    public void Dispose() {
        transact?.Dispose();
        Connection?.Dispose();
        StProfile.CurrentTransaction_.Value = null;
    }
}
