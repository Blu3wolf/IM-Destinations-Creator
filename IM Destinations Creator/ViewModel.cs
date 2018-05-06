using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace IM_Destinations_Creator
{
    class ViewModel : CommandSink, INotifyPropertyChanged
    {
        public ViewModel()
        {
			// instantiate RelayCommands
			DisplayTestCommand = new RelayCommand(o => DisplayTestMessage());
            NewFileCommand = new RelayCommand(obj => NewFile());
			ToggleCommand = new RelayCommand(obj => ToggleValidDest((IList<Yard>)obj));

            IsValidDestCommand = new RelayCommand(o => IsValidDest(CastToIList(o)), p => true);
            IsNotValidDestCommand = new RelayCommand(o => IsNotValidDest(CastToIList(o)), p => true);


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
            set
            {
                if (SourceYards.Contains(value))
                {
                    selSourceYard = SourceYards.IndexOf(value);
                    NotifyPropertyChanged();
                    NotifyPropertyChanged("PossibleDestYards");
                }
            }
        }

        public ObservableCollection<Yard> PossibleDestYards
        {
            get
            {
                ObservableCollection<Yard> yards = new ObservableCollection<Yard>(SourceYards);
                if (SelSourceYard.DestYards.Count == 0)
                {
                    return yards;
                }
                foreach (Yard o in SelSourceYard.DestYards)
                {
                    yards.Remove(o);
                }
                return yards;
            }
        }

		// Commands (Still actually Properties though)

		public static readonly RoutedUICommand CustomNewCommand = new RoutedUICommand();

        public event PropertyChangedEventHandler PropertyChanged;

        public RelayCommand NewFileCommand { get; }

		public RelayCommand DisplayTestCommand { get; }

		public RelayCommand ToggleCommand { get; }

        public RelayCommand IsValidDestCommand { get; }

        public RelayCommand IsNotValidDestCommand { get; }

        // Methods

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

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
                SelSourceYard.DestYards.Add(yard);
            }
            NotifyPropertyChanged("PossibleDestYards");
        }

        private void IsNotValidDest(IList<Yard> selYards)
        {
            // make the selected Yards not valid destination yards
            foreach (Yard yard in selYards)
            {
                SelSourceYard.DestYards.Remove(yard);
            }
            NotifyPropertyChanged("PossibleDestYards");
        }

        private void ToggleValidDest(IList<Yard> selYards) // To Do
        {
            // for every selected yard, if it is a valid destination yard, make it invalid, otherwise make it valid
            // this would be easy to implement if I just knew how to have one collection of selected yards rather than one collection per listbox
            foreach(Yard yard in selYards)
            {
                if (SelSourceYard.DestYards.Contains(yard))
                {
                    SelSourceYard.DestYards.Remove(yard);
                }
                else
                {
                    SelSourceYard.DestYards.Add(yard);
                }
            }
        }
    }
}
