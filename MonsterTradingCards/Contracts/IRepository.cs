using System.Collections.Generic;

namespace MonsterTradingCards.Contracts;

public interface IRepository<T>
{

    // READ
    T Get(Guid id);

    IEnumerable<T> GetAll();

    // CREATE
    void Add(T t);

    // UPDATE
    void Update(T t, params string[] parameters);

    // DELETE
    void Delete(Guid id);
}
