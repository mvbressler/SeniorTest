namespace SeniorTest.Core.Repositories.Base
{
    public interface IRepository<T>: IReadRepository<T>, IWriteRepository<T> where T : class {
        public Boolean IsDisposed();
    }

}
