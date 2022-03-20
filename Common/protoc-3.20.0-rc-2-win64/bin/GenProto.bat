protoc.exe -I=./ --csharp_out=./ ./Protocol.proto
IF ERRORLEVEL 1 PAUSE

START ../../../Server/PacketGenerator/bin/Debug/net6.0/PacketGenerator.exe ./Protocol.proto

XCOPY /Y Protocol.cs "../../../Client/Assets/Scripts/Packet"
XCOPY /Y Protocol.cs "../../../Server/Server/Packet"

REM PacketGenerator.exe 을 실행하면 ClientPacketManager.cs와 ServerPacketManager.cs 생성
XCOPY /Y ClientPacketManager.cs "../../../Client/Assets/Scripts/Packet"
XCOPY /Y ServerPacketManager.cs "../../../Server/Server/Packet"
