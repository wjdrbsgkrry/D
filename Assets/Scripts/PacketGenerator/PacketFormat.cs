using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacketGenerator
{
    public class PacketFormat
    {
        public static string managerFormat =
@"using System;
using System.Collections.Generic;
using manager;
using ServerCore;

class PacketManager
{{
    #region SingleTon
    static PacketManager _instance = new PacketManager();
    public static PacketManager Instance {{ get{{ return _instance; }} }}

    #endregion

    PacketManager()
    {{
        Register();
    }}
    Dictionary<ushort, Action<PacketSession, ArraySegment<byte>>> _onRecv = 
        new Dictionary<ushort, Action<PacketSession, ArraySegment<byte>>>();

    Dictionary<ushort, Action<PacketSession, IPacket>> _handler = 
        new Dictionary<ushort, Action<PacketSession, IPacket>>();

    public void Register()
    {{
        {0}
    }}

    public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer)
    {{
        ushort count = 0;

        ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
        count += 2;
        ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
        count += 4;
        Console.WriteLine($""id : {{id}}, size : {{size}}"");

        Action<PacketSession, ArraySegment<byte>> action = null;
        if(_onRecv.TryGetValue(id, out action))
            action.Invoke(session, buffer);
    }}

    void MakePacket<T>(PacketSession session, ArraySegment<byte> buffer) where T : IPacket, new()
    {{
        T p = new T();
        p.Read(buffer);

        Action<PacketSession, IPacket> action = null;
        if (_handler.TryGetValue(p.Protocol, out action))
        {{
            action.Invoke(session, p);
        }}
    }}
}}
";

        public static string managerRegisterFormat =
@"      _onRecv.Add((ushort)PacketID.{0}, MakePacket<{0}>);
        _handler.Add((ushort)PacketID.{0}, PacketHandler.{0}Handler);
";

        public static string fileFormat =
@"using System;
using System.Net;
using System.Collections.Generic;
using System.Text;
using ServerCore;

public enum PacketID
{{
   {0}
}}

public interface IPacket
{{
    ushort Protocol {{ get; }}
    void Read(ArraySegment<byte> segment);
    ArraySegment<byte> Write();
}}

{1}
";

        public static string packetEnumFormat = 
@"{0} = {1},";



        public static string packetFormat =
@"
public class {0}:IPacket
{{
    {1}

public ushort Protocol {{ get {{ return (ushort)PacketID.{0};  }} }}

    public void Read(ArraySegment<byte> segment)
    {{
        ushort count = 0;
        ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);

        {2}
    }}

    public ArraySegment<byte> Write()
    {{
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);

        ushort count = 0;
        bool success = true;
        Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        // 기본 정보
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.{0});
        count += sizeof(ushort);

        {3}

        success &= BitConverter.TryWriteBytes(s, count);
        if (success == false)
            return null;
        return SendBufferHelper.Close(count);
    }}
}}
";

        // {0} 변수 형식
        // {1} 변수 이름
        public static string memberFormat =
@"public {0} {1};";

        // {0] 변수 이름
        // {1} 변환 형식
        // {2} 변수 형식
        public static string readFormat =
@"this.{0} = BitConverter.{1}(s.Slice(count, s.Length - count));
count += sizeof({2});
";

        // {0] 변수 이름
        public static string readStringFormat =
@"ushort {0}Len = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
count += sizeof(ushort);
this.{0} = Encoding.Unicode.GetString(s.Slice(count, {0}Len));
count += {0}Len;
";

        // {0] 변수 이름
        // {1} 변환 형식
        public static string writeFormat =
@"success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), {0});
count += sizeof({1});
";
        // {0] 변수 이름
        public static string writeStringFormat =
@"ushort {0}Len = (ushort)Encoding.Unicode.GetBytes(this.{0}, 0, {0}.Length, segment.Array, segment.Offset + count + sizeof(ushort));
success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), {0}Len);
count += sizeof(ushort);
count += {0}Len;
";

        // {0} 리스트 이름 [대문자]
        // {1} 리스트 이름 [소문자]
        // {2] 멤버 변수들
        // {3} 멤버 변수 read
        // {4} 멤버 변수 write
        public static string memberListFormat =
@"public List<{0}> {1}s = new List<{0}>();

public struct {0}
{{
    {2} 

    public void Read(ReadOnlySpan<byte> s, ref ushort count, ref ArraySegment<byte> segment)
    {{
        {3}
    }}

    public bool Write(Span<byte> s, ref ushort count, ref ArraySegment<byte> segment)
    {{
        bool success = true;
        {4}
        return success;
    }}

}}
";

        public static string readListFormat = 
@"{1}s.Clear();
ushort {1}Count = BitConverter.ToUInt16(s.Slice(count, s.Length - count));
count += sizeof(ushort);
for (int i = 0; i < {1}Count; i++)
{{
    {0} {1} = new {0}();
    {1}.Read(s, ref count, ref segment);
    {1}s.Add({1});
}}
";


        public static string writeListFormat =
@"success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort){1}s.Count);
count += sizeof(ushort);

foreach ({0} {1} in this.{1}s)
    {1}.Write(s, ref count, ref segment);
";

        public static string readByteFormat =
@"this.{0} = ({1})segment.Array[segment.Offset + count];
count += sizeof({1});";

        public static string writeByteFormat =
@"segment.Array[segment.Offset + count] = (byte)this.{0};
count += sizeof({1});";

    }
}
