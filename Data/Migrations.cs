using Dapper;
using Microsoft.Data.Sqlite;

namespace Bloom.Data;

public static class Migrations
{
    private static readonly string[] Scripts =
    {
        SchemaV1
    };

    public static void Run(SqliteConnection connection)
    {
        long current = connection.ExecuteScalar<long>("PRAGMA user_version;");
        for (int version = (int)current; version < Scripts.Length; version++)
        {
            using SqliteTransaction transaction = connection.BeginTransaction();
            connection.Execute(Scripts[version], transaction: transaction);
            transaction.Commit();
            connection.Execute($"PRAGMA user_version = {version + 1};");
        }
    }

    private const string SchemaV1 = @"
CREATE TABLE IF NOT EXISTS app_settings (
    key   TEXT PRIMARY KEY,
    value TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS wallet (
    id      INTEGER PRIMARY KEY CHECK (id = 1),
    balance INTEGER NOT NULL DEFAULT 0,
    lifetime_earned INTEGER NOT NULL DEFAULT 0
);
INSERT OR IGNORE INTO wallet (id, balance, lifetime_earned) VALUES (1, 0, 0);

CREATE TABLE IF NOT EXISTS coin_transactions (
    id         INTEGER PRIMARY KEY AUTOINCREMENT,
    amount     INTEGER NOT NULL,
    reason     TEXT NOT NULL,
    ref_type   TEXT,
    ref_id     INTEGER,
    created_at TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS daily_earn (
    id         INTEGER PRIMARY KEY AUTOINCREMENT,
    earn_key   TEXT NOT NULL,
    earn_date  TEXT NOT NULL,
    amount     INTEGER NOT NULL,
    created_at TEXT NOT NULL,
    UNIQUE (earn_key, earn_date)
);

CREATE TABLE IF NOT EXISTS conditions (
    id          INTEGER PRIMARY KEY AUTOINCREMENT,
    name        TEXT NOT NULL,
    color       TEXT NOT NULL DEFAULT '#C9A7EB',
    icon        TEXT NOT NULL DEFAULT '🌸',
    notes       TEXT,
    sort_order  INTEGER NOT NULL DEFAULT 0,
    is_archived INTEGER NOT NULL DEFAULT 0,
    created_at  TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS symptoms (
    id           INTEGER PRIMARY KEY AUTOINCREMENT,
    condition_id INTEGER NOT NULL REFERENCES conditions(id) ON DELETE CASCADE,
    name         TEXT NOT NULL,
    track_scale  INTEGER NOT NULL DEFAULT 1,
    track_text   INTEGER NOT NULL DEFAULT 1,
    scale_min    INTEGER NOT NULL DEFAULT 1,
    scale_max    INTEGER NOT NULL DEFAULT 10,
    widget       INTEGER NOT NULL DEFAULT 0,
    color        TEXT NOT NULL DEFAULT '#C9A7EB',
    sort_order   INTEGER NOT NULL DEFAULT 0,
    is_archived  INTEGER NOT NULL DEFAULT 0,
    created_at   TEXT NOT NULL
);
CREATE INDEX IF NOT EXISTS idx_symptoms_condition ON symptoms(condition_id);

CREATE TABLE IF NOT EXISTS symptom_logs (
    id         INTEGER PRIMARY KEY AUTOINCREMENT,
    symptom_id INTEGER NOT NULL REFERENCES symptoms(id) ON DELETE CASCADE,
    entry_date TEXT NOT NULL,
    scale_value INTEGER,
    text_value  TEXT,
    created_at  TEXT NOT NULL
);
CREATE INDEX IF NOT EXISTS idx_symptom_logs_date ON symptom_logs(entry_date);
CREATE INDEX IF NOT EXISTS idx_symptom_logs_symptom ON symptom_logs(symptom_id);

CREATE TABLE IF NOT EXISTS alters (
    id          INTEGER PRIMARY KEY AUTOINCREMENT,
    name        TEXT NOT NULL,
    pronouns    TEXT,
    role        TEXT,
    age         TEXT,
    color       TEXT NOT NULL DEFAULT '#8FD3C7',
    description TEXT,
    avatar_path TEXT,
    sort_order  INTEGER NOT NULL DEFAULT 0,
    is_archived INTEGER NOT NULL DEFAULT 0,
    created_at  TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS front_events (
    id         INTEGER PRIMARY KEY AUTOINCREMENT,
    alter_id   INTEGER NOT NULL REFERENCES alters(id) ON DELETE CASCADE,
    entry_date TEXT NOT NULL,
    fronted    INTEGER NOT NULL DEFAULT 1,
    co_conscious INTEGER NOT NULL DEFAULT 0,
    start_time TEXT,
    end_time   TEXT,
    mood       INTEGER,
    opinion    TEXT,
    note       TEXT,
    created_at TEXT NOT NULL
);
CREATE INDEX IF NOT EXISTS idx_front_events_date ON front_events(entry_date);
CREATE INDEX IF NOT EXISTS idx_front_events_alter ON front_events(alter_id);

CREATE TABLE IF NOT EXISTS checkins (
    id           INTEGER PRIMARY KEY AUTOINCREMENT,
    entry_date   TEXT NOT NULL,
    mood         INTEGER NOT NULL DEFAULT 5,
    energy       INTEGER NOT NULL DEFAULT 5,
    productivity INTEGER NOT NULL DEFAULT 5,
    note         TEXT,
    created_at   TEXT NOT NULL
);
CREATE INDEX IF NOT EXISTS idx_checkins_date ON checkins(entry_date);

CREATE TABLE IF NOT EXISTS wot_checkins (
    id         INTEGER PRIMARY KEY AUTOINCREMENT,
    entry_date TEXT NOT NULL,
    zone       INTEGER NOT NULL DEFAULT 1,
    intensity  INTEGER NOT NULL DEFAULT 5,
    cause      TEXT,
    helped     TEXT,
    note       TEXT,
    created_at TEXT NOT NULL
);
CREATE INDEX IF NOT EXISTS idx_wot_date ON wot_checkins(entry_date);

CREATE TABLE IF NOT EXISTS food_logs (
    id         INTEGER PRIMARY KEY AUTOINCREMENT,
    entry_date TEXT NOT NULL,
    name       TEXT NOT NULL,
    note       TEXT,
    meal_time  TEXT,
    created_at TEXT NOT NULL
);
CREATE INDEX IF NOT EXISTS idx_food_date ON food_logs(entry_date);

CREATE TABLE IF NOT EXISTS journal_bodies (
    id         INTEGER PRIMARY KEY AUTOINCREMENT,
    entry_date TEXT NOT NULL UNIQUE,
    title      TEXT NOT NULL DEFAULT '',
    content    TEXT NOT NULL DEFAULT '',
    updated_at TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS journal_pages (
    id            INTEGER PRIMARY KEY AUTOINCREMENT,
    entry_date    TEXT NOT NULL,
    page_index    INTEGER NOT NULL DEFAULT 0,
    background_id TEXT NOT NULL DEFAULT 'paper-cream',
    font_family   TEXT NOT NULL DEFAULT 'Segoe UI',
    decor_json    TEXT NOT NULL DEFAULT '[]',
    updated_at    TEXT NOT NULL,
    UNIQUE (entry_date, page_index)
);

CREATE TABLE IF NOT EXISTS stickers (
    id         INTEGER PRIMARY KEY AUTOINCREMENT,
    name       TEXT NOT NULL,
    category   INTEGER NOT NULL DEFAULT 0,
    image_path TEXT NOT NULL,
    is_unlocked INTEGER NOT NULL DEFAULT 1,
    unlock_cost INTEGER NOT NULL DEFAULT 0,
    sort_order  INTEGER NOT NULL DEFAULT 0
);

CREATE TABLE IF NOT EXISTS page_backgrounds (
    id          INTEGER PRIMARY KEY AUTOINCREMENT,
    key         TEXT NOT NULL UNIQUE,
    name        TEXT NOT NULL,
    kind        TEXT NOT NULL DEFAULT 'color',
    value       TEXT NOT NULL DEFAULT '#FBF3EA',
    image_path  TEXT,
    is_unlocked INTEGER NOT NULL DEFAULT 1,
    unlock_cost INTEGER NOT NULL DEFAULT 0,
    sort_order  INTEGER NOT NULL DEFAULT 0
);

CREATE TABLE IF NOT EXISTS habits (
    id           INTEGER PRIMARY KEY AUTOINCREMENT,
    name         TEXT NOT NULL,
    icon         TEXT NOT NULL DEFAULT '🌱',
    color        TEXT NOT NULL DEFAULT '#A7D7C5',
    cadence      INTEGER NOT NULL DEFAULT 0,
    interval_days INTEGER NOT NULL DEFAULT 1,
    target_per_period INTEGER NOT NULL DEFAULT 1,
    coin_reward  INTEGER NOT NULL DEFAULT 5,
    note         TEXT,
    sort_order   INTEGER NOT NULL DEFAULT 0,
    is_archived  INTEGER NOT NULL DEFAULT 0,
    created_at   TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS habit_logs (
    id         INTEGER PRIMARY KEY AUTOINCREMENT,
    habit_id   INTEGER NOT NULL REFERENCES habits(id) ON DELETE CASCADE,
    log_date   TEXT NOT NULL,
    count      INTEGER NOT NULL DEFAULT 1,
    created_at TEXT NOT NULL
);
CREATE INDEX IF NOT EXISTS idx_habit_logs_date ON habit_logs(log_date);
CREATE INDEX IF NOT EXISTS idx_habit_logs_habit ON habit_logs(habit_id);

CREATE TABLE IF NOT EXISTS tasks (
    id          INTEGER PRIMARY KEY AUTOINCREMENT,
    title       TEXT NOT NULL,
    note        TEXT,
    due_date    TEXT,
    is_done     INTEGER NOT NULL DEFAULT 0,
    done_at     TEXT,
    priority    INTEGER NOT NULL DEFAULT 1,
    coin_reward INTEGER NOT NULL DEFAULT 8,
    sort_order  INTEGER NOT NULL DEFAULT 0,
    created_at  TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS rewards (
    id          INTEGER PRIMARY KEY AUTOINCREMENT,
    name        TEXT NOT NULL,
    description TEXT,
    cost        INTEGER NOT NULL DEFAULT 25,
    icon        TEXT NOT NULL DEFAULT '🎁',
    category    INTEGER NOT NULL DEFAULT 0,
    is_custom   INTEGER NOT NULL DEFAULT 0,
    is_archived INTEGER NOT NULL DEFAULT 0,
    sort_order  INTEGER NOT NULL DEFAULT 0,
    created_at  TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS reward_redemptions (
    id          INTEGER PRIMARY KEY AUTOINCREMENT,
    reward_id   INTEGER,
    reward_name TEXT NOT NULL,
    cost        INTEGER NOT NULL,
    redeemed_at TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS creatures (
    id             INTEGER PRIMARY KEY AUTOINCREMENT,
    name           TEXT NOT NULL,
    species        TEXT NOT NULL DEFAULT '',
    blurb          TEXT NOT NULL DEFAULT '',
    image_path     TEXT NOT NULL DEFAULT '',
    silhouette_path TEXT NOT NULL DEFAULT '',
    accent_color   TEXT NOT NULL DEFAULT '#F6C6D8',
    rarity         INTEGER NOT NULL DEFAULT 0,
    unlock_cost    INTEGER NOT NULL DEFAULT 100,
    is_unlocked    INTEGER NOT NULL DEFAULT 0,
    discovered_at  TEXT,
    affection      INTEGER NOT NULL DEFAULT 0,
    times_pet      INTEGER NOT NULL DEFAULT 0,
    last_pet_at    TEXT,
    sort_order     INTEGER NOT NULL DEFAULT 0
);
";
}
