using ImGuiNET;
using System.Numerics;

namespace Veneer.Controls;

public class RichTextLabel : Control
{
	public string Text = "";
	public bool AutoScroll = false;
	public bool WrapText = true;

    private bool shouldScroll = true;

	protected override void OnLayout()
	{
		if (ImGui.BeginChild($"##richlabel_{Guid}", GetSize()))
		{
			// drawing formatted text is certainly one of the things ever to do.
			Vector2 startPos = ImGui.GetCursorScreenPos();
            ImDrawListPtr drawList = ImGui.GetWindowDrawList();

            Vector4 curColor = ImGui.GetStyle().Colors[(int)ImGuiCol.Text];
            Vector2 curPos = startPos;
			for (var i = 0; i < Text.Length; i++)
			{
                if (WrapText && curPos.X > GetSize().X + startPos.X)
                {
                    curPos.Y += ImGui.GetTextLineHeight();
                    curPos.X = startPos.X;
                }

				if (Text[i] == '\\')
				{
					if (Text[i+1] == '[')
					{
                        drawList.AddText(curPos, ImGui.ColorConvertFloat4ToU32(curColor), "[");
                        curPos.X += ImGui.CalcTextSize("[").X;
                        i++;
					}
				}
                else if (Text[i] == '[')
				{
                    string fullTag = "";
                    
                    i++; // increment past '['
                    while (Text[i] != ']')
                    {
                        fullTag += Text[i].ToString();
                        i++;
                    }
                    string[] splitTag = fullTag.Split("=");

                    switch (splitTag[0])
                    {
                        case "/color":
                            curColor = ImGui.GetStyle().Colors[(int)ImGuiCol.Text];
                            break;
                        case "color":
                            string[] splitRGBA = splitTag[1].Split(",");
                            curColor = new Vector4(
                                float.Parse(splitRGBA[0]) / 255,
                                float.Parse(splitRGBA[1]) / 255,
                                float.Parse(splitRGBA[2]) / 255,
                                float.Parse(splitRGBA[3]) / 255
                            );
                            break;
                    }
                }
                else if (Text[i] == '\n')
                {
                    curPos.Y += ImGui.GetTextLineHeight();
                    curPos.X = startPos.X;
                }
                else
                {
                    drawList.AddText(curPos, ImGui.ColorConvertFloat4ToU32(curColor), Text[i].ToString());
                    curPos.X += ImGui.CalcTextSize(Text[i].ToString()).X;
                }
			}

            ImGui.Dummy(curPos - startPos);

            if (AutoScroll)
            {
                if (shouldScroll)
                {
                    shouldScroll = false;
                    ImGui.SetScrollY(ImGui.GetScrollMaxY());
                }
                if (ImGui.GetScrollY() >= ImGui.GetScrollMaxY() - 25)
                    shouldScroll = true;
            }

            ImGui.EndChild();
		}
	}

    public void PushColor(int r, int g, int b, int a)
    {
        Text += $"[color={r},{g},{b},{a}]";
    }

    public void PopColor()
    {
        Text += "[/color]";
    }

	// automatically escapes the control character
	public void AppendText(string text)
	{
        string escapedText = "";

        for (int i = 0; i < text.Length; i++)
        {
            if (text[i] == '\\')
            {
                escapedText += text[i].ToString();
                escapedText += text[i+1].ToString();
                i++;
            }
            else if (text[i] == '[')
            {
                escapedText += "\\[";
            }
            else
            {
                escapedText += text[i].ToString();
            }
        }

        Text += escapedText;
	}
}
