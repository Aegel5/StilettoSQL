using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace StilettoSQL;
public sealed class StAutoTransaction : IDisposable {

    private readonly TransactionScope _scope;
    private bool _ok;

    public StAutoTransaction() =>
        _scope = new TransactionScope(
            TransactionScopeOption.Required, // Присоединиться к существующей, если есть
            new TransactionOptions { 
                IsolationLevel = IsolationLevel.ReadCommitted,
                Timeout = TimeSpan.FromMinutes(10),
            },
            TransactionScopeAsyncFlowOption.Enabled);

    public void Success() => _ok = true;

    public void Dispose() {
        if (_ok) _scope.Complete();
        _scope.Dispose();
    }
}
