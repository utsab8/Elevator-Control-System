using System.Drawing;
using System.Windows.Forms;

namespace ElevatorControl
{
    partial class LogForm
    {
        private System.ComponentModel.IContainer components = null;
        private TextBox txtLog;
        private Button btnClose;
        private Button btnClear;
        private Button btnExport;
        private Label lblTitle;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            // Form
            this.Text = "Elevator Operation Log";
            this.Size = new Size(700, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.WhiteSmoke;

            // Title
            lblTitle = new Label();
            lblTitle.Text = "Elevator Historical Operation Log";
            lblTitle.Font = new Font("Arial", 14, FontStyle.Bold);
            lblTitle.Location = new Point(20, 20);
            lblTitle.Size = new Size(640, 30);
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            this.Controls.Add(lblTitle);

            // Log TextBox
            txtLog = new TextBox();
            txtLog.Location = new Point(20, 60);
            txtLog.Size = new Size(640, 330);
            txtLog.Multiline = true;
            txtLog.ScrollBars = ScrollBars.Vertical;
            txtLog.ReadOnly = true;
            txtLog.Font = new Font("Consolas", 9);
            txtLog.BackColor = Color.White;
            txtLog.BorderStyle = BorderStyle.Fixed3D;
            this.Controls.Add(txtLog);

            // Export Button
            btnExport = new Button();
            btnExport.Text = "Export to File";
            btnExport.Font = new Font("Arial", 10, FontStyle.Bold);
            btnExport.Location = new Point(20, 410);
            btnExport.Size = new Size(150, 40);
            btnExport.BackColor = Color.LightBlue;
            btnExport.Click += btnExport_Click;
            this.Controls.Add(btnExport);

            // Clear Button
            btnClear = new Button();
            btnClear.Text = "Clear Display";
            btnClear.Font = new Font("Arial", 10, FontStyle.Bold);
            btnClear.Location = new Point(270, 410);
            btnClear.Size = new Size(150, 40);
            btnClear.BackColor = Color.LightCoral;
            btnClear.Click += btnClear_Click;
            this.Controls.Add(btnClear);

            // Close Button
            btnClose = new Button();
            btnClose.Text = "Close";
            btnClose.Font = new Font("Arial", 10, FontStyle.Bold);
            btnClose.Location = new Point(510, 410);
            btnClose.Size = new Size(150, 40);
            btnClose.BackColor = Color.LightGreen;
            btnClose.Click += btnClose_Click;
            this.Controls.Add(btnClose);
        }
    }
}
