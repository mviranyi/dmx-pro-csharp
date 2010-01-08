using System;
using System.Collections.Generic;
using System.Text;

namespace basic_light_board
{
    public enum DMXProMsgLabel
    {
            REPROGRAM_FIRMWARE_REQUEST = 1,
            PROGRAM_FLASH_PAGE_REQUEST = 2,
            PROGRAM_FLASH_PAGE_REPLY = 2,
            GET_WIDGET_PARAMETERS_REQUEST = 3,
            GET_WIDGET_PARAMETERS_REPLY = 3,
            SET_WIDGET_PARAMETERS_REQUEST = 4,
            SET_WIDGET_PARAMETERS_REPLY = 4,
            RECEIVED_DMX_PACKET = 5,
            OUTPUT_ONLY_SEND_DMX_PACKET_REQUEST = 6,
            SEND_RDM_PACKET_REQUEST = 7,
            RECEIVE_DMX_ON_CHANGE = 8,
            RECEIVED_DMX_CHANGE_OF_STATE_PACKET = 9,
            GET_WIDGET_SERIAL_NUMBER_REQUEST = 10,
            GET_WIDGET_SERIAL_NUMBER_REPLY = 10,
            SEND_RDM_DISCOVERY_REQUEST = 11
    }
    class DMXMessage : EventArgs 
    {
        public DMXProMsgLabel type;
        public byte[] message;

        public DMXMessage(DMXProMsgLabel t, byte[] m)
        {
            message = m;
            type = t;
        }
    }
    class VComWrapper
    {
        public System.IO.Ports.SerialPort m_port;
        public const byte msgStart = 0x7e;
        public const byte msgEnd = 0xe7;
        public string buffer;

        public event EventHandler<DMXMessage> dataReceived;
        
        

        public VComWrapper()
        {
            m_port = new System.IO.Ports.SerialPort();
            //m_port.BaudRate = 9600;
            //m_port.DataBits = 8;
            m_port.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(m_port_DataReceived);
            m_port.Encoding = Encoding.UTF8;
            m_port.ErrorReceived += new System.IO.Ports.SerialErrorReceivedEventHandler(m_port_ErrorReceived);
            m_port.PortName = "COM4";
            //m_port.NewLine = string.Format("{0}", (char)msgEnd);
        
            
        }

        void m_port_ErrorReceived(object sender, System.IO.Ports.SerialErrorReceivedEventArgs e)
        {
            throw new System.NotImplementedException();
        }


        public void initPro()
        {
            if (!m_port.IsOpen)
            {
                try
                {
                    m_port.Open();
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("Failed to open USB DMX Pro on comm port: {0} - Check Settings, Device",m_port.PortName),ex);
                }
            }
        }

        void m_port_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            byte[] header = new byte[4];
            int length;
            m_port.Read(header, 0, 4);
            if (header[0] != VComWrapper.msgStart) return;
            length = header[2] | (header[3] << 8);
            byte[] message = new byte[length+1];
            m_port.Read(message, 0, length);
            byte[] footer = new byte[1];
            m_port.Read(footer,0,1);
            if (footer[0] != VComWrapper.msgEnd) return;
            buffer = Encoding.UTF8.GetString(header) + Encoding.UTF8.GetString(message);
            if (dataReceived != null) dataReceived(this, new DMXMessage((DMXProMsgLabel)header[1], message));
        }


        public void sendMsg(DMXProMsgLabel label,byte []data)
        {
            if (!m_port.IsOpen) return;
            
            List<byte> temp = new List<byte>();
            temp.Add(msgStart);
            temp.Add((byte)label);
            temp.Add((byte)(data.Length & 0xff));
            temp.Add((byte)(data.Length >>8));
            temp.AddRange(data);
            temp.Add(msgEnd);
            m_port.Write(temp.ToArray(), 0, temp.Count);
        }


        public void sendReprogramFirmwareRequest()
        {
            // not sure this makes sense
            sendMsg(DMXProMsgLabel.REPROGRAM_FIRMWARE_REQUEST, new byte[0]);
        }

        public void sendProgramFlashPageRequest(byte[] page)
        {
            if (page.Length != 64) throw new Exception("page file must be 64 bytes");
            sendMsg(DMXProMsgLabel.PROGRAM_FLASH_PAGE_REQUEST, page);
        }

        public void sendGetWidgetParametersRequest(UInt16 configSize)
        {
            if (configSize > 508) throw new Exception("Config Size must be <= 508 bytes");
            byte[] size = new byte[2];
            size[0] = (byte)(configSize & 0xff);
            size[1] = (byte)((configSize >> 8) & 0xff);
            sendMsg(DMXProMsgLabel.GET_WIDGET_PARAMETERS_REQUEST, size);
        }

        public void SetWidgetParametersRequest(float DMXOutputBreakTime, float DMXOutputMarkTime, int DMXOutRate, byte[] configData)
        {
            byte breakTime =(byte)( DMXOutputBreakTime / 10.67);
            byte markTime = (byte)(DMXOutputMarkTime / 10.67);
            byte[] msg = new byte[5 + configData.Length];
            msg[0] = (byte)(configData.Length & 0xff);
            msg[1] = (byte)((configData.Length & 0xff00) >> 8);
            msg[2] = breakTime;
            msg[3] = markTime;
            msg[4] = (byte)DMXOutRate;
            if (configData.Length>0)
                Array.Copy(configData,0,msg,5,configData.Length);
            sendMsg(DMXProMsgLabel.SET_WIDGET_PARAMETERS_REQUEST, msg);
        }

        public void sendDMXPacketRequest(byte[] Levels)
        {
            if (Levels.Length < 24 || Levels.Length > 512) throw new Exception("The valid number of dimmer channels must be between 24 and 512.");
            byte[] msg = new byte[1 + Levels.Length];
            Array.Copy(Levels, 0, msg, 1, Levels.Length);
            msg[0] = (byte)0;
            sendMsg(DMXProMsgLabel.OUTPUT_ONLY_SEND_DMX_PACKET_REQUEST, msg);
        }

        public void setSendDMXalways()
        {
            sendMsg(DMXProMsgLabel.RECEIVE_DMX_ON_CHANGE, new byte[1] { 1 });
        }
        public void setSendDMXOnChangeOnly()
        {
            sendMsg(DMXProMsgLabel.RECEIVE_DMX_ON_CHANGE, new byte[1] { 0 });
        }
        public void GetWidgetSerialNumber()
        {
            sendMsg(DMXProMsgLabel.GET_WIDGET_SERIAL_NUMBER_REQUEST, new byte[0]);
        }
    }
}
