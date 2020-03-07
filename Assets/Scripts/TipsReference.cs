public static class TipsReference
{
    public static string TEXT_IS_HERE = "讲话的内容在这里";
    public static string CANT_FIND_MICROPHONE = "似乎找不到麦克风哦...";
    public static string NOTHING_RECORD = "似乎没有录到声音哦...";

    public enum RECORD_TYPE
    {
        None,
        Normal,
        NoMicroPhone,
        TooShort
    }

}