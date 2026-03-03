using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace StilettoSQL.Query;

public sealed class AutoTransaction : IDisposable {

    private readonly TransactionScope _scope;
    private bool _ok;

    public AutoTransaction() =>
        _scope = new TransactionScope(
            TransactionScopeOption.Required, // Присоединиться к существующей, если есть
            new TransactionOptions { 
                IsolationLevel = IsolationLevel.ReadCommitted,
                Timeout = TimeSpan.FromMinutes(10),
            },
            TransactionScopeAsyncFlowOption.Enabled // самое главное, чтобы работала в async контесте.
            );

    public void Commit() => _ok = true;

    public void Dispose() {
        if (_ok) _scope.Complete();
        _scope.Dispose();
    }
}
