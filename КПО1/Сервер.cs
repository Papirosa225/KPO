using System.Net;
using System.Net.Sockets;

ServerObject server = new ServerObject();// создаем сервер
await server.ListenAsync(); // запускаем сервер

class ServerObject
{
    TcpListener tcpListener = new TcpListener(IPAddress.Any, 8866); // сервер для прослушивания
    List<ClientObject> AllClients = new List<ClientObject>(); // все подключения
    List<ClientObject> GameClients = new List<ClientObject>(); // игроки в комнате
    #region Работа с сервером

   
    protected internal void RemoveConnection(string id)
    {
        // получаем по id закрытое подключение
        ClientObject? client = AllClients.FirstOrDefault(c => c.Id == id);
        // и удаляем его из списка подключений
        if (client != null) AllClients.Remove(client);
        client?.Close();
    }
    protected internal void Disconnect()
    {
        foreach (var client in AllClients)
        {
            client.Close(); //отключение клиента
        }
        tcpListener.Stop(); //остановка сервера
    }
    // прослушивание входящих подключений
    protected internal async Task ListenAsync()
    {
        try
        {
            tcpListener.Start();
            Console.WriteLine("Сервер запущен. Ожидание подключений...");

            while (true)
            {
                TcpClient tcpClient = await tcpListener.AcceptTcpClientAsync();

                ClientObject clientObject = new ClientObject(tcpClient, this);
                AllClients.Add(clientObject);
                Task.Run(clientObject.ProcessAsync);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            Disconnect();
        }
    }
    #endregion
    #region Крестики-нолики
    protected internal void CreateGame(string ClientId, string userName,ClientObject clientMaster)
    {

        clientMaster.roomClients[0] = clientMaster;
        clientMaster.roomClients[0].Writer.Write("Ожидайте игроков\n");
        clientMaster.Writer.Flush();
        Board board = new Board();
        clientMaster.roomCreate = true;
        Console.WriteLine("Игра создана");
        while (!board._isFinished)
        {
            if (clientMaster.roomClients[1]!=null)
            {
                while (!board._isFinished)
                {
                    while (!board._isFinished)
                    {
                        Random random = new Random();
                        if (random.Next(2) == 1)     Array.Reverse(clientMaster.roomClients);
                        foreach (var player in clientMaster.roomClients)
                        {
                            board.printGame(player);

                            char team = 'X';
                            if (board.gen % 2 != 0)
                                team = '0';
                            player.Writer.WriteLine($"Ход <{player.userName}>");
                            player.Writer.Flush();
                            int x, y;
                            do
                            {
                                x = Convert.ToInt32(player.Reader.ReadLine()) - 1;
                                Console.WriteLine(x);
                                y = Convert.ToInt32(player.Reader.ReadLine()) - 1;
                                Console.WriteLine(y);
                            }
                            while (!board.markCell(x, y, team, player,clientMaster));
                            board.gen++;
                            Console.WriteLine("Новый ход");
                            if (board._isFinished) break;
                        }

                    }

                }
            }
        }


    }
    protected internal void JoinGame(int GameId,ClientObject client)
    {
        foreach (var clientsMaster in AllClients)
        {
            if (clientsMaster.GameID==GameId && clientsMaster.roomCreate)
            {
                clientsMaster.roomClients[1] = client;
                clientsMaster.roomClients[0].Writer.WriteLine($"Игрок {client.userName} подключился к игре");
                clientsMaster.roomClients[0].Writer.Flush();
                client.Writer.WriteLine($"Вы подключились к игроку: {clientsMaster.roomClients[0].userName}");
                client.Writer.Flush();
                while (true)
                {
                    if (clientsMaster.roomClients[1]==null)
                    {
                        break;
                    }
                }
                break;
            }
        }
            client.Writer.WriteLine("Неправильный код комнаты");

        /*  foreach (var client in clients)
          {
              if (Id == client.Id)
              {
                  GameClients.Add(client);
                  GameClients.First().Writer.WriteLine($"Игрок {userName} подключился к игре");
                  GameClients.First().Writer.Flush();
                  client.Writer.WriteLine($"Вы подключились к игроку: {GameClients.First().userName}");
                  client.Writer.Flush();
                  while (true)
                  {
                      if (GameClients.Count() != 2)
                      {
                          break;
                      }
                  }

              }
          }*/

    }
    // трансляция сообщения подключенным клиентам
    class Board
    {
        private char[,] cells = new char[3, 3] { { ' ', ' ', ' ' }, { ' ', ' ', ' ' }, { ' ', ' ', ' ' } };
        public int gen { get; set; } = 0;
        public bool _isFinished { get; set; } = false;
        public bool isFinished
        {
            get { return _isFinished; }
        }
        public void RefreshBoard()
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    cells[i, j] = ' ';
                }
            }
            gen = -1;
        }
        public bool markCell(int x, int y, char team,ClientObject client,ClientObject clientMaster)
        {
            if (x > 3 || x < 0 || y > 3 || y < 0)
            {
                client.Writer.WriteLine("Вне диапазона доступных позиций");
                client.Writer.Flush();
                return false;
            }
            //throw new Exception("Position is out of range!");
            if (_isFinished)
            {
                client.Writer.WriteLine("Игра окончена");
                client.Writer.Flush();
                return false;
            }
            //throw new Exception("Game is finished!");
            if (cells[x, y] != ' ')
            {
                client.Writer.WriteLine("Клетка занята");
                client.Writer.Flush();
                return false;
            }
            cells[x, y] = team;
            printGame(client);
            checkWinner(team, client,clientMaster);
            return true;
        }
        public void IsDraw(ClientObject clientMaster)
        {
            Array.Reverse(clientMaster.roomClients);
            foreach (var client in clientMaster.roomClients)
            {
                client.Writer.WriteLine($"Ничья");
                client.Writer.Flush();
                if (client!=clientMaster)
                {
                    client.Writer.WriteLine("Введите 1 для реванша, 0 для отмены");
                    client.Writer.Flush();

                    while (true)
                    {
                        switch (client.Reader.ReadLine())
                        {
                            case "1":
                                {
                                    clientMaster.Writer.WriteLine($"{client.userName} согласился на реванш");
                                    client.Writer.Flush();
                                    RefreshBoard();
                                    _isFinished=false;
                                    return;
                                }
                            case "0":
                                {
                                    clientMaster.Writer.WriteLine($"{client.userName} отказался от реванша");
                                    client.Writer.Flush();
                                    Array.Clear(clientMaster.roomClients);
                                    _isFinished =true;
                                    return;
                                }
                            default:
                                client.Writer.WriteLine("Неправильное число");
                                client.Writer.Flush();
                                break;
                        }
                    }
                }
            }
        }
        private void checkWinner(char team, ClientObject client,ClientObject clientMaster)
        {

            // * _ _
            // _ * _
            // _ _ *
            bool trigger = true;
            for (int i = 0; i < 3; i++)
            {
                if (cells[i, i] != team)
                    trigger = false;
            }
            if (trigger)
            {
                defineWinner(client,clientMaster);
                return;
            }
            // _ _ *
            // _ * _
            // * _ _
            trigger = true;
            for (int i = 0; i < 3; i++)
            {
                if (cells[i, 2 - i] != team)
                    trigger = false;
            }
            if (trigger)
            {
                defineWinner(client, clientMaster);
                return;
            }
            // _ * _
            // _ * _
            // _ * _
            bool triggerI = true;
            bool triggerJ = true;
            for (int i = 0; i < 3; i++)
            {
                triggerJ = true;
                for (int j = 0; j < 3; j++)
                {
                    if (cells[i, j] != team)
                        triggerJ = false;
                }
                if (triggerJ)
                {
                    defineWinner(client, clientMaster);
                    return;
                }
            }
            // _ _ _
            // * * *
            // _ _ _
            for (int j = 0; j < 3; j++)
            {
                triggerI = true;
                for (int i = 0; i < 3; i++)
                {
                    if (cells[i, j] != team)
                        triggerI = false;
                }
                if (triggerI)
                {
                    defineWinner(client, clientMaster);
                    return;
                }
            }
            // * * *
            // * * *
            // * * *
            if (gen == 8)
            {
                IsDraw(clientMaster);
                return;
            }

        }
        public void printGame(ClientObject client)
        {
            client.Writer.WriteLine("\n\n======");
            client.Writer.Flush();
            client.Writer.WriteLine($"  {cells[0, 0]}{cells[0, 1]}{cells[0, 2]}\n  {cells[1, 0]}{cells[1, 1]}{cells[1, 2]}\n  {cells[2, 0]}{cells[2, 1]}{cells[2, 2]}");
            client.Writer.Flush();
            client.Writer.WriteLine("======");
            client.Writer.Flush();
        }
        private void defineWinner(ClientObject winClient,ClientObject clientMaster)
        {
            Array.Reverse(clientMaster.roomClients);
            winClient.Writer.WriteLine($"Игрок {winClient.userName} победитель!");
            winClient.Writer.Flush();
            foreach (var loseClient in clientMaster.roomClients)
            {                
                if(winClient!=loseClient)
                {
                    loseClient.Writer.WriteLine($"Игрок {winClient.userName} победитель!");
                    loseClient.Writer.Flush();
                    _isFinished = true;
                    loseClient.Writer.WriteLine("Введите 1 для реванша, 0 для отмены");
                    loseClient.Writer.Flush();
                    while (true)
                    {
                        switch (loseClient.Reader.ReadLine())
                        {
                            case "1":
                                {
                                    winClient.Writer.WriteLine($"{loseClient.userName} согласился на реванш");
                                    winClient.Writer.Flush();
                                    _isFinished = false;
                                    RefreshBoard();
                                    return;
                                }
                            case "0":
                                {
                                    winClient.Writer.WriteLine($"{loseClient.userName} отказался от реванша");
                                    winClient.Writer.Flush();
                                    Array.Clear(clientMaster.roomClients);
                                    return;
                                }
                            default:
                                loseClient.Writer.WriteLine("Неправильное число");
                                loseClient.Writer.Flush();
                                break;
                        }
                    }
                }
                
            }
           
        }
    }

    #endregion
    protected internal async Task BroadcastMessageAsync(string message, string id)
    {
        foreach (var client in AllClients)
        {
            if (client.Id != id) // если id клиента не равно id отправителя
            {
                await client.Writer.WriteLineAsync(message); //передача данных
                await client.Writer.FlushAsync();
            }
        }
    }
    // отключение всех клиентов


}
class ClientObject
{
    protected internal string Id { get; } = Guid.NewGuid().ToString();
    protected internal StreamWriter Writer { get; }
    protected internal StreamReader Reader { get; }
    public string? userName { get; set; }
    public ClientObject[] roomClients = new ClientObject[2];
    public  bool roomCreate = false;
    public int GameID { get; set; }
    TcpClient client;
    ServerObject server; // объект сервера

    public ClientObject(TcpClient tcpClient, ServerObject serverObject)
    {
        client = tcpClient;
        server = serverObject;
        // получаем NetworkStream для взаимодействия с сервером
        var stream = client.GetStream();
        // создаем StreamReader для чтения данных
        Reader = new StreamReader(stream);
        // создаем StreamWriter для отправки данных
        Writer = new StreamWriter(stream);
    }

    public async Task ProcessAsync()
    {
        try
        {
            userName = CheckVerification();
            Writer.Flush();
            while (true)
            {
                Writer.WriteLine("1-Создание комнаты\n2-Подключение к комнате");
                Writer.Flush();
                switch (Reader.ReadLine())
                {
                    case "1": //Создание комнаты
                        {
                            Random random = new Random();
                            GameID = random.Next(1000);
                            Writer.WriteLine($"Id вашей комнаты: {GameID}");
                            Writer.Flush();
                            server.CreateGame(Id, userName,this);
                            break;
                        }
                    case "2": //Подключение к комнате
                        {
                            Writer.WriteLine("Введите Id нужной вам комнаты");
                            Writer.Flush();
                            server.JoinGame(Convert.ToInt32(Reader.ReadLine()),this);
                            break;
                        }
                    default:
                        Writer.WriteLine("Неправильная цифра");
                        Writer.Flush();
                        break;
                }
            }
            // в бесконечном цикле получаем сообщения от клиента
            /* while (true)
             {
                 {
                     string? message = await Reader.ReadLineAsync();
                     if (message == null) continue;
                     Console.WriteLine(message);
                     await server.BroadcastMessageAsync(message, Id);
                 }
             }*/
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        finally
        {
            // в случае выхода из цикла закрываем ресурсы
            server.RemoveConnection(Id);
        }
    }

    protected internal string CheckVerification()
    {
        string[] AllInfo = File.ReadAllLines(@"C:\Users\dima_\source\repos\КПО1\КПО1\Data.txt");
        string[,]? FormedData = new string[AllInfo.Length, 2];
        for (int i = 0; i < AllInfo.Length; i++)
        {
            string[] Temporary = AllInfo[i].Split(' ').ToArray();
            for (int j = 0; j < 2; j++) FormedData[i, j] = Temporary[j];
        }
        Writer.WriteLine("Введите 0, если вы не зарегистрированы");
        Writer.FlushAsync();
        string? _isRegistred=Reader.ReadLine();
        if (_isRegistred != null && _isRegistred!="0")
        {
            while (true)
            {
                Writer.WriteLine("Введите логин");
                Writer.FlushAsync();
                string? userName = Reader.ReadLine();
                for (int i = 0; i < AllInfo.Length; i++)
                {

                    if (userName == FormedData[i, 0])
                    {
                        while (true)
                        {
                            Writer.WriteLine("Введите пароль");
                            Writer.Flush();
                            string? userPassword = Reader.ReadLine();
                            if (userPassword == FormedData[i, 1])
                            {
                                Writer.WriteLine($"Добро пожаловать {FormedData[i, 0]}");
                                Writer.Flush();
                                return FormedData[i, 0];
                            }
                        }
                    }
                }
            }
            
        }
        Writer.WriteLine("Введите логин для регистрации");
        Writer.FlushAsync();
        string? userRegistredName = Reader.ReadLine();
        Writer.WriteLine("Введите пароль для регистрации");
        Writer.FlushAsync();
        string? userRegistredPassword = Reader.ReadLine();
        string? ToFile='\n'+userRegistredName+' '+ userRegistredPassword;
        File.AppendAllText(@"C:\Users\dima_\source\repos\КПО1\КПО1\Data.txt",ToFile);
        return userRegistredName;
    }
    // закрытие подключения
    protected internal void Close()
    {
        Writer.Close();
        Reader.Close();
        client.Close();
    }

}
//