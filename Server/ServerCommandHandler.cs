using System;
using System.IO;
using Entities;
using Entities.Commands;
using Entities.TreeNodes;

namespace Server
{
    // Обработчик команд на сервере
    public class ServerCommandHandler
    {
        // абсолютное положение пользовательской папки 
        private const string SharedFolderLocation = "C:\\Projects\\CSharp\\WebDisk\\Server\\bin\\Debug";

        // название пользовательской папки
        private const string SharedFolderName = "shared";

        // стандартный метод-роутер обработчика любых команд
        public static Command Handle(Command command)
        {
            if (command is CommandSimpleMessage commandSimpleMessage)
            {
                if (Constants.Debug) Console.WriteLine("Server Processing { CommandSimpleMessage }");

                // Обработка простого сообщения (не используется, только для тестов)
                Console.WriteLine(commandSimpleMessage.GetData());

                // результата операции нет - (аналог void)
                return new CommandNone();
            }
            else if (command is CommandCompareHash commandCompareHash)
            {
                if (Constants.Debug) Console.WriteLine("Server Processing { CommandCompareHash }");

                // обработка сравнения хешей двух деревьев
                var treeEquals =
                    TreeAnalyzer.BuildTree(SharedFolderLocation, SharedFolderName) // строим дерево
                        .CalculateHash(SharedFolderLocation) == commandCompareHash.GetData(); // сравниваем хеши

                // собираем ответ
                CommandCompareHashResponse commandCompareHashResponse = new CommandCompareHashResponse(treeEquals);
                return commandCompareHashResponse;
            }
            else if (command is CommandGetTree commandGetTree)
            {
                if (Constants.Debug) Console.WriteLine("Server Processing { CommandGetTree }");

                // обработка получения серверного дерева
                CommandGetTreeResponse commandGetTreeResponse =
                    new CommandGetTreeResponse(TreeAnalyzer.BuildTree(SharedFolderLocation,
                        SharedFolderName)); // строим дерево и возвращаем его
                return commandGetTreeResponse;
            }
            else if (command is CommandGetFileSize commandGetFileSize)
            {
                if (Constants.Debug) Console.WriteLine("Server Processing { CommandGetFileSize }");

                // обработка получения размера файла
                var fileSizeData = commandGetFileSize.GetData();

                // получаем информацию о файле
                FileInfo info = new FileInfo(Path.Combine(SharedFolderLocation, fileSizeData.RelativeLocation,
                    fileSizeData.Name));

                if (!info.Exists)
                {
                    // если файл не существует
                    throw new InvalidOperationException("File doesn't exist");
                }

                int size = (int) info.Length; // получаем размер файла
                CommandGetFileSizeResponse
                    commandGetFileSizeResponse = new CommandGetFileSizeResponse(size); // запаковываем и возвращаем
                return commandGetFileSizeResponse;
            }
            else if (command is CommandGetFilePiece commandGetFilePiece)
            {
                if (Constants.Debug) Console.WriteLine("Server Processing { CommandGetFilePiece }");

                // обработка получения части файла
                var filePieceData = commandGetFilePiece.GetData();

                var path = Path.Combine(SharedFolderLocation, filePieceData.RelativeLocation, filePieceData.Name);

                if (!File.Exists(path))
                {
                    // если файл не существует
                    throw new InvalidOperationException("File doesn't exist");
                }

                // открываем поток к файлу
                FileStream fs = new FileStream(path, FileMode.Open);
                fs.Seek(filePieceData.Offset, SeekOrigin.Begin); // смещаемся от начало на заданный отступ

                // создаём буффер и читаем в него заданное количество байт
                byte[] buffer = new byte[filePieceData.Size];
                fs.Read(buffer, 0, filePieceData.Size);
                fs.Close();

                // запаковываем буфер и возвращаем
                CommandGetFilePieceResponse commandGetFilePieceResponse = new CommandGetFilePieceResponse(buffer);
                return commandGetFilePieceResponse;
            }
            else if (command is CommandUploadFilePiece commandUploadFilePiece)
            {
                if (Constants.Debug) Console.WriteLine("Server Processing { CommandUploadFilePiece }");

                // обработка загрузки части файла на сервер 
                var filePieceData = commandUploadFilePiece.GetData();

                var path = Path.Combine(SharedFolderLocation, filePieceData.RelativeLocation, filePieceData.Name);

                if (!File.Exists(path))
                {
                    // если файл не существует
                    throw new InvalidOperationException("File doesn't exist");
                }

                FileStream fs;

                if (filePieceData.Offset == 0)
                {
                    // если отступ в файле 0, то мы создаём файл заново (FileMode.Create)
                    Console.WriteLine($"Server created file: {{ {filePieceData.Name} }}");
                    fs = new FileStream(path, FileMode.Create);
                }
                else
                {
                    // если отступ в файле НЕ 0, то мы создаём файл добавляем в файл (FileMode.Append)
                    fs = new FileStream(path, FileMode.Append);
                }

                // записываем переданный буффер
                fs.Write(filePieceData.Data, 0, filePieceData.Size);
                fs.Flush(true); // форсированно записываем весь поток на диск

                fs.Close();

                // результата операции нет - (аналог void)
                return new CommandNone();
            }
            else if (command is CommandIsFilesEqual commandIsFilesEqual)
            {
                if (Constants.Debug) Console.WriteLine("Server Processing { CommandIsFilesEqual }");

                // обработка сравнения двух файлов
                var fileComparisonData = commandIsFilesEqual.GetData();

                string path = Path.Combine(SharedFolderLocation, fileComparisonData.RelativeLocation,
                    fileComparisonData.Name);

                if (File.Exists(path))
                {
                    // Если файл существует - открываем его
                    FileStream fs = new FileStream(path, FileMode.Open);

                    // строим хеш локального файла (только от контента файла, нам нет нужды использовать ещё и название, расхождение невозможно)
                    var localHash = Utils.CreateMD5(fs);
                    fs.Close(); // сразу закрываем поток

                    // сравниваем локальный хеш с переданным
                    if (localHash == fileComparisonData.Hash)
                    {
                        return new CommandIsFilesEqualResponse(true);
                    }
                    else
                    {
                        return new CommandIsFilesEqualResponse(false);
                    }
                }
                else
                {
                    // если файла на сервере нет вообще - он есть только на клиенте, полностью отличается
                    return new CommandIsFilesEqualResponse(false);
                }
            }
            else if (command is CommandCreateFolder commandCreateFolder)
            {
                if (Constants.Debug) Console.WriteLine("Server Processing { CommandCreateFolder }");

                // обработка создания папки
                var createFolderData = commandCreateFolder.GetData();

                var path = Path.Combine(SharedFolderLocation, createFolderData.RelativeLocation, createFolderData.Name);

                if (!Directory.Exists(path))
                {
                    // если папки нет - создаём её
                    Directory.CreateDirectory(path);

                    Console.WriteLine(
                        $"Server created folder: {{ {Path.Combine(createFolderData.RelativeLocation, createFolderData.Name)} }}");
                }

                // результата операции нет - (аналог void)
                return new CommandNone();
            }
            else if (command is CommandDeleteNonExistent commandDeleteNonExistent)
            {
                if (Constants.Debug) Console.WriteLine("Server Processing { CommandDeleteNonExistent } ");

                // обработка удаления несуществующих файлов для узлов
                var remoteTree = commandDeleteNonExistent.GetData();

                // вызываем удаления
                remoteTree.DeleteNonExistent(SharedFolderLocation);

                // результата операции нет - (аналог void)
                return new CommandNone();
            }

            if (Constants.Debug) Console.WriteLine("Server Processing Error { Found unsupported command }");

            // не нашёлся обработчик - возвращаем пустоту (ALERT!)
            return new CommandNone();
        }
    }
}