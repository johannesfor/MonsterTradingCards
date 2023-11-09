﻿using System.Collections.Generic;

namespace MonsterTradingCards.Repositories

// Implementation of the Repository Design Pattern
// Repository overview see: https://dotnettutorials.net/lesson/repository-design-pattern-csharp/
public interface IRepository<T>
{

    // READ
    T Get(Guid id);

    IEnumerable<T> GetAll();

    // CREATE
    void Add(T t);

    // UPDATE
    void Update(T t, string[] parameters);

    // DELETE
    void Delete(T t);
}
