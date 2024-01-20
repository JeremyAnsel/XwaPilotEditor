using JeremyAnsel.Xwa.Pilot;
using JeremyAnsel.Xwa.Workspace;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace XwaPilotEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            SetWorkingDirectory();
            InitializeComponent();
            Pilot = new PilotFile();
            PilotFileName = null;
            RefreshPilot();
        }

        public string WorkingDirectory { get; set; }

        public XwaWorkspace Workspace { get; set; }

        public List<string> CraftNames { get; set; }

        public PilotFile Pilot { get; set; }

        public string PilotFileName { get; set; }

        public void RefreshPilot()
        {
            this.DataContext = null;
            this.DataContext = this;
        }

        private void SetWorkingDirectory()
        {
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                return;
            }

            var dlg = new FolderBrowserForWPF.Dialog()
            {
                Title = "Choose a working directory containing " + XwaWorkspace.ExeName
            };

            if (dlg.ShowDialog() != true)
            {
                return;
            }

            WorkingDirectory = null;
            Workspace = null;

            try
            {
                string path = dlg.FileName;
                Workspace = new XwaWorkspace(path);
                WorkingDirectory = path;

                CraftNames = new List<string>(Workspace.SpeciesTable.Entries.Count);
                for (int spec = 0; spec < Workspace.SpeciesTable.Entries.Count; spec++)
                {
                    XwaExeSpeciesEntry specEntry = Workspace.SpeciesTable.Entries[spec];
                    string name = Workspace.GetModelName(specEntry.Value);
                    CraftNames.Add(name);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void ExecuteNewPilot(object sender, ExecutedRoutedEventArgs e)
        {
            PilotFileName = null;
            Pilot = new PilotFile();
            RefreshPilot();
        }

        public void ExecuteOpenPilot(object sender, ExecutedRoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                DefaultExt = ".plt",
                CheckFileExists = true,
                Filter = "PILOT files (*.plt)|*.plt"
            };

            string fileName;

            if (dialog.ShowDialog(this) == true)
            {
                fileName = dialog.FileName;
            }
            else
            {
                return;
            }

            try
            {
                PilotFile pilot = PilotFile.FromFile(fileName);
                Pilot = pilot;
                PilotFileName = fileName;
                RefreshPilot();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, fileName + "\n" + ex.ToString(), this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void ExecuteSavePilot(object sender, ExecutedRoutedEventArgs e)
        {
            if (Pilot is null)
            {
                return;
            }

            if (string.IsNullOrEmpty(PilotFileName))
            {
                ExecuteSaveAsPilot(null, null);
                return;
            }

            try
            {
                Pilot.Save(PilotFileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.ToString(), this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void ExecuteSaveAsPilot(object sender, ExecutedRoutedEventArgs e)
        {
            if (Pilot is null)
            {
                return;
            }

            var dialog = new SaveFileDialog
            {
                AddExtension = true,
                DefaultExt = ".plt",
                Filter = "PILOT files (*.plt)|*.plt",
                FileName = System.IO.Path.GetFileName(PilotFileName)
            };

            string fileName;

            if (dialog.ShowDialog(this) == true)
            {
                fileName = dialog.FileName;
            }
            else
            {
                return;
            }

            try
            {
                Pilot.Save(fileName);
                PilotFileName = fileName;
                RefreshPilot();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.ToString(), this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
