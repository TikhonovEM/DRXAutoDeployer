namespace DirectumRXAutoDeployer.Notifiers.AgileBoards
{
    public class ActionSetting
    {
        public ActionTarget Target { get; set; }

        public string ColumnFrom { get; set; }
        
        public string ColumnTo { get; set; }
        
        public string MarkName { get; set; }

        public MarkActionType Type { get; set; }
    }
}