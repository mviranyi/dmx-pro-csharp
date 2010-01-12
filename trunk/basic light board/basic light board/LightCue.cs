using System;
using System.Collections.Generic;
using System.Text;

namespace basic_light_board
{
    public class LightCue:IComparable<LightCue>
    {
        public static readonly int version = 100;
        // default values
        public static int defaultUpTime=5000;
        public static int defaultDownTime=5000;
        public static int defaultFollowTime=0;

        // private variables that have properties
        private int mCueNumber;
        private string mCueName;
        private bool mIsFollowCue;
        private byte[] mChannelLevels;
        private int mUpFadeTime;
        private int mDownFadeTime;
        private int mFollowTime;

        //events
        public event EventHandler cueNumberChanged;
        public event EventHandler cueNameChanged;
        public event EventHandler isFollowCueChanged;
        public event EventHandler channelLevelsChanged;
        public event EventHandler upTimeChanged;
        public event EventHandler downTimeChanged;
        public event EventHandler followTimeChanged;



        #region constructors
        public LightCue(int num, string name, byte[] levels)
            :this(num,name,defaultUpTime,defaultDownTime,false,defaultFollowTime,levels)
        {}
        public LightCue(int num, string name, int upTime, int downTime, byte[] levels)
            :this(num,name,upTime,downTime,false,defaultFollowTime,levels)
        {}
        public LightCue(int num, string name, int upTime, int downTime, bool isFollow, int followTime,byte[] levels)
        {
            mCueNumber = num;
            mCueName = name;
            mUpFadeTime = upTime;
            mDownFadeTime = downTime;
            mIsFollowCue = isFollow;
            mFollowTime = followTime;
            mChannelLevels = (byte[])levels.Clone();
        }

        public LightCue(string csvCueString)
        {
            string[] temp = csvCueString.Split(',');
            if (temp.Length < 6) throw new ArgumentException("there is not enough info to create a Cue");
            mCueNumber = int.Parse(temp[0]);
            mCueName= temp[1];
            mUpFadeTime = int.Parse(temp[2]);
            mDownFadeTime = int.Parse(temp[3]);
            mIsFollowCue  = bool.Parse(temp[4]);
            mFollowTime = int.Parse(temp[5]);

            int numChannels = temp.Length - 6;
            mChannelLevels = new byte[numChannels];
            for (int i = 6; i < temp.Length; i++)
                mChannelLevels[i-6] = byte.Parse(temp[i]);
        }
        #endregion

        #region simple properties
        public int upFadeTime
        {
            get
            {
                return mUpFadeTime;
            }
            set
            {
                mUpFadeTime = value;
                onUpTimeChanged();
            }
        }

        public int downFadeTime
        {
            get
            {
                return mDownFadeTime;
            }
            set
            {
                mDownFadeTime = value;
                onDownTimeChanged();
            }
        }

        public int followTime
        {
            get
            {
                return mFollowTime;
            }
            set
            {
                mFollowTime = value;
                onFollowTimeChanged();
            }
        }

        public bool isFollowCue
        {
            get
            {
                return mIsFollowCue;
            }
            set
            {
                mIsFollowCue = value;
                onIsFollowTimeChanged();
            }
        }

        public int cueNumber
        {
            get
            {
                return mCueNumber;
            }
            set
            {
                mCueNumber = value;
                onCueNumberChanged();
            }
        }

        public string cueName
        {
            get
            {
                return mCueName;
            }
            set
            {
                mCueName = value;
                onCueNameChanged();
            }
        }
        #endregion

        public byte this[int channel]
        {
            get
            {
                return mChannelLevels[channel - 1];
            }
            set
            {
                mChannelLevels[channel - 1] = value;
                onChannelLevelsChanged();
            }
        }
        public byte[] channelLevels
        {
            get { return mChannelLevels; }
            set { mChannelLevels = value; onChannelLevelsChanged(); }
        }

        #region onChanged methods
        private void onChannelLevelsChanged()
        {
            if (channelLevelsChanged != null) channelLevelsChanged(this, new EventArgs());
        }

        private void onCueNameChanged()
        {
            if (cueNameChanged != null) cueNameChanged(this, new EventArgs());
        }

        private void onCueNumberChanged()
        {
            if (cueNumberChanged  != null) cueNumberChanged(this, new EventArgs());
        }

        private void onDownTimeChanged()
        {
            if (downTimeChanged != null) downTimeChanged(this, new EventArgs());
        }

        private void onFollowTimeChanged()
        {
            if (followTimeChanged != null) followTimeChanged(this, new EventArgs());
        }

        private void onIsFollowTimeChanged()
        {
            if (isFollowCueChanged != null) isFollowCueChanged(this, new EventArgs());
        }

        private void onUpTimeChanged()
        {
            if (upTimeChanged != null) upTimeChanged(this, new EventArgs());
        }
        #endregion

        #region serialization
        public string serialize()
        {
            StringBuilder str = new StringBuilder();
            str.AppendFormat("{0},{1},{2},{3},{4},{5}", mCueNumber, mCueName, mUpFadeTime, mDownFadeTime, mIsFollowCue, mFollowTime);
            foreach (byte b in mChannelLevels)
            {
                str.AppendFormat(",{0}", b);
            }
            return str.ToString();
        }

        public override string ToString()
        {
            return cueName;
        }

        #endregion

        #region IComparable<LightCue> Members

        public int CompareTo(LightCue other)
        {
            return this.cueNumber.CompareTo(other.cueNumber);
        }

        #endregion

        private static LightCue mBlankCue;
        public static LightCue BlankCue
        {
            get
            {
                if (mBlankCue == null)
                {
                    byte[] levels=new byte[512];
                    mBlankCue=new LightCue(0,"Blank",levels);
                }
                return mBlankCue;
            }
        }
    }
}
