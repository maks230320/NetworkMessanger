namespace NetworkMessanger
{
    internal class Commands
    {
        public const string GETNICK = "GETNICK";
        public const string SETNICK = "SETNICK";


        public static string SetNickname(string nickname)
        {
            return SETNICK + ":" + nickname;
        }
        public static string GetNickname(string commandString)
        {
            return commandString.Substring(8);
        }
    }
}
