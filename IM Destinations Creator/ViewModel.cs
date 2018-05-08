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
using FYMFileZip;

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
            LoadYardsFileCommand = new RelayCommand(obj => LoadYardsFile());
            AddYardCommand = new RelayCommand(o => AddYard());
            AddThisYardCommand = new RelayCommand(obj => AddYard((Yard)obj));

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
				param => unsavedChanges,
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
            DefaultYards = new List<Yard>() { new Yard(1000, "Unassigned")};
            LoadYardsFile();
            NewFile();
            unsavedChanges = false;
        }

        // Fields

        private string saveLocation;

        private bool unsavedChanges;

        private int selSourceYard;

        private ObservableCollection<SourceYard> sourceYards;

        private List<Yard> DefaultYards;

        // Properties

        public string BindingTestProperty { get; set; }

        public ObservableCollection<SourceYard> SourceYards
        {
            get { return sourceYards; }
            private set
            {
                // still working on this part
                sourceYards = value;
                NotifyPropertyChanged();
            }
        }

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
                yards.Remove(SelSourceYard);
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

        public RelayCommand LoadYardsFileCommand { get; }

        public RelayCommand AddYardCommand { get; }

        public RelayCommand AddThisYardCommand { get; }

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
            string ypath = @"server1\FYMYards.zip";

            if (!File.Exists(ypath))
            {
                // display modal dialog asking for FYM root directory
                string initDir = Directory.GetCurrentDirectory();

                CommonOpenFileDialog fileDialog = new CommonOpenFileDialog()
                {
                    Title = "Open FYMYards.zip File",
                    InitialDirectory = initDir,
                    DefaultFileName = "FYMYards.zip"
                };

                fileDialog.Filters.Add(new CommonFileDialogFilter("Zipped File", ".zip"));

                if (!(fileDialog.ShowDialog() == CommonFileDialogResult.Ok && Path.GetFileName(fileDialog.FileName) == "FYMYards.zip" && File.Exists(fileDialog.FileName)))
                {
                    return;
                }
                ypath = fileDialog.FileName;
            }

            // path should now be valid, if the dialogresult was .Ok

            string yards = "foo";
            FYMZip.UnZipFiletoString(ypath, ref yards);

            // string yards should now be the contents of the FYMYards.zip file
            
            DefaultYards = new List<Yard>();

            using (StringReader reader = new StringReader(yards))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    // do something on a per line basis
                    if (line.StartsWith("ID="))
                    {
                        try
                        {
                            int yardID = Convert.ToInt32(line.Substring(3, 4));
                            int schar = line.IndexOf(":") + 1;
                            int echar = line.IndexOf(":", schar);
                            string yardName = line.Substring(schar, echar - schar);
                            Yard yard = new Yard(yardID, yardName);
                            DefaultYards.Add(yard);
                        }
                        catch (FormatException)
                        {
                            MessageBox.Show("The FYMYards.zip file is poorly formed and could not be read: A yardID could not be read as a number.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        catch (ArgumentOutOfRangeException)
                        {
                            MessageBox.Show("The FYMYards.zip file is poorly formed and could not be read: A yard entry does not contain a separator ':'", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }

            string message = String.Format("Yards loaded from FYMYards.zip {0}There are {1} yards loaded.", Environment.NewLine, DefaultYards.Count());

			MessageBox.Show(message, "Information", MessageBoxButton.OK, MessageBoxImage.Information);

            NewFile();
        }

        private void AddYard()
        {
            AddNewYard nywindow = new AddNewYard();
            nywindow.ShowDialog();
            if (nywindow.NewYard != null)
            {
                AddYard(nywindow.NewYard);
            }
        }

        private void AddYard(Yard yard)
        {
            if (sourceYards.Contains(yard))
            {
                MessageBox.Show("This yardID is already in use: " + yard.YardID, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                SourceYards.Add(new SourceYard(yard.YardID, yard.YardName, new ObservableCollection<Yard>()));
                unsavedChanges = true;
            }
        }

        private void NewFile()
        {
            // unload all current data
            // overwrite with new default data

            saveLocation = null;
            unsavedChanges = true;
            SourceYards = new ObservableCollection<SourceYard>();
            int i = 0;
            foreach (Yard yard in DefaultYards)
            {
                SourceYard syard = new SourceYard(yard.YardID, yard.YardName, new ObservableCollection<Yard>());
                SourceYards.Add(syard);
                // cycling through selected yard updates the display
                SelSourceYard = SourceYards[i];
                i++;
            }
            SelSourceYard = SourceYards[0];
        }

        private void OpenFile() // To Do
        {
            // prompt a save if there is open unsaved data already

            if (unsavedChanges)
            {
                var result = MessageBox.Show("There are unsaved changes. Do you wish to continue?", "Unsaved Changes", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.No)
                {
                    return;
                }
            }

            // display modal dialogue asking for the file to load
            
            string initDir = Path.GetDirectoryName(saveLocation);

            CommonOpenFileDialog fileDialog = new CommonOpenFileDialog()
            {
                Title = "Open File",
                InitialDirectory = initDir,
                DefaultExtension = "imd"
            };

            fileDialog.Filters.Add(new CommonFileDialogFilter("IM Destination File", ".imd"));
            fileDialog.Filters.Add(new CommonFileDialogFilter("Text File", ".txt"));

            if (!Directory.Exists(initDir))
            {
                fileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }

            if (fileDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                NewFile();
                saveLocation = fileDialog.FileName;

                // unload current data
                // read from file
                // set up ViewModel properties from file data

                using (StreamReader file = new StreamReader(saveLocation))
                {
                    string line;
                    while ((line = file.ReadLine()) != null)
                    {
                        // handle each line depending on what the line is
                        // for .imd saves, each line is one entry in the SourceYards list
                        // the save doesnt necessarily contain all the information needed
                        // yard names come from the loaded yards list instead of the save file

                        int yardID;
                        string yardName;

                        // figure out the yardID first
                        try
                        {
                            yardID = Convert.ToInt32(line.Substring(0, 4));
                            yardName = GetYardName(yardID);

                        }
                        catch (FormatException)
                        {
                            MessageBox.Show("The selected file has invalid characters: Could not convert yard ID to a number", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                            throw;
                        }

                        // so we now have hopefully got a valid yardID for the source yard, and a name
                        // next we need to iterate over the list of all destination yards for this source yard, and add those

                        ObservableCollection<Yard> destYards = new ObservableCollection<Yard>();
                        string deststring = line.Substring(7);
                        string[] separators = new string[] { ", " };
                        string[] yardIDs = deststring.Split(separators, StringSplitOptions.RemoveEmptyEntries);

                        foreach (string yardIDstring in yardIDs)
                        {
                            try
                            {
                                int DestyardID = Convert.ToInt32(yardIDstring);
                                string DestyardName = GetYardName(DestyardID);
                                destYards.Add(new Yard(DestyardID, DestyardName));
                            }
                            catch (FormatException)
                            {
                                MessageBox.Show("The selected file has invalid characters: Could not convert yard ID to a number", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                                throw;
                            }
                        }

                        // now we check to see if the SourceYard already exists (it probably does) and add the destYards to it

                        SourceYard NewYard = new SourceYard(yardID, yardName, destYards);
                        if (SourceYards.Contains(NewYard))
                        {
                            int i = SourceYards.IndexOf(NewYard);
                            SourceYards[i].DestYards = destYards;
                        }
                        else
                        {
                            SourceYards.Add(new SourceYard(yardID, yardName, destYards));
                        }
                    }
                }

                unsavedChanges = false;
            }
        }

        public string GetYardName(int YardID)
        {
            Yard placeholder = new Yard(YardID, null);
            if (DefaultYards.Contains(placeholder))
            {
                int DefYardIndex = DefaultYards.IndexOf(placeholder);
                return DefaultYards[DefYardIndex].YardName;
            }
            else
            {
                return null;
            }
        }

        private void WriteFile(string[] lines, string path)
        {
            using (StreamWriter file = new StreamWriter(path))
            {
                foreach (string line in lines)
                {
                    file.WriteLine(line);
                }
            }
        }

        private string[] Save()
        {
            List<SourceYard> SaveYards = new List<SourceYard>();
            foreach (SourceYard yard in SourceYards)
            {
                if (yard.DestYards.Count != 0)
                {
                    SaveYards.Add(yard);
                }
            }
            string[] lines = new string[SaveYards.Count()];
            int i = 0;
            foreach (SourceYard sourceYard in SaveYards)
            {
                StringBuilder destyards = new StringBuilder();
                foreach (Yard yard in sourceYard.DestYards)
                {
                    destyards.Append(yard.YardID);
                    destyards.Append(", ");
                }
                lines[i] = sourceYard.YardID.ToString() + " = " + destyards;
                i++;
            }
            return lines;
        }

        private void SaveFile() // To Do
        {
			// does the save file exist? If so, write the current data to file
			// if not, SaveAsFile();

			if (saveLocation != null)
			{
                WriteFile(Save(), saveLocation);
                unsavedChanges = false;
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
                DefaultExtension = "imd"

			};

            fileDialog.Filters.Add(new CommonFileDialogFilter("IM Destination File", ".imd"));
            fileDialog.Filters.Add(new CommonFileDialogFilter("Text File", ".txt"));

            if (!Directory.Exists(initDir))
			{
				fileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			}

			if (fileDialog.ShowDialog() == CommonFileDialogResult.Ok)
			{
				saveLocation = fileDialog.FileName;
                WriteFile(Save(), saveLocation);
                unsavedChanges = false;
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
            unsavedChanges = true;
        }

        private void IsNotValidDest(IList<Yard> selYards)
        {
            // make the selected Yards not valid destination yards
            foreach (Yard yard in selYards)
            {
                SelSourceYard.DestYards.Remove(yard);
            }
            NotifyPropertyChanged("PossibleDestYards");
            unsavedChanges = true;
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
            unsavedChanges = true;
        }
    }
}
