using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ElevatorControl
{
    public partial class LogForm : Form
    {
        private IReadOnlyList<string>? logEntries;
        private DatabaseManager dbManager;

        public LogForm()
        {
            this.dbManager = DatabaseManager.Instance;
            InitializeComponent();
            LoadLogs();
        }

        public LogForm(IReadOnlyList<string> logs)
        {
            this.logEntries = logs;
            this.dbManager = DatabaseManager.Instance;
            InitializeComponent();
            LoadLogs();
        }

        public void AddLogEntry(string logMessage)
        {
            if (txtLog != null)
            {
                if (txtLog.InvokeRequired)
                {
                    txtLog.Invoke(new Action(() => AddLogEntry(logMessage)));
                    return;
                }
                
                txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {logMessage}{Environment.NewLine}");
                txtLog.SelectionStart = txtLog.Text.Length;
                txtLog.ScrollToCaret();
            }
        }

        private void LoadLogs()
        {
            txtLog.Clear();
            
            // Load logs from database for persistent storage
            var dbLogs = dbManager.GetAllLogs();
            
            if (dbLogs.Count > 0)
            {
                txtLog.AppendText("=== DATABASE LOGS (Persistent) ===" + Environment.NewLine + Environment.NewLine);
                foreach (var entry in dbLogs)
                {
                    txtLog.AppendText(entry.ToString() + Environment.NewLine);
                }
            }
            else
            {
                txtLog.AppendText("No logs found in database." + Environment.NewLine);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "Are you sure you want to clear all logs?\n(This will permanently delete all database records)",
                "Clear All Logs",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                dbManager.ClearLogs();
                txtLog.Clear();
                MessageBox.Show("All logs have been cleared successfully.", "Logs Cleared", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveDialog = new SaveFileDialog())
            {
                saveDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
                saveDialog.DefaultExt = "txt";
                saveDialog.FileName = $"ElevatorLog_{DateTime.Now:yyyyMMdd_HHmmss}.txt";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // Export logs from database
                        var dbLogs = dbManager.GetAllLogs();
                        var exportLines = new List<string>();
                        exportLines.Add("=== ELEVATOR OPERATION LOG ===");
                        exportLines.Add($"Exported: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                        exportLines.Add("");
                        
                        foreach (var log in dbLogs)
                        {
                            exportLines.Add(log.ToString());
                        }
                        
                        System.IO.File.WriteAllLines(saveDialog.FileName, exportLines);
                        MessageBox.Show("Log exported successfully!", "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error exporting log: {ex.Message}", "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}
