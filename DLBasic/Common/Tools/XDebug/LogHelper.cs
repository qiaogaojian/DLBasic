public class LogHelper
{
    public const uint B13Port = 0x000001;
    public const uint CY = 0x000004;
    public const uint Test = 0x000008;
    public const uint NOLOG = 0;
    public const uint ALLLOG = uint.MaxValue;

    public static void Init(bool isOpen)
    {
        XDebug.AddKeyInfo(B13Port, "白旺");
        XDebug.AddKeyInfo(CY, "曹毅");
        XDebug.AddKeyInfo(Test, "测试");
        XDebug.SetLogActive(isOpen ? ALLLOG : NOLOG);
    }

    public static void OnGameOver()
    {
        XDebug.SafeReleaseWriteOut();
    }
}