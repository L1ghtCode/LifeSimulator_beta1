using System;
using System.Drawing;
using System.Windows.Forms;
using System.Media;

namespace LifeSimulator_beta1
{

    public partial class Form1 : Form
    {
        private Graphics _g;     // создаем обект класса график
        private int _resolution; // создаем переменную для подсчета разрешения
        private Life _objLife;   // создаем класс отвечающий за логику программы(клеточного автомата)
        private Brush _b;        // создаем обьект brush для отрисовки клеток

        private int _colorCounter;
        private readonly Color[] _color = 
        {
        ColorTranslator.FromHtml("#4F10AD"), //0  создаем массив цветов 
        ColorTranslator.FromHtml("#510FAD"), //1  для градиентной переливки
        ColorTranslator.FromHtml("#530FAD"), //2  цвета клетки
        ColorTranslator.FromHtml("#550FAD"), //3
        ColorTranslator.FromHtml("#570FAD"), //4
        ColorTranslator.FromHtml("#590FAD"), //5
        ColorTranslator.FromHtml("#5B0FAD"), //6
        ColorTranslator.FromHtml("#5D0FAD"), //7
        ColorTranslator.FromHtml("#5F0FAD"), //8
        ColorTranslator.FromHtml("#610FAD"), //9
        ColorTranslator.FromHtml("#640FAD"), //10
        ColorTranslator.FromHtml("#660FAD"), //11
        ColorTranslator.FromHtml("#680FAD"), //12
        ColorTranslator.FromHtml("#6A0FAD"), //13
        ColorTranslator.FromHtml("#6D0FAD"), //14
        ColorTranslator.FromHtml("#660FAD"), //11
        ColorTranslator.FromHtml("#5D0FAD"), //7
        ColorTranslator.FromHtml("#570FAD"), //4
        };
        
        public Form1() => InitializeComponent();
       
        private void StartGame()        // метод запуска игры
        {
            if (timer1.Enabled) return; // если игра уже запущена то ничего не делаем
            TurnOffControls();          // блокируем кнопки во время запуска автомата

            _resolution = (int)nudResolution.Value; // получаем данные о разрешении

            _objLife = new Life(pictureBox1.Height / _resolution, pictureBox1.Width / _resolution, (int)(nudDensity.Minimum + nudDensity.Maximum - nudDensity.Value));

            labelG.Text = Convert.ToString(_objLife.Generations); //записываем текущий номер поколения

            CreateGameField(); // создаем поле

            timer1.Start();   // запускаем таймер(старт)

        }

        private void StartEptyGame() // метод запуска игры с пустым полем
        {
            if (timer1.Enabled) return; // если игра уже запущена то ничего не делаем
            TurnOffControls(); // блокируем кнопки во время запуска автомата

            _resolution = (int)nudResolution.Value;

            _objLife = new Life(pictureBox1.Height / _resolution, pictureBox1.Width / _resolution);

            labelG.Text = Convert.ToString(_objLife.Generations);

            CreateGameField();

            timer1.Start();

        }

        private void CreateGameField() // метод создания игрового поля
        {
            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height); // создаем новое изображение
            _g = Graphics.FromImage(pictureBox1.Image);             //размерами в весь пикчербокс
        }

        private void ColorsGradient() // метод создания градиента для кнопок интерфейса
        {
            label1.ForeColor = _color[_colorCounter];
            label2.ForeColor = _color[_colorCounter];
            label3.ForeColor = _color[_colorCounter];
            label4.ForeColor = _color[_colorCounter];
            labelG.ForeColor = _color[_colorCounter];
            bStart.ForeColor = _color[_colorCounter];
            bEStart.ForeColor = _color[_colorCounter];
            bStop.ForeColor = _color[_colorCounter];
            bExit.ForeColor = _color[_colorCounter];
            groupBox1.ForeColor = _color[_colorCounter];

            _b = new SolidBrush(_color[_colorCounter]); // градиент для клеток
            
            _colorCounter++;
            if (_colorCounter == 18) _colorCounter = 0; // проверка чтобы не выйти за границы массива цветов
        }

        private void DrawNextGeneration() // метод отрисовки следующего поколения
        {
            _g.Clear(Color.Black);                     // очищаем прошлую генераию(закаршиваем в черный)
            ColorsGradient();

            bool[,] field = _objLife.GetGameField(); // создаем булево поле(клетка жива или нет(
                                                         // и передаем в него текущее поколение
            for (int x = 0; x < field.GetLength(0); x++) // в цикле происходит отрисовка
            {

                for (int y = 0; y < field.GetLength(1); y++)
                {
                    if (field[x, y])
                        _g.FillRectangle(_b, x * _resolution, y * _resolution, _resolution - 1, _resolution - 1);
                }

            }

            pictureBox1.Refresh(); // обновляем рисунок после отрисовки

            labelG.Text = Convert.ToString(_objLife.Generations); // записывваем текущее поколение

            if (rModeLife.Checked) _objLife.NextGeneration();            // условия для выбора режима
            if (rModeDayNight.Checked) _objLife.NextGenerationDayNight();// жизни клеток
            if (rCastleWalls.Checked) _objLife.NextGenerationCastleWalls();
        }

        private void StopGame() // метод остановки автомата
        {
            if (!timer1.Enabled) return; // если таймер уже выключен то возвращаемся
            timer1.Stop();
            TurnOnControls(); // разблокируем кнопки
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            DrawNextGeneration();  // вызываем отрисовку следующего поколения
            timer1.Interval = 1000 / tbSpeed.Value; // регулируем скорость таймера
        }

        private void bStart_Click(object sender, EventArgs e) // кнопка старт вызывает старт
        {
            StartGame();
        }

        private void bEStart_Click(object sender, EventArgs e) // кнопка с пустым стартом, его и вызывает
        {
            StartEptyGame();
        }

        private void bStop_Click(object sender, EventArgs e) // кнопка стоп вызывает остановку
        {
            StopGame();
        }

        private void bExit_Click(object sender, EventArgs e) // кнопка закрытия программы
        {
            Application.Exit(); // метод вызывающий выход(закрытие) приложения
        }

        private void TurnOnControls()
        {
            nudResolution.Enabled = true;
            nudDensity.Enabled = true;
            rModeLife.Enabled = true;
            rModeDayNight.Enabled = true;
            rCastleWalls.Enabled = true;
            groupBox1.Visible = true;
        }

        private void TurnOffControls()
        {
            nudResolution.Enabled = false;
            nudDensity.Enabled = false;
            rModeLife.Enabled = false;
            rModeDayNight.Enabled = false;
            rCastleWalls.Enabled = false;
            groupBox1.Visible = false;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e) // событие движении мышки
        {
            if (!timer1.Enabled) return; // Если таймер выключен то жизнь не добавляется

            FindMousePosition(e.Location, out int x, out int y);

            if (e.Button == MouseButtons.Left) // при зажатом ЛКМ добавляем клетки
            { 
                _objLife.AddCell(x, y);
            }

            if (e.Button == MouseButtons.Right) // при зажатом ПКМ удаляем их
            {
                _objLife.KillCell(x, y);
            }
        }

        private void FindMousePosition(Point eLocation, out int x, out int y)
        {
            x = eLocation.X / _resolution;
            y = eLocation.Y / _resolution;
        }

        private const string SoundName = "Life.wav";

        private void Form1OnLoad(object sender, EventArgs e) //указания при запуске формы
        {
            //метод получения адресса исполняемой программы, чтобы не указывать весь путь вручную до
            //музыкального файла, чтобы работало у всех
            string addPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            if (addPath == null) return;
            var path = System.IO.Path.Combine(addPath, SoundName); // указываем путь к аудио
            //запуск музыки
            SoundPlayer sp1 = new SoundPlayer(path);
            sp1.Play();
            sp1.PlayLooping();
        }

    }
}

