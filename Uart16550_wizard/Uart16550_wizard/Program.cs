using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace coal_usb_uart_console
{
    class Program
    {
        static int Main(string[] args)
        {
            string[] coms = SerialPort.GetPortNames();
            Console.WriteLine("Обнаружены порты:");
            foreach (string s in coms)
                Console.WriteLine(s);
            SerialPort port = new SerialPort("COM5", 115200, Parity.None, 8, StopBits.One);
            try
            {
                port.Open();
                Console.WriteLine("Открыт порт " + port.PortName + ".");
            }
            catch
            {
                Console.WriteLine("Открыть " + port.PortName + " не удалось.");
                return 1;
            }

            string BaseAddress = "40000000";
            string WrData = "00000000";

            byte[] AddBytes = new byte[WrData.Length / 2];

            for (int i = 0; i < BaseAddress.Length; i += 2)
                AddBytes[i / 2] = Convert.ToByte(BaseAddress.Substring(i, 2), 16);



            byte[] DataBytes = new byte[WrData.Length / 2];
            for (int i = 0; i < WrData.Length; i += 2)
                DataBytes[i / 2] = Convert.ToByte(WrData.Substring(i, 2), 16);



            byte[] trnData = { 36, AddBytes[3], AddBytes[2], AddBytes[1], AddBytes[0], DataBytes[3], DataBytes[2], DataBytes[1], DataBytes[0] };
            //byte[] trnData = { 32, AddBytes[3], AddBytes[2], AddBytes[1], AddBytes[0]};



            port.Write(trnData, 0, trnData.Length);
            Console.Write("Отправлено: ");
            foreach (byte b in trnData)
                Console.Write(b.ToString() + " ");
            Console.WriteLine();

            byte[] rcvData = new byte[256];
            Console.WriteLine("Ожидание приема.");

            Console.Write("Принято: ");
            int l = 0;
            for (int i = 0; i < trnData.Length; i += l)
            {
                l = port.Read(rcvData, i, (4 * trnData.Length) - i);
                for (int j = 0; j < l; j++)
                    Console.Write(rcvData[i + j].ToString() + " ");
            }
            Console.WriteLine();
            /*  for (int i = 0; i < 8; i += l)
             {
                 l = port.Read(rcvData, i, 11 - i);
                 for (int j = 0; j < l; j++)
                     Console.Write(rcvData[i + j].ToString() + " ");
             }
             Console.WriteLine();*/

            port.Close();

            Console.WriteLine("Программа выполнена. Нажмите <Enter>");
            Console.ReadLine();
            return 0;
        }
    }
}
