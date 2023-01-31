using ErnstTech.SoundCore;
using ErnstTech.SoundCore.Synthesis;

namespace GoSynth.Synthesis;

public class Synthesizer
{
    ExpressionBuilder _Builder = new ExpressionBuilder(new ErnstTech.SoundCore.Synthesis.Expressions.Antlr.ExpressionParser());

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

    public Func<double, double> Compile(string expression) => _Builder.Compile(expression);

    public Stream Generate(Func<double, double> generator, double duration) => Generate(48_000, duration, generator);
}
