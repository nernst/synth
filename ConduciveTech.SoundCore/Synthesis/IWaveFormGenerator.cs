using System;
using System.Collections.Generic;
using System.Text;

namespace ErnstTech.SoundCore.Synthesis
{
    /// <summary>
    ///     Defines the minimum requirements for a class that
    ///     generates a wave form.
    /// </summary>
    interface IWaveFormGenerator : IEnumerable<double>
    {
        /// <summary>
        ///     Generates a waveform for the specified number of samples.
        /// </summary>
        /// <param name="nSamples">The number of samples to generate.</param>
        /// <returns>An array of <seealso cref="double"/> containing the generated form.</returns>
        double[] Generate(long nSamples);

    }
}
