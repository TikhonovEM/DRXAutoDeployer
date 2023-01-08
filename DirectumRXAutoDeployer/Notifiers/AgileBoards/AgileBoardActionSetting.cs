namespace DirectumRXAutoDeployer.Notifiers.AgileBoards
{
    public class AgileBoardActionSetting
    {
        public AgileBoardActionTarget Target { get; set; }

        public string ColumnFrom { get; set; }
        
        public string ColumnTo { get; set; }
        
        public string MarkName { get; set; }

        public AgileBoardMarkActionType Type { get; set; }
    }
}