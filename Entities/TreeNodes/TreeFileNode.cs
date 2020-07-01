using System;
using System.IO;
using Entities.DataObjects;
using Entities.Commands;

namespace Entities.TreeNodes
{
    // узел файла в дереве
    [Serializable]
    public class TreeFileNode : TreeNode
    {
        public TreeFileNode(string name) : base(name)
        {
        }

        // метод для рассчёта хеша файла
        public override string CalculateHash(string rootLocation)
        {
            if (Constants.Debug) Console.WriteLine($"CalculateHash File: {{ {this} }};");
            string hash;
            using (FileStream stream =
                new FileStream(Path.Combine(rootLocation, RelativeLocation, Name), FileMode.Open))
            {
                // используя поток файла строим MD5
                hash = Utils.CreateMD5(stream);
            }

            Hash = hash;
            return hash;
        }

        // метод построения хешированного HTML
        public override string WrapHtml(string rootLocation)
        {
            if (Constants.Debug) Console.WriteLine($"WrapHtml File: {{ {this} }};");

            // строим хеш и запаковываем его
            string hash = CalculateHash(rootLocation);
            string selfHtml = $"<li>{Name} {hash}</li>";

            return selfHtml;
        }

        // метод построения хешированного XML
        public override string WrapHashedXML(string rootLocation, bool ignoreRoot = true)
        {
            if (Constants.Debug) Console.WriteLine($"WrapHashedXML File: {{ {this} }};");
            
            // строим хеш и запаковываем его
            string hash = CalculateHash(rootLocation);
            string selfXml = $"<file>{Name} {hash}</file>";

            return selfXml;
        }

        // рекурсивный метод загрузки файла
        public override void Download(string rootLocation, IRequestPerformer requestPerformer, bool ignoreRoot = true)
        {
            if (Constants.Debug) Console.WriteLine($"Download File: {{ {this} }};");

            // создаём объект для получения размера файла
            var fileSizeData = new FileSizeData(RelativeLocation, Name);

            // создаём команду и выполняем
            var getFileSizeCommand = new CommandGetFileSize(fileSizeData);
            var getFileSizeCommandBytes = getFileSizeCommand.Serialize();
            var responseGetFileSizeCommandBytes = requestPerformer.PerformRequest(getFileSizeCommandBytes);
            var responseGetFileSizeCommand =
                (CommandGetFileSizeResponse) Command.Deserialize(responseGetFileSizeCommandBytes);

            // узнаём размер файла на сервере
            int fileSize = responseGetFileSizeCommand.GetData();
            
            // создаём поток к файлу
            FileStream fs = new FileStream(Path.Combine(rootLocation, RelativeLocation, Name), FileMode.CreateNew);

            // пока не получили весь файл
            int received = 0;
            while (received != fileSize)
            {
                // считаем размер пакета загрузки
                int downloadSize;
                if (fileSize - received < Constants.SendingFilePieceSize)
                {
                    downloadSize = fileSize - received;
                }
                else
                {
                    downloadSize = Constants.SendingFilePieceSize;
                }

                // создаём команду получения части файла и выполняем её
                var getFilePieceCommand =
                    new CommandGetFilePiece(new FilePieceDataLocation(RelativeLocation, Name, received, downloadSize));
                var getFilePieceCommandBytes = getFilePieceCommand.Serialize();
                var responseGetFilePieceCommandBytes = requestPerformer.PerformRequest(getFilePieceCommandBytes);
                var responseGetFilePieceCommand =
                    (CommandGetFilePieceResponse) Command.Deserialize(responseGetFilePieceCommandBytes);

                var pieceBytes = responseGetFilePieceCommand.GetData();

                // записываем в поток полученные данные
                fs.Write(pieceBytes, 0, downloadSize);

                received += downloadSize;
            }

            // обязательно закрываем поток
            fs.Close();
        }

        // рекурсивный метод выгрузки папки
        public override void Upload(string rootLocation, IRequestPerformer requestPerformer, bool ignoreRoot = true)
        {
            if (Constants.Debug) Console.WriteLine($"Upload File: {{ {this} }};");

            // создаём команду сравнения файла и выполняем её
            var fileComparisonData = new FileComparisonData(RelativeLocation, Name, Hash);
            var isFileDiffersCommand = new CommandIsFilesEqual(fileComparisonData);
            var isFileDiffersCommandBytes = isFileDiffersCommand.Serialize();
            var responseIsFileDiffersCommandBytes = requestPerformer.PerformRequest(isFileDiffersCommandBytes);
            var responseIsFilesEqualCommand =
                (CommandIsFilesEqualResponse) Command.Deserialize(responseIsFileDiffersCommandBytes);

            // если файлы не равны
            if (!responseIsFilesEqualCommand.GetData())
            {
                Console.WriteLine($"Performing upload {Name}");

                // открываем поток к локальному файлу
                FileStream fs = new FileStream(Path.Combine(rootLocation, RelativeLocation, Name), FileMode.Open);
                int fileSize = (int) fs.Length; // длина файла

                // cоздаём буфер
                byte[] buffer = new byte[Constants.SendingFilePieceSize];

                // пока не отправили весь файл
                int sent = 0;
                while (sent != fileSize)
                {
                    // считаем размер пакета отправки
                    int uploadSize;
                    if (fileSize - sent < Constants.SendingFilePieceSize)
                    {
                        uploadSize = fileSize - sent;
                        buffer = new byte[uploadSize];
                    }
                    else
                    {
                        uploadSize = Constants.SendingFilePieceSize;
                    }

                    // читаем заданное количество байт из файла в буфер
                    fs.Read(buffer, 0, uploadSize);

                    // создаём команду выгрузки части файла и выполняем её
                    var uploadFilePieceCommand =
                        new CommandUploadFilePiece(new FilePieceData(RelativeLocation, Name, sent, uploadSize, buffer));
                    var uploadFilePieceCommandBytes = uploadFilePieceCommand.Serialize();
                    var responseUploadFilePieceCommandBytes =
                        requestPerformer.PerformRequest(uploadFilePieceCommandBytes);
                    // ignore response, it's empty

                    sent += uploadSize;
                }

                // закрываем файл
                fs.Close();
            }
            else
            {
                // файлы равны
                Console.WriteLine($"File is in sync {this}");
            }
        }
    }
}