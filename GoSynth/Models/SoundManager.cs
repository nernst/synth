using System.Text.Json;
using System.Text.Json.Nodes;

namespace GoSynth.Models;

public class SoundManager
{
    static SoundManager? current;
    public static SoundManager Current => current ??= new SoundManager();

    public IList<Sound> Sounds { get; private set;} = new List<Sound>();

    public string SoundsPath { get; private set;} = Path.Combine(FileSystem.Current.AppDataDirectory, "sounds.json");

    public SoundManager()
    {
        if (File.Exists(SoundsPath))
        {
            using var infile = new FileStream(SoundsPath, FileMode.Open, FileAccess.Read);
            var result = JsonSerializer.Deserialize(infile, typeof(List<Sound>), JsonSerializerOptions.Default) as List<Sound>;
            if (result != null)
                Sounds = result;
        }
    }

    public Sound? Load(Guid id) => this.Sounds.Where(s => s.Id == id).FirstOrDefault()?.Clone();

    public async Task Remove(Sound sound)
    {
        if (this.Sounds.Remove(sound))
            await Save();
    }

    public async Task Save(Sound sound)
    {
        var s = this.Sounds.Where(s => s.Id == sound.Id).FirstOrDefault();
        if (s != null)
            this.Sounds.Remove(s);
        this.Sounds.Add(sound);
        await this.Save();
    }

    public async Task Save()
    {
        await using var outfile = new FileStream(SoundsPath, FileMode.OpenOrCreate, FileAccess.Write);
        await JsonSerializer.SerializeAsync(outfile, Sounds, typeof(List<Sound>), JsonSerializerOptions.Default);
        await outfile.FlushAsync();
    }
}
