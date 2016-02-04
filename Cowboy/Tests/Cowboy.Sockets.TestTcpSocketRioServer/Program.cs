﻿using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cowboy.Logging;
using Cowboy.Logging.NLogIntegration;
using Cowboy.Sockets.Experimental;

namespace Cowboy.Sockets.TestTcpSocketRioServer
{
    class Program
    {
        static TcpSocketRioServer _server;

        static void Main(string[] args)
        {
            NLogLogger.Use();

            try
            {
                var config = new TcpSocketRioServerConfiguration();

                _server = new TcpSocketRioServer(22222, new SimpleMessageDispatcher(), config);
                _server.Listen();

                Console.WriteLine("TCP server has been started on [{0}].", _server.ListenedEndPoint);
                Console.WriteLine("Type something to send to clients...");
                while (true)
                {
                    try
                    {
                        string text = Console.ReadLine();
                        if (text == "quit")
                            break;
                        Task.Run(async () =>
                        {
                            await _server.BroadcastAsync(Encoding.UTF8.GetBytes(text));
                            Console.WriteLine("Server [{0}] broadcasts text -> [{1}].", _server.ListenedEndPoint, text);
                        });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }

                _server.Shutdown();
                Console.WriteLine("TCP server has been stopped on [{0}].", _server.ListenedEndPoint);
            }
            catch (Exception ex)
            {
                Logger.Get<Program>().Error(ex.Message, ex);
            }

            Console.ReadKey();
        }
    }
}
