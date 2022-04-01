﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Tetris.Models
{
    static class ReplayManager {

        private static StreamWriter writingFileStream;
        private static Replay replayToWirte;
        public static async Task<Replay> CreateReplay()

        {
            string filename = Guid.NewGuid().ToString() + ".json";
            Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            await storageFolder.CreateFileAsync(filename);
            Debug.WriteLine("Replay created: " + storageFolder.Path + "/" + filename);
            return new Replay(filename);
        }

        public static async Task WriteEventToJson(ReplayEvent replayEvent, Replay replay)
        {
            replay.Events.Add(replayEvent);

            if (replayToWirte != replay || writingFileStream == null)
            {
                replayToWirte = replay;
                Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                String path = storageFolder.Path + "/" + replay.Filename;
                writingFileStream = new StreamWriter(path, false);
            }
            String json = JsonSerializer.Serialize(replayEvent);
            await writingFileStream.WriteAsync(json);
        }

        public static void disposeWritingStream()
        {
            writingFileStream?.Dispose();
            writingFileStream = null;
        }

        public static async Task<Replay> GetReplayFromFile(String path)
        {
            FileStream stream = File.OpenRead(path);
            Replay replay = await JsonSerializer.DeserializeAsync<Replay>(stream);
            stream.Dispose();
            return replay;
        }

        //todo get all from given dir

        //todo get all from storage folder
    }
}
