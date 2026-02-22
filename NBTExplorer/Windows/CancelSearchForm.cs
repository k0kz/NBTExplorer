using NBTExplorer.Windows.Themes;
using System.Windows.Forms;

namespace NBTExplorer.Windows
{
    public partial class CancelSearchForm : DarkModeForm
    {
        public CancelSearchForm() : base()
        {
            InitializeComponent();
            DrawDarkMode();

        }

        public string SearchPathLabel
        {
            get { return _searchPathLabel.Text; }
            set { _searchPathLabel.Text = value; }
        }
    }
}
