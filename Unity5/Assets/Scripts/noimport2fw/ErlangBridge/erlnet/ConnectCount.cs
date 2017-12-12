using System.Collections;

/**
 * 通讯流水号生成器 
 * @author longlingquan
 * */
public class ConnectCount
{
	
	//instance
	private static  ConnectCount _count;
		 
	/** 流水号 */
	int _number;
	
	public ConnectCount ()
	{
		
	} 
	public int number {
		get { 
			if (_number >= int.MaxValue) {
				_number = 0;
			} else {
				_number++;
			}
			return _number; 
		}
	}
		 
	/** 获得实例 */
	public static ConnectCount  getInstance ()
	{
		if (_count == null) {
			_count = new ConnectCount ();
		}
		return _count;
	} 
}
