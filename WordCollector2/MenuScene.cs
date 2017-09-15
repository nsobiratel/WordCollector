using MonoGame.Extended.NuclexGui;
using MonoGame.Extended.NuclexGui.Controls.Desktop;

namespace WordCollector2
{
    internal class MenuScene : GuiWindowControl
    {
        public GuiButtonControl BtnStart { get; }

        public GuiInputControl TbNickName { get; }

        public MenuScene()
        {
            this.Bounds = new UniRectangle(
                new UniScalar(0f, 0),
                new UniScalar(0f, 0),
                new UniScalar(1f, 0),
                new UniScalar(1f, 0));
            this.EnableDragging = false;
            this.Title = "Меню";

            UniVector size = 
                new UniVector(
                    new UniScalar(0.8f, 0), 
                    new UniScalar(0.1f, 0));
            UniVector location = 
                new UniVector(
                    new UniScalar(0.1f, 0),
                    new UniScalar(0.6f, 0));

            this.BtnStart = new GuiButtonControl
            {
                Name = "btnStart",
                Bounds = new UniRectangle(location, size),
                Text = "Начать новую игру"
            };

            size = new UniVector(
                new UniScalar(0.8f, 0), 
                new UniScalar(0.1f, 0));
            location = new UniVector(
                new UniScalar(0.1f, 0),
                new UniScalar(0.5f, 0));

            this.TbNickName = new GuiInputControl
            {
                Name = "tbNickName",
                Bounds = new UniRectangle(location, size)
            };

            this.Children.Add(this.BtnStart);
            this.Children.Add(this.TbNickName);
        }
    }
}

