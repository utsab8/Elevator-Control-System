using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Threading;

namespace ElevatorControl
{
    public class DatabaseManager
    {
        private string connectionString;
        private static DatabaseManager? instance;
        private static readonly object lockObj = new object();
        private Queue<LogEntry> logQueue = new Queue<LogEntry>();
        private BackgroundWorker dbWorker;
        
        // Disconnected model components
        private DataSet dataSet;
        private SQLiteDataAdapter dataAdapter;
        private DataTable logTable;

        private DatabaseManager()
        {
            // Use relative path for portability
            string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ElevatorLog.db");
            connectionString = $"Data Source={dbPath};Version=3;";
            InitializeDatabase();
            InitializeBackgroundWorker();
        }

        private void InitializeBackgroundWorker()
        {
            dbWorker = new BackgroundWorker();
            dbWorker.WorkerSupportsCancellation = true;
            dbWorker.DoWork += DbWorker_DoWork;
            dbWorker.RunWorkerCompleted += DbWorker_RunWorkerCompleted;
        }

        private void DbWorker_DoWork(object? sender, DoWorkEventArgs e)
        {
            // Process queued log entries in background thread
            while (logQueue.Count > 0)
            {
                LogEntry? entry = null;
                lock (lockObj)
                {
                    if (logQueue.Count > 0)
                    {
                        entry = logQueue.Dequeue();
                    }
                }

                if (entry != null)
                {
                    AddLogSync(entry.Message, entry.Floor, entry.State);
                }
            }
        }

        private void DbWorker_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                string errorLog = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "error.log");
                File.AppendAllText(errorLog, $"[{DateTime.Now}] Background Worker Error: {e.Error.Message}\n");
            }
        }

        public static DatabaseManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (lockObj)
                    {
                        if (instance == null)
                        {
                            instance = new DatabaseManager();
                        }
                    }
                }
                return instance;
            }
        }

        private void InitializeDatabase()
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string createTableQuery = @"
                    CREATE TABLE IF NOT EXISTS OperationLog (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Timestamp DATETIME NOT NULL,
                        Message TEXT NOT NULL,
                        Floor INTEGER,
                        State TEXT
                    )";
                
                using (var command = new SQLiteCommand(createTableQuery, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
            
            // Initialize disconnected model components
            InitializeDisconnectedModel();
        }

        private void InitializeDisconnectedModel()
        {
            try
            {
                // Create DataSet and DataTable for disconnected model
                dataSet = new DataSet();
                logTable = new DataTable("OperationLog");
                
                // Define table structure
                logTable.Columns.Add("Id", typeof(int));
                logTable.Columns.Add("Timestamp", typeof(DateTime));
                logTable.Columns.Add("Message", typeof(string));
                logTable.Columns.Add("Floor", typeof(int));
                logTable.Columns.Add("State", typeof(string));
                
                // Set primary key
                logTable.PrimaryKey = new DataColumn[] { logTable.Columns["Id"]! };
                logTable.Columns["Id"]!.AutoIncrement = true;
                logTable.Columns["Id"]!.AutoIncrementSeed = 1;
                logTable.Columns["Id"]!.AutoIncrementStep = 1;
                
                dataSet.Tables.Add(logTable);
                
                // Create DataAdapter with command builders
                string selectQuery = "SELECT * FROM OperationLog";
                dataAdapter = new SQLiteDataAdapter(selectQuery, connectionString);
                
                // Use CommandBuilder for automatic INSERT, UPDATE, DELETE commands
                var commandBuilder = new SQLiteCommandBuilder(dataAdapter);
                
                // Fill initial data
                dataAdapter.Fill(dataSet, "OperationLog");
            }
            catch (Exception ex)
            {
                string errorLog = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "error.log");
                File.AppendAllText(errorLog, $"[{DateTime.Now}] Disconnected Model Init Error: {ex.Message}\n");
            }
        }

        // Overload for simple logging
        public void AddLog(string message)
        {
            AddLog(message, 0, "INFO");
        }

        public void AddLog(string message, int floor, string state)
        {
            // Queue the log entry for async processing
            var entry = new LogEntry
            {
                Timestamp = DateTime.Now,
                Message = message,
                Floor = floor,
                State = state
            };

            lock (lockObj)
            {
                logQueue.Enqueue(entry);
            }

            // Start background worker if not already running
            if (!dbWorker.IsBusy)
            {
                dbWorker.RunWorkerAsync();
            }
        }

        private void AddLogSync(string message, int floor, string state)
        {
            // DISCONNECTED MODEL: Add to DataTable, then use DataAdapter.Update()
            try
            {
                lock (lockObj)
                {
                    // Add new row to DataTable (in-memory)
                    DataRow newRow = logTable.NewRow();
                    newRow["Timestamp"] = DateTime.Now;
                    newRow["Message"] = message;
                    newRow["Floor"] = floor;
                    newRow["State"] = state;
                    
                    logTable.Rows.Add(newRow);
                    
                    // Update database using DataAdapter.Update() - DISCONNECTED MODEL
                    // This uses the automatically generated INSERT command from CommandBuilder
                    dataAdapter.Update(dataSet, "OperationLog");
                    
                    // Accept changes to DataTable
                    dataSet.AcceptChanges();
                }
            }
            catch (Exception ex)
            {
                // Log error to file as fallback
                string errorLog = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "error.log");
                File.AppendAllText(errorLog, $"[{DateTime.Now}] Database Error (Disconnected Model): {ex.Message}\n");
            }
        }

        public List<LogEntry> GetAllLogs()
        {
            List<LogEntry> logs = new List<LogEntry>();
            
            try
            {
                lock (lockObj)
                {
                    // DISCONNECTED MODEL: Refresh DataTable from database first
                    dataSet.Clear();
                    dataAdapter.Fill(dataSet, "OperationLog");
                    
                    // Read from DataTable (in-memory) instead of direct database query
                    DataView view = new DataView(logTable);
                    view.Sort = "Timestamp DESC";
                    
                    foreach (DataRowView rowView in view)
                    {
                        DataRow row = rowView.Row;
                        logs.Add(new LogEntry
                        {
                            Id = row["Id"] != DBNull.Value ? Convert.ToInt32(row["Id"]) : 0,
                            Timestamp = row["Timestamp"] != DBNull.Value ? Convert.ToDateTime(row["Timestamp"]) : DateTime.Now,
                            Message = row["Message"] != DBNull.Value ? row["Message"].ToString() ?? "" : "",
                            Floor = row["Floor"] != DBNull.Value ? Convert.ToInt32(row["Floor"]) : 0,
                            State = row["State"] != DBNull.Value ? row["State"].ToString() ?? "" : ""
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                // Return empty list on error
                string errorLog = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "error.log");
                File.AppendAllText(errorLog, $"[{DateTime.Now}] Database Read Error (Disconnected Model): {ex.Message}\n");
            }
            
            return logs;
        }

        public void ClearLogs()
        {
            try
            {
                lock (lockObj)
                {
                    // DISCONNECTED MODEL: Clear DataTable and update database
                    foreach (DataRow row in logTable.Rows)
                    {
                        row.Delete();
                    }
                    
                    // Update database using DataAdapter.Update() - DISCONNECTED MODEL
                    dataAdapter.Update(dataSet, "OperationLog");
                    
                    // Accept changes
                    dataSet.AcceptChanges();
                }
            }
            catch (Exception ex)
            {
                string errorLog = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "error.log");
                File.AppendAllText(errorLog, $"[{DateTime.Now}] Database Clear Error (Disconnected Model): {ex.Message}\n");
            }
        }
    }

    public class LogEntry
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string Message { get; set; } = "";
        public int Floor { get; set; }
        public string State { get; set; } = "";

        public override string ToString()
        {
            return $"[{Timestamp:yyyy-MM-dd HH:mm:ss}] Floor {Floor} - {State}: {Message}";
        }
    }
}
