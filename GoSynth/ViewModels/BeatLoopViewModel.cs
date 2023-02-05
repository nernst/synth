using CommunityToolkit.Mvvm.ComponentModel;
using GoSynth.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using ErnstTech.SoundCore;
using ErnstTech.SoundCore.Sampler;

using System.Diagnostics;
using CommunityToolkit.Mvvm.Input;
using GoSynth.Synthesis;
using Plugin.Maui.Audio;

namespace GoSynth.ViewModels;

public partial class BeatLoopViewModel : ObservableValidator
{
    [ObservableProperty]
    [Range(1, 17)]
    int beatCount = 16;

    public ObservableCollection<Beat> Beats { get; init; }
    public ObservableCollection<BeatViewModel> BeatViewModels { get; init; }

    public ObservableCollection<SoundViewModel> Sounds { get; init; }

    Func<double, double>? _Generator = null;

    public Func<double, double>? Generator
    {
        get => _Generator ??= Sound?.Sound.Compile();
    }

    [ObservableProperty]
    SoundViewModel? sound;

    public BeatLoopViewModel()
    {
        Beats = new ObservableCollection<Beat>(Enumerable.Range(0, beatCount).Select(_ => new Beat()));
        BeatViewModels = new ObservableCollection<BeatViewModel>(Beats.Select(b => new BeatViewModel(b)));
        Sounds = new ObservableCollection<SoundViewModel>(SoundManager.Current.Sounds.Select(s => new SoundViewModel(s)));

        PropertyChanged += (o, a) =>
        {
            if (a?.Equals(nameof(BeatCount)) ?? false)
            {
                ResizeCollections();
            }
        };
    }

    void ResizeCollections()
    {
        Debug.Assert(Beats.Count == BeatViewModels.Count);

        if (BeatCount == Beats.Count)
            return;

        if (BeatCount < Beats.Count)
        {
            for (int i = Beats.Count; i != BeatCount; --i)
            {
                Beats.RemoveAt(i - 1);
                BeatViewModels.RemoveAt(i - 1);
            }
        }
        else
        {
            for (int i = Beats.Count; i < BeatCount; ++i)
            {
                var b = new Beat();
                Beats.Add(b);
                BeatViewModels.Add(new BeatViewModel(b));
            }

        }
    }

    [RelayCommand]
    void Play()
    {
        var generator = this.Generator;
        if (generator == null)
            return;

        var length = (long)(this.Sound!.Sound.Duration * (int)SampleRate.Rate48000);

        var loop = new BeatLoop(this.Beats) { Sampler = new FuncSampler(SampleRate.Rate48000, AudioBits.Bits32, generator, length) };
        var manager = AudioManager.Current;
        var player = manager.CreatePlayer(loop.WAVStream);
        player.Play();
    }
}
