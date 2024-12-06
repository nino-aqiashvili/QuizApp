using System;
using System.Collections.Generic;

public interface IRepository<T>
{
    List<T> GetAll();
    T GetById(Func<T, bool> predicate);
    void Add(T item);
    void Update(Func<T, bool> predicate, T updatedItem);
    void Remove(Func<T, bool> predicate);
}
