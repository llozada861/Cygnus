using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace ClipBoardCygnus
{
    public enum ClipKind
    {
        Text = 0,
        Image = 1,
        File = 2
    }

    [DataContract]
    public class ClipItem
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "kind")]
        public string KindString { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "content")]
        public string Content { get; set; }

        [DataMember(Name = "fileName")]
        public string FileName { get; set; }

        [DataMember(Name = "mimeType")]
        public string MimeType { get; set; }

        [DataMember(Name = "sizeBytes")]
        public long SizeBytes { get; set; }

        [DataMember(Name = "width")]
        public int? Width { get; set; }

        [DataMember(Name = "height")]
        public int? Height { get; set; }

        [DataMember(Name = "hash")]
        public string Hash { get; set; }

        [DataMember(Name = "sourceApp")]
        public string SourceApp { get; set; }

        [DataMember(Name = "pinned")]
        public bool Pinned { get; set; }

        [DataMember(Name = "createdAt")]
        public string CreatedAt { get; set; }

        [DataMember(Name = "updatedAt")]
        public string UpdatedAt { get; set; }

        [DataMember(Name = "localBlobPath")]
        public string LocalBlobPath { get; set; }

        public ClipKind GetKind()
        {
            if (string.Equals(KindString, "image", StringComparison.OrdinalIgnoreCase))
                return ClipKind.Image;
            if (string.Equals(KindString, "file", StringComparison.OrdinalIgnoreCase))
                return ClipKind.File;
            return ClipKind.Text;
        }
    }

    [DataContract]
    public class CodeTemplate
    {
        private static readonly Regex BindVarRegex = new Regex(@"\{\{\s*([a-zA-Z0-9_.\-]+)\s*\}\}", RegexOptions.Compiled);
        private static readonly Regex SystemTokenRegex = new Regex(@"\{\{\s*([a-zA-Z0-9_.\-ñÑáéíóúÁÉÍÓÚ]+)\s*\}\}", RegexOptions.Compiled);
        private static readonly Regex ShorthandVarRegex = new Regex(@"\$(FECHA_HORA|FECHA|HORA|AHORA|USUARIO|EQUIPO|UUID)\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "groupName")]
        public string GroupName { get; set; } = "General";

        [DataMember(Name = "language")]
        public string Language { get; set; }

        [DataMember(Name = "content")]
        public string Content { get; set; }

        [DataMember(Name = "defaultsJson")]
        public string DefaultsJson { get; set; }

        [DataMember(Name = "createdAt")]
        public string CreatedAt { get; set; }

        [DataMember(Name = "updatedAt")]
        public string UpdatedAt { get; set; }

        public static string GetSystemTokenValue(string tokenName, DateTime now)
        {
            if (string.IsNullOrWhiteSpace(tokenName)) return null;
            string key = tokenName.Trim().ToLowerInvariant();
            switch (key)
            {
                case "fecha_actual":
                case "fecha":
                case "hoy":
                case "sysdate":
                case "current_date":
                    return now.ToString("yyyy-MM-dd");
                case "fecha_ddmmyyyy":
                    return now.ToString("dd/MM/yyyy");
                case "hora_actual":
                case "hora":
                case "current_time":
                    return now.ToString("HH:mm:ss");
                case "fecha_hora":
                case "ahora":
                case "now":
                    return now.ToString("yyyy-MM-dd HH:mm:ss");
                case "fecha_iso":
                case "iso_date":
                    return now.ToString("yyyy-MM-ddTHH:mm:ss");
                case "anio":
                case "año":
                case "year":
                    return now.ToString("yyyy");
                case "mes":
                case "month":
                    return now.ToString("MM");
                case "dia":
                case "día":
                case "day":
                    return now.ToString("dd");
                case "usuario":
                case "user":
                case "username":
                    return Environment.UserName;
                case "equipo":
                case "machine":
                case "pc":
                case "hostname":
                    return Environment.MachineName;
                case "uuid":
                case "guid":
                    return Guid.NewGuid().ToString();
                default:
                    return null;
            }
        }

        public static string ResolveDynamicDefault(string rawDefault)
        {
            if (string.IsNullOrEmpty(rawDefault)) return string.Empty;
            DateTime now = DateTime.Now;

            string resolved = SystemTokenRegex.Replace(rawDefault, match =>
            {
                string sysVal = GetSystemTokenValue(match.Groups[1].Value, now);
                return sysVal != null ? sysVal : match.Value;
            });

            resolved = ShorthandVarRegex.Replace(resolved, match =>
            {
                string key = match.Groups[1].Value.ToLowerInvariant();
                string sysVal = GetSystemTokenValue(key, now);
                return sysVal != null ? sysVal : match.Value;
            });

            return resolved;
        }

        public static string ResolveInitialValue(string varName, IDictionary<string, string> defaults)
        {
            string val;
            if (defaults != null && defaults.TryGetValue(varName, out val) && !string.IsNullOrEmpty(val))
            {
                return ResolveDynamicDefault(val);
            }
            string directSys = GetSystemTokenValue(varName, DateTime.Now);
            return directSys ?? string.Empty;
        }

        public List<string> ExtractVariables()
        {
            var list = new List<string>();
            if (string.IsNullOrEmpty(Content)) return list;

            var seen = new HashSet<string>(StringComparer.Ordinal);
            MatchCollection matches = BindVarRegex.Matches(Content);
            foreach (Match m in matches)
            {
                if (m.Success && m.Groups.Count > 1)
                {
                    string name = m.Groups[1].Value.Trim();
                    if (!string.IsNullOrEmpty(name) && seen.Add(name))
                    {
                        list.Add(name);
                    }
                }
            }
            return list;
        }

        public string Render(IDictionary<string, string> values, IDictionary<string, string> defaults = null)
        {
            if (string.IsNullOrEmpty(Content)) return string.Empty;
            DateTime now = DateTime.Now;

            return BindVarRegex.Replace(Content, match =>
            {
                string key = match.Groups[1].Value.Trim();
                string val;
                if (values != null && values.TryGetValue(key, out val) && val != null && val.Length > 0)
                    return val;
                if (defaults != null && defaults.TryGetValue(key, out val) && val != null && val.Length > 0)
                    return ResolveDynamicDefault(val);
                string directSys = GetSystemTokenValue(key, now);
                if (directSys != null)
                    return directSys;
                return match.Value;
            });
        }
    }

    [DataContract]
    public class PortableSettings
    {
        [DataMember(Name = "portableMode")]
        public bool PortableMode { get; set; } = true;

        [DataMember(Name = "dataDirectory")]
        public string DataDirectory { get; set; } = @".\CygnusData\ClipBoardCygnus.db";

        [DataMember(Name = "startWithWindows")]
        public bool StartWithWindows { get; set; } = true;

        [DataMember(Name = "startMinimized")]
        public bool StartMinimized { get; set; } = true;

        [DataMember(Name = "globalHotkey")]
        public string GlobalHotkey { get; set; } = "Ctrl + <";

        [DataMember(Name = "maxHistoryItems")]
        public int MaxHistoryItems { get; set; } = 1000;

        [DataMember(Name = "retentionDays")]
        public int RetentionDays { get; set; } = 30;

        [DataMember(Name = "deduplicateCopies")]
        public bool DeduplicateCopies { get; set; } = true;

        [DataMember(Name = "ignorePasswordManagers")]
        public bool IgnorePasswordManagers { get; set; } = true;

        [DataMember(Name = "popupWidth")]
        public int PopupWidth { get; set; } = 393;

        [DataMember(Name = "popupHeight")]
        public int PopupHeight { get; set; } = 473;
    }
}
