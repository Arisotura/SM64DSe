using System;

using System.Windows.Forms;

public class ParameterField
{
    public enum Type
    {
        BOOLEAN = 0,
        LIST = 1,
        NUMBER = 2,
        INVALID = 3
    };

    private Control control;
    private String description;
    private byte offset, length;
    private Type type;

    //type specific variables
    private String[] possibleValues;

    private float start;
    private float step;
    private float end;

    public ParameterField(Type type, object[] values, ushort[] hexValues, byte dataOffset, byte dataLength, String description = "No Description provided." )
	{
        this.description = description;
        this.offset = dataOffset;
        this.length = dataLength;

        if (type = Type.NUMBER)
        {
            try
            {
                this.start = (Decimal)values[0];
                this.step = (Decimal)values[1];
                this.end = (Decimal)values[2];
            } catch(Exception e)
            {
                this.type = Type.INVALID;
            }
        } else if (type = Type.LIST)
        {
            this.possibleValues = new String[values.Length];

            for (int i = 0; i<values.Length; i++)
            {
                this.possibleValues[i] = values[i].ToString();
            }
        }

	}

    public Control CreateControl(EventHandler eventHandler)
    {
        System.Windows.Forms.NumericUpDown numericUpDown = new System.Windows.Forms.NumericUpDown();
        numericUpDown.ValueChanged += eventHandler;
        this.control = numericUpDown;
        return control;
    }
}
