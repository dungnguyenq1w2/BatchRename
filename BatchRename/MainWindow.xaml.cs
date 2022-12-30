using Contract;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Forms;
using System.Linq;
using System.Diagnostics;
using MessageBox = System.Windows.Forms.MessageBox;
using System.Data;
using AddEndCounterRule;
using System.Windows.Threading;
using System.Windows.Controls.Primitives;
//using System.Text.Json;
//using System.Windows.Shapes;
using Newtonsoft.Json;
using System.Net.Http.Json;

namespace BatchRename
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        private BindingList<RunRule> _runRules = new BindingList<RunRule>();

        private BindingList<File> _files = new BindingList<File>();

        private BindingList<File> _folders = new BindingList<File>();

        private WorkingCondition _workingCondition = new WorkingCondition();

        private enum FileType
        {
            File,
            Folder
        }

        //private DispatcherTimer? _dispatcherTimer;

        //private const int AUTOSAVE_INTERVAL_SECONDS = 10;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void winMain_Loaded(object sender, RoutedEventArgs e)
        {
            RenameRuleParserFactory.Instance().Register();
            BaseWindowFactory.Instance().Register();

            var items = RenameRuleParserFactory.Instance().RuleParserPrototypes;

            foreach (var item in items)
            {
                var rule = item.Value;

                var button = new System.Windows.Controls.Button()
                {
                    Margin = new Thickness(0, 0, 5, 0),
                    Padding = new Thickness(5, 3, 5, 3),
                    BorderThickness = new Thickness(0),
                    Background = new SolidColorBrush(Colors.Transparent),
                    Content = rule.Title,
                    Tag = rule.Name
                };

                button.Click += btnAddRunRule_Click;
                wpRuleChooser.Children.Add(button);
            }

            // Load last preset, files, folders, window state to _workingCondition
            LoadLastWorkingCondition();

            // Apply working condition
            this.Top = _workingCondition.Top;
            this.Left = _workingCondition.Left;
            this.Height = _workingCondition.Height;
            this.Width = _workingCondition.Width;
            if (_workingCondition.Maximized)
            {
                this.WindowState = WindowState.Minimized;
            }

            // Apply renaming rules for files & folders after loaded
            EvokeToUpdateNewName();

            //
            lvRunRules.ItemsSource = _runRules;
            lvFiles.ItemsSource = _files;
            lvFolders.ItemsSource = _folders;

            // Start auto-save after each interval
            //_dispatcherTimer = new DispatcherTimer();
            //_dispatcherTimer.Interval = TimeSpan.FromMilliseconds(AUTOSAVE_INTERVAL_SECONDS * 1000);
            //_dispatcherTimer.Tick += PeriodicAutoSave;
            //_dispatcherTimer.Start();
        }

        private void PeriodicAutoSave(object? o, EventArgs e)
        {
            SaveWorkingCondition();
            //Debug.WriteLine("----- AUTO-SAVED !!! -----");
        }

        #region project handler
        private bool ReadProjectFile(string fileName, bool isOnlyPreset)
        {
            try
            {
                using (StreamReader reader = new StreamReader(fileName))
                {
                    if (!isOnlyPreset)
                    {
                        _files.Clear();
                        _folders.Clear();
                    }

                    _runRules.Clear();

                    string state = "";
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();

                        if (line == "Rules" || line == "Files" || line == "Folders")
                        {
                            state = line;
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(line))
                            {
                                if (state == "Rules")
                                {
                                    int firstSpaceIndex = line.IndexOf(" ");
                                    string firstWord = (firstSpaceIndex > 0) ? line.Substring(0, firstSpaceIndex) : line;

                                    IRenameRuleParser parser = RenameRuleParserFactory.Instance().GetRuleParser(firstWord);
                                    IRenameRule rule = parser.Parse(line);

                                    _runRules.Add(new RunRule()
                                    {
                                        Index = _runRules.Count,
                                        Name = firstWord,
                                        Title = parser.Title,
                                        IsPlugAndPlay = parser.IsPlugAndPlay,
                                        Command = line
                                    });
                                }
                                else if (state == "Files")
                                {
                                    _files.Add(new File()
                                    {
                                        Name = Path.GetFileName(line),
                                        NewName = "",
                                        Path = line
                                    });
                                }
                                else
                                {
                                    _folders.Add(new File()
                                    {
                                        Name = Path.GetFileName(line),
                                        NewName = "",
                                        Path = line
                                    });
                                }
                            }
                        }
                    }

                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void SaveProjectFile(string projectName)
        {
            string writer = "";

            writer += CreateWriterFromRunRules();
            writer += CreateWriterFromTargets((int)FileType.File);
            writer += CreateWriterFromTargets((int)FileType.Folder);

            System.IO.File.WriteAllText(projectName, writer);
        }

        private void SaveProjectHandler()
        {
            string projectName = Title.Split(new string[] { "Batch rename - " }, StringSplitOptions.None)[1];

            SaveProjectFile(projectName);
        }

        private void SaveAsProjectHandler()
        {
            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            saveFileDialog.Filter = "Project file (*.prj)|*.prj";

            if (saveFileDialog.ShowDialog() == true)
            {
                string projectName = saveFileDialog.FileName;

                SaveProjectFile(projectName);

                Title = $"Batch rename - {projectName}";
            }
        }

        private void btnOpenProject_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
            {
                if (ReadProjectFile(openFileDialog.FileName, false) == true)
                {
                    Title = $"Batch rename - {openFileDialog.FileName}";

                    EvokeToUpdateNewName();
                }
            }
        }

        private void btnSaveProject_Click(object sender, RoutedEventArgs e)
        {
            if (Title == "Batch rename")
                SaveAsProjectHandler();
            else
                SaveProjectHandler();
        }

        private void btnSaveAsProject_Click(object sender, RoutedEventArgs e)
        {
            SaveAsProjectHandler();
        }

        private void btnCloseProject_Click(object sender, RoutedEventArgs e)
        {
            _runRules.Clear();
            _files.Clear();
            _folders.Clear();
            lblPresetName.Content = "";
            Title = "Batch rename";
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            SaveWorkingCondition();
            Close();
        }
        #endregion

        #region start batch
        private void btnStartBatch_Click(object sender, RoutedEventArgs e)
        {
            int type = tcTargets.SelectedIndex;

            MessageBoxResult msg = System.Windows.MessageBox.Show(
                "Are you sure you want to make the renaming?",
                "Start renaming",
                MessageBoxButton.YesNo
            );

            if (msg == MessageBoxResult.Yes)
            {
                if (type == (int)FileType.File)
                {
                    Dictionary<string, int> duplications = new Dictionary<string, int>();

                    foreach (var file in _files)
                    {
                        string newIdealName = Path.Combine(Path.GetDirectoryName(file.Path)!, file.NewName);

                        try
                        {
                            System.IO.File.Move(
                                file.Path,
                                newIdealName
                            );
                        }
                        catch (Exception)
                        {
                            if (duplications.ContainsKey(newIdealName))
                            {
                                duplications[newIdealName]++;
                            }
                            else
                            {
                                duplications[newIdealName] = 1;
                            }

                            string newLessCollisionName = $"{Path.GetFileNameWithoutExtension(file.NewName)} ({duplications[newIdealName]}){Path.GetExtension(file.NewName)}";

                            System.IO.File.Move(
                                file.Path,
                                Path.Combine(Path.GetDirectoryName(file.Path)!, newLessCollisionName)
                            );
                        }
                    }

                    _files.Clear();

                    if (Title != "Batch rename")
                    {
                        SaveProjectHandler();
                    }
                }
                else
                {
                    Dictionary<string, int> duplications = new Dictionary<string, int>();

                    foreach (var folder in _folders)
                    {
                        string newIdealName = Path.Combine(Path.GetDirectoryName(folder.Path)!, folder.NewName);

                        try
                        {
                            Directory.Move(folder.Path, newIdealName);
                        }
                        catch (Exception)
                        {
                            if (duplications.ContainsKey(newIdealName))
                            {
                                duplications[newIdealName]++;
                            }
                            else
                            {
                                duplications[newIdealName] = 1;
                            }

                            string newLessCollisionName = $"{Path.GetFileNameWithoutExtension(folder.NewName)} ({duplications[newIdealName]})";

                            Directory.Move(
                                folder.Path,
                                Path.Combine(Path.GetDirectoryName(folder.Path)!, newLessCollisionName)
                            );
                        }
                    }

                    _folders.Clear();
                    if (Title != "Batch rename")
                    {
                        SaveProjectHandler();
                    }
                }
            }

            SaveWorkingCondition();
        }

        private void MoveAllFilesToCopyFolder(string srcPath, string desPath)
        {
            if (Directory.Exists(srcPath) && Directory.Exists(desPath))
            {
                foreach (string dirPath in Directory.GetDirectories(srcPath, "*", SearchOption.AllDirectories))
                {
                    Directory.CreateDirectory(dirPath.Replace(srcPath, desPath));
                }

                foreach (string filePath in Directory.GetFiles(srcPath, "*.*", SearchOption.AllDirectories))
                {
                    System.IO.File.Copy(filePath, filePath.Replace(srcPath, desPath), true);
                }
            }
        }

        private bool IsEmptyFolder(string folderPath)
        {
            bool result = !Directory.EnumerateFileSystemEntries(folderPath).Any();
            return result;
        }

        private void btnStartBatchCopy_Click(object sender, RoutedEventArgs e)
        {
            int type = tcTargets.SelectedIndex;
            MessageBoxResult msg = System.Windows.MessageBox.Show(
                "Are you sure you want to make the renaming?",
                "Start renaming",
                MessageBoxButton.YesNo
            );

            if (msg == MessageBoxResult.Yes)
            {
                FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();

                if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string directory = folderBrowserDialog.SelectedPath;
                    if (!IsEmptyFolder(directory))
                    {
                        MessageBox.Show("Destination must be an empty folder");
                        return;
                    }
                    if (type == (int)FileType.File)
                    {
                        Dictionary<string, int> duplicationsCopyfile = new Dictionary<string, int>();

                        foreach (var file in _files)
                        {
                            string newPath = Path.Combine(directory, file.Name);

                            if (!System.IO.File.Exists(newPath))
                            {
                                System.IO.File.Copy(
                                    file.Path,
                                    newPath
                                );
                            }
                            else
                            {
                                if (duplicationsCopyfile.ContainsKey(newPath))
                                {
                                    duplicationsCopyfile[newPath]++;
                                }
                                else
                                {
                                    duplicationsCopyfile[newPath] = 1;
                                }

                                string copyNameWithDuplicate = $"{Path.GetFileNameWithoutExtension(file.Name)} ({duplicationsCopyfile[newPath]}) {Path.GetExtension(file.Name)}";

                                newPath = Path.Combine(directory, copyNameWithDuplicate);

                                System.IO.File.Copy(
                                    file.Path,
                                    newPath
                                );
                            }

                            string newIdealName = Path.Combine(directory, file.NewName);
                            Dictionary<string, int> duplicationsNewFileName = new Dictionary<string, int>();

                            try
                            {
                                System.IO.File.Move(
                                    newPath,
                                    newIdealName
                                );
                            }
                            catch (Exception)
                            {
                                if (duplicationsNewFileName.ContainsKey(newIdealName))
                                {
                                    duplicationsNewFileName[newIdealName]++;
                                }
                                else
                                {
                                    duplicationsNewFileName[newIdealName] = 1;
                                }

                                string newLessCollisionName = $"{Path.GetFileNameWithoutExtension(file.NewName)} ({duplicationsNewFileName[newIdealName]}){Path.GetExtension(file.NewName)}";

                                System.IO.File.Move(
                                    newPath,
                                    Path.Combine(Path.GetDirectoryName(newPath)!, newLessCollisionName)
                                );
                            }
                        }

                        _files.Clear();

                        if (Title != "Batch rename")
                        {
                            SaveProjectHandler();
                        }
                    }
                    else
                    {
                        Dictionary<string, int> duplicationsCopyFolder = new Dictionary<string, int>();

                        foreach (var folder in _folders)
                        {
                            string newPath = Path.Combine(directory, folder.Name);

                            if (!Directory.Exists(newPath))
                            {
                                Directory.CreateDirectory(newPath);
                                MoveAllFilesToCopyFolder(folder.Path, newPath);
                            }
                            else
                            {
                                if (duplicationsCopyFolder.ContainsKey(newPath))
                                {
                                    duplicationsCopyFolder[newPath]++;
                                }
                                else
                                {
                                    duplicationsCopyFolder[newPath] = 1;
                                }
                                string copyNameWithDuplicate = $"{Path.GetFileNameWithoutExtension(folder.Name)} ({duplicationsCopyFolder[newPath]})";
                                newPath = Path.Combine(directory, copyNameWithDuplicate);

                                Directory.CreateDirectory(newPath);
                                MoveAllFilesToCopyFolder(folder.Path, newPath);
                            }

                            Dictionary<string, int> duplicationsNewFolderName = new Dictionary<string, int>();
                            string newIdealName = Path.Combine(directory, folder.NewName);

                            try
                            {
                                Directory.Move(newPath, newIdealName);
                            }
                            catch (Exception)
                            {
                                if (duplicationsNewFolderName.ContainsKey(newIdealName))
                                {
                                    duplicationsNewFolderName[newIdealName]++;
                                }
                                else
                                {
                                    duplicationsNewFolderName[newIdealName] = 1;
                                }

                                string newLessCollisionName = $"{Path.GetFileNameWithoutExtension(folder.NewName)} ({duplicationsNewFolderName[newIdealName]})";

                                Directory.Move(
                                    newPath,
                                    Path.Combine(directory, newLessCollisionName)
                                );
                            }
                        }

                        _folders.Clear();
                        if (Title != "Batch rename")
                        {
                            SaveProjectHandler();
                        }
                    }
                }
            }

            SaveWorkingCondition();
        }
        #endregion

        #region preset handler
        private void btnOpenPreset_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
            {
                if (ReadProjectFile(openFileDialog.FileName, true) == true)
                {
                    lblPresetName.Content = openFileDialog.SafeFileName;

                    EvokeToUpdateNewName();
                }
            }
        }

        private void btnSavePreset_Click(object sender, RoutedEventArgs e)
        {
            if (_runRules.Count > 0)
            {
                Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
                saveFileDialog.Filter = "Preset file (*.txt)|*.txt";

                if (saveFileDialog.ShowDialog() == true)
                {
                    string writer = CreateWriterFromRunRules();

                    System.IO.File.WriteAllText(saveFileDialog.FileName, writer);
                }
            }
            else
            {
                System.Windows.MessageBox.Show("You have to define at least one rule");
            }
        }
        #endregion

        #region rule handler

        #region rule helper
        private string ImposeRule(string original)
        {
            string newName = original;

            foreach (var runRule in _runRules)
            {
                if (!string.IsNullOrEmpty(runRule.Command))
                {
                    if (runRule.Command.Contains("AddEndCounter"))
                    {
                        //newName = ImposeAddEndCounterRule(newName, addEndCounterRule);
                        continue;
                    }

                    IRenameRuleParser parser = RenameRuleParserFactory.Instance().GetRuleParser(runRule.Name);
                    IRenameRule rule = parser.Parse(runRule.Command);
                    newName = rule.Rename(newName);
                }
            }

            return newName;
        }

        private void EvokeToUpdateNewName()
        {
            // Reset all "new name" value to "name" value
            foreach (var file in _files)
            {
                file.NewName = file.Name;
            }

            foreach (var folder in _folders)
            {
                folder.NewName = folder.Name;
            }

            // Each renaming rule will apply for all files & folders 
            foreach (var runRule in _runRules)
            {
                if (!string.IsNullOrEmpty(runRule.Command))
                {
                    IRenameRuleParser parser = RenameRuleParserFactory.Instance().GetRuleParser(runRule.Name);
                    IRenameRule rule = parser.Parse(runRule.Command);

                    foreach (var file in _files)
                    {
                        file.NewName = rule.Rename(file.NewName);
                    }

                    // AddEndCounterRule: Counter of folders start from "Start value input" too instead of continue increase Counter
                    if (runRule.Command.Contains("AddEndCounter"))
                    {
                        rule = parser.Parse(runRule.Command);
                    }

                    foreach (var folder in _folders)
                    {
                        folder.NewName = rule.Rename(folder.NewName);
                    }
                }
            }
        }

        //private void EvokeToUpdateNewName()
        //{
        //    foreach (var file in _files)
        //    {
        //        file.NewName = ImposeRule(file.Name);
        //    }

        //    foreach (var folder in _folders)
        //    {
        //        folder.NewName = ImposeRule(folder.Name);
        //    }

        //    EvokeToAddEndCounter();
        //}

        //// Add end counter to 'newName's; call after apply all other Rules
        //private void EvokeToAddEndCounter()
        //{
        //    foreach (var runRule in _runRules)
        //    {
        //        if (!string.IsNullOrEmpty(runRule.Command) && runRule.Command.Contains("AddEndCounter"))
        //        {
        //            IRenameRuleParser parser = RenameRuleParserFactory.Instance().GetRuleParser(runRule.Name);
        //            IRenameRule addEndCounterRule = parser.Parse(runRule.Command);

        //            foreach (var file in _files)
        //            {
        //                file.NewName = addEndCounterRule.Rename(file.NewName);
        //            }

        //            // Counter of folders start from "Start value input" too instead of continue increase Counter
        //            addEndCounterRule = parser.Parse(runRule.Command);

        //            foreach (var folder in _folders)
        //            {
        //                folder.NewName = addEndCounterRule.Rename(folder.NewName);
        //            }
        //        }
        //    }
        //}

        private void UpdateOrder()
        {
            for (int i = 0; i < _runRules.Count; i++)
                _runRules[i].Index = i;

            lvRunRules.ItemsSource = null;
            lvRunRules.ItemsSource = _runRules;
        }
        #endregion

        private void btnAddRunRule_Click(object sender, RoutedEventArgs e)
        {
            string selectedTagName = (string)((System.Windows.Controls.Button)sender).Tag;

            IRenameRuleParser parser = RenameRuleParserFactory.Instance().GetRuleParser(selectedTagName);

            _runRules.Add(new RunRule()
            {
                Index = _runRules.Count,
                Name = selectedTagName,
                Title = parser.Title,
                IsPlugAndPlay = parser.IsPlugAndPlay,
                Command = parser.IsPlugAndPlay ? selectedTagName : ""
            });

            EvokeToUpdateNewName();
            SaveWorkingCondition();
        }

        private void btnRemoveRunRule_Click(object sender, RoutedEventArgs e)
        {
            if (lvRunRules.SelectedIndex != -1)
            {
                _runRules.RemoveAt(lvRunRules.SelectedIndex);

                UpdateOrder();

                EvokeToUpdateNewName();
            }

            SaveWorkingCondition();
        }

        private void btnClearRunRule_Click(object sender, RoutedEventArgs e)
        {
            _runRules.Clear();

            EvokeToUpdateNewName();
            SaveWorkingCondition();
        }

        private void btnEditRunRule_Click(object sender, RoutedEventArgs e)
        {
            int index = int.Parse((sender as System.Windows.Controls.Button).Tag.ToString());
            RunRule rule = _runRules[index];

            if (!rule.IsPlugAndPlay)
            {
                var window = BaseWindowFactory.Instance().GetBaseWindow(rule.Name).CreateInstance();
                window.Command = rule.Command;

                if (window.ShowDialog() == true)
                {
                    _runRules[index].Command = window.Command;
                    EvokeToUpdateNewName();
                }
            }

            SaveWorkingCondition();
        }

        private void btnRemoveRunRuleItself_Click(object sender, RoutedEventArgs e)
        {
            var btnRemove = sender as System.Windows.Controls.Button;
            if (int.TryParse(btnRemove!.Tag.ToString(), out int tag))
            {
                _runRules.RemoveAt(tag);
            }

            UpdateOrder();

            EvokeToUpdateNewName();
            SaveWorkingCondition();
        }

        void LoadLastWorkingCondition()
        {
            //_runRules.Clear();
            //_files.Clear();
            //_folders.Clear();

            try
            {
                string exeFolderPath = AppDomain.CurrentDomain.BaseDirectory;
                if (System.IO.File.Exists($"{exeFolderPath}\\Autosave\\Autosave.json") == false)
                {
                    return;
                }

                string jsonString = System.IO.File.ReadAllText($"{exeFolderPath}\\Autosave\\Autosave.json");
                _workingCondition = JsonConvert.DeserializeObject<WorkingCondition>(jsonString);

                if (_workingCondition != null)
                {
                    foreach (string ruleCommand in _workingCondition.Preset)
                    {
                        string ruleName = ruleCommand.Split(" ")[0];
                        IRenameRuleParser parser = RenameRuleParserFactory.Instance().GetRuleParser(ruleName);
                        IRenameRule rule = parser.Parse(ruleCommand);

                        _runRules.Add(new RunRule()
                        {
                            Index = _runRules.Count,
                            //Index = i,
                            Name = ruleName,
                            Title = parser.Title,
                            IsPlugAndPlay = parser.IsPlugAndPlay,
                            Command = ruleCommand
                        });
                    }

                    foreach (string filePath in _workingCondition.ActiveFiles)
                    {
                        string fileName = Path.GetFileName(filePath);

                        _files.Add(new File()
                        {
                            Name = fileName,
                            //NewName= "",
                            Path = filePath,
                        });
                    }

                    foreach (string folderPath in _workingCondition.ActiveFolders)
                    {
                        string folderName = Path.GetFileName(folderPath);

                        _folders.Add(new File()
                        {
                            Name = folderName,
                            //NewName= "",
                            Path = folderPath,
                        });
                    }
                }
            }
            catch (Exception)
            {
                _workingCondition = new WorkingCondition();
            }
        }
        void loadLastPreset(string preset)
        {
            //_runRules.Clear();

            var rules = preset.Split("\n", StringSplitOptions.None);
            for (int i = 1; i < rules.Length - 1; i++)
            {
                int firstSpaceIndex = rules[i].IndexOf(" ");
                string firstWord = (firstSpaceIndex > 0) ? rules[i].Substring(0, firstSpaceIndex) : rules[i];

                IRenameRuleParser parser = RenameRuleParserFactory.Instance().GetRuleParser(firstWord);
                IRenameRule rule = parser.Parse(rules[i]);

                _runRules.Add(new RunRule()
                {
                    Index = _runRules.Count,
                    //Index = i,
                    Name = firstWord,
                    Title = parser.Title,
                    IsPlugAndPlay = parser.IsPlugAndPlay,
                    Command = rules[i]
                });
            }
        }

        #endregion

        #region file handler
        #region file helper
        private bool IsAdded(string path, int type)
        {
            BindingList<File> browser;
            browser = (type == (int)FileType.File) ? _files : _folders;
            foreach (var item in browser)
            {
                if (item.Path == path)
                {
                    return true;
                }
            }
            return false;
        }
        private string CreateWriterFromRunRules()
        {
            string writer = "Rules\n";

            for (int i = 0; i < _runRules.Count; i++)
            {
                if (!string.IsNullOrEmpty(_runRules[i].Command))
                {
                    writer += _runRules[i].Command + "\n";
                }
            }

            return writer;
        }

        private string CreateWriterFromTargets(int type)
        {
            BindingList<File> targets;
            string writer = "";

            if (type == (int)FileType.File)
            {
                writer = "Files\n";
                targets = _files;
            }
            else
            {
                writer = "Folders\n";
                targets = _folders;
            }

            for (int i = 0; i < targets.Count; i++)
            {
                if (!string.IsNullOrEmpty(targets[i].Path))
                {
                    writer += targets[i].Path + "\n";
                }
            }

            return writer;
        }

        #endregion
        private void btnAddFiles_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Multiselect = true;

            if (openFileDialog.ShowDialog() == true)
            {
                for (int i = 0; i < openFileDialog.FileNames.Length; i++)
                {
                    if (!IsAdded(openFileDialog.FileNames[i], (int)FileType.File))
                    {
                        _files.Add(new File()
                        {
                            Name = openFileDialog.SafeFileNames[i],
                            //NewName = ImposeRule(openFileDialog.SafeFileNames[i]),
                            Path = openFileDialog.FileNames[i]
                        });
                    }
                }
            }

            //
            EvokeToUpdateNewName();
            SaveWorkingCondition();
        }

        private void btnAddFilesInDirectory_Click(object sender, RoutedEventArgs e)
        {
            var folderBrowserDialog = new FolderBrowserDialog();

            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string directory = folderBrowserDialog.SelectedPath;

                var files = Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories);

                foreach (var file in files)
                {
                    if (!IsAdded(file, (int)FileType.File))
                    {
                        _files.Add(new File()
                        {
                            Name = Path.GetFileName(file),
                            //NewName = ImposeRule(Path.GetFileName(file)),
                            Path = file
                        });
                    }
                }
            }

            //
            EvokeToUpdateNewName();
            SaveWorkingCondition();
        }
        private void btnRemoveFile_Click(object sender, RoutedEventArgs e)
        {
            if (lvFiles.SelectedIndex != -1)
            {
                _files.RemoveAt(lvFiles.SelectedIndex);
                EvokeToUpdateNewName();
                SaveWorkingCondition();
            }
        }

        private void btnClearFiles_Click(object sender, RoutedEventArgs e)
        {
            _files.Clear();
            //EvokeToUpdateNewName();
            SaveWorkingCondition();
        }

        private void lvFiles_Drop(object sender, System.Windows.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop);

                foreach (var file in files)
                {
                    if (!IsAdded(file, (int)FileType.File))
                    {
                        _files.Add(new File()
                        {
                            Name = Path.GetFileName(file),
                            //NewName = ImposeRule(Path.GetFileName(file)),
                            Path = file
                        });
                    }
                }
            }

            //
            EvokeToUpdateNewName();
            SaveWorkingCondition();
        }
        #endregion

        #region folder handler
        #region folder helper

        #endregion
        private void btnAddFolders_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();

            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string directory = folderBrowserDialog.SelectedPath;

                if (!IsAdded(directory, (int)FileType.Folder))
                {
                    _folders.Add(new File()
                    {
                        Name = Path.GetFileName(directory),
                        //NewName = ImposeRule(Path.GetFileName(directory)),
                        Path = directory
                    });
                }
            }

            //
            EvokeToUpdateNewName();
            SaveWorkingCondition();
        }

        private void btnRemoveFolder_Click(object sender, RoutedEventArgs e)
        {
            if (lvFolders.SelectedIndex != -1)
            {
                _folders.RemoveAt(lvFolders.SelectedIndex);
                EvokeToUpdateNewName();
                SaveWorkingCondition();
            }
        }

        private void btnClearFolders_Click(object sender, RoutedEventArgs e)
        {
            _folders.Clear();
            //EvokeToUpdateNewName();
            SaveWorkingCondition();
        }

        private void lvFolders_Drop(object sender, System.Windows.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop);

                foreach (var file in files)
                {
                    if (Directory.Exists(file))
                    {
                        if (!IsAdded(file, (int)FileType.Folder))
                        {
                            _folders.Add(new File()
                            {
                                Name = Path.GetFileName(file),
                                //NewName = ImposeRule(Path.GetFileName(file)),
                                Path = file
                            });
                        }
                    }
                }
            }

            //
            EvokeToUpdateNewName();
            SaveWorkingCondition();
        }
        #endregion


        private void lvRunRules_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            SaveWorkingCondition();

            // 
            //if (_dispatcherTimer != null)
            //{
            //    _dispatcherTimer.Stop();
            //    _dispatcherTimer = null;
            //}
        }

        //private void SaveWindowState()
        //{
        //    // Save current size & position of screen
        //    if (WindowState == WindowState.Maximized)
        //    {
        //        _workingCondition.Top = RestoreBounds.Top;
        //        _workingCondition.Left = RestoreBounds.Left;
        //        _workingCondition.Height = RestoreBounds.Height;
        //        _workingCondition.Width = RestoreBounds.Width;
        //        _workingCondition.Maximized = true;
        //    }
        //    else
        //    {
        //        _workingCondition.Top = this.Top;
        //        _workingCondition.Left = this.Left;
        //        _workingCondition.Height = this.Height;
        //        _workingCondition.Width = this.Width;
        //        _workingCondition.Maximized = false;
        //    }
        //}

        private void SaveWorkingCondition()
        {
            // Reset all working condition
            _workingCondition.Reset();

            // Save current size & position of screen
            if (WindowState == WindowState.Maximized)
            {
                _workingCondition.Top = RestoreBounds.Top;
                _workingCondition.Left = RestoreBounds.Left;
                _workingCondition.Height = RestoreBounds.Height;
                _workingCondition.Width = RestoreBounds.Width;
                _workingCondition.Maximized = true;
            }
            else
            {
                _workingCondition.Top = this.Top;
                _workingCondition.Left = this.Left;
                _workingCondition.Height = this.Height;
                _workingCondition.Width = this.Width;
                _workingCondition.Maximized = false;
            }

            // Add preset
            foreach (var runRule in _runRules)
            {
                if (!string.IsNullOrEmpty(runRule.Command))
                {
                    _workingCondition.Preset.Add(runRule.Command);
                }
            }

            // Add active files
            foreach (var file in _files)
            {
                _workingCondition.ActiveFiles.Add(file.Path);
            }

            // Add active folders
            foreach (var folder in _folders)
            {
                _workingCondition.ActiveFolders.Add(folder.Path);
            }

            // Save all working condition to JSON file (location in [exe_folder_path]/Autosave/autosave.json)
            try
            {
                string jsonString = JsonConvert.SerializeObject(_workingCondition, Formatting.Indented);

                var exeFolderPath = AppDomain.CurrentDomain.BaseDirectory;
                if (System.IO.Directory.Exists($"{exeFolderPath}\\Autosave") == false)
                {
                    System.IO.Directory.CreateDirectory($"{exeFolderPath}\\Autosave");
                }

                System.IO.File.WriteAllText($"{exeFolderPath}\\Autosave\\Autosave.json", jsonString);
            }
            catch (Exception)
            {
                return;
            }
        }
    }
}
