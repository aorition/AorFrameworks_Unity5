public enum eYKNetEvent
{

	ConnectedDis,
	ConnectStart,
	ConnectSuccessPre,
	ConnectSuccessComplete,
	
	/// <summary>
	/// 网络重连
	/// </summary>
	Reconnect,

	SendMsg,
	AccessMsg,
	ReceiveMsg,
	ReceiveGlobalMsg,

    ReceiveMsgError,

    //ReConnet,
}
