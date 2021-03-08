using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErnstTech.SoundCore
{
    public class Mixer
    {
        public IEnumerable<double> Mix(IList<IEnumerable<double>> sources, IList<double>? levels = null)
        {
            if (levels == null)
                levels = Enumerable.Range(0, sources.Count).Select(_ => 1.0).ToArray();

            if (sources.Count <= 0)
                throw new ArgumentException("There must be at least one source.", nameof(sources));
            if (levels.Count != sources.Count)
                throw new ArgumentException("The number of levels must match the number of sources.", nameof(levels));

            var enumerators = sources.Select(s => s.GetEnumerator()).ToArray<IEnumerator<double>?>();

            while (true)
            {
                ulong flags = 0;
                double sum = 0.0;

                for (int i = 0; i < enumerators.Length; ++i)
                {
                    bool flag = enumerators[i]?.MoveNext() ?? false;
                    if (flag)
                        flags |= 1ul << i;
                    else
                        enumerators[i] = null;

                    sum += levels[i] * enumerators[i]?.Current ?? 0.0;
                }

                yield return sum;

                if (flags != 0)
                    yield break;
            }
        }
    }
}
