using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OSCVRCWiz.RJControls
{
    public partial class TranslucentPanel : Panel
    {
        public TranslucentPanel()
        {
            InitializeComponent();
            SetStyle(ControlStyles.SupportsTransparentBackColor |
             ControlStyles.OptimizedDoubleBuffer |
             ControlStyles.AllPaintingInWmPaint |
             ControlStyles.ResizeRedraw |
             ControlStyles.UserPaint, true);
            BackColor = Color.Transparent;
        }
    }
}
