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
using ErnstTech.SoundCore;


namespace Synthesizer.Controls
{
    /// <summary>
    /// Interaction logic for BaseDrumControl.xaml
    /// </summary>
    public partial class BaseDrumControl : UserControl
    {
        public static readonly RoutedUICommand TestCommand = new RoutedUICommand("Test Base Drum", nameof(TestCommand), typeof(BaseDrumControl));

        public Views.BaseDrumView View
        {
            get { return (Views.BaseDrumView)DataContext; }
            set { DataContext = value; }
        }

        public BaseDrumControl()
        {
            InitializeComponent();

            this.DataContext = new Views.BaseDrumView();

            this.CommandBindings.Add(new CommandBinding(
                TestCommand,
                (o, a) => { new System.Media.SoundPlayer(WaveWriter.Write(48_000, 48_000, View.Adapt())).Play(); },
                (o, a) => { a.CanExecute = true;  }
            ));
        }
    }
}
