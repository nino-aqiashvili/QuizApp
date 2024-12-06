using Newtonsoft.Json;
public class JsonRepository<T> : IRepository<T> where T : class
{
    private readonly string _filePath;
    private List<T> _items;

    public JsonRepository(string filePath)
    {
        _filePath = filePath;
        _items = LoadFromFile();
    }
    public List<T> GetAll()
    {
        return _items;
    }

    //ID
    public T GetById(Func<T, bool> predicate)
    {
        return _items.FirstOrDefault(predicate);
    }

    public void Add(T item)
    {
        _items.Add(item);
        SaveToFile();
    }

    public void Update(Func<T, bool> predicate, T updatedItem)
    {
        var index = _items.FindIndex(existingItem => predicate(existingItem));
        if (index >= 0)
        {
            _items[index] = updatedItem;
            SaveToFile();
        }
        else
        {
            throw new InvalidOperationException("Item to update not found.");
        }
    }
    public void Remove(Func<T, bool> predicate)
    {
        var itemToRemove = _items.FirstOrDefault(predicate);
        if (itemToRemove != null)
        {
            _items.Remove(itemToRemove);
            SaveToFile();
        }
        else
        {
            throw new InvalidOperationException("Item to remove not found.");
        }
    }
    private void SaveToFile()
    {
        var json = JsonConvert.SerializeObject(_items, Formatting.Indented);
        File.WriteAllText(_filePath, json);
    }
    private List<T> LoadFromFile()
    {
        if (File.Exists(_filePath))
        {
            var json = File.ReadAllText(_filePath);
            return JsonConvert.DeserializeObject<List<T>>(json) ?? new List<T>();
        }
        return new List<T>();
    }
}
