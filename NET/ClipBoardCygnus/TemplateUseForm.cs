using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ClipBoardCygnus
{
    public class TemplateUseForm : Form
    {
        private readonly CodeTemplate _template;
        private readonly Dictionary<string, string> _defaults;
        private readonly Dictionary<string, TextBox> _inputs = new Dictionary<string, TextBox>(StringComparer.Ordinal);
        private TextBox _previewBox;
        private TextBox _firstInput;
        private TextBox _lastFocusedInput;

        public TemplateUseForm(CodeTemplate template, Icon appIcon = null)
        {
            _template = template ?? new CodeTemplate();
            _defaults = SqliteStorageService.ParseDefaults(_template.DefaultsJson);
            if (appIcon != null) Icon = appIcon;
            BuildUi();
            UpdatePreview();
        }

        private void BuildUi()
        {
            Text = "Usar plantilla — " + (_template.Name ?? "Código");
            FormBorderStyle = FormBorderStyle.Sizable;
            MinimumSize = new Size(560, 440);
            Size = new Size(640, 540);
            MaximizeBox = true;
            MinimizeBox = true;
            ShowInTaskbar = true;
            TopMost = false;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.White;
            Font = new Font("Segoe UI", 9.25f);

            Activated += (s, e) => ClipboardManager.TrackActiveOwnWindow(this);
            Shown += (s, e) =>
            {
                Activate();
                BringToFront();
                _firstInput?.Focus();
                _firstInput?.SelectAll();
            };

            var header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 46,
                Padding = new Padding(16, 10, 16, 6),
                BackColor = Color.FromArgb(248, 250, 252)
            };
            header.Paint += (s, e) =>
            {
                using (var pen = new Pen(Color.FromArgb(226, 232, 240), 1))
                    e.Graphics.DrawLine(pen, 0, header.Height - 1, header.Width, header.Height - 1);
            };

            var clipboardBtn = new Button
            {
                Dock = DockStyle.Right,
                Width = 108,
                Text = "Portapapeles",
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = Color.FromArgb(30, 64, 175),
                Font = new Font("Segoe UI", 8.25f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            clipboardBtn.FlatAppearance.BorderColor = Color.FromArgb(191, 219, 254);
            clipboardBtn.Click += (s, e) =>
            {
                var targetTb = _lastFocusedInput ?? _firstInput;
                if (targetTb != null && !targetTb.IsDisposed) targetTb.Focus();
                ClipboardManager.ShowPopupForTarget(this);
            };

            var lblTitle = new Label
            {
                Dock = DockStyle.Fill,
                Text = (_template.Name ?? "Plantilla") + "  [" + (string.IsNullOrWhiteSpace(_template.GroupName) ? "General" : _template.GroupName) + " · " + (_template.Language ?? "Texto") + "]",
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                TextAlign = ContentAlignment.MiddleLeft
            };
            header.Controls.Add(lblTitle);
            header.Controls.Add(clipboardBtn);

            var footer = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 52,
                Padding = new Padding(16, 10, 16, 10),
                BackColor = Color.FromArgb(248, 250, 252)
            };
            footer.Paint += (s, e) =>
            {
                using (var pen = new Pen(Color.FromArgb(226, 232, 240), 1))
                    e.Graphics.DrawLine(pen, 0, 0, footer.Width, 0);
            };

            var cancelBtn = new Button
            {
                Dock = DockStyle.Right,
                Width = 88,
                Text = "Cancelar",
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = Color.FromArgb(51, 65, 85),
                Cursor = Cursors.Hand
            };
            cancelBtn.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
            cancelBtn.Click += (s, e) =>
            {
                DialogResult = DialogResult.Cancel;
                Close();
            };

            var copyBtn = new Button
            {
                Dock = DockStyle.Right,
                Width = 128,
                Text = "Copiar código",
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = Color.FromArgb(15, 23, 42),
                Font = new Font("Segoe UI", 8.75f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            copyBtn.FlatAppearance.BorderColor = Color.FromArgb(148, 163, 184);
            copyBtn.Click += (s, e) => ApplyTemplate(false);

            var pasteBtn = new Button
            {
                Dock = DockStyle.Right,
                Width = 182,
                Text = "Copiar y pegar en app activa",
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(37, 99, 235),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 8.75f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            pasteBtn.FlatAppearance.BorderSize = 0;
            pasteBtn.Click += (s, e) => ApplyTemplate(true);

            var sp1 = new Panel { Dock = DockStyle.Right, Width = 8 };
            var sp2 = new Panel { Dock = DockStyle.Right, Width = 8 };

            footer.Controls.Add(pasteBtn);
            footer.Controls.Add(sp1);
            footer.Controls.Add(copyBtn);
            footer.Controls.Add(sp2);
            footer.Controls.Add(cancelBtn);

            // Preview section
            var previewContainer = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 190,
                Padding = new Padding(16, 6, 16, 10)
            };

            var lblPreview = new Label
            {
                Dock = DockStyle.Top,
                Height = 20,
                Text = "Vista previa generada:",
                Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(71, 85, 105)
            };

            _previewBox = new TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ReadOnly = true,
                WordWrap = false,
                ScrollBars = ScrollBars.Both,
                Font = new Font("Consolas", 9.25f),
                BackColor = Color.FromArgb(15, 23, 42),
                ForeColor = Color.FromArgb(241, 245, 249),
                BorderStyle = BorderStyle.FixedSingle
            };

            previewContainer.Controls.Add(_previewBox);
            previewContainer.Controls.Add(lblPreview);

            // Variables section
            var varsPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(16, 12, 16, 8)
            };

            List<string> vars = _template.ExtractVariables();
            if (vars.Count == 0)
            {
                var emptyLbl = new Label
                {
                    Dock = DockStyle.Top,
                    Height = 36,
                    Text = "Esta plantilla no tiene variables bind ({{variable}}). Puedes copiarla o pegarla directamente.",
                    ForeColor = Color.FromArgb(100, 116, 139)
                };
                varsPanel.Controls.Add(emptyLbl);
            }
            else
            {
                var table = new TableLayoutPanel
                {
                    Dock = DockStyle.Top,
                    AutoSize = true,
                    ColumnCount = 2
                };
                table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 180));
                table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

                for (int i = 0; i < vars.Count; i++)
                {
                    string varName = vars[i];
                    string initialVal = CodeTemplate.ResolveInitialValue(varName, _defaults);

                    var lbl = new Label
                    {
                        Text = "{{" + varName + "}}",
                        Font = new Font("Consolas", 9f, FontStyle.Bold),
                        ForeColor = Color.FromArgb(30, 64, 175),
                        TextAlign = ContentAlignment.MiddleLeft,
                        Dock = DockStyle.Fill,
                        Margin = new Padding(0, 4, 8, 4)
                    };

                    var tb = new TextBox
                    {
                        Text = initialVal,
                        Dock = DockStyle.Fill,
                        Font = new Font("Segoe UI", 9.25f),
                        Margin = new Padding(0, 4, 0, 4)
                    };
                    tb.TextChanged += (s, e) => UpdatePreview();
                    tb.Enter += (s, e) => _lastFocusedInput = tb;

                    if (_firstInput == null) _firstInput = tb;
                    _inputs[varName] = tb;

                    table.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                    table.Controls.Add(lbl, 0, i);
                    table.Controls.Add(tb, 1, i);
                }

                varsPanel.Controls.Add(table);
            }

            Controls.Add(varsPanel);
            Controls.Add(previewContainer);
            Controls.Add(footer);
            Controls.Add(header);
        }

        private string BuildRenderedCode()
        {
            var values = new Dictionary<string, string>(StringComparer.Ordinal);
            foreach (var kv in _inputs)
            {
                values[kv.Key] = kv.Value.Text ?? string.Empty;
            }
            return _template.Render(values, _defaults);
        }

        private void UpdatePreview()
        {
            _previewBox.Text = BuildRenderedCode();
        }

        private void ApplyTemplate(bool pasteIntoExternalApp)
        {
            string rendered = BuildRenderedCode();
            ClipboardManager.ApplyRenderedTemplate(rendered, _template.Name, pasteIntoExternalApp);
            Close();
        }
    }
}
