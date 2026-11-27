using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Text;

namespace ClipBoardCygnus
{
    public class SqliteStorageService
    {
        private readonly object _dbLock = new object();
        private PortableSettings _settings;

        public string BaseDataPath { get; private set; }
        public string BlobsPath { get; private set; }
        public string DatabaseFilePath { get; private set; }
        public string ConfigFilePath { get; private set; }

        public SqliteStorageService(PortableSettings initialSettings)
        {
            _settings = initialSettings ?? new PortableSettings();
            ResolveDirectories();
            LoadSettingsIfPresent();
            InitializeSchema();
        }

        public PortableSettings Settings
        {
            get { lock (_dbLock) { return _settings; } }
        }

        private string ConnectionString =>
            $"Data Source={DatabaseFilePath};Version=3;Journal Mode=WAL;Synchronous=Normal;";

        private void ResolveDirectories()
        {
            if (_settings.PortableMode)
            {
                string exeDir = AppDomain.CurrentDomain.BaseDirectory;
                BaseDataPath = Path.Combine(exeDir, "CygnusData");
            }
            else
            {
                string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                BaseDataPath = Path.Combine(localAppData, "ClipBoardCygnus");
            }

            BlobsPath = Path.Combine(BaseDataPath, "blobs");
            DatabaseFilePath = Path.Combine(BaseDataPath, "ClipBoardCygnus.db");
            ConfigFilePath = Path.Combine(BaseDataPath, "clipboardcygnus.config.json");

            Directory.CreateDirectory(BaseDataPath);
            Directory.CreateDirectory(BlobsPath);
        }

        private void InitializeSchema()
        {
            lock (_dbLock)
            {
                using (var conn = new SQLiteConnection(ConnectionString))
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                            CREATE TABLE IF NOT EXISTS clips (
                                id TEXT PRIMARY KEY,
                                room_code TEXT NOT NULL DEFAULT 'LOCAL',
                                kind TEXT NOT NULL,
                                title TEXT NOT NULL,
                                content TEXT NOT NULL,
                                file_name TEXT,
                                mime_type TEXT,
                                size_bytes INTEGER NOT NULL DEFAULT 0,
                                width INTEGER,
                                height INTEGER,
                                hash TEXT NOT NULL,
                                device_name TEXT NOT NULL DEFAULT 'Equipo local',
                                source_app TEXT,
                                local_blob_path TEXT,
                                pinned INTEGER NOT NULL DEFAULT 0,
                                created_at TEXT NOT NULL,
                                updated_at TEXT NOT NULL
                            );
                            CREATE INDEX IF NOT EXISTS idx_clips_order
                                ON clips(pinned DESC, created_at DESC);
                            CREATE INDEX IF NOT EXISTS idx_clips_hash
                                ON clips(hash);
                            CREATE TABLE IF NOT EXISTS ui_state (
                                key TEXT PRIMARY KEY,
                                value TEXT NOT NULL
                            );
                            CREATE TABLE IF NOT EXISTS code_templates (
                                id TEXT PRIMARY KEY,
                                name TEXT NOT NULL,
                                group_name TEXT NOT NULL DEFAULT 'General',
                                language TEXT NOT NULL DEFAULT 'SQL',
                                content TEXT NOT NULL,
                                defaults_json TEXT NOT NULL DEFAULT '{}',
                                created_at TEXT NOT NULL,
                                updated_at TEXT NOT NULL
                            );
                            CREATE INDEX IF NOT EXISTS idx_templates_updated
                                ON code_templates(updated_at DESC);
                        ";
                        cmd.ExecuteNonQuery();
                    }

                    bool hasGroupCol = false;
                    using (var pragma = conn.CreateCommand())
                    {
                        pragma.CommandText = "PRAGMA table_info(code_templates)";
                        using (var r = pragma.ExecuteReader())
                        {
                            while (r.Read())
                            {
                                string colName = r.IsDBNull(1) ? string.Empty : r.GetString(1);
                                if (string.Equals(colName, "group_name", StringComparison.OrdinalIgnoreCase))
                                {
                                    hasGroupCol = true;
                                    break;
                                }
                            }
                        }
                    }
                    if (!hasGroupCol)
                    {
                        using (var alter = conn.CreateCommand())
                        {
                            alter.CommandText = "ALTER TABLE code_templates ADD COLUMN group_name TEXT NOT NULL DEFAULT 'General'";
                            alter.ExecuteNonQuery();
                        }
                    }

                    using (var idxCmd = conn.CreateCommand())
                    {
                        idxCmd.CommandText = "CREATE INDEX IF NOT EXISTS idx_templates_group ON code_templates(group_name, updated_at DESC)";
                        idxCmd.ExecuteNonQuery();
                    }

                    SeedStarterTemplatesIfNeeded(conn);
                }
            }
        }

        private void SeedStarterTemplatesIfNeeded(SQLiteConnection conn)
        {
            using (var checkSeed = conn.CreateCommand())
            {
                checkSeed.CommandText = "SELECT value FROM ui_state WHERE key = 'templates_seeded' LIMIT 1";
                if (checkSeed.ExecuteScalar() != null) return;
            }

            long existingCount = 0;
            using (var countCmd = conn.CreateCommand())
            {
                countCmd.CommandText = "SELECT COUNT(*) FROM code_templates";
                object raw = countCmd.ExecuteScalar();
                if (raw != null && raw != DBNull.Value)
                    existingCount = Convert.ToInt64(raw);
            }

            if (existingCount == 0)
            {
                string nowIso = DateTime.UtcNow.ToString("o");
                var starters = new[]
                {
                    new CodeTemplate
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Consulta SELECT SQL filtrada",
                        GroupName = "General",
                        Language = "SQL",
                        Content = "SELECT {{columnas}}\r\nFROM {{esquema}}.{{tabla}}\r\nWHERE {{campo_filtro}} = '{{valor}}'\r\nORDER BY {{orden}} DESC;",
                        DefaultsJson = SerializeDefaults(new Dictionary<string, string>
                        {
                            { "columnas", "id, nombre, estado" },
                            { "esquema", "dbo" },
                            { "orden", "created_at" }
                        }),
                        CreatedAt = nowIso,
                        UpdatedAt = nowIso
                    },
                    new CodeTemplate
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Método de servicio asíncrono C#",
                        GroupName = "General",
                        Language = "C#",
                        Content = "public async Task<{{retorno}}> {{nombre_metodo}}Async({{parametro}} id, CancellationToken ct = default)\r\n{\r\n    return await _repository.GetByIdAsync(id, ct);\r\n}",
                        DefaultsJson = SerializeDefaults(new Dictionary<string, string>
                        {
                            { "retorno", "DtoResult" },
                            { "parametro", "Guid" }
                        }),
                        CreatedAt = nowIso,
                        UpdatedAt = nowIso
                    }
                };

                foreach (var tpl in starters)
                {
                    InsertOrUpdateTemplate(conn, tpl);
                }
            }

            using (var markCmd = conn.CreateCommand())
            {
                markCmd.CommandText = "INSERT OR REPLACE INTO ui_state (key, value) VALUES ('templates_seeded', '1')";
                markCmd.ExecuteNonQuery();
            }
        }

        public void SaveWindowSize(int width, int height)
        {
            int safeW = Math.Max(340, width);
            int safeH = Math.Max(360, height);
            lock (_dbLock)
            {
                _settings.PopupWidth = safeW;
                _settings.PopupHeight = safeH;

                using (var conn = new SQLiteConnection(ConnectionString))
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                            INSERT OR REPLACE INTO ui_state (key, value) VALUES ('popup_width', @w);
                            INSERT OR REPLACE INTO ui_state (key, value) VALUES ('popup_height', @h);";
                        cmd.Parameters.AddWithValue("@w", safeW.ToString());
                        cmd.Parameters.AddWithValue("@h", safeH.ToString());
                        cmd.ExecuteNonQuery();
                    }
                }

                try { WriteJson(ConfigFilePath, _settings); } catch { }
            }
        }

        public void SaveWindowBounds(int x, int y, int width, int height)
        {
            int safeW = Math.Max(340, width);
            int safeH = Math.Max(360, height);
            lock (_dbLock)
            {
                _settings.PopupWidth = safeW;
                _settings.PopupHeight = safeH;

                using (var conn = new SQLiteConnection(ConnectionString))
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                            INSERT OR REPLACE INTO ui_state (key, value) VALUES ('popup_x', @x);
                            INSERT OR REPLACE INTO ui_state (key, value) VALUES ('popup_y', @y);
                            INSERT OR REPLACE INTO ui_state (key, value) VALUES ('popup_width', @w);
                            INSERT OR REPLACE INTO ui_state (key, value) VALUES ('popup_height', @h);";
                        cmd.Parameters.AddWithValue("@x", x.ToString());
                        cmd.Parameters.AddWithValue("@y", y.ToString());
                        cmd.Parameters.AddWithValue("@w", safeW.ToString());
                        cmd.Parameters.AddWithValue("@h", safeH.ToString());
                        cmd.ExecuteNonQuery();
                    }
                }

                try { WriteJson(ConfigFilePath, _settings); } catch { }
            }
        }

        public void GetWindowSize(out int width, out int height)
        {
            int? unusedX, unusedY;
            GetWindowBounds(out unusedX, out unusedY, out width, out height);
        }

        public void GetWindowBounds(out int? x, out int? y, out int width, out int height)
        {
            lock (_dbLock)
            {
                x = null;
                y = null;
                width = _settings.PopupWidth >= 340 ? _settings.PopupWidth : 440;
                height = _settings.PopupHeight >= 360 ? _settings.PopupHeight : 540;

                try
                {
                    using (var conn = new SQLiteConnection(ConnectionString))
                    {
                        conn.Open();
                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = "SELECT key, value FROM ui_state WHERE key IN ('popup_x', 'popup_y', 'popup_width', 'popup_height')";
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    string k = reader.GetString(0);
                                    int parsed;
                                    if (int.TryParse(reader.GetString(1), out parsed))
                                    {
                                        if (k == "popup_x") x = parsed;
                                        if (k == "popup_y") y = parsed;
                                        if (k == "popup_width" && parsed >= 340) width = parsed;
                                        if (k == "popup_height" && parsed >= 360) height = parsed;
                                    }
                                }
                            }
                        }
                    }
                }
                catch { }
            }
        }

        public void SaveSettings(PortableSettings updated)
        {
            lock (_dbLock)
            {
                _settings = updated;
                ResolveDirectories();
                InitializeSchema();
                WriteJson(ConfigFilePath, _settings);
            }
        }

        private void LoadSettingsIfPresent()
        {
            if (!File.Exists(ConfigFilePath))
            {
                WriteJson(ConfigFilePath, _settings);
                return;
            }
            try
            {
                var loaded = ReadJson<PortableSettings>(ConfigFilePath);
                if (loaded != null) _settings = loaded;
            }
            catch { }
        }

        public List<ClipItem> Search(string query, string kindFilter = "all")
        {
            lock (_dbLock)
            {
                var results = new List<ClipItem>();
                using (var conn = new SQLiteConnection(ConnectionString))
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        var where = new List<string> { "1=1" };

                        if (!string.IsNullOrEmpty(kindFilter) && kindFilter != "all")
                        {
                            if (kindFilter == "pinned")
                                where.Add("pinned = 1");
                            else
                            {
                                where.Add("kind = @kind");
                                cmd.Parameters.AddWithValue("@kind", kindFilter);
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(query))
                        {
                            where.Add(@"(
                                LOWER(title) LIKE @q OR
                                LOWER(COALESCE(file_name, '')) LIKE @q OR
                                LOWER(COALESCE(mime_type, '')) LIKE @q OR
                                LOWER(COALESCE(source_app, '')) LIKE @q OR
                                (kind = 'text' AND LOWER(content) LIKE @q)
                            )");
                            cmd.Parameters.AddWithValue("@q", "%" + query.Trim().ToLowerInvariant() + "%");
                        }

                        cmd.CommandText = $@"
                            SELECT id, kind, title, content, file_name, mime_type,
                                   size_bytes, width, height, hash, source_app,
                                   local_blob_path, pinned, created_at, updated_at
                            FROM clips
                            WHERE {string.Join(" AND ", where)}
                            ORDER BY pinned DESC, created_at DESC
                            LIMIT @limit";
                        cmd.Parameters.AddWithValue("@limit", _settings.MaxHistoryItems);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                results.Add(new ClipItem
                                {
                                    Id = reader.GetString(0),
                                    KindString = reader.GetString(1),
                                    Title = reader.GetString(2),
                                    Content = reader.GetString(3),
                                    FileName = reader.IsDBNull(4) ? null : reader.GetString(4),
                                    MimeType = reader.IsDBNull(5) ? null : reader.GetString(5),
                                    SizeBytes = reader.GetInt64(6),
                                    Width = reader.IsDBNull(7) ? (int?)null : reader.GetInt32(7),
                                    Height = reader.IsDBNull(8) ? (int?)null : reader.GetInt32(8),
                                    Hash = reader.GetString(9),
                                    SourceApp = reader.IsDBNull(10) ? null : reader.GetString(10),
                                    LocalBlobPath = reader.IsDBNull(11) ? null : reader.GetString(11),
                                    Pinned = reader.GetInt32(12) == 1,
                                    CreatedAt = reader.GetString(13),
                                    UpdatedAt = reader.GetString(14)
                                });
                            }
                        }
                    }
                }
                return results;
            }
        }

        public ClipItem Upsert(ClipItem incoming)
        {
            lock (_dbLock)
            {
                using (var conn = new SQLiteConnection(ConnectionString))
                {
                    conn.Open();

                    if (_settings.DeduplicateCopies && !string.IsNullOrEmpty(incoming.Hash))
                    {
                        using (var checkCmd = conn.CreateCommand())
                        {
                            checkCmd.CommandText = "SELECT id FROM clips WHERE hash = @hash LIMIT 1";
                            checkCmd.Parameters.AddWithValue("@hash", incoming.Hash);
                            var existingId = checkCmd.ExecuteScalar() as string;
                            if (!string.IsNullOrEmpty(existingId))
                            {
                                using (var upd = conn.CreateCommand())
                                {
                                    upd.CommandText = @"
                                        UPDATE clips
                                        SET created_at = @created, updated_at = @updated, source_app = COALESCE(@source, source_app)
                                        WHERE id = @id";
                                    upd.Parameters.AddWithValue("@created", incoming.CreatedAt);
                                    upd.Parameters.AddWithValue("@updated", incoming.UpdatedAt);
                                    upd.Parameters.AddWithValue("@source", (object)incoming.SourceApp ?? DBNull.Value);
                                    upd.Parameters.AddWithValue("@id", existingId);
                                    upd.ExecuteNonQuery();
                                }
                                incoming.Id = existingId;
                                return incoming;
                            }
                        }
                    }

                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                            INSERT OR REPLACE INTO clips (
                                id, room_code, kind, title, content, file_name, mime_type,
                                size_bytes, width, height, hash, device_name, source_app,
                                local_blob_path, pinned, created_at, updated_at
                            ) VALUES (
                                @id, 'LOCAL', @kind, @title, @content, @fileName, @mime,
                                @size, @width, @height, @hash, 'Equipo local', @source,
                                @blob, @pinned, @created, @updated
                            )";

                        cmd.Parameters.AddWithValue("@id", incoming.Id);
                        cmd.Parameters.AddWithValue("@kind", incoming.KindString ?? "text");
                        cmd.Parameters.AddWithValue("@title", incoming.Title ?? "Elemento");
                        cmd.Parameters.AddWithValue("@content", incoming.Content ?? string.Empty);
                        cmd.Parameters.AddWithValue("@fileName", (object)incoming.FileName ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@mime", (object)incoming.MimeType ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@size", incoming.SizeBytes);
                        cmd.Parameters.AddWithValue("@width", (object)incoming.Width ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@height", (object)incoming.Height ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@hash", incoming.Hash ?? string.Empty);
                        cmd.Parameters.AddWithValue("@source", (object)incoming.SourceApp ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@blob", (object)incoming.LocalBlobPath ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@pinned", incoming.Pinned ? 1 : 0);
                        cmd.Parameters.AddWithValue("@created", incoming.CreatedAt);
                        cmd.Parameters.AddWithValue("@updated", incoming.UpdatedAt);
                        cmd.ExecuteNonQuery();
                    }

                    EnforceLimits(conn);
                }
                return incoming;
            }
        }

        public void TogglePin(string id)
        {
            lock (_dbLock)
            {
                using (var conn = new SQLiteConnection(ConnectionString))
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                            UPDATE clips
                            SET pinned = CASE WHEN pinned = 1 THEN 0 ELSE 1 END,
                                updated_at = @updated
                            WHERE id = @id";
                        cmd.Parameters.AddWithValue("@updated", DateTime.UtcNow.ToString("o"));
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        public void MarkAsUsed(IEnumerable<string> ids)
        {
            if (ids == null) return;
            var idList = new List<string>(ids);
            if (idList.Count == 0) return;

            lock (_dbLock)
            {
                using (var conn = new SQLiteConnection(ConnectionString))
                {
                    conn.Open();
                    DateTime baseTime = DateTime.UtcNow;
                    for (int i = 0; i < idList.Count; i++)
                    {
                        string ts = baseTime.AddMilliseconds(idList.Count - i).ToString("o");
                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = "UPDATE clips SET created_at = @ts, updated_at = @ts WHERE id = @id";
                            cmd.Parameters.AddWithValue("@ts", ts);
                            cmd.Parameters.AddWithValue("@id", idList[i]);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        public void Delete(string id)
        {
            lock (_dbLock)
            {
                using (var conn = new SQLiteConnection(ConnectionString))
                {
                    conn.Open();
                    string blobPath = null;
                    using (var sel = conn.CreateCommand())
                    {
                        sel.CommandText = "SELECT local_blob_path FROM clips WHERE id = @id";
                        sel.Parameters.AddWithValue("@id", id);
                        blobPath = sel.ExecuteScalar() as string;
                    }

                    if (!string.IsNullOrEmpty(blobPath))
                    {
                        string fullBlob = Path.Combine(BaseDataPath, blobPath);
                        if (File.Exists(fullBlob)) File.Delete(fullBlob);
                    }

                    using (var del = conn.CreateCommand())
                    {
                        del.CommandText = "DELETE FROM clips WHERE id = @id";
                        del.Parameters.AddWithValue("@id", id);
                        del.ExecuteNonQuery();
                    }
                }
            }
        }

        private void EnforceLimits(SQLiteConnection conn)
        {
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
                    DELETE FROM clips
                    WHERE pinned = 0 AND id NOT IN (
                        SELECT id FROM clips
                        WHERE pinned = 0
                        ORDER BY created_at DESC
                        LIMIT @maxUnpinned
                    )";
                cmd.Parameters.AddWithValue("@maxUnpinned", _settings.MaxHistoryItems);
                cmd.ExecuteNonQuery();
            }
        }

        public string SaveBlobBytes(string fileName, byte[] bytes)
        {
            string safeName = Guid.NewGuid().ToString("N").Substring(0, 8) + "_" + Path.GetFileName(fileName);
            string relative = Path.Combine("blobs", safeName);
            File.WriteAllBytes(Path.Combine(BaseDataPath, relative), bytes);
            return relative;
        }

        public static string ComputeSha256(byte[] data)
        {
            using (var sha = SHA256.Create())
            {
                byte[] hash = sha.ComputeHash(data);
                var sb = new StringBuilder(hash.Length * 2);
                for (int i = 0; i < hash.Length; i++)
                    sb.Append(hash[i].ToString("x2"));
                return sb.ToString();
            }
        }

        public List<string> GetTemplateGroups()
        {
            lock (_dbLock)
            {
                var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "General" };
                var groups = new List<string> { "General" };
                using (var conn = new SQLiteConnection(ConnectionString))
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                            SELECT DISTINCT COALESCE(NULLIF(TRIM(group_name), ''), 'General') AS grp
                            FROM code_templates
                            ORDER BY LOWER(grp) ASC";
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string grp = reader.IsDBNull(0) ? "General" : reader.GetString(0).Trim();
                                if (!string.IsNullOrEmpty(grp) && seen.Add(grp))
                                {
                                    groups.Add(grp);
                                }
                            }
                        }
                    }
                }
                return groups;
            }
        }

        public List<CodeTemplate> GetTemplates(string query = "", string groupFilter = "")
        {
            lock (_dbLock)
            {
                var results = new List<CodeTemplate>();
                using (var conn = new SQLiteConnection(ConnectionString))
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        var clauses = new List<string>();
                        if (!string.IsNullOrWhiteSpace(groupFilter) &&
                            !string.Equals(groupFilter.Trim(), "Todos los grupos", StringComparison.OrdinalIgnoreCase))
                        {
                            clauses.Add("LOWER(COALESCE(NULLIF(TRIM(group_name), ''), 'General')) = @grp");
                            cmd.Parameters.AddWithValue("@grp", groupFilter.Trim().ToLowerInvariant());
                        }
                        if (!string.IsNullOrWhiteSpace(query))
                        {
                            clauses.Add("(LOWER(name) LIKE @q OR LOWER(COALESCE(group_name, 'General')) LIKE @q OR LOWER(language) LIKE @q OR LOWER(content) LIKE @q)");
                            cmd.Parameters.AddWithValue("@q", "%" + query.Trim().ToLowerInvariant() + "%");
                        }

                        string whereSql = clauses.Count > 0 ? "WHERE " + string.Join(" AND ", clauses.ToArray()) : string.Empty;
                        cmd.CommandText = @"
                            SELECT id, name, COALESCE(NULLIF(TRIM(group_name), ''), 'General'), language, content, defaults_json, created_at, updated_at
                            FROM code_templates
                            " + whereSql + @"
                            ORDER BY CASE WHEN LOWER(COALESCE(NULLIF(TRIM(group_name), ''), 'General')) = 'general' THEN 0 ELSE 1 END,
                                     LOWER(COALESCE(NULLIF(TRIM(group_name), ''), 'General')) ASC,
                                     updated_at DESC";

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                results.Add(new CodeTemplate
                                {
                                    Id = reader.GetString(0),
                                    Name = reader.GetString(1),
                                    GroupName = reader.IsDBNull(2) ? "General" : reader.GetString(2),
                                    Language = reader.GetString(3),
                                    Content = reader.GetString(4),
                                    DefaultsJson = reader.IsDBNull(5) ? "{}" : reader.GetString(5),
                                    CreatedAt = reader.GetString(6),
                                    UpdatedAt = reader.GetString(7)
                                });
                            }
                        }
                    }
                }
                return results;
            }
        }

        public CodeTemplate SaveTemplate(CodeTemplate template)
        {
            if (template == null) throw new ArgumentNullException("template");
            string nowIso = DateTime.UtcNow.ToString("o");
            if (string.IsNullOrWhiteSpace(template.Id)) template.Id = Guid.NewGuid().ToString();
            if (string.IsNullOrWhiteSpace(template.CreatedAt)) template.CreatedAt = nowIso;
            template.UpdatedAt = nowIso;
            template.GroupName = string.IsNullOrWhiteSpace(template.GroupName) ? "General" : template.GroupName.Trim();
            if (string.IsNullOrWhiteSpace(template.Language)) template.Language = "Texto";
            if (string.IsNullOrWhiteSpace(template.DefaultsJson)) template.DefaultsJson = "{}";

            lock (_dbLock)
            {
                using (var conn = new SQLiteConnection(ConnectionString))
                {
                    conn.Open();
                    InsertOrUpdateTemplate(conn, template);
                }
                return template;
            }
        }

        private static void InsertOrUpdateTemplate(SQLiteConnection conn, CodeTemplate template)
        {
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"
                    INSERT OR REPLACE INTO code_templates (
                        id, name, group_name, language, content, defaults_json, created_at, updated_at
                    ) VALUES (
                        @id, @name, @group_name, @language, @content, @defaults_json, @created_at, @updated_at
                    )";
                cmd.Parameters.AddWithValue("@id", template.Id);
                cmd.Parameters.AddWithValue("@name", template.Name ?? "Plantilla");
                cmd.Parameters.AddWithValue("@group_name", string.IsNullOrWhiteSpace(template.GroupName) ? "General" : template.GroupName.Trim());
                cmd.Parameters.AddWithValue("@language", template.Language ?? "Texto");
                cmd.Parameters.AddWithValue("@content", template.Content ?? string.Empty);
                cmd.Parameters.AddWithValue("@defaults_json", template.DefaultsJson ?? "{}");
                cmd.Parameters.AddWithValue("@created_at", template.CreatedAt);
                cmd.Parameters.AddWithValue("@updated_at", template.UpdatedAt);
                cmd.ExecuteNonQuery();
            }
        }

        public void DeleteTemplate(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return;
            lock (_dbLock)
            {
                using (var conn = new SQLiteConnection(ConnectionString))
                {
                    conn.Open();
                    using (var del = conn.CreateCommand())
                    {
                        del.CommandText = "DELETE FROM code_templates WHERE id = @id";
                        del.Parameters.AddWithValue("@id", id);
                        del.ExecuteNonQuery();
                    }
                }
            }
        }

        public static Dictionary<string, string> ParseDefaults(string json)
        {
            var result = new Dictionary<string, string>(StringComparer.Ordinal);
            if (string.IsNullOrWhiteSpace(json)) return result;

            try
            {
                var settings = new DataContractJsonSerializerSettings { UseSimpleDictionaryFormat = true };
                var serializer = new DataContractJsonSerializer(typeof(Dictionary<string, string>), settings);
                using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                {
                    var parsed = serializer.ReadObject(ms) as Dictionary<string, string>;
                    if (parsed != null)
                    {
                        foreach (var kv in parsed)
                        {
                            result[kv.Key] = kv.Value ?? string.Empty;
                        }
                    }
                }
            }
            catch { }
            return result;
        }

        public static string SerializeDefaults(IDictionary<string, string> dict)
        {
            var clean = new Dictionary<string, string>(StringComparer.Ordinal);
            if (dict != null)
            {
                foreach (var kv in dict)
                {
                    if (!string.IsNullOrWhiteSpace(kv.Key) && !string.IsNullOrEmpty(kv.Value))
                    {
                        clean[kv.Key] = kv.Value;
                    }
                }
            }

            try
            {
                var settings = new DataContractJsonSerializerSettings { UseSimpleDictionaryFormat = true };
                var serializer = new DataContractJsonSerializer(typeof(Dictionary<string, string>), settings);
                using (var ms = new MemoryStream())
                {
                    serializer.WriteObject(ms, clean);
                    return Encoding.UTF8.GetString(ms.ToArray());
                }
            }
            catch
            {
                return "{}";
            }
        }

        private static void WriteJson<T>(string path, T obj)
        {
            var serializer = new DataContractJsonSerializer(typeof(T));
            using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                serializer.WriteObject(fs, obj);
            }
        }

        private static T ReadJson<T>(string path) where T : class
        {
            var serializer = new DataContractJsonSerializer(typeof(T));
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                return serializer.ReadObject(fs) as T;
            }
        }
    }
}
