using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
            e.TextColor = DarkModeRendererHelper.bDarkMode ? DarkMenuStripColorTable.TextWhite : default;
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

        public static bool bDarkMode { get => _bDarkMode; set => OnSetDarkMode(value); }
        private static bool _bDarkMode = false;
        private static void OnSetDarkMode(bool darkMode)
        {
            _bDarkMode = darkMode;
            foreach (DarkModeForm f in openedForms)
            {
                f.DrawDarkMode();
            }
        }

        public static List<Form> openedForms = new List<Form>();

        public static Image InvertIconColors(Bitmap bmp)
        {
            Bitmap invertedBmp = new Bitmap(bmp);
            for (int x = 0; x < invertedBmp.Width; x++)
            {
                for (int y = 0; y < invertedBmp.Height; y++)
                {
                    // pc - pixel color
                    Color pc = invertedBmp.GetPixel(x, y);
                    const int maxRgbVal = 256;
                    pc = Color.FromArgb(pc.A,
                        (~pc.R + maxRgbVal) % maxRgbVal,
                        (~pc.G + maxRgbVal) % maxRgbVal,
                        (~pc.B + maxRgbVal) % maxRgbVal);
                    invertedBmp.SetPixel(x, y, pc);

                }
            }
            return invertedBmp as Image;
        }

        public static Image InvertIconColors(Image im) => InvertIconColors(im as Bitmap);

    }

    public class DarkModeForm : Form
    {
        public DarkModeForm()
        {
            ControlAdded += OnControlAdded;
            DarkModeRendererHelper.openedForms.Add(this);
            this.FormClosed += 
                (object sender, FormClosedEventArgs args) =>
                { DarkModeRendererHelper.openedForms.Remove(this); };

            DrawDarkMode();
        }

        protected List<ToolStripButton> buttonstoSwapIconsOf;
        protected Dictionary<String, Image> lightModeIcons = new Dictionary<String, Image>();
        protected Dictionary<String, Image> darkModeIcons = new Dictionary<String, Image>();


        protected List<Control> themableControls = new List<Control>();

        protected virtual void OnControlAdded(object sender, ControlEventArgs e)
        {
            //if ((e.Control.Tag as String) == "")
            {
                themableControls.Add(e.Control);
            }
        }

        public virtual void ToggleDarkMode()
        {
            DarkModeRendererHelper.bDarkMode ^= true;
            if(darkModeIcons == null || darkModeIcons.Count == 0
                || lightModeIcons == null || lightModeIcons.Count == 0) GenerateDarkModeIcons();

            DrawDarkMode();
        }

        protected virtual void GenerateDarkModeIcons()
        {
            // No buttons to invert colors of, bail out early
            if (buttonstoSwapIconsOf == null || buttonstoSwapIconsOf.Count == 0) return;

            if(darkModeIcons == null) darkModeIcons = new Dictionary<String, Image>();
            if(lightModeIcons == null) lightModeIcons = new Dictionary<String, Image>();

            foreach (var button in buttonstoSwapIconsOf)
            {
                lightModeIcons[button.Name] = button.Image;

                String iconKey = button.Name;
                Image replacement = DarkModeRendererHelper.InvertIconColors(button.Image);
                darkModeIcons[iconKey] = replacement;
            }
        }

        public virtual void DrawDarkMode()
        {
            //if (darkModeIcons.Count == 0) GenerateDarkModeIcons();

            if (DarkModeRendererHelper.bDarkMode)
            {
                if ((this.Tag as String) != "ExcludedFromThemes")
                {
                    this.BackColor = DarkMenuStripColorTable.BackgroundDark;
                    this.ForeColor = DarkMenuStripColorTable.TextWhite;
                }

                if (darkModeIcons != null && darkModeIcons.Count > 0
                    && buttonstoSwapIconsOf != null)
                {
                    foreach (var button in buttonstoSwapIconsOf)
                    {
                        button.Image = darkModeIcons[button.Name];
                    }
                }

                ToolStripManager.Renderer = DarkModeRendererHelper.darkModeRenderer;

                // Despite assigning the renderer to manager,
                // this loop needs to stay or the UI colors go haywire
                // I could fix this but I am too frustrated to at this point
                // due to dealing with all of the .NET Framework 2.0's
                // unconvenices compared to modern .NET versions.
                //
                // To any poor soul reading this comment: please be fucking
                // glad new environemnts have these QOL improvements.
                foreach (Control c in themableControls)
                {
                    switch (c)
                    {
                        case ContextMenuStrip cms:
                            cms.Renderer = Themes.DarkModeRendererHelper.darkModeRenderer;
                            cms.BackColor = DarkMenuStripColorTable.BackgroundLight;
                            cms.ForeColor = DarkMenuStripColorTable.TextWhite;
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
                        case WatermarkTextBox wtxt:
                            wtxt.BackColor = Themes.DarkMenuStripColorTable.BackgroundLight;
                            wtxt.TextColor = Themes.DarkMenuStripColorTable.TextWhite;
                            wtxt.BorderStyle = BorderStyle.FixedSingle;
                            break;
                        case TextBox txt:
                            txt.BackColor = Themes.DarkMenuStripColorTable.BackgroundDark;
                            txt.ForeColor = Themes.DarkMenuStripColorTable.TextWhite;
                            txt.BorderStyle = BorderStyle.FixedSingle;
                            break;

                        default:
                            c.BackColor = Themes.DarkMenuStripColorTable.BackgroundDark;
                            c.ForeColor = Themes.DarkMenuStripColorTable.TextWhite;
                            break;
                    }
                }

                this.BackColor = DarkMenuStripColorTable.BackgroundDark;
                this.ForeColor = DarkMenuStripColorTable.TextWhite;
            }
            else
            {
                if (lightModeIcons != null && lightModeIcons.Count > 0
                    && buttonstoSwapIconsOf != null)
                {
                    foreach (var button in buttonstoSwapIconsOf)
                    {
                        button.Image = lightModeIcons[button.Name];
                    }
                }

                ToolStripManager.Renderer = default;

                foreach (Control c in themableControls)
                {
                    switch (c)
                    {
                        case ContextMenuStrip cms:
                            cms.Renderer = default;
                            break;
                        case ToolStrip ts:
                            ts.Renderer = default;
                            break;
                        case ToolStripPanel tsp:
                            tsp.Renderer = default;
                            break;
                        case ToolStripContentPanel tscp:
                            tscp.Renderer = default;
                            break;
                        case WatermarkTextBox wtxt:
                            wtxt.BackColor = DefaultBackColor;
                            wtxt.TextColor = DefaultForeColor;
                            break;
                        case TextBox txt:
                            // Contrast for text field
                            txt.BackColor = DefaultBackColor;
                            txt.ForeColor = DefaultForeColor;
                            txt.BorderStyle = BorderStyle.FixedSingle;
                            break;
                        default:
                            c.BackColor = DefaultBackColor;
                            c.ForeColor = DefaultForeColor;
                            break;
                    }
                }
                this.BackColor = default;
                this.ForeColor = default;
            }
        }
    }
}       