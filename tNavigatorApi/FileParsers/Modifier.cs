using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tNavigatorLauncher.FileParsers
{
    public class Modifier(string filePath)
    {
        public List<string> DataText = File.ReadAllLines(filePath).ToList();

        public int FindIndex(string match)
            => DataText.FindIndex(line => line == match);

        public int FindMatchIndex(string match) =>
            DataText.FindIndex(line => line.Contains(match));

        public void ModifySubTagInTag(string tag, string subTag, string subTagValue)
        {
            var tagIndex = FindIndex(tag);

            if (tagIndex == -1)
            {
                DataText.Add($"{tag}\r\n {subTag} {subTagValue} /\r\n /");
            }

            var tagCloseIndex = DataText[tagIndex..].FindIndex(l => l == "/") + tagIndex;
            var subTagIndex = FindMatchIndex(subTag);

            if (subTagIndex > 0)
                DataText[subTagIndex] = subTagValue;
            else
                DataText.Insert(tagCloseIndex, subTagValue);
        }

        public void Write() => File.WriteAllText(filePath, string.Join('\n', DataText));
    }
}