using System.Collections.Generic;

namespace LibraryBotUtn.Controllers
{
    public interface IController<T,L> where T : new()
    {
        IEnumerable<T> Get();
        L Post(T entity) ;
    }
}