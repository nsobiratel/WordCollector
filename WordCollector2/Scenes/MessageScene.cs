using System;
using MonoGame.Extended.NuclexGui.Controls.Desktop;
using MonoGame.Extended.NuclexGui;
using MonoGame.Extended.NuclexGui.Controls;

namespace WordCollector2
{
    public class MessageScene : GuiWindowControl
    {
        GuiButtonControl BtnOk;
        GuiLabelControl LbText;

        public MessageScene()
        {
            this.Bounds = new UniRectangle(
                new UniScalar(0.1f, 0),
                new UniScalar(0.2f, 0),
                new UniScalar(0.8f, 0),
                new UniScalar(0.6f, 0));
            this.EnableDragging = true;
            this.Title = "Сообщение";

            UniVector size = 
                new UniVector(
                    new UniScalar(0.8f, 0), 
                    new UniScalar(0.6f, 0));
            UniVector location = 
                new UniVector(
                    new UniScalar(0.1f, 0),
                    new UniScalar(0.1f, 0));

            this.LbText = new GuiLabelControl
            {
                Name = "LbText",
                Bounds = new UniRectangle(location, size),
            };

            size = new UniVector(
                new UniScalar(0.8f, 0), 
                new UniScalar(0.2f, 0));
            location = new UniVector(
                new UniScalar(0.1f, 0),
                new UniScalar(0.75f, 0));

            this.BtnOk = new GuiButtonControl
            {
                Name = "btnOk",
                Bounds = new UniRectangle(location, size),
                Text = "OK"
            };
            this.BtnOk.Pressed += (sender, e) => this.Close();

            this.Children.Add(this.LbText);
            this.Children.Add(this.BtnOk);
        }

        public void SetText(string msg)
        {
            this.LbText.Text = msg;
        }
    }
}

