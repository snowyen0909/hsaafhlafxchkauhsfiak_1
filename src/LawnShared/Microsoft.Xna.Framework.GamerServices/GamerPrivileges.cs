namespace Microsoft.Xna.Framework.GamerServices;

public class GamerPrivileges
{
	public GamerPrivilegeSetting AllowCommunication => GamerPrivilegeSetting.Everyone;

	public bool AllowOnlineSessions => true;

	public GamerPrivilegeSetting AllowProfileViewing => GamerPrivilegeSetting.Blocked;

	public bool AllowPurchaseContent => false;

	public bool AllowTradeContent => false;

	public GamerPrivilegeSetting AllowUserCreatedContent => GamerPrivilegeSetting.Blocked;
}
