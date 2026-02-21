using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace NBTExplorer.Windows.Themes
{
    public class DarkMenuStripColorTable : ProfessionalColorTable
    {
        // Main palette (Visual Studio Dark Theme style)
        public static Color BackgroundDark = Color.FromArgb(30, 30, 30);
        public static Color BackgroundLight = Color.FromArgb(45, 45, 48);
        public static Color SelectionGray = Color.FromArgb(62, 62, 64);
        public static Color AccentBlue = Color.FromArgb(0, 122, 204);

        // Additional colors for standard controls
        public static Color TextWhite = Color.FromArgb(241, 241, 241);
        public static Color TextDisabled = Color.FromArgb(153, 153, 153);
        public static Color BorderDark = Color.FromArgb(67, 67, 70);

        // Menu bar (upper)
        public override Color MenuStripGradientBegin => BackgroundDark;
        public override Color MenuStripGradientEnd => BackgroundDark;

        // Effects when hovering mouse over menu button
        public override Color MenuItemSelected => SelectionGray;
        public override Color MenuItemSelectedGradientBegin => SelectionGray;
        public override Color MenuItemSelectedGradientEnd => SelectionGray;

        // Effects used immeddiately after pressing a button
        public override Color MenuItemPressedGradientBegin => SelectionGray;
        public override Color MenuItemPressedGradientEnd => SelectionGray;

        // Dropdown lists
        public override Color ToolStripDropDownBackground => BackgroundLight;
        public override Color ImageMarginGradientBegin => BackgroundLight;
        public override Color ImageMarginGradientMiddle => BackgroundLight;
        public override Color ImageMarginGradientEnd => BackgroundLight;
        public override Color MenuBorder => BorderDark;
        public override Color MenuItemBorder => SelectionGray;

        // Toolbar (Toolbar with icons)
        public override Color ToolStripGradientBegin => BackgroundDark;
        public override Color ToolStripGradientMiddle => BackgroundDark;
        public override Color ToolStripGradientEnd => BackgroundDark;
        public override Color ToolStripBorder => BackgroundDark;

    }

    public class DarkModeRenderer : ToolStripProfessionalRenderer
    {
        public DarkModeRenderer(ProfessionalColorTable table) : base(table) { }

        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            // Wymuszamy nasz kolor tuż przed faktycznym rysowaniem
            e.TextColor = DarkMenuStripColorTable.TextWhite;
            base.OnRenderItemText(e);
        }
    }

    public class DarkModeRendererHelper
    {
        //public DarkModeRenderer(ProfessionalColorTable table) : base(table) { }

        public static DarkMenuStripColorTable darkMenuStripColorTable = new DarkMenuStripColorTable();

        public static DarkModeRenderer darkModeRenderer { get; private set; } = new DarkModeRenderer(darkMenuStripColorTable);

        public static void OnRenderItemText(object sender, ToolStripItemTextRenderEventArgs e)
        {
            // Wymuszamy nasz kolor tuż przed faktycznym rysowaniem
            e.TextColor = DarkMenuStripColorTable.TextWhite;
            TextRenderer.DrawText(
                e.Graphics,
                e.Text,
                e.TextFont,
                e.TextRectangle,
                DarkMenuStripColorTable.TextWhite,
                e.TextFormat);
            e.TextColor = Color.Transparent;
        }

        public static bool darkMode { get; set; } = false;

    }

    public class DarkModeForm : Form
    {
        public DarkModeForm()
        {
            ControlAdded += OnControlAdded;
            DrawDarkMode();
        }


        protected List<Control> themableControls = new List<Control>();

        protected virtual void OnControlAdded(object sender, ControlEventArgs e)
        {
            //if ((e.Control.Tag as String) == "")
            {
                themableControls.Add(e.Control);
            }
        }

        protected void ToggleDarkMode()
        {
            DarkModeRendererHelper.darkMode = !DarkModeRendererHelper.darkMode;
            DrawDarkMode();
        }

        protected void DrawDarkMode()
        {

            if (DarkModeRendererHelper.darkMode)
            {
                if ((this.Tag as String) != "ExcludedFromThemes")
                {
                    this.BackColor = DarkMenuStripColorTable.BackgroundDark;
                    this.ForeColor = DarkMenuStripColorTable.TextWhite;
                }

                foreach (Control c in themableControls)
                {
                    switch (c)
                    {
                        case ContextMenuStrip cms:
                            cms.Renderer = Themes.DarkModeRendererHelper.darkModeRenderer;
                            break;
                        case ToolStrip ts:
                            ts.Renderer = Themes.DarkModeRendererHelper.darkModeRenderer;
                            break;
                        case ToolStripPanel tsp:
                            tsp.Renderer = Themes.DarkModeRendererHelper.darkModeRenderer;
                            break;
                        case ToolStripContentPanel tscp:
                            tscp.Renderer = Themes.DarkModeRendererHelper.darkModeRenderer;
                            break;
                        case Button btn:
                            // Wymuszenie ciemnego, płaskiego stylu dla przycisków
                            btn.UseVisualStyleBackColor = false;
                            btn.FlatStyle = FlatStyle.Flat;
                            btn.BackColor = Themes.DarkMenuStripColorTable.BackgroundLight;
                            btn.ForeColor = Themes.DarkMenuStripColorTable.TextWhite;
                            btn.FlatAppearance.BorderColor = Themes.DarkMenuStripColorTable.BorderDark;
                            break;
                        case TextBox txt:
                            // Odpowiedni kontrast dla pola tekstowego
                            txt.BackColor = Themes.DarkMenuStripColorTable.BackgroundLight;
                            txt.ForeColor = Themes.DarkMenuStripColorTable.TextWhite;
                            txt.BorderStyle = BorderStyle.FixedSingle;
                            break;
                        default:
                            c.BackColor = Themes.DarkMenuStripColorTable.BackgroundDark;
                            c.ForeColor = Themes.DarkMenuStripColorTable.TextWhite;
                            break;
                    }
                }
            }
            else
            {
                foreach (Control c in themableControls)
                {
                    switch (c)
                    {
                        case ContextMenuStrip cms:
                            cms.Renderer = new ToolStripProfessionalRenderer();
                            break;
                        case ToolStrip ts:
                            ts.Renderer = new ToolStripProfessionalRenderer();
                            break;
                        case ToolStripPanel tsp:
                            tsp.Renderer = new ToolStripProfessionalRenderer();
                            break;
                        case ToolStripContentPanel tscp:
                            tscp.Renderer = new ToolStripProfessionalRenderer();
                            break;
                        default:
                            c.BackColor = Control.DefaultBackColor;
                            c.ForeColor = Control.DefaultForeColor;
                            break;
                    }
                }

            }
        }
    }
}