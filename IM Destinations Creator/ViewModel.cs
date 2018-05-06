using System;
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
            NewFileCommand = new RelayCommand(obj => NewFile());
			ToggleCommand = new RelayCommand(obj => ToggleValidDest((IList<Yard>)obj));

            IsValidDestCommand = new RelayCommand(o => IsValidDest(CastToIList(o)), p => true);
            IsNotValidDestCommand = new RelayCommand(o => IsNotValidDest((IList<Yard>)o), p => true);


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

        public SourceYard SelSourceYard // do I need this?
        {
            get { return SourceYards[selSourceYard]; }
            set { /* still working on this part */ }
        }

        public ObservableCollection<Yard> PossibleDestYards { get; }

        public ObservableCollection<Yard> ActualDestYards
        {
            get { return SelSourceYard.DestYards; }
        }

		// Commands (Still actually Properties though)

		public static readonly RoutedUICommand CustomNewCommand = new RoutedUICommand();

        public RelayCommand NewFileCommand { get; }

		public RelayCommand DisplayTestCommand { get; }

		public RelayCommand ToggleCommand { get; }

        public RelayCommand IsValidDestCommand { get; }

        public RelayCommand IsNotValidDestCommand { get; }

        // Methods

        private IList<Yard> CastToIList(Object param)
        {
            System.Collections.IList items = (System.Collections.IList)param;
            IList<Yard> collection = items.Cast<Yard>().ToList<Yard>();
            return collection;
        }

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

        private void IsValidDest(IList<Yard> selYards)
        {
            // make the selected Yards valid destination yards
            foreach (Yard yard in selYards)
            {
                ActualDestYards.Add(yard);
                PossibleDestYards.Remove(yard);
            }
        }

        private void IsNotValidDest(IList<Yard> selYards)
        {
            // make the selected Yards not valid destination yards
            foreach (Yard yard in selYards)
            {
                ActualDestYards.Remove(yard);
                PossibleDestYards.Add(yard);
            }
        }

        private void ToggleValidDest(IList<Yard> selYards) // To Do
        {
            // for every selected yard, if it is a valid destination yard, make it invalid, otherwise make it valid
            // this would be easy to implement if I just knew how to have one collection of selected yards rather than one collection per listbox
            foreach(Yard yard in selYards)
            {
                if (ActualDestYards.Contains(yard))
                {
                    ActualDestYards.Remove(yard);
                    PossibleDestYards.Add(yard);
                }
                else
                {
                    ActualDestYards.Add(yard);
                    PossibleDestYards.Remove(yard);
                }
            }
        }
    }
}
