using System.Text;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input.InputListeners;
using MonoGame.Extended.NuclexGui;
using MonoGame.Extended.NuclexGui.Controls.Desktop;
using MonoGame.Extended.NuclexGui.Controls;
using System;

namespace WordCollector2
{
    public class GameScene : GuiWindowControl
    {
        public GuiButtonControl BtnNextStep { get; }

        GuiInputControl TbWord { get; }

        GuiListControl ListMessages { get; }

        int _lastProcessedLength = 1;
        bool _backspaceAlreadyApplied = true;
        char _lastChar;

        public Action<GuiControl> OnNeedSetFocus { get; set; }

        public GameScene(string gameId, char startChar, bool canDoStep)
        {
            this._lastChar = startChar;

            this.Bounds = new UniRectangle(
                new UniScalar(0f, 0),
                new UniScalar(0f, 0),
                new UniScalar(1f, 0),
                new UniScalar(1f, 0));
            this.EnableDragging = false;
            this.Title = "Игра [" + gameId + "]";

            UniVector size = new UniVector(
                                 new UniScalar(0.8f, 0), 
                                 new UniScalar(0.1f, 0));
            UniVector location = new UniVector(
                                     new UniScalar(0.1f, 0),
                                     new UniScalar(0.1f, 0));

            this.TbWord = new GuiInputControl
            {
                Name = "tbWord",
                Bounds = new UniRectangle(location, size),
                Enabled = canDoStep
            };
            this.TbWord.Text += startChar;

            size = new UniVector(
                new UniScalar(0.8f, 0), 
                new UniScalar(0.1f, 0));
            location = new UniVector(
                new UniScalar(0.1f, 0),
                new UniScalar(0.2f, 0));
           
            this.BtnNextStep = new GuiButtonControl
            {
                Name = "btnNextStep",
                Bounds = new UniRectangle(location, size),
                Text = "Сделать ход",
                Enabled = canDoStep
            };

            size = new UniVector(
                new UniScalar(0.8f, 0), 
                new UniScalar(0.62f, 0));
            location = new UniVector(
                new UniScalar(0.1f, 0),
                new UniScalar(0.32f, 0));
            
            this.ListMessages = new GuiListControl
            {
                SelectionMode = ListSelectionMode.None,
                Name = "ListMessages",
                Bounds = new UniRectangle(location, size)
            };

            this.Children.Add(this.BtnNextStep);
            this.Children.Add(this.TbWord);
            this.Children.Add(this.ListMessages);
        }

        public void AddMessage(string msg)
        {
            this.ListMessages.Items.Add(msg);
        }

        void AfterAddChar(char newChar, int newLength, bool enabled, bool bsApplied)
        {
            this._lastChar = newChar;
            this._lastProcessedLength = newLength;
            this._backspaceAlreadyApplied = bsApplied;
            this.TbWord.CaretPosition = this._lastProcessedLength;
            this.TbWord.Enabled = enabled;
            //this.OnNeedSetFocus?.Invoke(this.BtnNextStep);
        }

        public void AddNewChar(char newChar)
        {
            this.AddMessage("Противник добавил букву [" + newChar + "]");
            this.TbWord.Enabled = true;
            this.BtnNextStep.Enabled = true;
            this.TbWord.Text += newChar;
            this.AfterAddChar(newChar, this.TbWord.Text.Length, true, true);
            this.OnNeedSetFocus?.Invoke(this.TbWord);
            this.AddMessage("Ваш ход");
        }

        public char GetLastChar()
        {
            return this._lastChar;
        }

        public void RemoveLastChar()
        {
            this.AddMessage("Мы не знаем слов с такой последовательностью букв :(");
            this.AddMessage("Последняя (неподходящая) буква убрана, придумайте другую ;)");
            this._ReplaceLastSymbol('\0');
        }

        void _ReplaceLastSymbol(char newChar)
        {
            if (newChar == '\0')
            {
                if (this.TbWord.Text.Length <= this._lastProcessedLength)
                {
                    this.AddMessage("один");
                    return;
                }

                if (this.TbWord.CaretPosition == this._lastProcessedLength)
                {
                    // ввелся один символ, который нужно удалить
                    this.AddMessage("три");
                    this.TbWord.Text = this.TbWord.Text.Remove(this._lastProcessedLength - 1, 1);
                }
                else
                {
                    // нужно удалить несоклько символов
                    this.AddMessage("два");
                    this.TbWord.Text = this.TbWord.Text.Remove(this._lastProcessedLength);
                }

                this.AfterAddChar(
                    this.TbWord.Text[this._lastProcessedLength - 1], 
                    this._lastProcessedLength - 1, false, false);
                return;
            }

            if (this.TbWord.Text.Length - this._lastProcessedLength > 1)
                this.TbWord.Text = this.TbWord.Text.Remove(this._lastProcessedLength + 1);

            StringBuilder builder = new StringBuilder(this.TbWord.Text);
            if (this.TbWord.Text.Length > 0)
                builder[this.TbWord.Text.Length - 1] = newChar;
            else
                builder.Append(newChar);
            this.TbWord.Text = builder.ToString();
            this.AfterAddChar(newChar, this.TbWord.Text.Length, false, false);
        }

        public void OnKeyTyped(object sender, KeyboardEventArgs e)
        {
            if (!this.TbWord.HasFocus)
                return;

            Keys key = e.Key;

            switch (key)
            {
                case Keys.Delete:
                case Keys.Back:
                    if (this._backspaceAlreadyApplied)
                    {
                        this.TbWord.Text += this._lastChar;
                        this.TbWord.CaretPosition = this._lastProcessedLength;
                        return;
                    }
                    this._lastProcessedLength--;
                    this._backspaceAlreadyApplied = true;
                    this._lastChar = this.TbWord.Text[this._lastProcessedLength - 1];
                    this.TbWord.Enabled = true;
                    break;
                case Keys.A:
                    this._ReplaceLastSymbol('Ф');
                    break;
                case Keys.B:
                    this._ReplaceLastSymbol('И');
                    break;
                case Keys.C:
                    this._ReplaceLastSymbol('С');
                    break;
                case Keys.D:
                    this._ReplaceLastSymbol('В');
                    break;
                case Keys.E:
                    this._ReplaceLastSymbol('У');
                    break;
                case Keys.F:
                    this._ReplaceLastSymbol('А');
                    break;
                case Keys.G:
                    this._ReplaceLastSymbol('П');
                    break;
                case Keys.H:
                    this._ReplaceLastSymbol('Р');
                    break;
                case Keys.I:
                    this._ReplaceLastSymbol('Ш');
                    break;
                case Keys.J:
                    this._ReplaceLastSymbol('О');
                    break;
                case Keys.K:
                    this._ReplaceLastSymbol('Л');
                    break;
                case Keys.L:
                    this._ReplaceLastSymbol('Д');
                    break;
                case Keys.M:
                    this._ReplaceLastSymbol('Ь');
                    break;
                case Keys.N:
                    this._ReplaceLastSymbol('Т');
                    break;
                case Keys.O:
                    this._ReplaceLastSymbol('Щ');
                    break;
                case Keys.P:
                    this._ReplaceLastSymbol('З');
                    break;
                case Keys.Q:
                    this._ReplaceLastSymbol('Й');
                    break;
                case Keys.R:
                    this._ReplaceLastSymbol('К');
                    break;
                case Keys.S:
                    this._ReplaceLastSymbol('Ы');
                    break;
                case Keys.T:
                    this._ReplaceLastSymbol('Е');
                    break;
                case Keys.U:
                    this._ReplaceLastSymbol('Г');
                    break;
                case Keys.V:
                    this._ReplaceLastSymbol('М');
                    break;
                case Keys.W:
                    this._ReplaceLastSymbol('Ц');
                    break;
                case Keys.X:
                    this._ReplaceLastSymbol('Ч');
                    break;
                case Keys.Y:
                    this._ReplaceLastSymbol('Н');
                    break;
                case Keys.Z:
                    this._ReplaceLastSymbol('Я');
                    break;
                case Keys.OemTilde:
                    this._ReplaceLastSymbol('Ё');
                    break;
                case Keys.OemPeriod:
                    this._ReplaceLastSymbol('Ю');
                    break;
                case Keys.OemComma:
                    this._ReplaceLastSymbol('Б');
                    break;
                case Keys.OemCloseBrackets:
                    this._ReplaceLastSymbol('Ъ');
                    break;
                case Keys.OemOpenBrackets:
                    this._ReplaceLastSymbol('Х');
                    break;
                default:
                    this._ReplaceLastSymbol('\0');
                    break;
            }
        }
    }
}

