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
using System.Windows.Shapes;

namespace IM_Destinations_Creator
{
    /// <summary>
    /// Interaction logic for AddNewYard.xaml
    /// </summary>
    public partial class AddNewYard : Window
    {
        public AddNewYard()
        {
            InitializeComponent();
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            YardID.SelectAll();
            YardID.Focus();
        }

        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        public Yard NewYard
        {
            get
            {
                int yardID;
                try
                {
                    yardID = Convert.ToInt32(YardID.Text);
                }
                catch (FormatException)
                {
                    MessageBox.Show("Invalid characters in Yard ID (integer values only).");
                    return null;
                }
                return new Yard(yardID, YardName.Text);
            }
        }
    }
}
