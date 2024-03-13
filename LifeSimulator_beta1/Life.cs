using System;

namespace LifeSimulator_beta1
{
    class Life           // класс жихнь который отвечает за всю логику
    {
        private bool[,] _field; // поле с клетками
        private readonly int _rows; // readonly - можно присвоить значение только 1 раз в конструкторe
        private readonly int _cols;
        public uint Generations { get; private set; } // получить текущую генераию можно как Public, а назначить Private; для сохранности данных
        
        public Life(int rows, int cols, int density) // конструктор с тремя параметрами
        {
            _rows = rows;
            _cols = cols;
            _field = new bool[cols, rows];
            Random rnd = new Random();
            for (int x = 0; x < cols; x++) // первичаня генераия массива
            {

                for (int y = 0; y < rows; y++)
                {
                    _field[x, y] = rnd.Next(density) == 0; // если случайное число 0 то в клетке будет жизнь
                }

            }
        }

        public Life(int rows, int cols) // без первичной генерации, для пустого старта
        {
            _rows = rows;
            _cols = cols;
            _field = new bool[cols, rows];
        }

        ~Life() { }

        private int CountNb(int x, int y)   //метод посчета соседей у клетки
        {                                   //  []  []   []    
            int count = 0;                  //  [] [x,y] []
            for (int i = -1; i < 2; i++)    //  []  []   []
            {

                for (int j = -1; j < 2; j++)
                {
                    int col = (x + i + _cols) % _cols; // данная формула позволяет перебегать с одной
                    int row = (y + j + _rows) % _rows; // части экрана на другую, как на карте мира

                    bool isSelf = col == x && row == y; // проверка исключения себя из подсчета
                    bool has = _field[col, row];

                    if (has && !isSelf)
                        count++;
                }

            }
            return count;
        }

        public void NextGeneration() // метод построения следующего поколения по обычным правилам
        {

            bool[,] newfield = new bool[_cols, _rows]; // создаем новый массив для работы

            for (int x = 0; x < _cols; x++)
            {

                for (int y = 0; y < _rows; y++)
                {
                    int nbCount = CountNb(x, y);
                    bool has = _field[x, y];

                    if (!has && nbCount == 3) // условие B3\S23
                        newfield[x, y] = true;
                    else if (has && (nbCount < 2 || nbCount > 3))
                        newfield[x, y] = false;
                    else
                        newfield[x, y] = _field[x, y];
                }

            }
            _field = newfield; // текущее поле обновляем до нового
            Generations++; // инкремируем поколения

        }

        public void NextGenerationDayNight() // метод создания поля по правилам День Ночь
        {

            bool[,] newfield = new bool[_cols, _rows];

            for (int x = 0; x < _cols; x++)
            {

                for (int y = 0; y < _rows; y++)
                {
                    int nbCount = CountNb(x, y);
                    bool has = _field[x, y];

                    if (!has && (nbCount == 3 || nbCount > 5)) // правило B3678/S34678
                        newfield[x, y] = true;
                    else if (has && (nbCount < 3 || nbCount == 5))
                        newfield[x, y] = false;
                    else
                        newfield[x, y] = _field[x, y];
                }

            }
            _field = newfield;
            Generations++;
        }

        public void NextGenerationCastleWalls() // Метод создания по правилам Стены Замка
        {
                                                // рекомендуется начинать с путым полем
            bool[,] newfield = new bool[_cols, _rows];

            for (int x = 0; x < _cols; x++)
            {

                for (int y = 0; y < _rows; y++)
                {
                    int nbCount = CountNb(x, y);
                    bool has = _field[x, y];

                    if (!has && (nbCount > 3))   // B45678/S2345
                        newfield[x, y] = true;
                    else if (has && (nbCount < 2 || nbCount > 5))
                        newfield[x, y] = false;
                    else
                        newfield[x, y] = _field[x, y];
                }

            }
            _field = newfield;
            Generations++;
        }

        public bool[,] GetGameField()   // метод получения текущего поколения
        {                                           //для защиты инкапсуляции,
            bool[,] gameField = new bool[_cols, _rows]; //в противном случае мы будем передавать ссылку 
            for (int x = 0; x < _cols; x++)         // на исходный массив, и в интерфейсе
            {                                       // пользователя его можно будет изменить

                for (int y = 0; y < _rows; y++)
                {
                    gameField[x, y] = _field[x, y];
                }

            }
            return gameField;
        }

        private bool CellinBox(int x, int y) // проверка, находится ли клетка в массиве, требуется для 
        {                                    // корректной работы с добавлением и удалением клеток мышью
            return x >= 0 && y >= 0 && x < _cols && y < _rows;
        }

        private void ChangeCell(int x, int y, bool status) // закрытый метод изменения клетки
        {
            if (CellinBox(x, y))
                _field[x, y] = status;
        }

        public void AddCell(int x, int y) // открытый метод добавления клетки по координатам
        {
            ChangeCell(x, y, true);
        }

        public void KillCell(int x, int y) // открытый метод удаления
        {
            ChangeCell(x, y, false);
        }
    }
}
