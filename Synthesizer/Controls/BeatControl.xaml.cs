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
using ErnstTech.SoundCore.Sampler;

namespace Synthesizer.Controls
{
    /// <summary>
    /// Interaction logic for BeatControl.xaml
    /// </summary>
    public partial class BeatControl : UserControl
    {
        public static readonly RoutedUICommand ToggleStateCommand = new RoutedUICommand("Toggle State", "ToggleState", typeof(BeatControl));

        public BeatLoop.Beat State
        {
            get { return (BeatLoop.Beat)DataContext; }
            set { DataContext = value; }
        }

        public static readonly DependencyProperty FullColorProperty = DependencyProperty.Register(
            "FullColor", 
            typeof(Color), 
            typeof(BeatControl),
            new PropertyMetadata(Colors.Red)
            );
        public static readonly DependencyProperty HalfColorProperty = DependencyProperty.Register(
            "HalfColor", 
            typeof(Color), 
            typeof(BeatControl),
            new PropertyMetadata(Colors.Orange)
            );
        public static readonly DependencyProperty OffColorProperty = DependencyProperty.Register(
            "OffColor", 
            typeof(Color), 
            typeof(BeatControl),
            new PropertyMetadata(Colors.Gray)
            );

        public Color FullColor
        {
            get { return (Color)GetValue(FullColorProperty); }
            set { SetValue(FullColorProperty, value); }
        }

        public Color HalfColor
        {
            get { return (Color)GetValue(HalfColorProperty); }
            set { SetValue(HalfColorProperty, value); }
        }

        public Color OffColor
        {
            get { return (Color)GetValue(OffColorProperty); }
            set { SetValue(OffColorProperty, value); }
        }

        public static readonly DependencyProperty BeatColorProperty = DependencyProperty.Register(
            "BeatColor", 
            typeof(Brush), 
            typeof(BeatControl),
            new PropertyMetadata(new SolidColorBrush(Colors.Gray))
            );

        public Brush BeatBrush
        {
            get { return (Brush)GetValue(BeatColorProperty); }
            set { SetValue(BeatColorProperty, value); }
        }

        public BeatControl()
        {
            InitializeComponent();

            this.CommandBindings.Add(new CommandBinding(
                ToggleStateCommand,
                (o, e) => {
                    this.State = this.State switch
                    {
                        BeatLoop.Beat.Off => BeatLoop.Beat.Full,
                        BeatLoop.Beat.Full => BeatLoop.Beat.Half,
                        BeatLoop.Beat.Half => BeatLoop.Beat.Off,
                        _ => throw new InvalidOperationException("Unknown Beat value.")
                    };
                    this.BeatBrush = new SolidColorBrush(this.State switch
                    {
                        BeatLoop.Beat.Off => OffColor,
                        BeatLoop.Beat.Half => HalfColor,
                        BeatLoop.Beat.Full => FullColor,
                        _ => throw new InvalidOperationException("Unknown Beat value.")
                    });
                    e.Handled = true;
                },
                (o, e) => e.CanExecute = true
            ));
        }
    }
}
