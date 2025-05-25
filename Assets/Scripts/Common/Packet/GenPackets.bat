@echo off
:: start ..\\..\\Scripts\\PacketGenerator\\bin\\PacketGenerator.exe "..\\..\\Scripts\\PacketGenerator\\PDL.xml"

timeout /t 1
move "..\\..\\..\\..\\GenPackets.cs" %~dp0
move "..\\..\\..\\..\\ClientPacketManager.cs" %~dp0
move "..\\..\\..\\..\\ServerPacketManager.cs" %~dp0

timeout /t 2

:: xCOPY /Y GenPackets.cs "..\\..\\Server\Packet"
xCOPY /Y GenPackets.cs "..\\..\\Networks\\Packet"

xCOPY /Y ClientPacketManager.cs "..\\..\\Networks\\Packet"
:: xCOPY /Y ServerPacketManager.cs "..\\..\\Server\Packet"	