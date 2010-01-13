using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace basic_light_board
{
    public class CueList 
    {
        
        public event EventHandler cueChanged;
        public event EventHandler nextCueChanged;

        public List<LightCue> mCues;

        public int mCurrentCueIndex=0;
        public int mNextCueIndex=0;
        public int mPrevCueIndex=0;
    
        public CueList()
        {
            mCues = new List<LightCue>();
            mCues.Add(LightCue.BlankCue);
            mCurrentCueIndex = 0;
            mNextCueIndex = 0;
            mPrevCueIndex = 0;
        }

        public bool setNextCue(string cueName)
        {
            if (this[cueName] == null) return false;
            NextCueNumber = this[cueName].cueNumber;
            return true;
        }

        public bool setNextCue(int cueNumber)
        {
            if (this[cueNumber] == null) return false;
            NextCueNumber = cueNumber;
            return true;
        }

    
        public LightCue this[int cueNum]
        {
            get
            {
                return mCues.Find(delegate(LightCue l) { return l.cueNumber == cueNum; });
            }
        }
        public LightCue this[string cueName]
        {
            get
            {
                return mCues.Find(delegate(LightCue l) { return l.cueName == cueName; });
            }
            
        }

        public LightCue CurrentCue
        {
            get
            {
                return mCues[mCurrentCueIndex];
            }
        }
        public LightCue NextCue
        {
            get
            {
                return mCues[mNextCueIndex];
            }
        }
        public LightCue PrevCue
        {
            get
            {
                return mCues[mPrevCueIndex];
            }
        }

        public int CurrentCueNumber
        {
            get
            {
                return mCues[mCurrentCueIndex].cueNumber;
            }
        }
        public int NextCueNumber
        {
            get
            {
                return mCues[mNextCueIndex].cueNumber;
            }
            set
            {
                int index = mCues.FindIndex(delegate(LightCue l) { return l.cueNumber == value; });
                if (index!=-1)
                {
                    mNextCueIndex=index;
                    onNextCueChanged();
                }
            }
        }
        public int PrevCueNumber
        {
            get
            {
                return mCues[mPrevCueIndex].cueNumber;
            }
        }

        public bool AddCue(LightCue cue)
        {
            if (mCues.Exists(delegate(LightCue l) { return l.cueNumber == cue.cueNumber; }))
                return false;
            mCues.Add(cue);
            mCues.Sort(); 
            return true;
            // since LightCue implements IComparable<LightCue> 
            //we dont need to specify a delegate. it will sort by cue number
        }

        public bool RemoveCue(int cueNumber)
        {
            int index = mCues.FindIndex(delegate(LightCue l){return l.cueNumber==cueNumber;});
            if (index ==-1) return false;
            mCues.RemoveAt(index);
            if (index < mPrevCueIndex) mPrevCueIndex--;
            if (index < mCurrentCueIndex) mCurrentCueIndex--;
            if (index < mNextCueIndex) mNextCueIndex--;
            if (index == mPrevCueIndex) mPrevCueIndex = index-1;
            if (index == mCurrentCueIndex) mCurrentCueIndex = 0;
            //if (index == mNextCueIndex) mCurrentCueIndex = index;

            return true;
        }

        public bool RemoveCue(string cueName)
        {
            int index = mCues.FindIndex(delegate(LightCue l) { return l.cueName==cueName; });
            if (index == -1) return false;
            mCues.RemoveAt(index);
            return true;
        }

        public void gotoNextCue()
        {
            mPrevCueIndex = mCurrentCueIndex;
            mCurrentCueIndex = mNextCueIndex;
            mNextCueIndex++;
            mNextCueIndex %= mCues.Count;
            onCueChanged();
            Console.WriteLine("cue incremented");
        }

        public void onCueChanged()
        {
            if (cueChanged!=null) cueChanged(this,new EventArgs());
        }
        public void onNextCueChanged()
        {
            Console.WriteLine("nextCueChanged");
            if (nextCueChanged != null) nextCueChanged(this, new EventArgs());
        }

        public void saveToFile(string fileName)
        {
            System.IO.StreamWriter f = new System.IO.StreamWriter(fileName,false,Encoding.UTF8);
            f.WriteLine("{0}", LightCue.version);
            f.WriteLine("{0}", this.mCues.Count);
            foreach (LightCue l in mCues)
            {
                f.WriteLine(l.serialize());
            }
            f.Close();
        }

        public void loadFromFile(string fileName)
        {
            StreamReader f = new System.IO.StreamReader(fileName, Encoding.UTF8);
            mCues.Clear();
            try
            {
                if (int.Parse(f.ReadLine()) != LightCue.version) throw new InvalidDataException("version of cue file not compatable");
                int num = int.Parse(f.ReadLine());
                for (int i = 0; i < num; i++)
                    AddCue(new LightCue(f.ReadLine()));
            }
            catch
            {
                if (mCues.Count == 0)
                    AddCue(LightCue.BlankCue);
            }
        }
        public string Serialize()
        {
            StringBuilder s = new StringBuilder();
            s.Append(LightCue.version);
            s.AppendLine(string.Format("{0}",this.mCues.Count));
            foreach (LightCue l in mCues)
            {
                s.AppendLine(l.serialize());
            }



            return s.ToString();
        }
    }
}
