using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RemoteX.Connection.Attribute
{
    public enum AttributeReadState { Successful = 0, NonExsited = 1, Error = 2}

    public struct AttributeReadRequest
    {
        public string RequestKey { get; set; }
        public Guid RequestGuid { get; set; }
        public byte[] EncodeToBytesArray()
        {
            var byteList = new List<byte>();
            byteList.AddRange(RequestGuid.ToByteArray());
            byteList.AddRange(Encoding.UTF8.GetBytes(RequestKey));
            return byteList.ToArray();
        }

        public static AttributeReadRequest DecodeFromByteArray(byte[] byteArray)
        {
            var byteList = new List<byte>(byteArray);
            var guid = new Guid(byteList.GetRange(0, 16).ToArray());
            byteList.RemoveRange(0, 16);
            var requestKey = Encoding.UTF8.GetString(byteList.ToArray());
            return new AttributeReadRequest
            {
                RequestGuid = guid,
                RequestKey = requestKey
            };
        }
    }
    public struct AttributeReadResult
    {
        public AttributeReadState ReadState { get; set; }
        public byte[] Value { get; set; }
        public Guid RequestGuid { get; set; }
        public byte[] EncodeToBytesArray()
        {
            
            byte stateByte = (byte)ReadState;
            var sendByteList = new List<byte>();
            sendByteList.AddRange(RequestGuid.ToByteArray());
            sendByteList.Add(stateByte);
            if(Value != null)
            {
                sendByteList.AddRange(Value);
            }
            return sendByteList.ToArray();
        }

        public static AttributeReadResult DecodeFromByteArray(byte[] byteArray)
        {
            if(byteArray.Length == 0)
            {
                return new AttributeReadResult
                {
                    ReadState = AttributeReadState.Error
                };
            }
            var receiveByteList = new List<byte>(byteArray);
            var requestGuid = new Guid(receiveByteList.GetRange(0, 16).ToArray());
            receiveByteList.RemoveRange(0, 16);
            var state = (AttributeReadState)receiveByteList[0];
            receiveByteList.RemoveAt(0);
            var receiveValues = receiveByteList.ToArray();
            return new AttributeReadResult
            {
                RequestGuid = requestGuid,
                ReadState = state,
                Value = receiveValues
            };
        }
    }

    public class AttributeServer
    {
        const Int16 ATTRIBUTE_CHANNEL_CODE = 0;
        public RXDevice RXDevice { get; }
        public Dictionary<byte[], byte[]> Attributes { get; }

        public AttributeServer(RXDevice rxDevice)
        {
            RXDevice = rxDevice;
        }
    }

    public class AttributeClient
    {
        const Int16 ATTRIBUTE_CHANNEL_CODE = 0;

        public int SessionTimeOutInMs
        {
            get
            {
                return 10000;
            }
        }
        public RXDevice RemoteRXDevice { get; }

        object _WaitingThreadDictLock;
        Dictionary<Guid, ThreadResponseBundle> _WaitingThreadDict;
        public RXConnectionManager ConnectionManager
        {
            get
            {
                return RemoteRXDevice.ConnectionManager;
            }
        }
        
        public AttributeClient(RXDevice remoteRXDevice)
        {
            RemoteRXDevice = remoteRXDevice;
            ConnectionManager.OnReceived += ConnectionManager_OnReceived;
            _WaitingThreadDictLock = new object();
            _WaitingThreadDict = new Dictionary<Guid, ThreadResponseBundle>();
        }

        private void ConnectionManager_OnReceived(object sender, RXReceiveMessage e)
        {
            if(e.ChannelCode == ATTRIBUTE_CHANNEL_CODE)
            {
                var response = AttributeReadResult.DecodeFromByteArray(e.Bytes);
                lock(_WaitingThreadDictLock)
                {
                    if (_WaitingThreadDict.ContainsKey(response.RequestGuid))
                    {
                        var bundle = _WaitingThreadDict[response.RequestGuid];
                        bundle.Result = response;
                    }
                }
            }
        }

        public async Task<AttributeReadResult> ReadAsync(string attributeKey)
        {
            Guid requestGuid = Guid.NewGuid();
            AttributeReadRequest readRequest = new AttributeReadRequest
            {
                RequestKey = attributeKey,
                RequestGuid = requestGuid
            };
            var sendMsg = new RXSendMessage()
            {
                ChannelCode = ATTRIBUTE_CHANNEL_CODE,
                Bytes = readRequest.EncodeToBytesArray()
            };
            await RemoteRXDevice.ConnectionManager.SendAsync(sendMsg);
            lock(_WaitingThreadDictLock)
            {
                _WaitingThreadDict.Add(requestGuid, new ThreadResponseBundle { Thread = Thread.CurrentThread});
            }
            AttributeReadResult result;
            try
            {
                Thread.Sleep(SessionTimeOutInMs);
                result = new AttributeReadResult
                {
                    RequestGuid = requestGuid,
                    ReadState = AttributeReadState.Error
                };
            }
            catch(ThreadInterruptedException)
            {
                lock(_WaitingThreadDictLock)
                {
                    result = _WaitingThreadDict[requestGuid].Result;
                }
                
            }
            finally
            {
                _WaitingThreadDict.Remove(requestGuid);
            }
            return result;
        }

        class ThreadResponseBundle
        {
            public Thread Thread { get; set; }
            public AttributeReadResult Result { get; set; }
        }
    }
    
}
