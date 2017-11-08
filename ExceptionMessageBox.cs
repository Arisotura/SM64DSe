using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SM64DSe
{
    public partial class ExceptionMessageBox : Form
    {
        private static readonly string DEFAULT_TITLE = "Error!";

        public ExceptionMessageBox()
        {
            InitializeComponent();
        }

        public ExceptionMessageBox(string title, Exception e) 
            : this()
        {
            this.Text = title;
            this.txtMessage.Text = e.Message;
            this.txtStackTrace.Text = e.StackTrace;
        }

        public ExceptionMessageBox(Exception e)
            : this(DEFAULT_TITLE, e) { }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
