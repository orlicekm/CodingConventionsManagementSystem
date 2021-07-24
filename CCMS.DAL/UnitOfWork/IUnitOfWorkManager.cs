namespace CCMS.DAL.UnitOfWork
{
    public interface IUnitOfWorkManager
    {
        public IUnitOfWork Get();
    }
}