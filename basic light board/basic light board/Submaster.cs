using System;
using System.Collections.Generic;
using System.Text;

namespace basic_light_board 
{
    /// <summary>
    /// represents a pile on type submaster 
    /// </summary>
    public class Submaster: IComparable<Submaster>
    {
        private const int numHeaderItems = 2;
        private int mNumber;
        private string mSubName;
        private byte[] mChannelsLevels;
        

        public event EventHandler SubMasterNameChanged;
        public event EventHandler SubMasterNumberChanged;
        public event EventHandler ChannelValueChanged;

        public Submaster(string csvSubString)
        {
            string[] temp = csvSubString.Split(',');
            int.TryParse(temp[0], out mNumber);
            mSubName = temp[1];
            mChannelsLevels = new byte[LightingConstants.maxChannels];
            int numChannels = Math.Min(temp.Length - numHeaderItems, LightingConstants.maxChannels);
            for (int i = 0; i < numChannels; i++)
            {
                mChannelsLevels[i] = byte.Parse(temp[i + numHeaderItems]);
            }
        }
        public Submaster(int num)
        {
            mNumber = num;
            mSubName = String.Empty;
            mChannelsLevels = new byte[LightingConstants.maxChannels];
        }
        public Submaster(int num,string name, byte[] Levels)
        {
            if (Levels.Length != LightingConstants.maxChannels) 
                throw new ArgumentException(string.Format("Levels.Length={0}. should be {1}", Levels.Length, LightingConstants.maxChannels));
            mNumber = num;
            mSubName = name;
            Array.Copy(Levels, mChannelsLevels, LightingConstants.maxChannels);
        }

        #region Submaster Properties
        public byte this[int channelNum] // index is a 1 based index.
        {
            get
            {
                return mChannelsLevels[channelNum - 1];
            }
            set
            {
                if (value == mChannelsLevels[channelNum - 1]) return;
                mChannelsLevels[channelNum - 1] = value;
                OnChannelLevelChanged();
            }
        }

        public int SubNumber
        {
            get
            {
                return mNumber;
            }
            set
            {
                if (value == mNumber) return;
                mNumber = value;
                onSubMasterNumberChanged();
            }
        }
        public string SubName
        {
            get
            {
                return mSubName;
            }
            set
            {
                if (value == mSubName) return;
                mSubName = value;
                OnSubMasterNameChanged();
            }
        }
        public byte[] SubValues
        {
            get
            {
                return mChannelsLevels;
            }
            set
            {
                mChannelsLevels = value;
                OnChannelLevelChanged();
            }
        }
        #endregion

        #region on change handlers
        public void OnChannelLevelChanged()
        {
            if (ChannelValueChanged != null) ChannelValueChanged(this, new EventArgs());
        }
        public void onSubMasterNumberChanged()
        {
            if (SubMasterNumberChanged!=null) SubMasterNumberChanged(this,new EventArgs());
        }

        public void OnSubMasterNameChanged()
        {
            if (SubMasterNameChanged != null) SubMasterNameChanged(this, new EventArgs());
        }
        #endregion

        public string serialize()
        {
            StringBuilder s = new StringBuilder(3 * LightingConstants.maxChannels + mSubName.Length + 3);
            s.AppendFormat("{0},{1}", mNumber, mSubName);
            foreach (byte b in mChannelsLevels)
            {
                s.AppendFormat(",{0}", b);
            }
            return s.ToString();
        }

        public override string ToString()
        {
            if (mSubName != "") return string.Format("Sub{0} - {1}", mNumber, mSubName);
            return string.Format("Sub{0}", mNumber);
        }

        #region IComparable<Submaster> Members

        public int CompareTo(Submaster other)
        {
            return this.SubNumber.CompareTo(other.SubNumber);
        }

        #endregion
    }
}
