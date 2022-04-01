using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Tetris.Models
{
    class ReplayManager
    {
        public Replay createReplay()
        {
            string timestamp = DateTime.Now.ToString();
            File.Create("replays/" + timestamp);
            return new Replay(timestamp);
        }

        public async Task writeEventToJson(ReplayEvent replayEvent, Replay replay)
        {
            replay.Events.Add(replayEvent);

            String path = "replays/" + replay.Filename;
            FileStream stream = File.OpenWrite(path);
            await JsonSerializer.SerializeAsync(stream, replay);
            stream.Dispose();
        }

        public async Task<Replay> getReplayFromFile(String path)
        {
            FileStream stream = File.OpenRead(path);
            Replay replay = await JsonSerializer.DeserializeAsync<Replay>(stream);
            return replay;
        }
    }
}
