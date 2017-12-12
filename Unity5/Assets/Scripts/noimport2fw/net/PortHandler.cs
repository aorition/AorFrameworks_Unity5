using System.Collections;
using AorBaseUtility;
using YoukiaCore;

public class PortHandler  
{ 
		/**通讯接收*/
		public virtual void receive(Connect connect,ByteBuffer data)
		{
		
		}
	
		/**通讯接收*/
		public virtual void erlReceive(Connect connect,ErlKVMessage message)
		{
			
		} 
}
