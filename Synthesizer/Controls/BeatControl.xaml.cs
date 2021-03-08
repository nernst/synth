using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ErnstTech.SoundCore.Sampler;
using Synthesizer.Views;

namespace Synthesizer.Controls
{
    /// <summary>
    /// Interaction logic for BeatControl.xaml
    /// </summary>
    public partial class BeatControl : UserControl
    {
        public static readonly RoutedUICommand ToggleStateCommand = new RoutedUICommand("Toggle State", "ToggleState", typeof(BeatControl));

        public BeatView View
        {
            get { return ((BeatView)DataContext); }
        }

        public Views.BeatState State
        {
            get { return View?.State ?? BeatState.Off; }
            set { View.State = value; }
        }

        #region FullColor Property
        public static readonly DependencyProperty FullColorProperty = DependencyProperty.Register(
            "FullColor", 
            typeof(Color), 
            typeof(BeatControl),
            new PropertyMetadata(Colors.Red)
            );

        public Color FullColor
        {
            get { return (Color)GetValue(FullColorProperty); }
            set { SetValue(FullColorProperty, value); }
        }
        #endregion

        #region HalfColor Property
        public static readonly DependencyProperty HalfColorProperty = DependencyProperty.Register(
            "HalfColor", 
            typeof(Color), 
            typeof(BeatControl),
            new PropertyMetadata(Colors.Orange)
            );

        public Color HalfColor
        {
            get { return (Color)GetValue(HalfColorProperty); }
            set { SetValue(HalfColorProperty, value); }
        }
        #endregion

        #region OffColor Property
        public static readonly DependencyProperty OffColorProperty = DependencyProperty.Register(
            "OffColor", 
            typeof(Color), 
            typeof(BeatControl),
            new PropertyMetadata(Colors.Gray)
            );

        public Color OffColor
        {
            get { return (Color)GetValue(OffColorProperty); }
            set { SetValue(OffColorProperty, value); }
        }
        #endregion

        #region BeatBrush Property
        public static readonly DependencyProperty BeatBrushProperty = DependencyProperty.Register(
            "BeatBrush", 
            typeof(Brush), 
            typeof(BeatControl)
            );

        public Brush BeatBrush
        {
            get { return (Brush)GetValue(BeatBrushProperty); }
            set { SetValue(BeatBrushProperty, value); }
        }
        #endregion

        #region CustomColor Property
        public static readonly DependencyProperty CustomColorProperty = DependencyProperty.Register(
            "CustomColor", 
            typeof(Color), 
            typeof(BeatControl),
            new PropertyMetadata(Colors.Yellow));

        public Color CustomColor
        {
            get { return (Color)GetValue(CustomColorProperty); }
            set { SetValue(CustomColorProperty, value); }
        }
        #endregion

        public BeatControl()
        {
            InitializeComponent();

            this.CommandBindings.Add(new CommandBinding(
                ToggleStateCommand,
                (o, e) => {
                    this.State = this.State switch
                    {
                        BeatState.Off => BeatState.Full,
                        BeatState.Full => BeatState.Half,
                        BeatState.Half => BeatState.Off,
                        _ => throw new InvalidOperationException("Unknown Beat value.")
                    };
                    e.Handled = true;
                },
                (o, e) => e.CanExecute = true
            ));

            DataContextChanged += (o, e) =>
            {
                View.PropertyChanged += (o, e) => { if (e.PropertyName?.Equals("State") ?? true) SetBeatBrushState(); };

                this.SetBeatBrushState();
            };
        }

        void SetBeatBrushState()
        {
            this.BeatBrush = new SolidColorBrush(this.State switch
            {
                BeatState.Off => this.OffColor,
                BeatState.Half => this.HalfColor,
                BeatState.Full => this.FullColor,
                BeatState.Custom => this.CustomColor,
                _ => throw new InvalidOperationException("Unknown Beat value.")
            });
        }
    }
}
