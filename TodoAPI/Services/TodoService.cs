using TodoAPI.Models;

namespace TodoAPI.Services
{
    public class TodoService
    {
        private readonly List<TodoItem> _todoItems = new();
        private int _nextId = 1;

        public IEnumerable<TodoItem> GetAll() => _todoItems;

        public TodoItem? GetItemById(int id)
        {
            return _todoItems.FirstOrDefault(t => t.Id == id);
        }

        public void Add(TodoItem item)
        {
            item.Id = _nextId++;
            _todoItems.Add(item);
        }

        public void Update(TodoItem item)
        {
            var existingItem = GetItemById(item.Id);
            if (existingItem != null)
            {
                existingItem.Title = item.Title;
                existingItem.IsCompleted = item.IsCompleted;
            }
        }

        public void Delete(int id)
        {
            var item = GetItemById(id);
            if (item != null)
            {
                _todoItems.Remove(item);
            }
        }

        public void DeleteAll()
        {
            _todoItems.Clear();

        }
    }
}
