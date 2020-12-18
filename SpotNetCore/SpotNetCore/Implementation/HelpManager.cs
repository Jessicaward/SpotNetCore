namespace SpotNetCore.Implementation
{
    public class HelpManager
    {
        public static void DisplayHelp()
        {
            Terminal.WriteMagenta("---Help---");
            Terminal.WriteWhite("This is a full list of SpotNetCore commands:");
            Terminal.WriteBlue("");
        }
    }
}