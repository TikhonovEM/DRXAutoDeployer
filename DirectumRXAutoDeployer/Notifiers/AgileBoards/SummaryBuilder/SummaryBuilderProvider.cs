namespace DirectumRXAutoDeployer.Notifiers.AgileBoards.SummaryBuilder
{
    public static class SummaryBuilderProvider
    {
        public static ISummaryBuilder GetBuilderByTarget(string summaryTarget)
        {
            return new PlainTextSummaryBuilder();
        }
    }
}