using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OharaNet.Core.Data
{
    public record PeerInfo(IPAddress IpAddress, int TcpPort);
}
