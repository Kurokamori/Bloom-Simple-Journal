using Dapper;
using Microsoft.Data.Sqlite;
using Bloom.Models;

namespace Bloom.Data.Repositories;

public sealed class TaskRepository
{
    private readonly Database _database;

    public TaskRepository(Database database) => _database = database;

    public IReadOnlyList<TaskItem> All(bool includeDone = true)
    {
        using SqliteConnection connection = _database.Open();
        string filter = includeDone ? string.Empty : "WHERE is_done = 0";
        return connection.Query<TaskItem>(
            $"SELECT * FROM tasks {filter} ORDER BY is_done, sort_order, priority DESC, id;").ToList();
    }

    public TaskItem? Find(long id)
    {
        using SqliteConnection connection = _database.Open();
        return connection.QuerySingleOrDefault<TaskItem>("SELECT * FROM tasks WHERE id = @id;", new { id });
    }

    public long Insert(TaskItem task)
    {
        using SqliteConnection connection = _database.Open();
        return connection.ExecuteScalar<long>(
            "INSERT INTO tasks (title, note, due_date, is_done, done_at, priority, coin_reward, sort_order, created_at) " +
            "VALUES (@Title, @Note, @DueDate, @IsDone, @DoneAt, @Priority, @CoinReward, @SortOrder, @CreatedAt); " +
            "SELECT last_insert_rowid();",
            new
            {
                task.Title, task.Note, task.DueDate, task.IsDone, task.DoneAt, Priority = (int)task.Priority,
                task.CoinReward, task.SortOrder, CreatedAt = task.CreatedAt == default ? DateTime.Now : task.CreatedAt
            });
    }

    public void Update(TaskItem task)
    {
        using SqliteConnection connection = _database.Open();
        connection.Execute(
            "UPDATE tasks SET title = @Title, note = @Note, due_date = @DueDate, is_done = @IsDone, " +
            "done_at = @DoneAt, priority = @Priority, coin_reward = @CoinReward, sort_order = @SortOrder WHERE id = @Id;",
            new
            {
                task.Id, task.Title, task.Note, task.DueDate, task.IsDone, task.DoneAt,
                Priority = (int)task.Priority, task.CoinReward, task.SortOrder
            });
    }

    public void SetDone(long id, bool done)
    {
        using SqliteConnection connection = _database.Open();
        connection.Execute("UPDATE tasks SET is_done = @done, done_at = @doneAt WHERE id = @id;",
            new { id, done, doneAt = done ? DateTime.Now : (DateTime?)null });
    }

    public void Delete(long id)
    {
        using SqliteConnection connection = _database.Open();
        connection.Execute("DELETE FROM tasks WHERE id = @id;", new { id });
    }
}
