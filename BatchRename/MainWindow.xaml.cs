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

        private enum FileType
        {
            File,
            Folder
        }

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

            lvRunRules.ItemsSource = _runRules;
            lvFiles.ItemsSource = _files;
            lvFolders.ItemsSource = _folders;
        }

        #region project hanler
        private void btnOpenProject_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnSaveProject_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnSaveAsProject_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnCloseProject_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        #region start batch
        private void btnStartBatch_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        #region preset handler
        private void btnOpenPreset_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnSavePreset_Click(object sender, RoutedEventArgs e)
        {

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
                    IRenameRuleParser parser = RenameRuleParserFactory.Instance().GetRuleParser(runRule.Name);
                    IRenameRule rule = parser.Parse(runRule.Command);
                    newName = rule.Rename(newName);
                }
            }

            return newName;
        }

        private void EvokeToUpdateNewName()
        {
            foreach (var file in _files)
            {
                file.NewName = ImposeRule(file.Name);
            }

            foreach (var folder in _folders)
            {
                folder.NewName = ImposeRule(folder.Name);
            }
        }

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
        }

        private void btnRemoveRunRule_Click(object sender, RoutedEventArgs e)
        {
            if (lvRunRules.SelectedIndex != -1)
            {
                _runRules.RemoveAt(lvRunRules.SelectedIndex);

                UpdateOrder();

                EvokeToUpdateNewName();
            }
        }

        private void btnClearRunRule_Click(object sender, RoutedEventArgs e)
        {
            _runRules.Clear();

            EvokeToUpdateNewName();
        }

        private void btnEditRunRule_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnRemoveRunRuleItself_Click(object sender, RoutedEventArgs e)
        {
            var btnRemove = sender as System.Windows.Controls.Button;
            if(int.TryParse(btnRemove!.Tag.ToString(), out int tag))
            {
                _runRules.RemoveAt(tag);
            }

            UpdateOrder();

            EvokeToUpdateNewName();
        }
        #endregion

        #region file handler
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
                            NewName = ImposeRule(openFileDialog.SafeFileNames[i]),
                            Path = openFileDialog.FileNames[i]
                        });
                    }
                }
            }
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
                            NewName = ImposeRule(Path.GetFileName(file)),
                            Path = file
                        });
                    }
                }
            }
        }
        private void btnRemoveFile_Click(object sender, RoutedEventArgs e)
        {
            if (lvFiles.SelectedIndex != -1)
            {
                _files.RemoveAt(lvFiles.SelectedIndex);
            }
        }

        private void btnClearFiles_Click(object sender, RoutedEventArgs e)
        {
            _files.Clear();
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
                            NewName = ImposeRule(Path.GetFileName(file)),
                            Path = file
                        });
                    }
                }
            }
        }
        #endregion

        #region folder handler
        private void btnAddFolders_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnRemoveFolder_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnClearFolders_Click(object sender, RoutedEventArgs e)
        {

        }

        private void lvFolders_Drop(object sender, System.Windows.DragEventArgs e)
        {

        }
        #endregion
    }
}
