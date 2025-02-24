using System;
using System.Data;
using System.Threading.Tasks;

namespace IOITCore.Repositories.Interfaces.Bases
{
    public interface IUnitOfWork
    {
        IDisposable BeginTransaction(IsolationLevel level);
        void CommitChanges();
        Task CommitChangesAsync();
        void CommitTransaction();
        void RollbackTransaction();
    }
}
