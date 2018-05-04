﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace IM_Destinations_Creator
{
    class ViewModel : CommandSink
    {
        public ViewModel()
        {
			// instantiate RelayCommands
			DisplayTestCommand = new RelayCommand(o => DisplayTestMessage());
			NewFileCommand = new RelayCommand(o => NewFile());
			ToggleCommand = new RelayCommand(o => ToggleValidDest());


			// register RoutedUICommands? Im very unclear on this but it works
			base.RegisterCommand(
				ApplicationCommands.New,
				param => true,
				param => this.NewFile());

			base.RegisterCommand(
				ApplicationCommands.Open,
				param => true,
				param => this.OpenFile());

			base.RegisterCommand(
				ApplicationCommands.Save,
				param => true,
				param => this.SaveFile());

			base.RegisterCommand(
				ApplicationCommands.SaveAs,
				param => true,
				param => this.SaveAsFile());

			base.RegisterCommand(
				CustomNewCommand,
				param => true,
				param => this.NewFile());


			// lets initialise some data to use
			SourceYards = new ObservableCollection<SourceYard>
			{
				new SourceYard(1001, "Dolores ICTF, CA", new ObservableCollection<Yard>()),
				new SourceYard(1280, "Jackson High Oak, MS", new ObservableCollection<Yard>()),
				new SourceYard(1998, "Chicago 63rd St, IL", new ObservableCollection<Yard>()),
				new SourceYard(1389, "Terminal Island, CA", new ObservableCollection<Yard>()),
				new SourceYard(1385, "Rutherford, PA", new ObservableCollection<Yard>()),
				new SourceYard(1911, "Morrisville, PA", new ObservableCollection<Yard>()),
				new SourceYard(1372, "Houston Englewood, TX", new ObservableCollection<Yard>())
			};

			BindingTestProperty = "Test Text Here";
            selSourceYard = 0;

        }

        // Fields

        private string saveLocation;

        private int selSourceYard;

        // Properties

        public string BindingTestProperty { get; set; }

        public ObservableCollection<SourceYard> SourceYards { get; }

        private SourceYard SelSourceYard // do I need this?
        {
            get { return SourceYards[selSourceYard]; }
        }

        public ObservableCollection<Yard> PossibleDestYards
        {
            get
            {
                ObservableCollection<Yard> yards = new ObservableCollection<Yard>(SourceYards);
                if (ActualDestYards.Count == 0)
                {
                    return yards;
                }
                foreach (Yard o in ActualDestYards)
                {
                    yards.Remove(o);
                }
                return yards;
            }
        }

        public ObservableCollection<Yard> ActualDestYards
        {
            get { return SelSourceYard.DestYards; }
        }

		// Commands (Still actually Properties though)

		public static readonly RoutedUICommand CustomNewCommand = new RoutedUICommand();

        public RelayCommand NewFileCommand
        {
            get;
        }

		public RelayCommand DisplayTestCommand
		{
			get;
		}

		public RelayCommand ToggleCommand
		{
			get;
		}

        // Methods

		private void DisplayTestMessage()
		{
			MessageBox.Show("Test Message Here");
		}

        private void LoadYardsFile() // To Do
        {
			// load the list of yards as used by FYM, from the FYM install
			// assume we are in the FYM root directory first, then if we are not display modal dialogue asking for the FYM root directory

			// NOTE: This is not yet implemented
			MessageBox.Show("Pretend I loaded all new yards data just now.");

		}

        private void NewFile() // To Do
        {
			// unload all current data
			// overwrite with new default data

			// current default data is a hardcoded short list of yards
			// test behavior:
			DisplayTestMessage();
        }

        private void OpenFile() // To Do
        {
            // prompt a save if there is open unsaved data already
            // display modal dialogue asking for the file to load
            // unload all current data
            // read file
            // populate new data from file contents
        }

        private void SaveFile() // To Do
        {
			// does the save file exist? If so, write the current data to file
			// if not, SaveAsFile();

			if (saveLocation != null)
			{
				// write to saveLocation
				MessageBox.Show("Pretend I saved just now.");
			}
			else
			{
				SaveAsFile();
			}
        }

		private void SaveAsFile() // To Do
		{
			// display a modal dialogue asking where to save the file
			// write the current data to file
			// update the location of the save file
			string initDir = Path.GetDirectoryName(saveLocation);
			
			CommonSaveFileDialog fileDialog = new CommonSaveFileDialog()
			{
				Title = "Save As...",
				InitialDirectory = initDir,

			};
			if (!Directory.Exists(initDir))
			{
				fileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			}

			

			if (fileDialog.ShowDialog() == CommonFileDialogResult.Ok && !File.Exists(fileDialog.FileName))
			{
				// write to fileDialog.FileName
				MessageBox.Show("Pretend I saved just now.");
				saveLocation = fileDialog.FileName;
			}
		}

        private void SelectOrigin() // To Do
        {
            // set the currently selected origin yard
            // update the list of available and currently set destinations

        }

        private void ToggleValidDest() // To Do
        {

        }
    }
}
