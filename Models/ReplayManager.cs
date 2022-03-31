using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris.Models
{
    class ReplayManager
    {

        public ReplayManager()
        {

        }

        public async void loadFromFile(string path)
        {
            Task task = new Task (() => loadFromFileTask(path));
        }

        public async void writeToFile(string fileName, string userInput)
        {
            Task task = new Task (() => writeToFileTask(fileName, userInput));
        }

        public async Task loadFromFileTask(string path)
        {
            string text = await File.ReadAllTextAsync(path);
        }

        public async Task writeToFileTask(string fileName, string userInput)
        {
            string timestamp = System.DateTime.Now;
            string fileText = userInput + " : " + timestamp;
            using StreamWriter streamWriter = new(fileName, append: true);
            await streamWriter.WriteLineAsync(fileText);
        }

        public void startReplay()
        {

        }
    }
}
