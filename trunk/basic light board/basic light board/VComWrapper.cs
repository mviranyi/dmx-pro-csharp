using System;
using System.Collections.Generic;
using System.Text;

namespace basic_light_board
{

    class VComWrapper
    {
        public System.IO.Ports.SerialPort m_port;
        public const byte msgStart = 0x7e;
        public const byte msgEnd = 0xe7;
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

        public VComWrapper()
        {
            m_port = new System.IO.Ports.SerialPort();
            m_port.BaudRate = 9600;
            m_port.DataBits = 8;
            m_port.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(m_port_DataReceived);
            m_port.Encoding = Encoding.UTF8;
            m_port.ErrorReceived += new System.IO.Ports.SerialErrorReceivedEventHandler(m_port_ErrorReceived);
            
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
                    m_port.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(m_port_DataReceived);
                    m_port.Open();
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("Failed to open USB DMX Pro on comm port: {0} - Check Settings, Device",m_port.PortName));
                }
            }
        }

        void m_port_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            m_port.NewLine = string.Format("{0}", (char)msgEnd);
            m_port.ReadLine();
        }

        public void sendMsg(DMXProMsgLabel label,byte []data)
        {
            if (!m_port.IsOpen) return;
            
            List<byte> temp = new List<byte>();
            temp.Add(msgStart);
            temp.Add((byte)label);
            temp.Add((byte)(((byte)data.Length) % 256));
            temp.Add((byte)(((byte)data.Length) / 256));
            temp.AddRange(data);
            temp.Add(msgEnd);
            m_port.Write(temp.ToArray(), 0, temp.Count);
        }
        
    }
}
