using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ClipBoardCygnus
{
    public class TemplatesForm : Form
    {
        private readonly SqliteStorageService _storage;

        private TextBox _searchBox;
        private ComboBox _groupFilterBox;
        private ListBox _templateList;
        private Button _newBtn;
        private Button _deleteBtn;

        private TextBox _nameBox;
        private ComboBox _groupBox;
        private ComboBox _langBox;
        private TextBox _varNameBox;
        private Button _insertVarBtn;
        private TextBox _codeBox;
        private DataGridView _defaultsGrid;
        private ComboBox _sysVarCombo;
        private Button _applySysVarBtn;
        private Label _statusLabel;
        private Button _saveBtn;
        private Button _useBtn;
        private Button _closeBtn;

        private List<CodeTemplate> _templates = new List<CodeTemplate>();
        private string _editingId;
        private string _createdAt;
        private bool _suppressSelectionChange;
        private bool _suppressGroupFilterChange;
        private bool _suppressGridSync;
        private Dictionary<string, string> _currentDefaults = new Dictionary<string, string>(StringComparer.Ordinal);

        public TemplatesForm(SqliteStorageService storage)
        {
            _storage = storage;
            BuildUi();
            RefreshGroupSelectors();
            LoadTemplates();
        }

        private void BuildUi()
        {
            Text = "ClipBoardCygnus — Plantillas de código";
            FormBorderStyle = FormBorderStyle.Sizable;
            MinimumSize = new Size(760, 520);
            Size = new Size(880, 600);
            MaximizeBox = true;
            MinimizeBox = false;
            ShowInTaskbar = false;
            TopMost = true;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.White;
            Font = new Font("Segoe UI", 9.25f);

            Shown += (s, e) =>
            {
                Activate();
                BringToFront();
            };

            var leftPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 265,
                Padding = new Padding(12),
                BackColor = Color.FromArgb(248, 250, 252)
            };
            leftPanel.Paint += (s, e) =>
            {
                using (var pen = new Pen(Color.FromArgb(226, 232, 240), 1))
                    e.Graphics.DrawLine(pen, leftPanel.Width - 1, 0, leftPanel.Width - 1, leftPanel.Height);
            };

            var leftTitle = new Label
            {
                Dock = DockStyle.Top,
                Height = 24,
                Text = "Plantillas por grupo / cliente",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42)
            };

            _searchBox = new TextBox
            {
                Dock = DockStyle.Top,
                Font = new Font("Segoe UI", 9f)
            };
            _searchBox.TextChanged += (s, e) => LoadTemplates();

            var searchSpacer = new Panel { Dock = DockStyle.Top, Height = 6 };

            _groupFilterBox = new ComboBox
            {
                Dock = DockStyle.Top,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 8.75f)
            };
            _groupFilterBox.SelectedIndexChanged += (s, e) =>
            {
                if (_suppressGroupFilterChange) return;
                LoadTemplates();
            };

            var filterSpacer = new Panel { Dock = DockStyle.Top, Height = 8 };

            var leftActions = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 36,
                Padding = new Padding(0, 6, 0, 0)
            };

            _newBtn = new Button
            {
                Dock = DockStyle.Left,
                Width = 114,
                Text = "+ Nueva",
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(37, 99, 235),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            _newBtn.FlatAppearance.BorderSize = 0;
            _newBtn.Click += (s, e) => StartNewTemplate();

            _deleteBtn = new Button
            {
                Dock = DockStyle.Right,
                Width = 114,
                Text = "Eliminar",
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = Color.FromArgb(185, 28, 28),
                Font = new Font("Segoe UI", 8.5f),
                Cursor = Cursors.Hand
            };
            _deleteBtn.FlatAppearance.BorderColor = Color.FromArgb(254, 202, 202);
            _deleteBtn.Click += (s, e) => DeleteSelectedTemplate();

            leftActions.Controls.Add(_newBtn);
            leftActions.Controls.Add(_deleteBtn);

            _templateList = new ListBox
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                DrawMode = DrawMode.OwnerDrawFixed,
                ItemHeight = 44,
                BackColor = Color.White,
                IntegralHeight = false
            };
            _templateList.DrawItem += OnDrawTemplateItem;
            _templateList.SelectedIndexChanged += (s, e) =>
            {
                if (_suppressSelectionChange) return;
                int idx = _templateList.SelectedIndex;
                if (idx >= 0 && idx < _templates.Count)
                {
                    PopulateEditor(_templates[idx]);
                }
            };
            _templateList.MouseDoubleClick += (s, e) =>
            {
                if (_templateList.SelectedIndex >= 0) UseCurrentTemplate();
            };

            leftPanel.Controls.Add(_templateList);
            leftPanel.Controls.Add(leftActions);
            leftPanel.Controls.Add(filterSpacer);
            leftPanel.Controls.Add(_groupFilterBox);
            leftPanel.Controls.Add(searchSpacer);
            leftPanel.Controls.Add(_searchBox);
            leftPanel.Controls.Add(leftTitle);

            // Right Editor Panel
            var rightPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(16, 14, 16, 12),
                BackColor = Color.White
            };

            var metaRow = new Panel
            {
                Dock = DockStyle.Top,
                Height = 52
            };

            var nameCol = new Panel { Dock = DockStyle.Fill, Padding = new Padding(0, 0, 10, 0) };
            var lblName = new Label { Dock = DockStyle.Top, Height = 18, Text = "Nombre de la plantilla", Font = new Font("Segoe UI", 8.25f, FontStyle.Bold), ForeColor = Color.FromArgb(71, 85, 105) };
            _nameBox = new TextBox { Dock = DockStyle.Top, Font = new Font("Segoe UI", 9.5f) };
            nameCol.Controls.Add(_nameBox);
            nameCol.Controls.Add(lblName);

            var groupCol = new Panel { Dock = DockStyle.Right, Width = 175, Padding = new Padding(0, 0, 10, 0) };
            var lblGroup = new Label { Dock = DockStyle.Top, Height = 18, Text = "Grupo / Cliente", Font = new Font("Segoe UI", 8.25f, FontStyle.Bold), ForeColor = Color.FromArgb(71, 85, 105) };
            _groupBox = new ComboBox
            {
                Dock = DockStyle.Top,
                DropDownStyle = ComboBoxStyle.DropDown,
                Font = new Font("Segoe UI", 9.25f)
            };
            groupCol.Controls.Add(_groupBox);
            groupCol.Controls.Add(lblGroup);

            var langCol = new Panel { Dock = DockStyle.Right, Width = 120 };
            var lblLang = new Label { Dock = DockStyle.Top, Height = 18, Text = "Lenguaje", Font = new Font("Segoe UI", 8.25f, FontStyle.Bold), ForeColor = Color.FromArgb(71, 85, 105) };
            _langBox = new ComboBox
            {
                Dock = DockStyle.Top,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9.25f)
            };
            _langBox.Items.AddRange(new object[] { "SQL", "C#", "TypeScript", "JavaScript", "JSON", "PowerShell", "Bash", "HTML", "Texto" });
            _langBox.SelectedIndex = 0;
            langCol.Controls.Add(_langBox);
            langCol.Controls.Add(lblLang);

            metaRow.Controls.Add(nameCol);
            metaRow.Controls.Add(groupCol);
            metaRow.Controls.Add(langCol);

            // Bind variable insert bar
            var varBar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 36,
                Padding = new Padding(0, 4, 0, 4)
            };

            var lblCode = new Label
            {
                Dock = DockStyle.Left,
                Width = 230,
                Text = "Código (usa {{variable}} para bind):",
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 8.25f, FontStyle.Bold),
                ForeColor = Color.FromArgb(71, 85, 105)
            };

            _insertVarBtn = new Button
            {
                Dock = DockStyle.Right,
                Width = 148,
                Text = "+ Insertar {{variable}}",
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(239, 246, 255),
                ForeColor = Color.FromArgb(30, 64, 175),
                Font = new Font("Segoe UI", 8.25f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            _insertVarBtn.FlatAppearance.BorderColor = Color.FromArgb(191, 219, 254);
            _insertVarBtn.Click += (s, e) => InsertBindVariableAtCursor();

            _varNameBox = new TextBox
            {
                Dock = DockStyle.Right,
                Width = 128,
                Text = "variable",
                Font = new Font("Consolas", 9f)
            };
            _varNameBox.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    InsertBindVariableAtCursor();
                }
            };

            var varSpacer = new Panel { Dock = DockStyle.Right, Width = 6 };
            varBar.Controls.Add(lblCode);
            varBar.Controls.Add(_insertVarBtn);
            varBar.Controls.Add(varSpacer);
            varBar.Controls.Add(_varNameBox);

            // Footer buttons
            var footer = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 44,
                Padding = new Padding(0, 10, 0, 0)
            };

            _statusLabel = new Label
            {
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 8.25f),
                ForeColor = Color.FromArgb(100, 116, 139)
            };

            _closeBtn = new Button
            {
                Dock = DockStyle.Right,
                Width = 84,
                Text = "Cerrar",
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = Color.FromArgb(51, 65, 85),
                Cursor = Cursors.Hand
            };
            _closeBtn.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
            _closeBtn.Click += (s, e) => Close();

            _saveBtn = new Button
            {
                Dock = DockStyle.Right,
                Width = 98,
                Text = "Guardar",
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = Color.FromArgb(15, 23, 42),
                Font = new Font("Segoe UI", 8.75f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            _saveBtn.FlatAppearance.BorderColor = Color.FromArgb(148, 163, 184);
            _saveBtn.Click += (s, e) => SaveCurrentTemplate();

            _useBtn = new Button
            {
                Dock = DockStyle.Right,
                Width = 148,
                Text = "Usar plantilla...",
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(37, 99, 235),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 8.75f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            _useBtn.FlatAppearance.BorderSize = 0;
            _useBtn.Click += (s, e) => UseCurrentTemplate();

            var sep1 = new Panel { Dock = DockStyle.Right, Width = 8 };
            var sep2 = new Panel { Dock = DockStyle.Right, Width = 8 };

            footer.Controls.Add(_statusLabel);
            footer.Controls.Add(_useBtn);
            footer.Controls.Add(sep1);
            footer.Controls.Add(_saveBtn);
            footer.Controls.Add(sep2);
            footer.Controls.Add(_closeBtn);

            // Detected Bind Variables Grid + Dynamic System Variables Helper
            var defaultsPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 178,
                Padding = new Padding(0, 8, 0, 0)
            };

            var varsHeaderBar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 28,
                Padding = new Padding(0, 0, 0, 4)
            };

            var lblVars = new Label
            {
                Dock = DockStyle.Fill,
                Text = "Variables bind y valor por defecto (texto o {{fecha_actual}}):",
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 8.25f, FontStyle.Bold),
                ForeColor = Color.FromArgb(71, 85, 105)
            };

            _applySysVarBtn = new Button
            {
                Dock = DockStyle.Right,
                Width = 124,
                Text = "+ Usar en defecto",
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(248, 250, 252),
                ForeColor = Color.FromArgb(30, 64, 175),
                Font = new Font("Segoe UI", 8f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            _applySysVarBtn.FlatAppearance.BorderColor = Color.FromArgb(191, 219, 254);
            _applySysVarBtn.Click += (s, e) => ApplySelectedSystemVarToDefault();

            _sysVarCombo = new ComboBox
            {
                Dock = DockStyle.Right,
                Width = 195,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 8.25f)
            };
            _sysVarCombo.Items.AddRange(new object[]
            {
                "{{fecha_actual}} — Fecha YYYY-MM-DD",
                "{{fecha_ddmmyyyy}} — Fecha DD/MM/YYYY",
                "{{fecha_hora}} — Fecha y hora",
                "{{hora_actual}} — Hora HH:mm:ss",
                "{{anio}} — Año (YYYY)",
                "{{mes}} — Mes (MM)",
                "{{dia}} — Día (DD)",
                "{{usuario}} — Usuario Windows",
                "{{equipo}} — Equipo local",
                "{{uuid}} — GUID único"
            });
            _sysVarCombo.SelectedIndex = 0;

            var sysSpacer = new Panel { Dock = DockStyle.Right, Width = 6 };
            varsHeaderBar.Controls.Add(lblVars);
            varsHeaderBar.Controls.Add(_applySysVarBtn);
            varsHeaderBar.Controls.Add(sysSpacer);
            varsHeaderBar.Controls.Add(_sysVarCombo);

            _defaultsGrid = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                RowHeadersVisible = false,
                BackgroundColor = Color.FromArgb(248, 250, 252),
                BorderStyle = BorderStyle.FixedSingle,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.CellSelect
            };
            _defaultsGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "VarName",
                HeaderText = "Variable bind",
                ReadOnly = true,
                FillWeight = 40
            });
            _defaultsGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "DefaultVal",
                HeaderText = "Valor por defecto (texto o {{fecha_actual}})",
                FillWeight = 60
            });
            _defaultsGrid.CurrentCellDirtyStateChanged += (s, e) =>
            {
                if (_defaultsGrid.IsCurrentCellDirty)
                {
                    _defaultsGrid.CommitEdit(DataGridViewDataErrorContexts.Commit);
                }
            };
            _defaultsGrid.CellValueChanged += (s, e) =>
            {
                if (_suppressGridSync || e.RowIndex < 0 || e.ColumnIndex != 1) return;
                string key = Convert.ToString(_defaultsGrid.Rows[e.RowIndex].Cells[0].Tag);
                string val = Convert.ToString(_defaultsGrid.Rows[e.RowIndex].Cells[1].Value) ?? string.Empty;
                if (!string.IsNullOrEmpty(key)) _currentDefaults[key] = val;
            };

            defaultsPanel.Controls.Add(_defaultsGrid);
            defaultsPanel.Controls.Add(varsHeaderBar);

            _codeBox = new TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                AcceptsTab = true,
                AcceptsReturn = true,
                WordWrap = false,
                ScrollBars = ScrollBars.Both,
                Font = new Font("Consolas", 9.5f),
                BackColor = Color.FromArgb(248, 250, 252),
                BorderStyle = BorderStyle.FixedSingle
            };
            _codeBox.TextChanged += (s, e) => RefreshDetectedVariablesGrid();

            rightPanel.Controls.Add(_codeBox);
            rightPanel.Controls.Add(defaultsPanel);
            rightPanel.Controls.Add(footer);
            rightPanel.Controls.Add(varBar);
            rightPanel.Controls.Add(metaRow);

            Controls.Add(rightPanel);
            Controls.Add(leftPanel);
        }

        private void OnDrawTemplateItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0 || e.Index >= _templates.Count) return;
            CodeTemplate tpl = _templates[e.Index];
            bool selected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;

            using (var bg = new SolidBrush(selected ? Color.FromArgb(219, 234, 254) : Color.White))
                e.Graphics.FillRectangle(bg, e.Bounds);

            using (var div = new Pen(Color.FromArgb(241, 245, 249), 1))
                e.Graphics.DrawLine(div, e.Bounds.Left, e.Bounds.Bottom - 1, e.Bounds.Right, e.Bounds.Bottom - 1);

            string groupName = string.IsNullOrWhiteSpace(tpl.GroupName) ? "General" : tpl.GroupName;
            string lang = string.IsNullOrEmpty(tpl.Language) ? "TXT" : tpl.Language;

            var nameRect = new Rectangle(e.Bounds.X + 8, e.Bounds.Y + 4, Math.Max(60, e.Bounds.Width - 16), 19);
            TextRenderer.DrawText(
                e.Graphics,
                tpl.Name ?? "Sin nombre",
                new Font("Segoe UI", 9f, selected ? FontStyle.Bold : FontStyle.Regular),
                nameRect,
                selected ? Color.FromArgb(30, 64, 175) : Color.FromArgb(15, 23, 42),
                TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix | TextFormatFlags.VerticalCenter
            );

            var subRect = new Rectangle(e.Bounds.X + 8, e.Bounds.Y + 23, Math.Max(60, e.Bounds.Width - 16), 16);
            TextRenderer.DrawText(
                e.Graphics,
                groupName + "  ·  " + lang,
                new Font("Segoe UI", 7.75f),
                subRect,
                selected ? Color.FromArgb(37, 99, 235) : Color.FromArgb(100, 116, 139),
                TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix | TextFormatFlags.VerticalCenter
            );
        }

        private void RefreshGroupSelectors(string preferredFilter = null)
        {
            List<string> groups = _storage.GetTemplateGroups();
            string currentFilter = preferredFilter ?? (_groupFilterBox.SelectedItem != null ? _groupFilterBox.SelectedItem.ToString() : "Todos los grupos");
            string currentDraftGroup = _groupBox.Text;

            _suppressGroupFilterChange = true;
            try
            {
                _groupFilterBox.BeginUpdate();
                _groupFilterBox.Items.Clear();
                _groupFilterBox.Items.Add("Todos los grupos");
                foreach (string g in groups)
                {
                    _groupFilterBox.Items.Add(g);
                }
                int idx = _groupFilterBox.Items.IndexOf(currentFilter);
                _groupFilterBox.SelectedIndex = idx >= 0 ? idx : 0;
                _groupFilterBox.EndUpdate();
            }
            finally
            {
                _suppressGroupFilterChange = false;
            }

            _groupBox.BeginUpdate();
            _groupBox.Items.Clear();
            foreach (string g in groups)
            {
                _groupBox.Items.Add(g);
            }
            _groupBox.EndUpdate();
            if (!string.IsNullOrWhiteSpace(currentDraftGroup))
            {
                _groupBox.Text = currentDraftGroup;
            }
        }

        private void LoadTemplates(string selectId = null)
        {
            string targetId = selectId ?? _editingId;
            string groupFilter = _groupFilterBox != null && _groupFilterBox.SelectedIndex > 0 && _groupFilterBox.SelectedItem != null
                ? _groupFilterBox.SelectedItem.ToString()
                : string.Empty;
            _templates = _storage.GetTemplates(_searchBox != null ? _searchBox.Text : string.Empty, groupFilter);

            _suppressSelectionChange = true;
            try
            {
                _templateList.BeginUpdate();
                _templateList.Items.Clear();
                int foundIndex = -1;
                for (int i = 0; i < _templates.Count; i++)
                {
                    _templateList.Items.Add(_templates[i]);
                    if (!string.IsNullOrEmpty(targetId) && _templates[i].Id == targetId)
                        foundIndex = i;
                }
                _templateList.EndUpdate();

                if (foundIndex >= 0)
                {
                    _templateList.SelectedIndex = foundIndex;
                    PopulateEditor(_templates[foundIndex]);
                }
                else if (_templates.Count > 0 && string.IsNullOrEmpty(_editingId))
                {
                    _templateList.SelectedIndex = 0;
                    PopulateEditor(_templates[0]);
                }
                else if (_templates.Count == 0 && string.IsNullOrEmpty(_editingId))
                {
                    StartNewTemplate();
                }
            }
            finally
            {
                _suppressSelectionChange = false;
            }
        }

        private void PopulateEditor(CodeTemplate tpl)
        {
            _editingId = tpl.Id;
            _createdAt = tpl.CreatedAt;
            _nameBox.Text = tpl.Name ?? string.Empty;
            _groupBox.Text = string.IsNullOrWhiteSpace(tpl.GroupName) ? "General" : tpl.GroupName;

            int langIdx = _langBox.Items.IndexOf(tpl.Language ?? "Texto");
            _langBox.SelectedIndex = langIdx >= 0 ? langIdx : _langBox.Items.Count - 1;

            _currentDefaults = SqliteStorageService.ParseDefaults(tpl.DefaultsJson);
            _codeBox.Text = tpl.Content ?? string.Empty;
            RefreshDetectedVariablesGrid();
            _statusLabel.Text = "Editando plantilla guardada.";
            _statusLabel.ForeColor = Color.FromArgb(100, 116, 139);
        }

        private void StartNewTemplate()
        {
            _suppressSelectionChange = true;
            _templateList.ClearSelected();
            _suppressSelectionChange = false;

            _editingId = null;
            _createdAt = null;
            _nameBox.Text = string.Empty;
            _groupBox.Text = (_groupFilterBox != null && _groupFilterBox.SelectedIndex > 0 && _groupFilterBox.SelectedItem != null)
                ? _groupFilterBox.SelectedItem.ToString()
                : "General";
            _langBox.SelectedIndex = 0;
            _currentDefaults = new Dictionary<string, string>(StringComparer.Ordinal);
            _codeBox.Text = string.Empty;
            RefreshDetectedVariablesGrid();
            _statusLabel.Text = "Creando nueva plantilla...";
            _statusLabel.ForeColor = Color.FromArgb(37, 99, 235);
            _nameBox.Focus();
        }

        private void InsertBindVariableAtCursor()
        {
            string raw = (_varNameBox.Text ?? string.Empty).Trim();
            if (string.IsNullOrEmpty(raw)) raw = "variable";

            var cleanChars = new System.Text.StringBuilder();
            foreach (char ch in raw)
            {
                if (char.IsLetterOrDigit(ch) || ch == '_' || ch == '.' || ch == '-')
                    cleanChars.Append(ch);
                else if (char.IsWhiteSpace(ch))
                    cleanChars.Append('_');
            }
            string varName = cleanChars.Length > 0 ? cleanChars.ToString() : "variable";
            string token = "{{" + varName + "}}";

            int selStart = _codeBox.SelectionStart;
            int selLen = _codeBox.SelectionLength;
            string current = _codeBox.Text ?? string.Empty;
            _codeBox.Text = current.Substring(0, selStart) + token + current.Substring(selStart + selLen);
            _codeBox.SelectionStart = selStart + token.Length;
            _codeBox.SelectionLength = 0;
            _codeBox.Focus();
        }

        private void RefreshDetectedVariablesGrid()
        {
            var probe = new CodeTemplate { Content = _codeBox.Text ?? string.Empty };
            List<string> vars = probe.ExtractVariables();

            _suppressGridSync = true;
            try
            {
                _defaultsGrid.Rows.Clear();
                foreach (string v in vars)
                {
                    string defVal;
                    if (!_currentDefaults.TryGetValue(v, out defVal)) defVal = string.Empty;
                    int rowIdx = _defaultsGrid.Rows.Add("{{" + v + "}}", defVal);
                    _defaultsGrid.Rows[rowIdx].Cells[0].Tag = v;
                }
            }
            finally
            {
                _suppressGridSync = false;
            }
        }

        private CodeTemplate BuildDraftTemplate()
        {
            _defaultsGrid.EndEdit();
            var defaultsToSave = new Dictionary<string, string>(StringComparer.Ordinal);
            foreach (DataGridViewRow row in _defaultsGrid.Rows)
            {
                string key = Convert.ToString(row.Cells[0].Tag);
                string val = Convert.ToString(row.Cells[1].Value) ?? string.Empty;
                if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(val))
                {
                    defaultsToSave[key] = val;
                }
            }

            string groupName = (_groupBox.Text ?? string.Empty).Trim();
            if (string.IsNullOrEmpty(groupName)) groupName = "General";

            return new CodeTemplate
            {
                Id = _editingId,
                Name = (_nameBox.Text ?? string.Empty).Trim(),
                GroupName = groupName,
                Language = _langBox.SelectedItem != null ? _langBox.SelectedItem.ToString() : "Texto",
                Content = _codeBox.Text ?? string.Empty,
                DefaultsJson = SqliteStorageService.SerializeDefaults(defaultsToSave),
                CreatedAt = _createdAt
            };
        }

        private void ApplySelectedSystemVarToDefault()
        {
            _defaultsGrid.EndEdit();
            if (_defaultsGrid.Rows.Count == 0 || _sysVarCombo.SelectedItem == null)
            {
                _statusLabel.Text = "Agrega primero una variable {{nombre}} en el código.";
                _statusLabel.ForeColor = Color.FromArgb(180, 83, 9);
                return;
            }

            string selectedText = _sysVarCombo.SelectedItem.ToString();
            int sepIdx = selectedText.IndexOf(" —");
            string token = sepIdx > 0 ? selectedText.Substring(0, sepIdx).Trim() : selectedText.Trim();

            int rowIndex = _defaultsGrid.CurrentCell != null ? _defaultsGrid.CurrentCell.RowIndex : 0;
            if (rowIndex < 0 || rowIndex >= _defaultsGrid.Rows.Count) rowIndex = 0;

            string key = Convert.ToString(_defaultsGrid.Rows[rowIndex].Cells[0].Tag);
            string existing = Convert.ToString(_defaultsGrid.Rows[rowIndex].Cells[1].Value) ?? string.Empty;
            string nextVal = string.IsNullOrEmpty(existing) ? token : existing + token;

            _defaultsGrid.Rows[rowIndex].Cells[1].Value = nextVal;
            if (!string.IsNullOrEmpty(key)) _currentDefaults[key] = nextVal;
            _statusLabel.Text = $"Variable {token} asignada como defecto (actual: {CodeTemplate.ResolveDynamicDefault(nextVal)}).";
            _statusLabel.ForeColor = Color.FromArgb(37, 99, 235);
        }

        private bool SaveCurrentTemplate()
        {
            CodeTemplate draft = BuildDraftTemplate();
            if (string.IsNullOrWhiteSpace(draft.Name) || string.IsNullOrWhiteSpace(draft.Content))
            {
                _statusLabel.Text = "Ingresa un nombre y el contenido de código antes de guardar.";
                _statusLabel.ForeColor = Color.FromArgb(185, 28, 28);
                return false;
            }

            CodeTemplate saved = _storage.SaveTemplate(draft);
            _editingId = saved.Id;
            _createdAt = saved.CreatedAt;
            RefreshGroupSelectors();
            LoadTemplates(saved.Id);
            _statusLabel.Text = "Plantilla guardada en SQLite.";
            _statusLabel.ForeColor = Color.FromArgb(22, 163, 74);
            return true;
        }

        private void DeleteSelectedTemplate()
        {
            if (string.IsNullOrEmpty(_editingId))
            {
                StartNewTemplate();
                return;
            }

            _storage.DeleteTemplate(_editingId);
            _editingId = null;
            _createdAt = null;
            RefreshGroupSelectors();
            LoadTemplates();
            _statusLabel.Text = "Plantilla eliminada.";
            _statusLabel.ForeColor = Color.FromArgb(100, 116, 139);
        }

        private void UseCurrentTemplate()
        {
            CodeTemplate draft = BuildDraftTemplate();
            if (string.IsNullOrWhiteSpace(draft.Content))
            {
                _statusLabel.Text = "Escribe o selecciona una plantilla con código antes de usarla.";
                _statusLabel.ForeColor = Color.FromArgb(185, 28, 28);
                return;
            }
            if (string.IsNullOrWhiteSpace(draft.Name)) draft.Name = "Plantilla rápida";

            using (var useForm = new TemplateUseForm(draft))
            {
                if (useForm.ShowDialog(this) == DialogResult.OK && useForm.ShouldCloseParentAfterPaste)
                {
                    Close();
                }
            }
        }
    }
}
