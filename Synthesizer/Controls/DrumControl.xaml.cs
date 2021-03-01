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
    /// Interaction logic for DrumControl.xaml
    /// </summary>
    public partial class DrumControl : UserControl
    {
        public static readonly DependencyProperty SampleRateProperty = DependencyProperty.Register("SampleRate", typeof(int), typeof(DrumControl), new PropertyMetadata(48_000));
        public static readonly RoutedUICommand ShowCommand = new RoutedUICommand("Show Waveform", "Show", typeof(DrumControl));
        public static readonly RoutedUICommand PlayCommand = new RoutedUICommand("Play Waveform", "Show", typeof(DrumControl));

        public int SampleRate
        {
            get { return (int)GetValue(SampleRateProperty); }
            set { SetValue(SampleRateProperty, value); }
        }

        public Views.DrumView View
        {
            get { return this.DataContext as Views.DrumView; }
            set { this.DataContext = value; }
        }

        public DrumControl()
        {
            InitializeComponent();

            this.CommandBindings.Add(new CommandBinding(
                ShowCommand,
                (o, e) => { }
            ));

            this.CommandBindings.Add(new CommandBinding(
                PlayCommand,
                (o, e) => { new System.Media.SoundPlayer(WaveWriter.Write(SampleRate, (int)(SampleRate * View.EffectTime), View.Adapt())).Play(); }
            ));
        }
    }
}
