﻿using System;
using System.IO;
using System.Xml;

namespace  PacketGenerator
{ 
    // json, xml, 기타 자체 정의, xml이 json에 비해서 hierarchy가 편하게 보여서 패킷은 xml을 선호
    class Program
    {
        static string? genPackets;

        static ushort packetId; // 0번 부터 시작해서 +1씩 늘려줄거임. 해킹 대비해서 shuffle 해도됨
        static string? packetEnums;

        static string? clientRegister;
        static string? serverRegister;

        static void Main(string[] args)
        {
            string pdlPath = "../../../PDL.xml";
                
            XmlReaderSettings settings = new XmlReaderSettings()
            {
                IgnoreComments = true,
                IgnoreWhitespace = true
            };

            if(args.Length >= 1) pdlPath = args[0];

            using (XmlReader r = XmlReader.Create(pdlPath, settings))
            {
                r.MoveToContent();

                // stream 방식으로 한줄 한줄 읽음 r.Name r["name"]
                while (r.Read()) 
                {
                    // Packet인지 아닌지 먼저 구분하고 시작, 마지막에 있는 Packet tag를 무시하기 위해서 
                    // XmlNodeType.Element가 시작하는것이고, XmlNodeType.EndElement는 끝나는것
                    if (r.Depth == 1 && r.NodeType == XmlNodeType.Element)
                        ParsePacket(r);

                    // Console.WriteLine(r.Name + " " + r["name"]);
                }

                string FileText = string.Format(PacketFormat.fileFormat, packetEnums, genPackets);
                File.WriteAllText("GenPackets.cs", FileText);

                string clientManagerText = string.Format(PacketFormat.managerFormat, clientRegister);
                File.WriteAllText("ClientPacketManager.cs", clientManagerText);
                string serverManagerText = string.Format(PacketFormat.managerFormat, serverRegister);
                File.WriteAllText("ServerPacketManager.cs", serverManagerText);
            }
        }

        public static void ParsePacket(XmlReader r)
        {
            if (r.NodeType == XmlNodeType.EndElement) return;
            if (r.Name.ToLower() != "packet") // packet인지 재차 확인
            {
                Console.WriteLine("Invalid packet node");
                return;
            }

            string? packetName = r["name"];
            if (string.IsNullOrEmpty(packetName))
            {
                Console.WriteLine("Packet without name");
                return;
            }

            Tuple<string, string, string>? t = ParseMembers(r);
            if (t == null) return;

            genPackets += string.Format(PacketFormat.packetFormat, packetName, t.Item1, t.Item2, t.Item3);
            packetEnums += string.Format(PacketFormat.packetEnumFormat, packetName, ++packetId) + Environment.NewLine + "\t";
            
            if(packetName.StartsWith("S_") || packetName.StartsWith("s_"))
                clientRegister += string.Format(PacketFormat.managerRegisterFormat, packetName) + Environment.NewLine;
            else
                serverRegister += string.Format(PacketFormat.managerRegisterFormat, packetName) + Environment.NewLine;
        }

        public static Tuple<string, string, string>? ParseMembers(XmlReader r)
        {
            string? packetName = r["name"];

            string? memberCode = "";
            string? readCode = "";
            string? writeCode = "";

            int depth = r.Depth + 1;
            while(r.Read())
            {
                // 바로 아래 단계가 아니면 스킵
                if (r.Depth != depth) break;

                string? memberName = r["name"];
                if(string.IsNullOrEmpty(memberName))
                {
                    Console.WriteLine("Member without name");
                    return null;
                }

                // formatting 개행 문자 삽입
                if (string.IsNullOrEmpty(memberCode) == false) memberCode += Environment.NewLine;
                if (string.IsNullOrEmpty(readCode) == false) readCode += Environment.NewLine;
                if (string.IsNullOrEmpty(writeCode) == false) writeCode += Environment.NewLine;

                string memberType = r.Name.ToLower();
                switch(memberType)
                {
                    case "byte":
                    case "sbyte":
                        memberCode += string.Format(PacketFormat.memberFormat, memberType, memberName);
                        readCode += string.Format(PacketFormat.readByteFormat, memberName, memberType);
                        writeCode += string.Format(PacketFormat.writeByteFormat, memberName, memberType);
                        break;

                    case "bool":
                    case "short":
                    case "ushort":
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
                        Tuple<string, string, string>? t = ParseList(r);
                        if (t != null)
                        {
                            memberCode += t.Item1;
                            readCode += t.Item2;
                            writeCode += t.Item3;
                        }
                        break;
                    default:
                        break;
                }
            }

            // formatting
            memberCode = memberCode.Replace("\n", "\n\t");
            readCode = readCode.Replace("\n", "\n\t\t");
            writeCode = writeCode.Replace("\n", "\n\t\t");
            return new Tuple<string, string, string>(memberCode, readCode, writeCode);
        }

        public static Tuple<string, string, string>? ParseList(XmlReader r)
        {
            string? listName = r["name"];
            if(string.IsNullOrEmpty(listName))
            {
                Console.WriteLine("List without name");
                return null;
            }

            Tuple<string , string, string>? t = ParseMembers(r);
            if (t == null) return null;

            string memberCode = string.Format(PacketFormat.memberListFormat, 
                FirstCharToUpper(listName),
                FirstCharToLower(listName),
                t.Item1,
                t.Item2,
                t.Item3);

            string readCode = string.Format(PacketFormat.readListFormat,
                FirstCharToUpper(listName),
                FirstCharToLower(listName));

            string writeCode = string.Format(PacketFormat.writeListFormat,
                FirstCharToUpper(listName),
                FirstCharToLower(listName));

            return new Tuple<string, string, string>(memberCode, readCode, writeCode);
        }

        public static string ToMemberType(string memberType)
        {
            switch (memberType)
            {
                case "bool":
                    return "ToBoolean";
                // case "byte": byte배열에서 byte배열로 변환하는걸 따로 정의할 필요 없다. 
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

        public static string FirstCharToUpper(string? input)
        {
            if (string.IsNullOrEmpty(input)) return "";
            return input[0].ToString().ToUpper() + input.Substring(1);
        }

        public static string FirstCharToLower(string? input)
        {
            if (string.IsNullOrEmpty(input)) return "";
            return input[0].ToString().ToLower() + input.Substring(1);
        }
    }
}
