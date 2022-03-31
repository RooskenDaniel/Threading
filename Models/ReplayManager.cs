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
        public async Task loadFromFile(string path)
        {
            string text = await File.ReadAllTextAsync(path);
        }

        public async Task writeToFileTask(string fileName, string userInput)
        {
            using StreamWriter streamWriter = new(fileName, append: true);
            await streamWriter.WriteLineAsync(userInput);
        }
    }
}
