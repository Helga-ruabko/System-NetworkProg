namespace Lab3
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Windows.Forms;

    public partial class Form1 : Form
    {
        protected readonly PerformanceCounter ramCounter = new PerformanceCounter("Memory", "Available MBytes");
        private string logFilePath = "log.txt"; 
        private bool isWatching = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Список усіх логічниих дисків             
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            foreach (DriveInfo d in allDrives)
            {
                //Тип кожного диска
                string driveType = "";
                switch (d.DriveType)
                {
                    case DriveType.Fixed:
                        driveType = "Hard Disk Drive (HDD)";
                        break;
                    case DriveType.Removable:
                        driveType = "Removable Drive (USB)";
                        break;
                    case DriveType.CDRom:
                        driveType = "CD/DVD Drive";
                        break;
                    case DriveType.Network:
                        driveType = "Network Drive";
                        break;
                    case DriveType.Ram:
                        driveType = "RAM Disk";
                        break;
                    default:
                        driveType = "Unknown";
                        break;
                }
                //Диски та файлові системи
                listBox1.Items.Add($"Drive {d.Name} File type: {driveType}");
                if (d.IsReady)
                {
                    listBox1.Items.Add($"Volume label: {d.VolumeLabel}");
                    listBox1.Items.Add($"File system: {d.DriveFormat}");
                    listBox1.Items.Add($"Root directory: {d.RootDirectory}");
                    listBox1.Items.Add($"Available space to current user: {d.AvailableFreeSpace / 1024 / 1024 / 1024} Gbytes");
                    listBox1.Items.Add($"Total available space: {d.TotalFreeSpace / 1024 / 1024 / 1024} Gbytes");
                    listBox1.Items.Add($"Total size of drive: {d.TotalSize / 1024 / 1024 / 1024} Gbytes");
                    //Зайнятість та вільне місце
                    listBox1.Items.Add($"System memory: {ramCounter.NextValue()} Mbytes");
                    listBox1.Items.Add(" ");
                }
            }

            //Системна пам'ять
            listBox2.Items.Add($"System memory: {ramCounter.NextValue()} Mbytes");

            //Назва комп'ютера
            listBox2.Items.Add($"Machine name: {Environment.MachineName}");

            //Ім'я поточного користувача
            listBox2.Items.Add($"User name: {SystemInformation.UserName}");

            //Поточний системний каталог, Тимчасовий каталог
            string systemDirectory = Environment.SystemDirectory;
            string tempDirectory = Path.GetTempPath();
            string currentDirectory = Directory.GetCurrentDirectory();
            DirectoryInfo systemDirInfo = new DirectoryInfo(systemDirectory);
            DirectoryInfo tempDirInfo = new DirectoryInfo(tempDirectory);
            DirectoryInfo currentDirInfo = new DirectoryInfo(currentDirectory);
            listBox3.Items.Add($"System directory: {systemDirectory}");
            listBox3.Items.Add($"Name of catalog: {systemDirInfo.Name}");
            listBox3.Items.Add($"Full name of catalog: {systemDirInfo.FullName}");
            listBox3.Items.Add($"Creation time of catalog: {systemDirInfo.CreationTime}");
            listBox3.Items.Add($"Root of catalog: {systemDirInfo.Root}");
            listBox3.Items.Add(" ");
            listBox3.Items.Add($"Temporary directory: {tempDirectory}");
            listBox3.Items.Add($"Name of catalog: {tempDirInfo.Name}");
            listBox3.Items.Add($"Full name of catalog: {tempDirInfo.FullName}");
            listBox3.Items.Add($"Creation time of catalog: {tempDirInfo.CreationTime}");
            listBox3.Items.Add($"Root of catalog: {tempDirInfo.Root}");
            listBox3.Items.Add(" ");
            listBox3.Items.Add($"Current directory: {currentDirectory}");
            listBox3.Items.Add($"Name of catalog: {currentDirInfo.Name}");
            listBox3.Items.Add($"Full name of catalog: {currentDirInfo.FullName}");
            listBox3.Items.Add($"Creation time of catalog: {currentDirInfo.CreationTime}");
            listBox3.Items.Add($"Root of catalog: {currentDirInfo.Root}");

            //Спостереження за змінами
            if (!File.Exists(logFilePath))
            {
                try
                {
                    using (StreamWriter writer = File.CreateText(logFilePath))
                    {
                        writer.WriteLine($"Log file created at: {DateTime.Now}");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error creating log file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            StartWatchingDirectory(currentDirectory);
        }
        private void StartWatchingDirectory(string path)
        {
            if (!isWatching)
            {
                FileSystemWatcher watcher = new FileSystemWatcher();
                watcher.Path = path;
                watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
                watcher.Filter = "*.*";
                watcher.Changed += OnChanged;
                watcher.Created += OnChanged;
                watcher.Deleted += OnChanged;
                watcher.Renamed += OnRenamed;
                watcher.EnableRaisingEvents = true;
                isWatching = true;
            }
        }

        private Dictionary<string, bool> fileChangeRecord = new Dictionary<string, bool>();

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (!fileChangeRecord.ContainsKey(e.FullPath))
            {
                string logMessage = $"[{DateTime.Now}] {e.ChangeType} {e.FullPath}";
                WriteToLogFile(logMessage);
                fileChangeRecord[e.FullPath] = true;
            }
        }

        private void OnRenamed(object sender, RenamedEventArgs e)
        {

            string logMessage = $"[{DateTime.Now}] {e.ChangeType} {e.OldFullPath} -> {e.FullPath}";
            WriteToLogFile(logMessage);
        }

        private void WriteToLogFile(string message)
        {
            try
            {

                using (StreamWriter writer = File.AppendText(logFilePath))
                {
                    writer.WriteLine(message);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error writing to log file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
