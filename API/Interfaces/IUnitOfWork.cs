using System.Threading.Tasks;

namespace API.Interfaces
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepository {get;}
        IMessageRepository MessageRepository {get; }
        ILikesRepository likesRepository {get; }
        IBlockRepository blockRepository {get;}
        Task<bool> Complete();
        bool HasChanges();
    }
}