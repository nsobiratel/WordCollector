using MonoGame.Extended.NuclexGui;
using MonoGame.Extended.NuclexGui.Controls.Desktop;
using MonoGame.Extended.NuclexGui.Controls;

namespace WordCollector2
{
    internal class MenuScene : GuiWindowControl
    {
        public GuiButtonControl BtnStart { get; }

        public GuiInputControl TbNickName { get; }

        public GuiButtonControl BtnSaveNick { get; }

        const string NickPlaceholder = "Введите ваше имя";

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
                Text = "Новая игра"
            };

            size = new UniVector(
                new UniScalar(0.8f, 0), 
                new UniScalar(0.1f, 0));
            location = new UniVector(
                new UniScalar(0.1f, 0),
                new UniScalar(0.1f, 0));

            this.TbNickName = new GuiInputControl
            {
                Name = "tbNickName",
                Bounds = new UniRectangle(location, size),
                Text = NickPlaceholder
            };

            size = new UniVector(
                new UniScalar(0.8f, 0), 
                new UniScalar(0.1f, 0));
            location = new UniVector(
                new UniScalar(0.1f, 0),
                new UniScalar(0.2f, 0));

            this.BtnSaveNick = new GuiButtonControl
            {
                Name = "BtnSaveNick",
                Bounds = new UniRectangle(location, size),
                Text = "Сохранить"
            };
            
            this.Children.Add(this.BtnStart);
            this.Children.Add(this.TbNickName);
            this.Children.Add(this.BtnSaveNick);
        }

        public bool IsValidNick()
        {
            return this.TbNickName.Text != NickPlaceholder
            && !string.IsNullOrWhiteSpace(this.TbNickName.Text)
            && this.TbNickName.Text.Length > 3
            && this.TbNickName.Text.Length < 100;
        }

        public void OnFocusChanged(object sender, ControlEventArgs e)
        {
            if (e.Control == this.TbNickName)
            {
                if (this.TbNickName.Text == NickPlaceholder)
                    this.TbNickName.Text = string.Empty;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(this.TbNickName.Text))
                    this.TbNickName.Text = NickPlaceholder;
            }
        }
    }
}

