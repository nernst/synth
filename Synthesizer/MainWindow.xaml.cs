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
using System.IO;
using ErnstTech.SoundCore;
using ErnstTech.SoundCore.Synthesis;


namespace Synthesizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static readonly RoutedCommand TestSynthCommand = new RoutedCommand("TestSynth", typeof(MainWindow));
        public static readonly RoutedCommand ShowSynthCommand = new RoutedCommand("ShowSynth", typeof(MainWindow));
        public static readonly RoutedCommand TestLoopCommand = new RoutedCommand("TestLoop", typeof(MainWindow));


        public static readonly DependencyProperty ExpressionTextProperty = DependencyProperty.Register(
            "ExpressionText", 
            typeof(string), 
            typeof(MainWindow),
            new PropertyMetadata("cos(2 * PI * (220 + 4 * cos(2 * PI * 10 * t)) * t) * 0.5")
        );

        public string ExpressionText
        {
            get { return (string)GetValue(ExpressionTextProperty); }
            set { SetValue(ExpressionTextProperty, value); }
        }

        static readonly ExpressionBuilder _Parser = new ExpressionBuilder(new ErnstTech.SoundCore.Synthesis.Expressions.Antlr.ExpressionParser());


        Views.DrumView drumView = new Views.DrumView();
        Views.BeatLoopView beatLoopView = new Views.BeatLoopView();

        // readonly MediaPlayer _Player = new MediaPlayer();
        public MainWindow()
        {
            InitializeComponent();

            this.CommandBindings.Add(new CommandBinding(TestSynthCommand,
                new ExecutedRoutedEventHandler(TestSynthCommandExecuted),
                new CanExecuteRoutedEventHandler(TestSynthCommandCanExecute)));

            this.CommandBindings.Add(new CommandBinding(ShowSynthCommand,
                new ExecutedRoutedEventHandler(ShowSynthCommandExecuted),
                new CanExecuteRoutedEventHandler(ShowSynthCommandCanExecute)));

            this.CommandBindings.Add(new CommandBinding(TestLoopCommand,
                (o, e) => TestLoopCommandExecute(o, e),
                (o, e) => TestSynthCommandCanExecute(o, e)
            ));

            drumControl.DataContext = drumView;
            beatLoopControl.DataContext = beatLoopView;
        }

        static IEnumerable<float> ToEnumerable(int sampleRate, Func<double, double> func)
        {
            var count = 0;
            var delta = 1.0 / sampleRate;

            while (true)
                yield return (float)func(count++ * delta);
        }

        Stream Generate(int sampleRate, double duration, Func<double, double> func)
        {
            int nSamples = (int)(sampleRate * duration);
            var dataSize = nSamples * sizeof(float);
            var format = new WaveFormat(1, sampleRate, 32);

            var ms = new MemoryStream(dataSize + WaveFormat.HeaderSize);
            new WaveWriter(ms, sampleRate).Write(nSamples, ToEnumerable(sampleRate, func));

            ms.Position = 0;
            return ms;
        }

        void TestSynthCommandExecuted(object sender, ExecutedRoutedEventArgs args)
        {
            const int sampleRate = 44100;
            var func = _Parser.Compile(this.ExpressionText);
            var sample = Generate(sampleRate, 1.0, func);
            new System.Media.SoundPlayer(sample).Play();
        }

        void TestSynthCommandCanExecute(object sender, CanExecuteRoutedEventArgs args)
        {
            try
            {
                var expr = this.ExpressionText;
                if (string.IsNullOrWhiteSpace(expr))
                {
                    args.CanExecute = false;
                    return;
                }

                var func = _Parser.Compile(expr);
                args.CanExecute = func != null;
            }
            catch
            {
                args.CanExecute = false;
            }
        }

        void ShowSynthCommandExecuted(object sender, ExecutedRoutedEventArgs args)
        {
            const int sampleRate = 44100;
            var func = _Parser.Compile(this.ExpressionText);
            var sample = Generate(sampleRate, 1.0, func);

            var delta = 1.0 / sampleRate;
            var reader = new WaveReader(sample);
            var fmt = reader.Format;
            var xSeries = Enumerable.Range(0, (int)reader.NumberOfSamples).Select(i => i * delta);
            var ySeries = reader.GetChannelFloat(0).Select(x => (double)x);
            channel1.Plot(xSeries, ySeries);
        }

        void ShowSynthCommandCanExecute(object sender, CanExecuteRoutedEventArgs args) => TestSynthCommandCanExecute(sender, args);

        void TestLoopCommandExecute(object sender, ExecutedRoutedEventArgs args)
        {
            const int sampleRate = 48_000;
            var sampler = new ErnstTech.SoundCore.Sampler.FuncSampler()
            {
                SampleFunc = drumView.Adapt(),
                BitsPerSample = 32,
                SampleRate = sampleRate,
                Length = (long)(drumView.EffectTime * sampleRate)
            };

            beatLoopView.Sampler = sampler;

            var stream = beatLoopView.WAVStream;
            stream.Position = 0;

            new System.Media.SoundPlayer(stream).Play();
        }

    }
}
