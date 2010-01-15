using System;
namespace basic_light_board
{
    interface ISingleSlider
    {
        int Channel { get; set; }
        event EventHandler LabelChanged;
        event System.Windows.Forms.ScrollEventHandler Scroll;
        string ToString();
        byte Value { get; set; }
        event EventHandler ValueChanged;
    }
}
