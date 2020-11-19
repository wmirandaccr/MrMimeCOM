using System;
using System.IO.Ports;
using System.Threading;

public class PortChat
{
    static bool _continue;
    private static SerialPort _sourcePortPair;
    private static SerialPort _destinationPort;
    private static bool _useDefaults = true;
    public static void Main()
    {
        string name;
        string message;
        StringComparer stringComparer = StringComparer.OrdinalIgnoreCase;
        Thread readThread = new Thread(Read);

        Thread readThreadDestination = new Thread(ReadDestination);


        // Create a new SerialPort object with default settings.
        _sourcePortPair = new SerialPort();

        Console.Write("*********************************************************************************");
        Console.Write("SOURCE PAIR PORT DEFITIONS");
        Console.Write("*********************************************************************************");

        // Allow the user to set the appropriate properties.
        _sourcePortPair.PortName = SetPortName(_sourcePortPair.PortName);
        _sourcePortPair.BaudRate = SetPortBaudRate(_sourcePortPair.BaudRate);
        _sourcePortPair.Parity = SetPortParity(_sourcePortPair.Parity);
        _sourcePortPair.DataBits = SetPortDataBits(_sourcePortPair.DataBits);
        _sourcePortPair.StopBits = SetPortStopBits(_sourcePortPair.StopBits);
        _sourcePortPair.Handshake = SetPortHandshake(_sourcePortPair.Handshake);

        // Set the read/write timeouts
        _sourcePortPair.ReadTimeout = 5000;
        _sourcePortPair.WriteTimeout = 5000;

        _sourcePortPair.Open();
    

        Console.WriteLine("********************************************************************************");
        Console.WriteLine("DESTINATION PORT DEFITIONS");
        Console.WriteLine("********************************************************************************");

        _destinationPort = new SerialPort();

        // Allow the user to set the appropriate properties.
        _destinationPort.PortName = SetPortName(_destinationPort.PortName);
        if (_destinationPort.PortName != "COMX")
        {
            _destinationPort.BaudRate = SetPortBaudRate(_destinationPort.BaudRate);
            _destinationPort.Parity = SetPortParity(_destinationPort.Parity);
            _destinationPort.DataBits = SetPortDataBits(_destinationPort.DataBits);
            _destinationPort.StopBits = SetPortStopBits(_destinationPort.StopBits);
            _destinationPort.Handshake = SetPortHandshake(_destinationPort.Handshake);
            _destinationPort.Open();
        }
        

        _continue = true;
        if (_destinationPort.PortName != "COMX")
            readThreadDestination.Start();
        readThread.Start();
        


        Console.WriteLine("Type QUIT to exit");

        while (_continue)
        {
            Console.WriteLine("Send Message to <{0}>: ", _sourcePortPair.PortName);
            message = Console.ReadLine();

            if (stringComparer.Equals("quit", message))
            {
                _continue = false;
            }
            else
            {
                _sourcePortPair.WriteLine(
                    String.Format("{1}", _sourcePortPair.PortName, message));
            }
        }

        readThread.Join();
        _sourcePortPair.Close();
    }

    public static void ReadDestination()
    {
        while (_continue)
        {
            try
            {
                string message = string.Format("{1}", _destinationPort.PortName, _destinationPort.ReadLine());              
                Console.WriteLine(message);
                // Preparing RX  - comment to avoid loop in case of using two pairs of ports.
                _sourcePortPair.WriteLine(string.Format("{1}", _destinationPort.PortName, message));
            }
            catch (TimeoutException) { }
        }
    }

    public static void Read()
    {
        while (_continue)
        {
            try
            {
                string message = string.Format("{1}", _sourcePortPair.PortName, _sourcePortPair.ReadLine());
                Console.WriteLine(message);
                if (_destinationPort.PortName != "COMX")
                    _destinationPort.WriteLine(string.Format("{1}",_destinationPort.PortName , message.Replace("Cartao credito", "PIX")));
            }
            catch (TimeoutException) { }
        }
    }

    // Display Port values and prompt user to enter a port.
    public static string SetPortName(string defaultPortName)
    {
        string portName;

        Console.WriteLine("Available Ports:");
        foreach (string s in SerialPort.GetPortNames())
        {
            Console.WriteLine("   {0}", s);
        }

        Console.WriteLine("Enter COM port value (Default: {0}): ", defaultPortName);
        portName = Console.ReadLine().ToUpper();

        if (portName == "" )
        {
            portName = defaultPortName;
        }
        return portName;
    }
    // Display BaudRate values and prompt user to enter a value.
    public static int SetPortBaudRate(int defaultPortBaudRate)
    {
        string baudRate = "";

        Console.WriteLine("Baud Rate(default:{0}): ", defaultPortBaudRate);
        if (!PortChat._useDefaults)
            baudRate = Console.ReadLine();

        if (baudRate == "")
        {
            baudRate = defaultPortBaudRate.ToString();
        }

        return int.Parse(baudRate);
    }

    // Display PortParity values and prompt user to enter a value.
    public static Parity SetPortParity(Parity defaultPortParity)
    {
        string parity = "";

        Console.WriteLine("Available Parity options:");
        foreach (string s in Enum.GetNames(typeof(Parity)))
        {
            Console.WriteLine("   {0}", s);
        }

        Console.WriteLine("Enter Parity value (Default: {0}):", defaultPortParity.ToString(), true);
        if (!PortChat._useDefaults)
            parity = Console.ReadLine();

        if (parity == "")
        {
            parity = defaultPortParity.ToString();
        }

        return (Parity)Enum.Parse(typeof(Parity), parity, true);
    }
    // Display DataBits values and prompt user to enter a value.
    public static int SetPortDataBits(int defaultPortDataBits)
    {
        string dataBits = "";

        Console.WriteLine("Enter DataBits value (Default: {0}): ", defaultPortDataBits);
        if (!PortChat._useDefaults)
            dataBits = Console.ReadLine();

        if (dataBits == "")
        {
            dataBits = defaultPortDataBits.ToString();
        }

        return int.Parse(dataBits.ToUpperInvariant());
    }

    // Display StopBits values and prompt user to enter a value.
    public static StopBits SetPortStopBits(StopBits defaultPortStopBits)
    {
        string stopBits = "";

        Console.WriteLine("Available StopBits options:");
        foreach (string s in Enum.GetNames(typeof(StopBits)))
        {
            Console.WriteLine("   {0}", s);
        }

        Console.WriteLine("Enter StopBits value (None is not supported and \n" +
         "raises an ArgumentOutOfRangeException. \n (Default: {0}):", defaultPortStopBits.ToString());
        if (!PortChat._useDefaults)
            stopBits = Console.ReadLine();

        if (stopBits == "")
        {
            stopBits = defaultPortStopBits.ToString();
        }

        return (StopBits)Enum.Parse(typeof(StopBits), stopBits, true);
    }
    public static Handshake SetPortHandshake(Handshake defaultPortHandshake)
    {
        string handshake = "";

        Console.WriteLine("Available Handshake options:");
        foreach (string s in Enum.GetNames(typeof(Handshake)))
        {
            Console.WriteLine("   {0}", s);
        }

        Console.WriteLine("Enter Handshake value (Default: {0}):", defaultPortHandshake.ToString());
        if (!PortChat._useDefaults)
            handshake = Console.ReadLine();

        if (handshake == "")
        {
            handshake = defaultPortHandshake.ToString();
        }

        return (Handshake)Enum.Parse(typeof(Handshake), handshake, true);
    }
}
