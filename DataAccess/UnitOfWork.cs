using Microsoft.EntityFrameworkCore;

namespace DataAccess;

public class UnitOfWork : IUnitOfWork
{
    private readonly DbContext _context;

    public UnitOfWork(DbContext context)
    {
        _context = context;
    }

    public void Save()
    {
        //Since this is trying to handle a unit of work, is it worth having a process to check whether we're within a dB transaction?
        //Or setting default transaction behaviour
        //_context.Database.AutoTransactionBehavior
        _context.SaveChanges();
    }
}