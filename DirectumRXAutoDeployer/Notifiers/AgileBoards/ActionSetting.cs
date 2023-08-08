namespace DirectumRXAutoDeployer.Notifiers.AgileBoards
{
    public class ActionSetting
    {
        public ActionTarget Target { get; set; }

        public string ColumnFrom { get; set; }
        
        public string ColumnTo { get; set; }
        
        public string TagName { get; set; }

        public string SummaryHeader { get; set; }

        public TagActionType Type { get; set; }
    }
}