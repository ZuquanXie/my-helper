using System.Collections;
using System.Collections.ObjectModel;

namespace AutoScroll
{
    public class Score
    {
        public Collection<string> FileRefrences;

        public Score()
        {
            Name = "New Score";
            BPM = 120;
            Description = "";
            FileRefrences = new Collection<string>();
        }

        public Score(string name)
        {
            Name = name;
            BPM = 120;
            Description = "";
            FileRefrences = new Collection<string>();
        }

        public Score(string name, int bpm, string description, Collection<string> refrences)
        {
            Name = name;
            BPM = bpm;
            Description = description;
            FileRefrences = refrences;
        }

        public string Name { get; set; }

        public int BPM { get; set; }

        public string Description { get; set; }
    }
}
