using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Xml;

namespace PacketGenerator
{
    // S가 붙으면 서버 -> 클라이언트 / C가 붙으면 클라이언트 -> 서버 
    public class Program
    {
        static string genPacket;
        static ushort packetId;
        static string packetEnums;

        static string serverRegister; 
        static string clientRegister;

        static public void Init(string[] args)
        {
            string pdlPath = "PDL.xml";

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true; // 주석 무시
            settings.IgnoreWhitespace = true; // 스페이스 바 무시

            if (args.Length > 0)
            {
                pdlPath = args[0];  // "..\..\PacketGenerator\PDL.xml"
                Console.WriteLine("Received file path: " + pdlPath);
            }

            XmlReader r = null ;
            try
            {
                string fullPath = Path.GetFullPath(pdlPath);
                r = XmlReader.Create(fullPath, settings);
                {
                    r.MoveToContent();

                    while (r.Read())
                    {
                        if (r.Depth == 1 && r.NodeType == XmlNodeType.Element)
                            ParsePacket(r);

                        Console.WriteLine(r.Name);
                    }
                }
                string fileText = string.Format(PacketFormat.fileFormat, packetEnums, genPacket);
                File.WriteAllText("GenPackets.cs", fileText);

                string clientManagerText = string.Format(PacketFormat.managerFormat, clientRegister);
                string serverManagerText = string.Format(PacketFormat.managerFormat, serverRegister);
                File.WriteAllText("ClientPacketManager.cs", clientManagerText);
                File.WriteAllText("ServerPacketManager.cs", serverManagerText);
            }
            finally
            {
                if (r != null)
                   r.Dispose(); // 리소스 해제
            }


        }

        static public void batFileStart()
        {
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = @"Common\\Packet\\GenPackets.bat";
            psi.UseShellExecute = true; // true면 cmd창이 뜸
            psi.WindowStyle = ProcessWindowStyle.Normal; // 또는 Hidden

            try
            {
                Process.Start(psi);
            }
            catch (System.Exception e)
            {
                Console.WriteLine("BAT 실행 실패: " + e.Message);
            }
        }
        public static void ParsePacket(XmlReader r)
        {
            if (r.NodeType == XmlNodeType.EndElement)
                return;

            if (r.Name.ToLower() != "packet")
            {
                Console.WriteLine("Invaild packet node");
                return;
            }

            string packetName = r["name"];
            if (string.IsNullOrEmpty(packetName))
            {
                Console.WriteLine("Packet without name");
                return;
            }

            Tuple<string, string, string> t = ParseMembers(r);
            genPacket += string.Format(PacketFormat.packetFormat, packetName, t.Item1, t.Item2, t.Item3);
            packetEnums += string.Format(PacketFormat.packetEnumFormat, packetName, ++packetId) + Environment.NewLine +"\t";
            
            if(packetName.StartsWith("S_") || packetName.StartsWith("s_"))
                serverRegister += string.Format(PacketFormat.managerRegisterFormat, packetName) + Environment.NewLine;
            else
                clientRegister += string.Format(PacketFormat.managerRegisterFormat, packetName) + Environment.NewLine;
        }

        public static Tuple<string, string, string> ParseMembers(XmlReader r)
        {
            string packetName = r["name"];
            string memberCode = "";
            string readCode = "";
            string writeCode = "";

            int depth = r.Depth + 1;
            while (r.Read())
            {
                if (r.Depth != depth)
                    break;

                string memberName = r["name"];
                if (string.IsNullOrEmpty(memberName))
                {
                    Console.WriteLine("Member without name");
                    return null;
                }

                if (string.IsNullOrEmpty(memberCode) == false)
                    memberCode += Environment.NewLine;

                string memberType = r.Name.ToLower();
                switch (memberType)
                {
                    case "short":
                    case "ushort":
                        memberCode += string.Format(PacketFormat.memberFormat, memberType, memberName);
                        readCode += string.Format(PacketFormat.readByteFormat, memberName, memberType);
                        writeCode += string.Format(PacketFormat.writeByteFormat, memberName, memberType);
                        break;
                    case "bool":
                    case "int":
                    case "long":
                    case "float":
                    case "double":
                        memberCode += string.Format(PacketFormat.memberFormat, memberType, memberName);
                        readCode += string.Format(PacketFormat.readFormat, memberName, ToMemberType(memberType), memberType);
                        writeCode += string.Format(PacketFormat.writeFormat, memberName, memberType);
                        break;
                    case "string":
                        memberCode += string.Format(PacketFormat.memberFormat, memberType, memberName);
                        readCode += string.Format(PacketFormat.readStringFormat, memberName);
                        writeCode += string.Format(PacketFormat.writeStringFormat, memberName);
                        break;
                    case "list":
                        Tuple<string, string, string> t = ParseList(r);
                        memberCode += t.Item1;
                        readCode += t.Item2;
                        writeCode += t.Item3;
                        break;
                    default:
                        break;
                }
            }
            memberCode = memberCode.Replace("\n", "\n\t");
            readCode = readCode.Replace("\n", "\n\t\t");
            writeCode = writeCode.Replace("\n", "\n\t\t");
            return new Tuple<string, string, string>(memberCode, readCode, writeCode);
        }

        public static string ToMemberType(string memberType)
        {
            switch (memberType)
            {
                case "bool":
                    return "ToBoolean";
                case "short":
                    return "ToInt16";
                case "ushort":
                    return "ToUInt16";
                case "int":
                    return "ToInt32";
                case "long":
                    return "ToInt64";
                case "float":
                    return "ToSingle";
                case "double":
                    return "ToDouble";
                default:
                    return "";

            }
        }

        public static Tuple<string, string, string> ParseList(XmlReader r)
        {
            string listName = r["name"];
            if (string.IsNullOrEmpty(listName))
            {
                Console.WriteLine("List without name");
                return null;
            }


            Tuple<string, string, string> t = ParseMembers(r);

            string memberCode = string.Format(PacketFormat.memberListFormat,
                FirstCharToUper(listName),
                FirstCharToLower(listName),
                t.Item1,
                t.Item2,
                t.Item3);

            string readCode = string.Format(PacketFormat.readListFormat,
                FirstCharToUper(listName),
                FirstCharToLower(listName));

            string writeCode = string.Format(PacketFormat.writeListFormat,
                FirstCharToUper(listName),
                FirstCharToLower(listName));

            return new Tuple<string, string, string>(memberCode, readCode, writeCode);
        }


        public static string FirstCharToUper(string input)
        {
            if (string.IsNullOrEmpty(input))
                return "";

            return input[0].ToString().ToUpper() + input.Substring(1);
        }


        public static string FirstCharToLower(string input)
        {
            if (string.IsNullOrEmpty(input))
                return "";

            return input[0].ToString().ToLower() + input.Substring(1);
        }
    }
}

