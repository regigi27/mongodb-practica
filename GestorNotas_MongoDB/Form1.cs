using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NotasApp
{
    public partial class MainForm : Form
    {
        private readonly MongoDBServices _mongoDBServices;
        private List<Nota> _notas;
        private Nota _notaSeleccionada;

        // Controles principales
        private TableLayoutPanel mainLayout;
        private Panel editorPanel, listPanel, headerPanel;
        private FlowLayoutPanel notesFlowPanel;
        private TextBox txtTitulo, txtContenido, txtBuscar, txtTags;
        private Button btnNueva, btnGuardar, btnEliminar, btnBuscar;
        private Label lblTituloApp, lblContador;

        // Paleta de colores femenina y elegante
        private readonly Color backgroundColor = Color.FromArgb(250, 245, 250);      // Fondo principal lila muy claro
        private readonly Color panelColor = Color.FromArgb(255, 250, 255);           // Paneles secundarios blanco lila
        private readonly Color cardColor = Color.FromArgb(255, 255, 255);            // Tarjetas de notas blanco puro
        private readonly Color primaryColor = Color.FromArgb(219, 112, 147);         // Rosa polvoriento elegante
        private readonly Color accentColor = Color.FromArgb(199, 92, 127);           // Rosa más oscuro
        private readonly Color borderColor = Color.FromArgb(230, 220, 230);          // Bordes sutiles lila claro
        private readonly Color textColor = Color.FromArgb(80, 70, 90);               // Texto principal gris oscuro
        private readonly Color lightTextColor = Color.FromArgb(150, 140, 160);       // Texto secundario gris medio
        private readonly Color hoverColor = Color.FromArgb(245, 240, 245);           // Hover estado lila muy claro
        private readonly Color selectedColor = Color.FromArgb(255, 240, 245);        // Seleccionado rosa muy claro

        public MainForm()
        {
            InitializeComponent();
            _mongoDBServices = new MongoDBServices();
            _notas = new List<Nota>();
            _notaSeleccionada = null;
            InitializeUI();
            this.Load += async (s, e) => await CargarNotas();
            this.Resize += MainForm_Resize;
        }

        private void InitializeUI()
        {
            this.Text = "Notas";
            this.Size = new Size(1500, 900);
            this.MinimumSize = new Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = backgroundColor;
            this.Font = new Font("Segoe UI", 11);
            this.Padding = new Padding(20);

            // Layout principal
            mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = backgroundColor,
                ColumnCount = 1,
                RowCount = 2,
                RowStyles = {
                    new RowStyle(SizeType.Absolute, 80f),    // Header
                    new RowStyle(SizeType.Percent, 100f)     // Content
                },
                Padding = new Padding(0),
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None
            };
            this.Controls.Add(mainLayout);

            CreateHeader();
            CreateContentArea();
        }

        private void CreateHeader()
        {
            headerPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                Margin = new Padding(0, 0, 0, 20)
            };
            mainLayout.Controls.Add(headerPanel, 0, 0);

            // Título principal
            lblTituloApp = new Label
            {
                Text = "GESTOR DE NOTAS",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = primaryColor,
                Size = new Size(450, 50),
                Location = new Point(0, 15),
                TextAlign = ContentAlignment.MiddleLeft
            };
            headerPanel.Controls.Add(lblTituloApp);

            // Contador
            lblContador = new Label
            {
                Text = "0 notas",
                Font = new Font("Segoe UI", 12, FontStyle.Regular),
                ForeColor = lightTextColor,
                Size = new Size(150, 40),
                Location = new Point(headerPanel.Width - 160, 20),
                TextAlign = ContentAlignment.MiddleRight
            };
            lblContador.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            headerPanel.Controls.Add(lblContador);
        }

        private void CreateContentArea()
        {
            var contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent
            };
            mainLayout.Controls.Add(contentPanel, 0, 1);

            var contentLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                ColumnCount = 2,
                RowCount = 1,
                ColumnStyles = {
                    new ColumnStyle(SizeType.Percent, 40f),  // Editor
                    new ColumnStyle(SizeType.Percent, 60f)   // Lista
                },
                Margin = new Padding(0),
                Padding = new Padding(0)
            };
            contentPanel.Controls.Add(contentLayout);

            CreateEditorPanel(contentLayout);
            CreateListPanel(contentLayout);
        }

        private void CreateEditorPanel(TableLayoutPanel parent)
        {
            editorPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = panelColor,
                Margin = new Padding(0, 0, 15, 0),
                Padding = new Padding(30, 25, 30, 25)
            };
            parent.Controls.Add(editorPanel, 0, 0);

            // Borde elegante
            editorPanel.Paint += (s, e) =>
            {
                using (var pen = new Pen(borderColor, 2))
                {
                    e.Graphics.DrawRectangle(pen, 1, 1, editorPanel.Width - 3, editorPanel.Height - 3);
                }
            };

            int yPos = 0;
            int controlWidth = editorPanel.Width - 60;

            // Título del editor
            var editorTitle = new Label
            {
                Text = "EDITOR DE NOTA",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = primaryColor,
                Size = new Size(controlWidth, 40),
                Location = new Point(0, yPos)
            };
            editorPanel.Controls.Add(editorTitle);

            yPos += 50;

            // Campo título
            var lblTitulo = new Label
            {
                Text = "Título",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = textColor,
                Size = new Size(controlWidth, 25),
                Location = new Point(0, yPos)
            };
            editorPanel.Controls.Add(lblTitulo);

            yPos += 30;

            txtTitulo = new TextBox
            {
                PlaceholderText = "Escribe el título de la nota...",
                Size = new Size(controlWidth, 45),
                Location = new Point(0, yPos),
                Font = new Font("Segoe UI", 11),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                ForeColor = textColor
            };
            editorPanel.Controls.Add(txtTitulo);

            yPos += 60;

            // Campo etiquetas
            var lblTags = new Label
            {
                Text = "Etiquetas",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = textColor,
                Size = new Size(controlWidth, 25),
                Location = new Point(0, yPos)
            };
            editorPanel.Controls.Add(lblTags);

            yPos += 30;

            txtTags = new TextBox
            {
                PlaceholderText = "trabajo, personal, ideas... (separadas por comas)",
                Size = new Size(controlWidth, 45),
                Location = new Point(0, yPos),
                Font = new Font("Segoe UI", 11),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                ForeColor = textColor
            };
            editorPanel.Controls.Add(txtTags);

            yPos += 60;

            // Campo contenido
            var lblContenido = new Label
            {
                Text = "Contenido",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = textColor,
                Size = new Size(controlWidth, 25),
                Location = new Point(0, yPos)
            };
            editorPanel.Controls.Add(lblContenido);

            yPos += 30;

            txtContenido = new TextBox
            {
                PlaceholderText = "Escribe el contenido de tu nota aquí...",
                Size = new Size(controlWidth, 200),
                Location = new Point(0, yPos),
                Multiline = true,
                Font = new Font("Segoe UI", 11),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                ForeColor = textColor,
                ScrollBars = ScrollBars.Vertical
            };
            editorPanel.Controls.Add(txtContenido);

            yPos += 220;

            // Panel de botones - más espaciado
            var buttonsPanel = new Panel
            {
                Size = new Size(controlWidth, 55),
                Location = new Point(0, yPos),
                BackColor = Color.Transparent
            };
            editorPanel.Controls.Add(buttonsPanel);

            btnNueva = CreateModernButton("NUEVA", Color.Transparent, lightTextColor, new Point(0, 0));
            btnNueva.Size = new Size(100, 45);
            btnNueva.FlatAppearance.BorderColor = borderColor;

            btnGuardar = CreateModernButton("GUARDAR", primaryColor, Color.White, new Point(110, 0));
            btnGuardar.Size = new Size(120, 45);

            btnEliminar = CreateModernButton("ELIMINAR", Color.Transparent, Color.FromArgb(220, 120, 140), new Point(240, 0));
            btnEliminar.Size = new Size(100, 45);
            btnEliminar.FlatAppearance.BorderColor = Color.FromArgb(220, 120, 140);

            buttonsPanel.Controls.AddRange(new Control[] { btnNueva, btnGuardar, btnEliminar });

            // Eventos
            btnNueva.Click += (s, e) => LimpiarEditor();
            btnGuardar.Click += btnGuardar_Click;
            btnEliminar.Click += btnEliminar_Click;
        }

        private void CreateListPanel(TableLayoutPanel parent)
        {
            listPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                Padding = new Padding(0)
            };
            parent.Controls.Add(listPanel, 1, 0);

            // Panel de búsqueda
            var searchPanel = new Panel
            {
                Size = new Size(listPanel.Width, 100),
                Location = new Point(0, 0),
                BackColor = panelColor,
                Padding = new Padding(25, 25, 25, 15)
            };
            listPanel.Controls.Add(searchPanel);

            // Borde elegante
            searchPanel.Paint += (s, e) =>
            {
                using (var pen = new Pen(borderColor, 2))
                {
                    e.Graphics.DrawRectangle(pen, 1, 1, searchPanel.Width - 3, searchPanel.Height - 3);
                }
            };

            int searchWidth = listPanel.Width - 50;

            txtBuscar = new TextBox
            {
                PlaceholderText = "Buscar por título o etiquetas...",
                Size = new Size(searchWidth - 110, 50),
                Location = new Point(0, 0),
                Font = new Font("Segoe UI", 11),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                ForeColor = textColor
            };
            txtBuscar.KeyPress += (s, e) => { if (e.KeyChar == (char)Keys.Enter) BuscarNotas(); };
            searchPanel.Controls.Add(txtBuscar);

            btnBuscar = CreateModernButton("BUSCAR", primaryColor, Color.White, new Point(searchWidth - 100, 0));
            btnBuscar.Size = new Size(100, 50);
            btnBuscar.Click += (s, e) => BuscarNotas();
            searchPanel.Controls.Add(btnBuscar);

            // Panel de notas
            notesFlowPanel = new FlowLayoutPanel
            {
                AutoScroll = true,
                BackColor = Color.Transparent,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                Size = new Size(listPanel.Width, listPanel.Height - 110),
                Location = new Point(0, 110),
                AutoSize = false,
                Margin = new Padding(0),
                Padding = new Padding(10)
            };
            listPanel.Controls.Add(notesFlowPanel);
        }

        private Button CreateModernButton(string text, Color backColor, Color foreColor, Point location)
        {
            var button = new Button
            {
                Text = text,
                Size = new Size(95, 40),
                Location = location,
                BackColor = backColor,
                ForeColor = foreColor,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleCenter
            };

            button.FlatAppearance.BorderSize = 1;
            button.FlatAppearance.BorderColor = backColor != Color.Transparent ? backColor : borderColor;

            // Efectos hover
            button.MouseEnter += (s, e) =>
            {
                if (backColor == Color.Transparent)
                    button.BackColor = Color.FromArgb(245, 240, 245);
                else
                    button.BackColor = ControlPaint.Light(backColor, 0.3f);
            };

            button.MouseLeave += (s, e) => button.BackColor = backColor;
            button.MouseDown += (s, e) =>
            {
                if (backColor == Color.Transparent)
                    button.BackColor = Color.FromArgb(235, 230, 235);
                else
                    button.BackColor = ControlPaint.Dark(backColor, 0.2f);
            };

            return button;
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (listPanel != null && notesFlowPanel != null)
            {
                notesFlowPanel.Size = new Size(listPanel.Width, listPanel.Height - 110);

                if (listPanel.Controls.Count > 0 && listPanel.Controls[0] is Panel searchPanel)
                {
                    searchPanel.Size = new Size(listPanel.Width, 100);
                    if (searchPanel.Controls.Count > 1)
                    {
                        var btn = searchPanel.Controls[1] as Button;
                        var txt = searchPanel.Controls[0] as TextBox;
                        if (btn != null && txt != null)
                        {
                            int searchWidth = listPanel.Width - 50;
                            txt.Size = new Size(searchWidth - 110, 50);
                            btn.Location = new Point(searchWidth - 100, 0);
                        }
                    }
                }

                if (_notas?.Count > 0)
                {
                    MostrarNotas(_notas);
                }
            }

            if (lblContador != null && headerPanel != null)
            {
                lblContador.Location = new Point(headerPanel.Width - 160, 20);
            }

            // Ajustar controles del editor al redimensionar
            if (editorPanel != null)
            {
                int controlWidth = editorPanel.Width - 60;
                foreach (Control control in editorPanel.Controls)
                {
                    if (control is TextBox || control is Label)
                    {
                        control.Size = new Size(controlWidth, control.Height);
                    }
                    else if (control is Panel buttonsPanel && buttonsPanel.Controls.Count > 0)
                    {
                        buttonsPanel.Size = new Size(controlWidth, 55);
                    }
                }
            }
        }

        private async Task CargarNotas()
        {
            try
            {
                _notas = await _mongoDBServices.GetNotasAsync();
                MostrarNotas(_notas);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar notas: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MostrarNotas(List<Nota> notas)
        {
            notesFlowPanel.SuspendLayout();
            notesFlowPanel.Controls.Clear();

            if (!notas.Any())
            {
                var emptyLabel = new Label
                {
                    Text = "No hay notas disponibles\n\nHaz clic en 'NUEVA' para crear tu primera nota",
                    Size = new Size(500, 120),
                    Font = new Font("Segoe UI", 13),
                    ForeColor = lightTextColor,
                    TextAlign = ContentAlignment.MiddleCenter
                };
                notesFlowPanel.Controls.Add(emptyLabel);
                lblContador.Text = "0 notas";
                notesFlowPanel.ResumeLayout();
                return;
            }

            int cardWidth = CalculateCardWidth();

            foreach (var nota in notas.OrderByDescending(n => n.FechaCreacion))
            {
                var noteCard = CreateNoteCard(nota, cardWidth);
                notesFlowPanel.Controls.Add(noteCard);
            }

            lblContador.Text = $"{notas.Count} nota{(notas.Count != 1 ? "s" : "")}";
            notesFlowPanel.ResumeLayout();
        }

        private int CalculateCardWidth()
        {
            if (notesFlowPanel.Width <= 500)
                return notesFlowPanel.Width - 40;

            int availableWidth = notesFlowPanel.Width - 40;
            int minCardWidth = 350;
            int maxCardWidth = 400;

            int columns = Math.Max(1, availableWidth / minCardWidth);
            int cardWidth = availableWidth / columns;

            return Math.Min(Math.Max(cardWidth, minCardWidth), maxCardWidth);
        }

        private Panel CreateNoteCard(Nota nota, int width)
        {
            var card = new Panel
            {
                Size = new Size(width, 180),
                BackColor = cardColor,
                Margin = new Padding(12),
                Cursor = Cursors.Hand,
                Tag = nota,
                Padding = new Padding(25, 20, 25, 20)
            };

            // Borde elegante
            card.Paint += (s, e) =>
            {
                using (var pen = new Pen(borderColor, 1))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, card.Width - 1, card.Height - 1);
                }
            };

            int contentWidth = width - 50;

            // Título
            var lblTitle = new Label
            {
                Text = TruncateText(nota.Titulo, 40),
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = primaryColor,
                Size = new Size(contentWidth, 30),
                Location = new Point(0, 0),
                Cursor = Cursors.Hand
            };
            card.Controls.Add(lblTitle);

            // Contenido
            var lblContent = new Label
            {
                Text = TruncateText(nota.Contenido, 120),
                Font = new Font("Segoe UI", 10),
                ForeColor = textColor,
                Size = new Size(contentWidth, 70),
                Location = new Point(0, 35),
                TextAlign = ContentAlignment.TopLeft,
                Cursor = Cursors.Hand
            };
            card.Controls.Add(lblContent);

            // Panel inferior para metadata
            var metaPanel = new Panel
            {
                Size = new Size(contentWidth, 40),
                Location = new Point(0, 110),
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };
            card.Controls.Add(metaPanel);

            // Fecha
            var lblDate = new Label
            {
                Text = nota.FechaCreacion.ToString("dd/MM/yyyy • HH:mm"),
                Font = new Font("Segoe UI", 9),
                ForeColor = lightTextColor,
                Size = new Size(180, 20),
                Location = new Point(0, 10),
                TextAlign = ContentAlignment.MiddleLeft,
                Cursor = Cursors.Hand
            };
            metaPanel.Controls.Add(lblDate);

            // Tags
            if (nota.Tags != null && nota.Tags.Any())
            {
                var tagsPanel = new FlowLayoutPanel
                {
                    Size = new Size(contentWidth - 190, 35),
                    Location = new Point(185, 0),
                    BackColor = Color.Transparent,
                    FlowDirection = FlowDirection.RightToLeft,
                    WrapContents = false,
                    AutoSize = false,
                    Cursor = Cursors.Hand
                };
                metaPanel.Controls.Add(tagsPanel);

                foreach (var tag in nota.Tags.Take(3))
                {
                    if (!string.IsNullOrWhiteSpace(tag))
                    {
                        var tagLabel = CreateTagLabel(tag.Trim());
                        tagsPanel.Controls.Add(tagLabel);
                    }
                }
            }

            // Eventos de clic y hover
            EventHandler clickHandler = (s, e) => SeleccionarNota(nota);
            MouseEventHandler enterHandler = (s, e) =>
            {
                if (_notaSeleccionada == null || _notaSeleccionada.Id != nota.Id)
                    card.BackColor = hoverColor;
            };
            MouseEventHandler leaveHandler = (s, e) =>
            {
                if (_notaSeleccionada == null || _notaSeleccionada.Id != nota.Id)
                    card.BackColor = cardColor;
            };

            // Agregar eventos a la card principal
            card.Click += clickHandler;

            // Agregar eventos a todos los controles de manera segura
            AddEventsToControlAndChildren(card, clickHandler, enterHandler, leaveHandler);

            return card;
        }

        private void AddEventsToControlAndChildren(Control parent, EventHandler click, MouseEventHandler enter, MouseEventHandler leave)
        {
            if (parent == null) return;

            foreach (Control control in parent.Controls)
            {
                if (control != null)
                {
                    control.Click += click;
                    control.Cursor = Cursors.Hand;

                    // Si es un contenedor, procesar sus hijos
                    if (control.HasChildren)
                    {
                        AddEventsToControlAndChildren(control, click, enter, leave);
                    }
                }
            }
        }

        private Label CreateTagLabel(string tag)
        {
            return new Label
            {
                Text = tag,
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = primaryColor,
                Size = new Size(60, 22),
                TextAlign = ContentAlignment.MiddleCenter,
                Margin = new Padding(3, 0, 0, 0),
                Padding = new Padding(5, 0, 5, 0),
                Cursor = Cursors.Hand
            };
        }

        private string TruncateText(string text, int maxLength)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            return text.Length > maxLength ? text.Substring(0, maxLength) + "..." : text;
        }

        private void SeleccionarNota(Nota nota)
        {
            _notaSeleccionada = nota;
            txtTitulo.Text = nota.Titulo;
            txtContenido.Text = nota.Contenido;
            txtTags.Text = nota.Tags != null ? string.Join(", ", nota.Tags) : "";

            // Resaltar nota seleccionada
            foreach (Control control in notesFlowPanel.Controls)
            {
                if (control is Panel card && card.Tag is Nota cardNote)
                {
                    card.BackColor = cardNote.Id == nota.Id ? selectedColor : cardColor;
                }
            }
        }

        private void LimpiarEditor()
        {
            _notaSeleccionada = null;
            txtTitulo.Clear();
            txtContenido.Clear();
            txtTags.Clear();

            foreach (Control control in notesFlowPanel.Controls)
            {
                if (control is Panel card)
                    card.BackColor = cardColor;
            }

            txtTitulo.Focus();
        }

        private void BuscarNotas()
        {
            var searchText = txtBuscar.Text.Trim();
            if (string.IsNullOrEmpty(searchText))
            {
                MostrarNotas(_notas);
                return;
            }

            var filteredNotes = _notas.Where(n =>
                n.Titulo.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                n.Contenido.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                (n.Tags != null && n.Tags.Any(t =>
                    t.Contains(searchText, StringComparison.OrdinalIgnoreCase)))
            ).ToList();

            MostrarNotas(filteredNotes);
        }

        private async void btnGuardar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitulo.Text))
            {
                MessageBox.Show("El título es obligatorio", "Advertencia",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTitulo.Focus();
                return;
            }

            try
            {
                var tags = txtTags.Text.Split(',')
                    .Select(t => t.Trim())
                    .Where(t => !string.IsNullOrEmpty(t))
                    .ToList();

                var nota = new Nota
                {
                    Titulo = txtTitulo.Text.Trim(),
                    Contenido = txtContenido.Text.Trim(),
                    Tags = tags
                };

                if (_notaSeleccionada == null)
                {
                    await _mongoDBServices.CreateNotaAsync(nota);
                    MessageBox.Show("Nota creada exitosamente", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    nota.Id = _notaSeleccionada.Id;
                    nota.FechaCreacion = _notaSeleccionada.FechaCreacion;
                    await _mongoDBServices.UpdateNotaAsync(_notaSeleccionada.Id, nota);
                    MessageBox.Show("Nota actualizada exitosamente", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                LimpiarEditor();
                await CargarNotas();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar la nota: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnEliminar_Click(object sender, EventArgs e)
        {
            if (_notaSeleccionada == null)
            {
                MessageBox.Show("Selecciona una nota para eliminar", "Advertencia",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show($"¿Estás seguro de eliminar la nota: \"{_notaSeleccionada.Titulo}\"?",
                "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    await _mongoDBServices.DeleteNotaAsync(_notaSeleccionada.Id);
                    MessageBox.Show("Nota eliminada", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimpiarEditor();
                    await CargarNotas();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al eliminar nota: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ClientSize = new System.Drawing.Size(1500, 900);
            this.Name = "MainForm";
            this.ResumeLayout(false);
        }
    }
}