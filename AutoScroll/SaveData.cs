using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace AutoScroll
{
    public class SaveData
    {
        public int ScoreCount = 0;

        public Collection<Score> ScoreList = new Collection<Score>();

        public string ScoreResourceDirectory;
    }
}
